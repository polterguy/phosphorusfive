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
        ///     Defines how to render the HTML element of your widget.
        /// </summary>
        public enum Rendering
        {
            /// <summary>
            ///     This is for elements that normally require both an opening tag, and a closing tag, such as "div" and "ul".
            /// </summary>
            normal,

            /// <summary>
            ///     This forces the element to close itself immediately, which is meaningful for XHTML renderings, where you wish
            ///     to immediately close the element, such as when using for instance a "br" element, or having an element with no content.
            /// </summary>
            immediate,

            /// <summary>
            ///     This is for elements that does not require a closing element at all, such as when rendering plain HTML (not XHTML),
            ///     and you render for instance a "p" or an "li" element, which doesn't in fact require a closing tag at all.
            /// </summary>
            open
        }

        /// <summary>
        ///     Defines how the widget is supposed to be rendered during the current request.
        /// </summary>
        protected enum RenderingMode
        {
            /// <summary>
            ///     The default value.
            /// </summary>
            Default,

            /// <summary>
            ///     Re-rendering mode, the entire widget will be re-rendered back to the client as HTML.
            /// </summary>
            ReRender,

            /// <summary>
            ///     The children collection of widget has been modified, which means we'll need to take care of rendering deleted widgets, and added
            ///     widgets back to the client.
            /// </summary>
            ChildrenCollectionModified,

            /// <summary>
            ///     Renders the widget as invisible, which means it'll render without any of its attributes or content, and have
            ///     a style property with "display:none !important", to simply serve as a placeholder for widget, as it later 
            ///     becomes visible.
            /// </summary>
            RenderInvisible
        }

        // Contains all attributes for widget.
        private readonly AttributeStorage _attributes = new AttributeStorage ();

        // Contains a reference to the AjaxPage owning this widget.
        private AjaxPage _page;

        // Used to hold the old ID of widget, in the rare case, that its ID is somehow changed.
        // Necessary to make sure we are able to retrieve widget, and update its ID on the client side, during Ajax requests.
        private string _oldId;

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
                if (_page == null)
                    _page = Page as AjaxPage;
                return _page;
            }
        }

        /// <summary>
        ///     Overridden to make it possible to change an element's ID.
        /// </summary>
        /// <value>The new ID</value>
        public override string ID {
            get { return base.ID; }
            set {
                // Storing old ID of element, since this is the stuff that'll be rendered over the wire,
                // to allow for retrieving the element on the client side, and change its ID on the client side.
                if (IsTrackingViewState && value != base.ID) {

                    // Notice, we only keep the "first original ID", in case ID is changed multiple times during page's life cycle.
                    if (_oldId == null)
                        _oldId = base.ClientID;
                    _attributes.ChangeAttribute ("id", value);
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
        public override bool Visible {
            get { return base.Visible; }
            set {
                if (value == base.Visible)
                    return; // Nothing to do here ...

                if (!base.Visible && value && IsTrackingViewState && AjaxPage.IsAjaxRequest) {

                    // This widget was made visible during this request, and should be re-rendered as HTML.
                    RenderMode = RenderingMode.ReRender;

                } else if (base.Visible && !value && IsTrackingViewState && AjaxPage.IsAjaxRequest) {

                    // This widget was made invisible during this request, and should be re-rendered as invisible.
                    RenderMode = RenderingMode.RenderInvisible;
                }
                base.Visible = value;
            }
        }

        /// <summary>
        ///     Gets or sets how to render the widget.
        /// 
        ///     Legal values are; "normal", "immediate" and "open".
        ///     "normal" means your widget will render with an opening tag, and a closing tag.
        ///     "immediate" means your widget's tag will immediately close, which is useful for XHTML pages for instance.
        ///     "open" is useful for HTML elements that doesn't require a closing tag, such as "p" or "li", but only in HTML (non XHTML pages).
        /// </summary>
        /// <value>The render type</value>
        public Rendering RenderAs {
            get { return (Rendering)(ViewState ["rt"] ?? Rendering.normal); }
            set { ViewState ["rt"] = value; }
        }

        /// <summary>
        ///     Gets or sets the element type used to render your widget.
        /// 
        ///     Set this value to whatever HTML element name you wish for your widget to render as, e.g. "p", "div", "span", etc.
        /// </summary>
        /// <value>The HTML element used to render widget</value>
        public virtual string Element {
            get { return _attributes.GetAttribute ("Element"); }
            set {

                // If Element name is set to "null", we entirely remove attribute, to let default value of sub classes kick in, and do its magic.
                if (value == null) {

                    // Deletion of Element, which means overridden defaults in sub classes will hopefully return something sane.
                    DeleteAttribute ("Element");

                } else {

                    // Sanity check, before figuring out what to do with new value for Element property.
                    SanitizeElementValue (value);

                    // Setting the element name, depending upon if we're serializing changes or not.
                    if (!IsTrackingViewState)
                        _attributes.SetAttributePreViewState ("Element", value);
                    else
                        _attributes.ChangeAttribute ("Element", value);
                }
            }
        }

        /// <summary>
        ///     Gets or sets an attribute value for your widget.
        /// 
        ///     Notice, to remove an attribute, please use the "DeleteAttribute" method, since even though you set an attribute's
        ///     value to "null", it will still exist on widget, as an "empty attribute".
        /// </summary>
        /// <param name="name">Name of attribute to retrieve or set value of.</param>
        public virtual string this [string name] {
            get { return _attributes.GetAttribute (name); }
            set {

                // Notice, we store the attribute differently, depending upon whether or not we have started tracking ViewState or not.
                // This is necessarily due to making sure we're able to track changes to attributes, and correctly pass them on to the client.
                if (!IsTrackingViewState)
                    _attributes.SetAttributePreViewState (name, value);
                else
                    _attributes.ChangeAttribute (name, value);
            }
        }

        /// <summary>
        ///     Deletes the specified attribute from widget entirely.
        /// </summary>
        /// <param name="name">Name of attribute you wish to delete.</param>
        public void DeleteAttribute (string name)
        {
            if (HasAttribute (name))
                _attributes.RemoveAttribute (name);
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
        ///     Returns all attribute names for widget.
        /// </summary>
        /// <value>All attribute keys for widget</value>
        public IEnumerable<string> AttributeKeys {
            get {
                return _attributes.Keys;
            }
        }

        /// <summary>
        ///     Forces a re-rendering of your widget.
        /// </summary>
        public void ReRender ()
        {
            RenderMode = RenderingMode.ReRender;
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
            var eventHandlerName = eventName; // Defaulting to event name for WebMethod invocations from JavaScript
            if (HasAttribute (eventName)) {

                // Probably "onclick" or other types of automatically generated mapping between server method and javascript handler
                eventHandlerName = this [eventName];
            }

            // Finding out at what context to invoke the method within
            var owner = Parent;
            while (!(owner is UserControl) && !(owner is Page))
                owner = owner.Parent;

            // Retrieving the method
            var method = owner.GetType ().GetMethod (eventHandlerName,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.FlattenHierarchy);
            if (method == null)
                throw new NotImplementedException ("Method + '" + eventHandlerName + "' could not be found");

            // Verifying method has the WebMethod attribute
            var atrs = method.GetCustomAttributes (typeof (WebMethod), false /* for security reasons we want method to be explicitly marked as WebMethod */);
            if (atrs == null || atrs.Length == 0)
                throw new AccessViolationException ("Method + '" + eventHandlerName + "' is illegal to invoke over http");

            // Invoking methods with the "this" widget and empty event args
            method.Invoke (owner, new object [] { this, new AjaxEventArgs (eventName) });
        }

        /// <summary>
        ///     Renders the widget.
        /// 
        ///     Overridden to entirely bypass the ASP.NET Web Forms rendering, and provide our own with Ajax support.
        /// </summary>
        /// <param name="writer">Writer.</param>
        public override void RenderControl (HtmlTextWriter writer)
        {
            // If ancestor is invisible, we do not render this control at all.
            if (AreAncestorsVisible ()) {

                // Figuring out if one of this widget's ancestor widgets are in "re-rendering mode", at which case we render the HTML.
                var ancestorReRendering = AncestorIsReRendering ();
                if (Visible) {
                    if (AjaxPage.IsAjaxRequest && !ancestorReRendering) {
                        if (RenderMode == RenderingMode.ReRender) {

                            // Re-rendering entire widget.
                            // Notice, since GetWidgetHtml will return HTML for widget, and its children controls, it's not necessary to invoke
                            // "RenderChildren" or any similar methods here.
                            AjaxPage.RegisterWidgetChanges (_oldId ?? ClientID, "outerHTML", GetWidgetHtml ());

                        } else if (RenderMode == RenderingMode.ChildrenCollectionModified) {

                            // Re-rendering all children controls, but also renders changes to widget.
                            _attributes.RegisterChanges (AjaxPage, _oldId ?? ClientID);
                            RenderChildrenWidgetsAsJson (writer);

                        } else {

                            // Only pass changes back to client as JSON.
                            _attributes.RegisterChanges (AjaxPage, _oldId ?? ClientID);
                            RenderChildren (writer);
                        }
                    } else {

                        // Not an Ajax request, or ancestors are re-rendering widget somehow.
                        RenderHtmlResponse (writer);
                    }
                } else {

                    // Invisible widget.
                    if (AjaxPage.IsAjaxRequest && RenderMode == RenderingMode.RenderInvisible && !ancestorReRendering) {

                        // Re-rendering widget's invisible markup.
                        // Widget was probably made invisible during the current request.
                        AjaxPage.RegisterWidgetChanges (_oldId ?? ClientID, "outerHTML", GetWidgetInvisibleHtml ());

                    } else if (!AjaxPage.IsAjaxRequest || ancestorReRendering) {

                        // Rendering invisible HTML.
                        writer.Write (GetWidgetInvisibleHtml ());
                    }
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance has content or not.
        /// 
        ///     This property is used to determine how a widget should be rendered, if its rendering mode is set to "immediate", and if its children should
        ///     be rendered or not.
        ///     Method is abstract, and must be overridden in derived classes.
        /// </summary>
        /// <value><c>true</c> if this instance has content; otherwise, <c>false</c></value>
        protected abstract bool HasContent {
            get;
        }

        /// <summary>
        ///     Verifies the Element name is legal for the current widget.
        /// 
        ///     Override this method in your own classes, to provide further restrictions. But please, call base class method.
        /// </summary>
        /// <param name="elementName">The new Element name.</param>
        protected virtual void SanitizeElementValue (string elementName)
        {
            // Making sure Element is not empty string "".
            if (elementName == "")
                throw new ArgumentException ("Sorry, but you must provide either an actual value, or 'null', as the Element name for your widget", nameof (Element));

            // Making sure Element does not contain other non-legal characters.
            if (elementName.Any (ix => !"abcdefghijklmnopqrstuvwxyz123456".Contains (ix)))
                throw new ArgumentException ("Sorry, but p5.ajax doesn't like these types of characters for its Element names", nameof (Element));
        }

        /// <summary>
        ///     Gets or sets the rendering mode for the widget.
        /// 
        ///     Allows you to explicitly force to have your widget re-rendered, or re-rendering its children, if you wish.
        /// </summary>
        /// <value>The render mode</value>
        protected RenderingMode RenderMode {
            get;
            set;
        }

        /// <summary>
        ///     Invoked when Controls collection of widget has been updated.
        /// </summary>
        protected void ChildrenCollectionIsModified ()
        {
            // Notice, if the widget is re-rendered entirely, there is no need to mark the children collection as changed, since widget will be entirely
            // re-rendered anyway, with all of its children also re-rendered.
            if (RenderMode != RenderingMode.ReRender)
                RenderMode = RenderingMode.ChildrenCollectionModified;
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
            // Sanity check.
            if (AjaxPage == null)
                throw new ApplicationException ("Oops, make sure you inherit your page from AjaxPage somehow.");

            if (Page.IsPostBack)
                LoadFormData ();

            // Making sure event handlers are being processed
            if (AjaxPage.IsAjaxRequest) {
                if (Page.Request.Params ["_p5_widget"] == ClientID) {
                    Page.LoadComplete += delegate {

                        // Event was raised for this widget
                        InvokeEventHandler (Page.Request.Params ["_p5_event"]);
                    };
                }
            }
            base.OnLoad (e);
        }

        /// <summary>
        ///     Renders all children as JSON update(s) back to client.
        /// 
        ///     Override this one to provide custom rendering.
        /// </summary>
        protected virtual void RenderChildrenWidgetsAsJson (HtmlTextWriter writer)
        {
            // re-rendering all children by default
            AjaxPage.RegisterWidgetChanges (ClientID, "innerValue", GetInnerHTML ());
        }

        /// <summary>
        ///     Renders widget's content as pure HTML into specified HtmlTextWriter.
        /// 
        ///     Override this one to provide custom rendering.
        ///     Notice, you can also override one of the other rendering methods, if you only wish to slightly modify the widget's rendering.
        /// </summary>
        /// <param name="writer">The HtmlTextWriter to render the widget into.</param>
        protected virtual void RenderHtmlResponse (HtmlTextWriter writer)
        {
            // Rendering opening tag for element.
            var noTabs = RenderTagOpening (writer);

            // Rendering widget's content (children or innerValue), but only if element had any content.
            if (HasContent)
                RenderChildren (writer);

            // Closing element, but only if element is rendered in "normal" mode.
            if (RenderAs == Rendering.normal)
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

            // Closing opening tag for HTML element, which depends upon whether or not widget has content and its rendering mode.
            if (!HasContent && RenderAs == Rendering.immediate)
                writer.Write (" />"); // No content, and tag should be closed immediately.
            else
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
         * Returns the widget's children as HTML.
         */
        private string GetInnerHTML ()
        {
            // Wrapping an HtmlTextWriter, using a MemoryStream as its base stream, to render widget's children into, 
            // and return as string to caller.
            using (var stream = new MemoryStream ()) {
                using (var txt = new HtmlTextWriter (new StreamWriter (stream))) {
                    RenderChildren (txt);
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
        private bool AncestorIsReRendering ()
        {
            // Returns true if any of its ancestors are rendering as HTML.
            var idx = Parent;
            while (idx != null) {
                var wdg = idx as Widget;
                if (wdg != null && (wdg.RenderMode == RenderingMode.ReRender || wdg.RenderMode == RenderingMode.ChildrenCollectionModified))
                    return true;
                idx = idx.Parent;
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