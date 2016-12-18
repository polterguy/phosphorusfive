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
    ///     Abstract base class for all widgets in p5.ajax.
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
            /// <param name="name">Name</param>
            public AjaxEventArgs (string name)
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
        public enum RenderingType
        {
            /// <summary>
            ///     This is for elements that require both an opening element, and a closing element, such as "div" and "ul".
            /// </summary>
            normal,

            /// <summary>
            ///     This forces the element to close itself immediately, which is meaningful for XHTML renderings, where you wish
            ///     to immediately close the element, such as when using for instance a "br" element.
            /// </summary>
            immediate,

            /// <summary>
            ///     This is for elements that does not require a closing element at all, such as when rendering plain HTML (not XHTML),
            ///     and you render for instance a "p" or an "li" element, which doesn't in fact even require a closing element at all.
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
            ///     The children collection of widget has been modified, which means we'll need to take care of rendering deleted widgets and added
            ///     widgets back to the client.
            /// </summary>
            ChildrenCollectionModified,

            /// <summary>
            ///     Renders the widget in as invisible, which means it'll render without any of its attributes or content, and have
            ///     a style property with "display:none !important" to simply serve as a placeholder for widget as it later possibly
            ///     becomes visible.
            /// </summary>
            RenderInvisible
        }

        // Contains all attributes for widget.
        private readonly AttributeStorage _attributes = new AttributeStorage ();

        // Contains a reference to the AjaxPage owning this widget.
        private AjaxPage _page;

        // Used to hold the old ID of widget, in the rare case that its ID is somehow updated.
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
        protected AjaxPage AjaxPage
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
        public override string ID
        {
            get {
                return base.ID;
            }
            set {
                // Storing old ID of element, since this is the stuff that'll be rendered over the wire,
                // to allow for retrieving the element on the client side.
                if (IsTrackingViewState) {
                    if (value != base.ID) {

                        // Notice, we only keep the "original ID", in case ID is changed multiple times during page's life cycle.
                        if (_oldId == null)
                            _oldId = base.ClientID;
                        _attributes.ChangeAttribute ("id", value);
                    }
                }
                base.ID = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="T:p5.ajax.widgets.Widget"/> is visible.
        ///     Overridden from base, to make sure we can automatically determine rendering mode for widget, during changes to visibility.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public override bool Visible
        {
            get { return base.Visible; }
            set {
                if (value == base.Visible)
                    return; // Nothing to do here ...

                if (!base.Visible && value && IsTrackingViewState && AjaxPage.IsAjaxRequest) {

                    // This widget was made visible during this request, and should be re-rendered as HTML, 
                    // unless any of its ancestors are invisible.
                    RenderMode = RenderingMode.ReRender;

                } else if (base.Visible && !value && IsTrackingViewState && AjaxPage.IsAjaxRequest) {

                    // This widget was made invisible during this request and should be re-rendered as invisible,
                    // unless any of its ancestors are invisible.
                    RenderMode = RenderingMode.RenderInvisible;
                }
                base.Visible = value;
            }
        }

        /// <summary>
        ///     Gets or sets the element type used to render your widget.
        /// 
        ///     Set this value to whatever HTML element name you wish having your widget rendered as, e.g. "p", "div", "span", etc.
        /// </summary>
        /// <value>The HTML element used to render widget</value>
        public virtual string Element
        {
            get { return this ["Element"]; }
            set {
                if (value != null && value.ToLower () != value)
                    throw new ArgumentException ("p5.ajax doesn't like uppercase element names", "value");
                if (value == null)
                    this.DeleteAttribute ("Element");
                this ["Element"] = value;
            }
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
        ///     Gets or sets how to render the widget.
        /// 
        ///     Legal values are; "normal", "immediate" and "open".
        ///     "normal" means your widget will render with an opening tag, and a closing tag.
        ///     "immediate" means your widget's tag will immediately close, which is useful for XHTML renderings for instance.
        ///     "open" is useful for HTML elements that doesn't require a closing tag, such as "p" or "li".
        /// </summary>
        /// <value>The render type</value>
        public RenderingType RenderType
        {
            get { return (RenderingType) (ViewState ["rt"] ?? RenderingType.normal); }
            set { ViewState ["rt"] = value; }
        }

        /// <summary>
        ///     Gets or sets an attribute value for your widget.
        /// 
        ///     Notice, to remove an attribute, please use the "DeleteAttribute" method, since even though you set an attribute's
        ///     value to "null", it will still exist on widget, as an "empty attribute".
        /// </summary>
        /// <param name="name">Name of attribute to retrieve or set value of.</param>
        public virtual string this [string name]
        {
            get { return _attributes.GetAttribute (name); }
            set {

                // Special handling for "selected" attribute for "option" elements, to make sure
                // we remove the selected attribute on any sibling "option" elements.
                if (Element == "option" && name == "selected") {
                    foreach (Widget idxWidget in Parent.Controls) {
                        if (idxWidget != this && idxWidget.HasAttribute ("selected"))
                            idxWidget.DeleteAttribute ("selected");
                    }

                    // Due to a bug in the way browsers handles the "selected" property on "option" elements, we need to re-render all
                    // select widgets, every time an "option" element's "selected" attribute is changed.
                    // Read more here; https://bugs.chromium.org/p/chromium/issues/detail?id=662669
                    (Parent as Widget).ReRender ();
                }
                if (!IsTrackingViewState) {
                    _attributes.SetAttributePreViewState (name, value);
                } else {
                    _attributes.ChangeAttribute (name, value);
                }
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

        /*
         * Forces a re-rendering of your widget's children controls.
         */
        protected void ChildrenCollectionIsModified ()
        {
            // Notice, if the widget is re-rendered entirely, there is no need to mark the children collection as changed, since widget will be entirely
            // re-rendered anyway, with all of its children also re-rendered.
            if (RenderMode != RenderingMode.ReRender)
                RenderMode = RenderingMode.ChildrenCollectionModified;
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
            if (AreAncestorsVisible ()) {
                var ancestorReRendering = IsAncestorReRenderingThisWidget ();
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
        ///     This property is used to determine how a widget should be rendered, if its rendering mode is set to "immediate".
        ///     Abstract, must be overridden in derived classes.
        /// </summary>
        /// <value><c>true</c> if this instance has content; otherwise, <c>false</c></value>
        protected abstract bool HasContent {
            get;
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
        /// </summary>
        protected virtual void RenderChildrenWidgetsAsJson (HtmlTextWriter writer)
        {
            // re-rendering all children by default
            AjaxPage.RegisterWidgetChanges (ClientID, "innerValue", GetChildrenHtml ());
        }

        /// <summary>
        ///     Overridden to make sure we can correctly handle additions of "option" elements to "select" HTML elements.
        /// 
        ///     This is necessary to make sure we can correctly keep track of the "selected" property/attribute on the client side, due to some
        ///     "funny" behavior in browsers' way of handling these things.
        /// </summary>
        /// <param name="control">Control.</param>
        /// <param name="index">Index.</param>
        protected override void AddedControl (Control control, int index)
        {
            // Due to a bug in the way browsers handles the "selected" property on "option" elements, we need to re-render all
            // select widgets, every time the "option" collection is changed.
            // Read more here; https://bugs.chromium.org/p/chromium/issues/detail?id=662669
            if (IsTrackingViewState && Element == "select") {

                // Making sure control added as a Widget, and that it has the "selected" attribute.
                var curWidget = control as Widget;
                if (curWidget.HasAttribute ("selected")) {
                    foreach (Widget idxWidget in Controls) {

                        // Checking if currently iterated widget contains the "selected" attribute.
                        if (idxWidget != null && idxWidget.HasAttribute ("selected")) {

                            // Removing the "selected" attribute from previously selected option element.
                            idxWidget.DeleteAttribute ("selected");
                        }
                    }
                }

                // Since insertion of "option" elements, with the "selected" attribute set, does not behave correctly in browser, according
                // to; https://bugs.chromium.org/p/chromium/issues/detail?id=662669
                // We need to resort to partial (re) rendering of entire "select" element here ...
                ReRender ();
            }
            base.AddedControl (control, index);
        }

        /// <summary>
        ///     Overridden to make sure we re-render "select" HTML elements in Ajax callbacks when an "option" element is deleted, 
        ///     due to a "bug" (or weird behavior to be more accurate) in browsers.
        /// </summary>
        /// <param name="control">Control.</param>
        protected override void RemovedControl (Control control)
        {
            // Due to a "bug" (or unexpected behavior) in the way browsers handles the "selected" property on "option" elements, we need to re-render all
            // select widgets, every time the "option" collection is changed.
            // Read more here; https://bugs.chromium.org/p/chromium/issues/detail?id=662669
            if (IsTrackingViewState && Element == "select") {

                // Since removal of "option" elements, with the "selected" attribute set, does not behave correctly in browser, according
                // to; https://bugs.chromium.org/p/chromium/issues/detail?id=662669
                // We need to resort to partial (re) rendering of entire "select" element here ...
                ReRender ();
            }
            base.RemovedControl (control);
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
            // Wrapping an HtmlTextWriter, using a MemoryStream as its base stream, to render widget's content into, and return as string to caller.
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
        private string GetChildrenHtml ()
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
        public bool AreAncestorsVisible ()
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
        private bool IsAncestorReRenderingThisWidget ()
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
         * Renders widget's content as pure HTML into specified HtmlTextWriter.
         */
        private void RenderHtmlResponse (HtmlTextWriter writer)
        {
            // Making sure we nicely indent widget if this is a normal HTTP POST or GET request.
            var noTabs = 0;
            if (!AjaxPage.IsAjaxRequest) {
                writer.Write ("\r\n\t");
                noTabs = 1;
                Control idxCtrl = this;
                while (idxCtrl != Page) {
                    writer.Write ("\t");
                    idxCtrl = idxCtrl.Parent;
                    noTabs += 1;
                }
            }

            // Render opening tag.
            // Notice, we do not render ID of widget if this is an "option" element.
            if (Element == "option") {

                // We do NOT render IDs of "option" elements, since there are few intelligent reasons why
                // you would want to de-reference them, and if you change the "select" controls collection in any ways, you
                // will anyways trigger a re-rendering of the entire parent "select" widget.
                writer.Write (@"<{0}", Element);

            } else {

                // Default rendering, passing in the ID of widget.
                writer.Write (@"<{0} id=""{1}""", Element, ClientID);
            }

            // Render attributes.
            _attributes.Render (writer);

            // Checking if widget has any content, and if not, and widget is in "immediate" mode, we simply close tag immediately.
            if (HasContent) {
                writer.Write (">");

                // Rendering children widgets, before we determine how, and if, to close widget's HTML.
                // Literal widgets are closed on the same line, if closed.
                // Container widgets are "nicely formatted".
                RenderChildren (writer);
                if (RenderType != RenderingType.open) {
                    if (this is Container) {

                        // Making sure we nicely format end tag for widget.
                        writer.Write ("\r\n");
                        while (noTabs != 0) {
                            writer.Write ("\t");
                            noTabs -= 1;
                        }
                    }
                    writer.Write ("</{0}>", Element);
                }
            } else {

                // No content in widget, figuring out how to close widget's HTML.
                switch (RenderType) {
                    case RenderingType.immediate:
                        writer.Write (" />");
                        break;
                    case RenderingType.open:
                        writer.Write (">");
                        break;
                    case RenderingType.normal:
                        writer.Write ("></{0}>", Element);
                        break;
                }
            }
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