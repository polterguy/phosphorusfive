/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Reflection;
using System.Collections.Generic;
using p5.ajax.core;
using p5.ajax.core.internals;

namespace p5.ajax.widgets
{
    /// <summary>
    ///     Abstract base class for all Ajax widgets.
    /// </summary>
    public abstract class Widget : Control, IAttributeAccessor
    {
        #region [ -- Nested class declarations and enums -- ]

        /// <summary>
        ///     EventArgs for an Ajax server-side event.
        /// </summary>
        public class AjaxEventArgs : EventArgs
        {
            /// <summary>
            ///     Initializes a new instance of the AjaxEventArgs class.
            /// </summary>
            /// <param name="name">Name of Ajax event that was raised.</param>
            internal AjaxEventArgs (string name)
            {
                Name = name;
            }

            /// <summary>
            ///     Retrieves the name of the event raised.
            /// </summary>
            /// <value>The name of the event raised on the client-side</value>
            public string Name { get; private set; }
        }

        /// <summary>
        ///     Defines how the widget is supposed to be rendered during the current request.
        /// </summary>
        protected internal enum RenderingMode
        {
            /// <summary>
            ///     The default rendering mode.
            /// 
            ///     This will render the widget either as plain HTML, or pass JSON updates back to client, depending upon whether or not the current
            ///     request is an Ajax request, or one of its ancestor widgets are being re-rendered.
            /// </summary>
            Default,

            /// <summary>
            ///     Re-rendering mode, the entire widget will be re-rendered back to the client as HTML, regardless of whether or not it's an Ajax callback,
            ///     or none of its ancestor widgets are being re-rendered.
            /// 
            ///     This will either replace the widget's entire HTML using "outerHTML", or render the widget as pure HTML, depending upon whether or not
            ///     any of its ancestor widgets are being re-rendered or not.
            /// </summary>
            ReRender,

            /// <summary>
            ///     Renders the widget as invisible, which means it'll render without any of its attributes or content, and have
            ///     a style property with "display:none !important", to simply serve as a placeholder for widget, as it later 
            ///     becomes visible.
            /// 
            ///     This will replace the entire widget's HTML with its invisible HTML version. Exactly how this is done, depends upon whether or not one of
            ///     its ancestor widgets are being re-rendered or not. It might either be an "outerHTML" replacement, or simply rendering widget's HTML into
            ///     the HtmlTextWriter.
            /// </summary>
            WidgetBecameInvisible
        }

        #endregion

        #region [ -- Private and protected fields -- ]

        // Contains a reference to the AjaxPage owning this widget.
        private AjaxPage _page;

        // Used to hold the old ID of widget, in the rare case, that its ID is somehow changed.
        // Necessary to make sure we are able to retrieve widget, and update its ID on the client side, during Ajax requests.
        private string _oldClientID;

        /// <summary>
        ///     Allows you to explicitly force to have your widget re-rendered, or re-rendering its children, if you wish.
        /// </summary>
        protected internal RenderingMode RenderMode = RenderingMode.Default;

        /// <summary>
        ///     Contains all attributes for widget.
        /// </summary>
        protected internal readonly AttributeStorage Attributes = new AttributeStorage ();

        #endregion

        #region [ -- Public properties and methods -- ]

        /// <summary>
        ///     Returns the owning AjaxPage for current widget.
        /// </summary>
        /// <value>The ajax page.</value>
        public AjaxPage AjaxPage
        {
            get {
                if (_page == null) {
                    _page = Page as AjaxPage;
                    if (_page == null)
                        throw new ApplicationException ("Oops, make sure you inherit your page from AjaxPage somehow.");
                }
                return _page;
            }
        }

        /// <summary>
        ///     Gets or sets the element type used to render your widget.
        /// 
        ///     Set this value to whatever HTML element name you wish for your widget to render as, e.g. "p", "div", "span", etc.
        /// </summary>
        /// <value>The HTML element used to render widget</value>
        public string Element
        {
            get { return Attributes.GetAttribute ("Element"); }
            set {

                // Verifying we actually have a change.
                if (value == Element)
                    return; // No need to continue, returning early.

                // Sanity check, before we change Element name, and possibly triggering a re-rendering of widget.
                SanitizeElementName (value);
                SetAttribute ("Element", value);

                // When we change a widget's Element, we also re-render it, but only if this is after we've "rooted" the widget, since CTORs
                // of derived widgets might provide a default Element, which means we'd re-render all widgets, every time we re-created them, unless we
                // checked if widget was already "rooted".
                if (Parent != null)
                    RenderMode = RenderingMode.ReRender;
            }
        }

        /// <summary>
        ///     Gets or sets an attribute value for your widget.
        /// 
        ///     Notice, to remove an attribute, please use the "DeleteAttribute" method, since even though you set an attribute's
        ///     value to "null", it will still exist on widget, as an "empty attribute".
        /// </summary>
        /// <param name="name">Name of attribute to retrieve or set value of.</param>
        public string this [string name]
        {
            get { return GetAttribute (name); }
            set { SetAttribute (name, value); }
        }

        /// <summary>
        ///     Determines whether this instance has an attribute with the specified name.
        /// </summary>
        /// <returns><c>true</c> if this instance has the attribute with the specified name; otherwise, <c>false</c></returns>
        /// <param name="name">Name of the attribute to check if exists.</param>
        public virtual bool HasAttribute (string name)
        {
            return Attributes.HasAttribute (name);
        }

        /// <summary>
        ///     Returns the value of the specified attribute for widget.
        /// 
        ///     Notice, an attribute can hold a "null" value, to determine attribute's existence, use HasAttribute instead.
        ///     You can also use the subscript operator instead of this method, if you wish.
        /// </summary>
        /// <returns>The attribute value</returns>
        /// <param name="key">Attribute name ot look for</param>
        public virtual string GetAttribute (string key)
        {
            return Attributes.GetAttribute (key);
        }

        /// <summary>
        ///     Sets the specified attribute key to the specified value.
        /// 
        ///     Notice, a widget can have attributes with "null" values, to remove an attribute, use the DeleteAttribute method instead.
        ///     You can also use the subscript operator instead of this method, if you wish.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public virtual void SetAttribute (string key, string value)
        {
            // Notice, we store the attribute differently, depending upon whether or not we have started tracking ViewState or not.
            // This is necessarily due to making sure we're able to track changes to attributes, and correctly pass them on to the client.
            if (!IsTrackingViewState)
                Attributes.SetAttributePreViewState (key, value);
            else
                Attributes.ChangeAttribute (key, value);
        }

        /// <summary>
        ///     Deletes the specified attribute from widget entirely.
        /// </summary>
        /// <param name="name">Name of attribute you wish to delete.</param>
        public virtual void DeleteAttribute (string name)
        {
            Attributes.DeleteAttribute (name);
        }

        /// <summary>
        ///     Returns all attribute keys for widget.
        /// </summary>
        /// <value>All attribute keys for widget</value>
        public virtual IEnumerable<string> AttributeKeys
        {
            get { return Attributes.Keys; }
        }

        /// <summary>
        ///     Invokes the given event handler.
        /// 
        ///     Useful for invoking for instance an Ajax DOM event handler explicitly.
        /// </summary>
        /// <param name="eventName">Event name such as 'onclick', or name of C# method on Page, UserControl, or MasterPage</param>
        /// <exception cref="NotImplementedException"></exception>
        public void InvokeEventHandler (string eventName)
        {
            // Defaulting to event name for WebMethod invocations from JavaScript.
            var eventHandlerName = eventName;

            // Checking is this is an automatically generated mapping between server method and JavaScript handler.
            if (HasAttribute (eventName))
                eventHandlerName = this [eventName]; // This is an "onXXX" event, retrieving underlaying method name we should invoke.

            // Finding out at what context to invoke the method within, which means iterating upwards in Control hierarchy, 
            // until we find a UserControl, or the Page itself.
            var owner = Parent;
            while (!(owner is UserControl) && !(owner is Page))
                owner = owner.Parent;

            // Retrieving the MethodInfo, such that we can invoke it using Reflection.
            var method = owner.GetType ().GetMethod (eventHandlerName,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.FlattenHierarchy);
            if (method == null)
                throw new NotImplementedException ("Method + '" + eventHandlerName + "' could not be found");

            // Verifying method has the WebMethod attribute.
            // For security reasons we want method to be explicitly marked as WebMethod, and not allow inherited methods to be implicitly legal.
            var atrs = method.GetCustomAttributes (typeof (WebMethod), false);

            // Notice, to not give away any information to malicious requests, allowing them to figure out which methods exists on UserControl/Page,
            // we throw the exact same exception as above.
            if (atrs == null || atrs.Length == 0)
                throw new NotImplementedException ("Method + '" + eventHandlerName + "' could not be found");

            // Invoking methods with the "this" widget, and an AjaxEventArg, passing in the name of the event that was raised on the client, to allow
            // for reusing the same event handler, for multiple events.
            method.Invoke (owner, new object [] { this, new AjaxEventArgs (eventName) });
        }

        #endregion

        #region [ -- Protected methods and properties -- ]

        /// <summary>
        ///     This is the ClientID to use when sending JSON updates back to the client for the widget.
        /// 
        ///     This is necessary for the rare occasions that a widget changes its ID during an Ajax callback, at which point, the DOM
        ///     element we're updating, does not have the ClientID the widget has during rendering. Hence, when transmitting updates
        ///     back to the client for a widget, using JSON updates, we must use this property, and not the ClientID.
        /// </summary>
        /// <value>The JSON client identifier.</value>
        protected string JsonClientID
        {
            get { return _oldClientID ?? ClientID; }
        }

        /// <summary>
        ///     Formats given HtmlTextWriter, by adding the correct number of TABs, plus one initial CR/LF.
        /// 
        ///     Notice, will not do anything if the current request is an Ajax request.
        /// </summary>
        /// <returns>The formatting.</returns>
        /// <param name="tabs">Tabs.</param>
        protected void IndentWidgetRendering (HtmlTextWriter writer)
        {
            // We don't format at all, unless this is an HTML request, somehow.
            if (!AjaxPage.IsAjaxRequest) {
                writer.Write ("\r\n");
                Control idxCtrl = this;
                while (idxCtrl != null) {
                    writer.Write ("\t");
                    idxCtrl = idxCtrl.Parent;
                }
            }
        }

        /// <summary>
        ///     Renders widget's content as pure HTML into specified HtmlTextWriter.
        /// 
        ///     Method is abstract in Widget class, make sure you override it, if you inherit directly from Widget, to provide custom rendering.
        /// </summary>
        /// <param name="writer">The HtmlTextWriter to render the widget into.</param>
        protected abstract void RenderHtmlResponse (HtmlTextWriter writer);

        /// <summary>
        ///     Loads the form data from the HTTP request object for the current widget, if there is any data.
        /// </summary>
        protected abstract void LoadFormData ();

        /// <summary>
        ///     Returns true if any of widget's ancestor widgets are re-rendered, or wants to have their children re-rendered, meaning they render as HTML.
        ///     At which case, this widget should also render as pure HTML into HtmlTextWriter, returning content to client.
        /// </summary>
        /// <returns><c>true</c>, if is re rendering was ancestored, <c>false</c> otherwise.</returns>
        protected bool AncestorIsReRendering ()
        {
            // Returns true if any of its ancestors are rendering as HTML.
            var idx = Parent as Widget;
            while (idx != null) {
                if (idx.RenderMode == RenderingMode.ReRender)
                    return true;
                idx = idx.Parent as Widget;
            }
            return false;
        }

        /// <summary>
        ///     Verifies the Element name is legal for the current widget.
        /// 
        ///     Override this method in your own classes, to provide further restrictions. But please, call base implementation, 
        ///     unless you wish to eliminate all the basic restrictions, allowing for "weird" elements to be created.
        ///     This implementation throws an exception if elementName is empty string (""), or contains anything but a-z or 1-6, which should
        ///     accommodate for most "sane" HTML elements.
        /// </summary>
        /// <param name="elementName">The new Element name.</param>
        protected virtual void SanitizeElementName (string elementName)
        {
            // Making sure Element is not empty string "".
            if (elementName == "")
                throw new ArgumentException ("Sorry, but you must provide either an actual value, or 'null', as the Element name for your widget", nameof (Element));

            // Making sure Element does not contain other non-legal characters.
            if (elementName.Any (ix => !"abcdefghijklmnopqrstuvwxyz123456".Contains (ix)))
                throw new ArgumentException ("Sorry, but p5.ajax doesn't like these types of characters for its Element names", nameof (Element));
        }

        #endregion

        #region [ -- Overrides from System.Web.UI.Control -- ]

        /// <summary>
        ///     Overridden to make it possible to change an element's ID during an Ajax callback.
        /// </summary>
        /// <value>The new ID</value>
        public override string ID
        {
            get { return base.ID; }
            set {
                // Storing old ID of element, since this is the stuff that'll be rendered over the wire,
                // to allow for retrieving the element on the client side, and change its ID on the client side.
                if (IsTrackingViewState && value != ID) {

                    // Notice, we only keep the "first original ID", in case ID is changed multiple times during page's life cycle.
                    if (_oldClientID == null)
                        _oldClientID = ClientID;
                    Attributes.ChangeAttribute ("id", value);
                }
                base.ID = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether Widget is visible or not.
        /// 
        ///     Overridden from base, to make sure we can automatically set rendering mode for widget, during changes to visibility.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public override bool Visible
        {
            get { return base.Visible; }
            set {
                if (value == base.Visible)
                    return; // No change, returning early ...

                // If we've changed widget's visibility after we've started tracking ViewState, and this is an Ajax request, 
                // we'll have to change widget's rendering mode.
                if (IsTrackingViewState && AjaxPage.IsAjaxRequest)
                    RenderMode = value ? RenderingMode.ReRender : RenderingMode.WidgetBecameInvisible;
                base.Visible = value;
            }
        }

        /// <summary>
        ///     Overridden to make sure we can load widget's attributes from ViewState.
        /// </summary>
        /// <param name="savedState">Saved state.</param>
        protected override void LoadViewState (object savedState)
        {
            var tmp = savedState as object [];
            base.LoadViewState (tmp [0]);
            Attributes.LoadFromViewState (tmp [1]);
            Attributes.LoadRemovedFromViewState (tmp [2]);
        }

        /// <summary>
        ///     Overridden to make sure we can load FORM data sent from client, in addition to invoking Ajax event handlers for widget.
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected override void OnLoad (EventArgs e)
        {
            // Making sure we load POST FORM data.
            if (Page.IsPostBack)
                LoadFormData ();

            // Making sure event handlers are being raised, before we call base.
            if (AjaxPage.IsAjaxRequest) {

                // Checking if current widget was the one creating this Ajax request, and if so, making sure we raise the event that was raised client-side.
                if (Page.Request.Params ["_p5_widget"] == ClientID) {

                    // Making sure we raise our event, after page is finished loading.
                    Page.LoadComplete += delegate { InvokeEventHandler (Page.Request.Params ["_p5_event"]); };
                }
            }
            base.OnLoad (e);
        }

        /// <summary>
        ///     Renders the widget.
        /// 
        ///     Overridden to entirely bypass the ASP.NET Web Forms rendering, and provide our own rendering, with Ajax support.
        ///     Notice, we never call base class implementation here!
        ///     RenderChildren might be invoked.
        /// </summary>
        /// <param name="writer">Writer.</param>
        public override void RenderControl (HtmlTextWriter writer)
        {
            // If one of its ancestors are invisible, we do not render this widget at all.
            if (AreAncestorsVisible ()) {

                // Rendering widget differently, according to whether or not it is visible or not.
                if (Visible)
                    RenderVisibleWidget (writer);
                else
                    RenderInvisibleWidget (writer);
            }
        }

        /// <summary>
        ///     Overridden to make sure we can persist widget's attributes into ViewState.
        /// </summary>
        /// <returns>The view state.</returns>
        protected override object SaveViewState ()
        {
            var retVal = new object [3];
            retVal [0] = base.SaveViewState ();
            retVal [1] = Attributes.SaveToViewState (this);
            retVal [2] = Attributes.SaveRemovedToViewState ();
            return retVal;
        }

        #endregion

        #region [ -- Private helper methods -- ]

        /*
         * Responsible for rendering all different permutations for a visible widget.
         */
        private void RenderVisibleWidget (HtmlTextWriter writer)
        {
            // How its ancestor Control(s) are being rendered, largely detremine how this widget is rendered, since an ancestor being shown,
            // that was previously hidden, or newly created for that matter, triggers a re-rendering of the entire widget, one way or another.
            if (!AjaxPage.IsAjaxRequest || AncestorIsReRendering ()) {

                // Not an Ajax request, or ancestors are re-rendering widget somehow.
                // Hence, we default to rendering widget as HTML into the given HtmlTextWriter.
                RenderHtmlResponse (writer);

            } else if (RenderMode == RenderingMode.ReRender) {

                // Re-rendering entire widget, including its children.
                using (var streamWriter = new StreamWriter (new MemoryStream ())) {
                    using (var txt = new HtmlTextWriter (streamWriter)) {
                        RenderHtmlResponse (txt);
                        txt.Flush ();
                        streamWriter.BaseStream.Seek (0, SeekOrigin.Begin);
                        using (TextReader reader = new StreamReader (streamWriter.BaseStream)) {
                            AjaxPage.RegisterWidgetChanges (JsonClientID, "outerHTML", reader.ReadToEnd ());
                        }
                    }
                }

            } else {

                // Only pass changes for this widget back to the client as JSON, before we render its children.
                Attributes.RegisterChanges (AjaxPage, JsonClientID);
                RenderChildren (writer);
            }
        }

        /*
         * Responsible for rendering all different permutations for an invisible widget.
         */
        private void RenderInvisibleWidget (HtmlTextWriter writer)
        {
            var ancestorReRendering = AncestorIsReRendering ();
            if (AjaxPage.IsAjaxRequest && RenderMode == RenderingMode.WidgetBecameInvisible && !ancestorReRendering) {

                // Re-rendering widget's invisible markup, since widget was made invisible during the current request.
                AjaxPage.RegisterWidgetChanges (
                    JsonClientID,
                    "outerHTML",
                    string.Format (@"<{0} id=""{1}"" style=""display:none important!;""></{0}>", Element, ClientID));

            } else if (!AjaxPage.IsAjaxRequest || ancestorReRendering) {

                // Rendering invisible HTML.
                writer.Write (string.Format (@"<{0} id=""{1}"" style=""display:none important!;""></{0}>", Element, ClientID));
            }
        }

        /*
         * Returns true if all of widget's ancestor widgets are visible.
         * Necessary to determine if this widget is visible or not.
         * Visibility of a widget is determined by AND'ing widget's own visibility together with visibility of all of its ancestor
         * widgets'/controls' visibility.
         */
        private bool AreAncestorsVisible ()
        {
            // If this widget, and all of its ancestor widgets are visible, then we return true, otherwise we return false.
            var idx = Parent;
            while (true) {
                if (!idx.Visible)
                    break;
                idx = idx.Parent;
                if (idx == null) // We traversed all the way up to beyond the Page, and found no invisible ancestor controls/widgets.
                    return true;
            }

            // Invisible control among widget's ancestor widgets was found.
            return false;
        }

        #endregion
    }
}