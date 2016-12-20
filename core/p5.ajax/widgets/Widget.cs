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
    [ViewStateModeById]
    public abstract class Widget : Control, IAttributeAccessor
    {
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
        protected enum RenderingMode
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
            /// </summary>
            ReRender,

            /// <summary>
            ///     Renders the widget as invisible, which means it'll render without any of its attributes or content, and have
            ///     a style property with "display:none !important", to simply serve as a placeholder for widget, as it later 
            ///     becomes visible.
            /// </summary>
            WidgetBecameInvisible
        }

        // Contains all attributes for widget.
        private readonly AttributeStorage _attributes = new AttributeStorage ();

        // Contains a reference to the AjaxPage owning this widget.
        private AjaxPage _page;

        // Used to hold the old ID of widget, in the rare case, that its ID is somehow changed.
        // Necessary to make sure we are able to retrieve widget, and update its ID on the client side, during Ajax requests.
        private string _oldClientID;

        /// <summary>
        ///     Initializes a new instance of a Widget.
        /// </summary>
        public Widget ()
        {
            RenderMode = RenderingMode.Default;
        }

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
                    _attributes.ChangeAttribute ("id", value);
                }
                base.ID = value;
            }
        }

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
        ///     Gets or sets a value indicating whether Widget is visible or not.
        /// 
        ///     Overridden from base, to make sure we can automatically set rendering mode for widget, during changes to visibility.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public override bool Visible
        {
            get { return base.Visible; }
            set {
                if (value == Visible)
                    return; // Nothing to do here ...

                if (!Visible && value && IsTrackingViewState && AjaxPage.IsAjaxRequest) {

                    // This widget was made visible during this request, and should be re-rendered as such.
                    RenderMode = RenderingMode.ReRender;

                } else if (Visible && !value && IsTrackingViewState && AjaxPage.IsAjaxRequest) {

                    // This widget was made invisible during this request, and should be re-rendered as invisible.
                    RenderMode = RenderingMode.WidgetBecameInvisible;
                }
                base.Visible = value;
            }
        }

        /// <summary>
        ///     Gets or sets the element type used to render your widget.
        /// 
        ///     Set this value to whatever HTML element name you wish for your widget to render as, e.g. "p", "div", "span", etc.
        /// </summary>
        /// <value>The HTML element used to render widget</value>
        public virtual string Element
        {
            get { return _attributes.GetAttribute ("Element"); }
            set {

                // Verifying we actually have a change.
                if (value == Element)
                    return; // No need to continue.

                // If Element name is set to "null", we entirely remove attribute, to let default value of sub classes kick in, and do its thing.
                if (value == null) {

                    // Deletion of Element, which means overridden defaults in sub classes will hopefully return some sane default.
                    DeleteAttribute ("Element");

                } else {

                    // Sanity check, before figuring out what to do with new value for Element property.
                    SanitizeElementName (value);

                    // Setting the element name, depending upon if we're serializing changes or not.
                    SetAttributeValue ("Element", value);
                }

                // When the element name changes, we re-render widget entirely automatically.
                ReRender ();
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
            get { return GetAttributeValue (name); }
            set { SetAttributeValue (name, value); }
        }

        /// <summary>
        ///     Determines whether this instance has an attribute with the specified name.
        /// </summary>
        /// <returns><c>true</c> if this instance has the attribute with the specified name; otherwise, <c>false</c></returns>
        /// <param name="name">Name of the attribute to check if exists.</param>
        public virtual bool HasAttribute (string name)
        {
            return _attributes.HasAttribute (name);
        }

        /// <summary>
        ///     Returns the value of the given attribute, if any.
        /// </summary>
        /// <returns>The attribute value.</returns>
        /// <param name="name">Name.</param>
        public virtual string GetAttributeValue (string name)
        {
            return _attributes.GetAttribute (name);
        }

        /// <summary>
        ///     Sets the specified attribute to the specified value.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public virtual void SetAttributeValue (string name, string value)
        {
            // Notice, we store the attribute differently, depending upon whether or not we have started tracking ViewState or not.
            // This is necessarily due to making sure we're able to track changes to attributes, and correctly pass them on to the client.
            if (!IsTrackingViewState)
                _attributes.SetAttributePreViewState (name, value);
            else
                _attributes.ChangeAttribute (name, value);
        }

        /// <summary>
        ///     Deletes the specified attribute from widget entirely.
        /// </summary>
        /// <param name="name">Name of attribute you wish to delete.</param>
        public virtual void DeleteAttribute (string name)
        {
            if (HasAttribute (name))
                _attributes.RemoveAttribute (name);
        }

        /// <summary>
        ///     Returns all attribute keys for widget.
        /// </summary>
        /// <value>All attribute keys for widget</value>
        public virtual IEnumerable<string> AttributeKeys
        {
            get { return _attributes.Keys; }
        }

        /// <summary>
        ///     Invokes the given event handler.
        /// 
        ///     Useful for invoking for instance an Ajax DOM event handler explicitly.
        /// </summary>
        /// <param name="eventName">Event name such as 'onclick', or name of C# method on Page, UserControl, or MasterPage</param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void InvokeEventHandler (string eventName)
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

        /// <summary>
        ///     Gets a value indicating whether this instance has content or not.
        /// 
        ///     This property is used to determine how a widget should be rendered, if its rendering mode is set to "immediate", 
        ///     and if its children should be rendered or not.
        ///     Method is abstract, and must be overridden in derived classes.
        /// </summary>
        /// <value><c>true</c> if this instance has content; otherwise, <c>false</c></value>
        protected abstract bool HasContent
        {
            get;
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

        /// <summary>
        ///     Overridden to make sure we can load widget's attributes from ViewState.
        /// </summary>
        /// <param name="savedState">Saved state.</param>
        protected override void LoadViewState (object savedState)
        {
            var tmp = savedState as object [];
            base.LoadViewState (tmp [0]);
            _attributes.LoadFromViewState (tmp [1]);
            _attributes.LoadRemovedFromViewState (tmp [2]);
        }

        /// <summary>
        ///     Loads the form data from the HTTP request object for the current widget, if there is any data.
        /// </summary>
        protected virtual void LoadFormData ()
        {
            if (!Visible)
                return;
            if (!HasAttribute ("disabled")) {
                if (!string.IsNullOrEmpty (this ["name"])) {
                    switch (Element) {
                        case "input":
                            switch (this ["type"]) {
                                case "radio":
                                case "checkbox":
                                    if (Page.Request.Params [this ["name"]] != null) {
                                        var splits = Page.Request.Params [this ["name"]].Split (',');
                                        bool found = false;
                                        foreach (var idxSplit in splits) {
                                            if (idxSplit == this ["value"] || (!HasAttribute ("value") && idxSplit == "on")) {
                                                found = true;
                                                break;
                                            }
                                        }
                                        if (found) {
                                            _attributes.SetAttributeFormData ("checked", null);
                                        } else {
                                            _attributes.RemoveAttribute ("checked", false);
                                        }
                                    } else {
                                        _attributes.RemoveAttribute ("checked", false);
                                    } break;
                                default:
                                    if (Page.Request.Params [this ["name"]] != null)
                                        _attributes.SetAttributeFormData ("value", Page.Request.Params [this ["name"]]);
                                    else
                                        _attributes.RemoveAttribute ("value");
                                    break;
                            }
                            break;
                        case "textarea":
                            if (Page.Request.Params [this ["name"]] != null)
                                _attributes.SetAttributeFormData ("innerValue", Page.Request.Params [this ["name"]]);
                            else
                                _attributes.RemoveAttribute ("innerValue");
                            break;
                        case "select":
                            if (Page.Request.Params [this ["name"]] != null) {
                                var splits = Page.Request.Params [this ["name"]].Split (',').Select(ix => Page.Server.UrlDecode (ix));
                                foreach (var idxChild in Controls) {
                                    var idxChildWidget = idxChild as Widget;
                                    if (idxChildWidget != null) {
                                        if (splits.Contains (idxChildWidget ["value"])) {
                                            idxChildWidget._attributes.SetAttributeFormData ("selected", null);
                                        } else {
                                            idxChildWidget._attributes.RemoveAttribute ("selected", false);
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Overridden to make sure we can load FORM data sent from client, in addition to invoking Ajax event handlers for widget.
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected override void OnLoad (EventArgs e)
        {
            if (Page.IsPostBack)
                LoadFormData ();

            // Making sure event handlers are being raised.
            if (AjaxPage.IsAjaxRequest) {
                if (Page.Request.Params ["_p5_widget"] == ClientID) {
                    Page.LoadComplete += delegate {

                        // Event was raised for this widget.
                        InvokeEventHandler (Page.Request.Params ["_p5_event"]);
                    };
                }
            }
            base.OnLoad (e);
        }

        /// <summary>
        ///     Gets or sets the rendering mode for the widget.
        /// 
        ///     Allows you to explicitly force to have your widget re-rendered, or re-rendering its children, if you wish.
        /// </summary>
        /// <value>The render mode</value>
        protected RenderingMode RenderMode
        {
            get;
            set;
        }

        /// <summary>
        ///     Forces a re-rendering of your widget.
        /// 
        ///     Notice, this is almost never necessary, but will invoke a "partial rendering", or re-rendering of the entire widget.
        /// </summary>
        protected internal virtual void ReRender ()
        {
            RenderMode = RenderingMode.ReRender;
        }

        /// <summary>
        ///     Renders the widget.
        /// 
        ///     Overridden to entirely bypass the ASP.NET Web Forms rendering, and provide our own with Ajax support.
        ///     Notice, we never call base class implementation here!
        ///     RenderChildren though, might get invoked.
        /// </summary>
        /// <param name="writer">Writer.</param>
        public override void RenderControl (HtmlTextWriter writer)
        {
            // If one of its ancestors are invisible, we do not render this control at all.
            if (AreAncestorsVisible ()) {

                // Rendering widget differently, according to whether or not it is visible or not.
                if (Visible)
                    RenderVisibleWidget (writer);
                else
                    RenderInvisibleWidget (writer);
            }
        }

        /*
         * Responsible for rendering all different permutations for a visible widget.
         */
        private void RenderVisibleWidget (HtmlTextWriter writer)
        {
            // How its ancestor Control(s) are being rendered, largely detremine how this widget is rendered, since an ancestor being shown,
            // that was previously hidden for instance, triggers a rendering of the entire widget, one way or another.
            if (!AjaxPage.IsAjaxRequest || AncestorIsReRendering ()) {

                // Not an Ajax request, or ancestors are re-rendering widget somehow.
                // Hence, we default to rendering widget as HTML into the given HtmlTextWriter.
                RenderHtmlResponse (writer);

            } else {

                // Checking the rendering mode of this widget, which also determines how widget should be rendered.
                if (RenderMode == RenderingMode.ReRender) {

                    // Re-rendering entire widget.
                    AjaxPage.RegisterWidgetChanges (JsonClientID, "outerHTML", GetWidgetHtml ());

                } else {

                    // Only pass changes for this widget back to the client as JSON, before we render its children.
                    _attributes.RegisterChanges (AjaxPage, JsonClientID);
                    RenderChildren (writer);
                }
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
                AjaxPage.RegisterWidgetChanges (JsonClientID, "outerHTML", GetWidgetInvisibleHtml ());

            } else if (!AjaxPage.IsAjaxRequest || ancestorReRendering) {

                // Rendering invisible HTML.
                writer.Write (GetWidgetInvisibleHtml ());
            }
        }

        /// <summary>
        ///     Renders widget's content as pure HTML into specified HtmlTextWriter.
        /// 
        ///     Override this one to provide custom rendering.
        ///     Notice, you can also override one of the other rendering methods, if you only wish to slightly modify the widget's rendering, such as
        ///     its opening or closing tag rendering.
        /// </summary>
        /// <param name="writer">The HtmlTextWriter to render the widget into.</param>
        protected virtual void RenderHtmlResponse (HtmlTextWriter writer)
        {
            // Rendering opening tag for element, then its children, before we render the closing tag.
            var noTabs = RenderTagOpening (writer);
            RenderChildren (writer);
            RenderTagClosing (writer, noTabs);
        }

        /// <summary>
        ///     Renders the HTML opening tag of widget.
        /// 
        ///     Override to provide custom rendering for opening tag of widget.
        /// </summary>
        /// <returns>The number of tabs we had to add, to nicely format element into HTML.</returns>
        /// <param name="writer">The HtmlTextWriter we should render into.</param>
        protected virtual int RenderTagOpening (HtmlTextWriter writer)
        {
            // Making sure we nicely indent element, unless this is an Ajax request.
            var noTabs = 0;
            if (!AjaxPage.IsAjaxRequest) {

                // Appending one one CR/LF sequence.
                // Then appending and one TAB, for each ancestor Control this instance has, between itself and the Page object.
                // This ensures that widget is nicely formatted if this is not an Ajax request.
                writer.Write ("\r\n\t");
                noTabs = 1;
                Control idxCtrl = this;
                while (idxCtrl != Page) {
                    writer.Write ("\t");
                    idxCtrl = idxCtrl.Parent;
                    noTabs += 1;
                }
            }

            // Render start of opening tag, before we render all attributes.
            writer.Write (@"<{0} id=""{1}""", Element, ClientID);
            _attributes.Render (writer);

            // Closing opening tag for HTML element.
            writer.Write (">");

            // Returning the number of TAB characters we created due to trying to nicely format the element in the HTML.
            return noTabs;
        }

        /// <summary>
        ///     Renders HTML closing tag of widget.
        /// 
        ///     Override to provide customer rendering for closing tag of widget.
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="noTabs">No tabs.</param>
        protected virtual void RenderTagClosing (HtmlTextWriter writer, int noTabs)
        {
            writer.Write ("</{0}>", Element);
        }

        /*
         * Returns the HTML necessary to render widget on client side.
         */
        private string GetWidgetHtml ()
        {
            // Wrapping an HtmlTextWriter, using a MemoryStream as its base stream, to render widget's content into, 
            // and return as string to caller.
            using (var stream = new MemoryStream ()) {
                using (var txt = new HtmlTextWriter (new StreamWriter (stream))) {
                    RenderHtmlResponse (txt);
                    txt.Flush ();
                }
                stream.Seek (0, SeekOrigin.Begin);
                using (TextReader reader = new StreamReader (stream)) {
                    return reader.ReadToEnd ();
                }
            }
        }

        /*
         * Returns the HTML necessary to render widget as invisible (as an invisible placeholder) on client side.
         */
        private string GetWidgetInvisibleHtml ()
        {
            return string.Format (@"<{0} id=""{1}"" style=""display:none important!;""></{0}>", Element, ClientID);
        }

        /// <summary>
        ///     Overridden to make sure we can persist widget's attributes into ViewState.
        /// </summary>
        /// <returns>The view state.</returns>
        protected override object SaveViewState ()
        {
            var retVal = new object [3];
            retVal [0] = base.SaveViewState ();
            retVal [1] = _attributes.SaveToViewState (this);
            retVal [2] = _attributes.SaveRemovedToViewState ();
            return retVal;
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

        /*
         * Returns true if any of widget's ancestor widgets are re-rendered, or wants to have their children re-rendered, meaning they render as HTML.
         * At which case, this widget should also render as pure HTML into HtmlTextWriter, returning content to client.
         */
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

        /*
         * Hidden implementation of IAttributeAccessor, to keep the "API" for the Widget class as encapsulated as possible.
         */
        string IAttributeAccessor.GetAttribute (string key)
        {
            return _attributes.GetAttribute (key);
        }

        void IAttributeAccessor.SetAttribute (string key, string value)
        {
            _attributes.SetAttributePreViewState (key, value);
        }
    }
}