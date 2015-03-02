/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Reflection;
using System.Web.UI;
using phosphorus.ajax.core;
using phosphorus.ajax.core.internals;

// ReSharper disable PossibleNullReferenceException
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace phosphorus.ajax.widgets
{
    /// <summary>
    ///     general ajax html element
    /// </summary>
    [ViewStateModeById]
    public abstract class Widget : Control, IAttributeAccessor
    {
        /// <summary>
        ///     rules for how to render the tag
        /// </summary>
        public enum RenderingType
        {
            /// <summary>
            ///     this is for elements that require both an opening element, and a closing element, such as "div" and "ul"
            /// </summary>
            Default,

            /// <summary>
            ///     this forces the element to close itself, even when there is no content, which means it will be rendered with a
            ///     slash (/) just
            ///     before the greater-than angle-bracket of the opening element. this creates xhtml compliant rendering for you on
            ///     your page. examples
            ///     of element that requires this type of rendering are "input" and "br", but only if you wish to follow xhtml
            ///     practices
            /// </summary>
            SelfClosing,

            /// <summary>
            ///     this is for elements that does not require a closing element. examples of elements that should be rendered
            ///     with this type are "p", "li", "input" and "br"
            /// </summary>
            NoClose
        }

        // contains all attributes of widget
        private readonly AttributeStorage _attributes = new AttributeStorage ();

        // how to render the widget. normally this is automatically determined, but sometimes it needs to be overridden explicitly
        protected RenderingMode RenderMode = RenderingMode.Default;

        /// <summary>
        ///     gets or sets the element type used to render the html element such as "p", "div", "ul" etc
        /// </summary>
        /// <value>the tag name</value>
        public virtual string ElementType
        {
            get { return this ["Tag"]; }
            set
            {
                if (value.ToLower () != value)
                    throw new ArgumentException ("phosphorus.ajax doesn't like uppercase element names", "value");
                this ["Tag"] = value;
            }
        }

        /// <summary>
        ///     gets or sets the tag name used to render the html element when it is invisible. this is sometimes useful since the
        ///     default
        ///     tag rendered when widget is invisible is a span tag, which is not necessarily compatible with the position in the
        ///     dom you're
        ///     rendering it. for instance, if you have a "ul" tag or widget, which has an invisible "li" widget, then rendering a
        ///     span
        ///     tag as a child of a "ul" is illegal according to the html standard. in such circumstances you must change the
        ///     invisible
        ///     tag rendered to become an "li" element
        /// </summary>
        /// <value>the tag name</value>
        public string InvisibleElement
        {
            get { return ViewState ["ie"] == null ? "span" : ViewState ["ie"] as string; }
            set { ViewState ["ie"] = value; }
        }

        /// <summary>
        ///     gets or sets the rendering type of the element, such as whether or not the element is self-closed, has an end
        ///     element, and so on
        /// </summary>
        /// <value>the rendering type of the element</value>
        public RenderingType RenderType
        {
            get { return (RenderingType) (ViewState ["rt"] ?? RenderingType.Default); }
            set { ViewState ["rt"] = value; }
        }

        /// <summary>
        ///     if this property is true, then no ID will be rendered back to the client for the given Widget, which is useful for
        ///     for instance option widgets, and similar constructs, where the id attribute is not supposed to be rendered to the
        ///     client
        /// </summary>
        /// <value>the tag name</value>
        public bool NoIdAttribute
        {
            get { return ViewState ["noid"] != null && (bool) ViewState ["noid"]; }
            set { ViewState ["noid"] = value; }
        }

        /// <summary>
        ///     gets or sets the named attribute for the widget. notice that attribute might exist, even if
        ///     return value is null, since attributes can have "null values", such as for instance "controls"
        ///     for the html5 video element, or the "disabled" attribute on form elements. if you wish to
        ///     check for the existence of an attribute, then use <see cref="phosphorus.ajax.widgets.Widget.HasAttribute" />.
        ///     if you wish to remove an attribute, use the <see cref="phosphorus.ajax.widgets.Widget.RemoveAttribute" />
        /// </summary>
        /// <param name="name">attribute to retrieve or set</param>
        public virtual string this [string name]
        {
            get { return _attributes.GetAttribute (name); }
            set {
                if (!IsTrackingViewState) {
                    _attributes.SetAttributePreViewState (name, value);
                } else {
                    _attributes.ChangeAttribute (name, value);
                }
            }
        }

        /// <summary>
        ///     gets a value indicating whether this instance has content or not
        /// </summary>
        /// <value><c>true</c> if this instance has content; otherwise, <c>false</c></value>
        protected abstract bool HasContent { get; }

        // overridden asp.net properties and methods

        public override bool Visible
        {
            get { return base.Visible; }
            set
            {
                if (!base.Visible && value && IsTrackingViewState && IsPhosphorusRequest) {
                    // this control was made visible during this request and should be rendered as html
                    // unless any of its ancestors are invisible
                    RenderMode = RenderingMode.ReRender;
                } else if (base.Visible && !value && IsTrackingViewState && IsPhosphorusRequest) {
                    // this control was made invisible during this request and should be rendered 
                    // with its invisible html, unless any of its ancestors are invisible
                    RenderMode = RenderingMode.RenderInvisible;
                }
                base.Visible = value;
            }
        }

        private bool IsPhosphorusRequest
        {
            get { return !string.IsNullOrEmpty (Page.Request.Params ["__pf_event"]); }
        }

        string IAttributeAccessor.GetAttribute (string key) { return _attributes.GetAttribute (key); }
        void IAttributeAccessor.SetAttribute (string key, string value) { _attributes.SetAttributePreViewState (key, value); }

        /// <summary>
        ///     determines whether this instance has an attribute aith the specified naame
        /// </summary>
        /// <returns><c>true</c> if this instance has the attribute with the specified name; otherwise, <c>false</c></returns>
        /// <param name="name">name of attribute to check for existence of</param>
        public virtual bool HasAttribute (string name)
        {
            return _attributes.HasAttribute (name);
        }

        /// <summary>
        ///     removes an attribute
        /// </summary>
        /// <param name="name">name of attribute to remove</param>
        public void RemoveAttribute (string name) { _attributes.RemoveAttribute (name); }

        /// <summary>
        ///     forces a re-rendering of the widget. normally this is not something you should have to mess with yourself, but
        ///     something the
        ///     framework itself will take care of. however, if you wish to force the control to re-render itself entirely as html
        ///     back to
        ///     the client, you can call this method
        /// </summary>
        public void ReRender () { RenderMode = RenderingMode.ReRender; }

        /// <summary>
        ///     forces a re-rendering of the widget's children. normally this is not something you should have to mess with
        ///     yourself, but
        ///     something the framework itself will take care of. however, if you wish to force the control to re-render its
        ///     children or content
        ///     as html back to the client, you can call this method
        /// </summary>
        public void ReRenderChildren ()
        {
            if (RenderMode != RenderingMode.ReRender)
                RenderMode = RenderingMode.ReRenderChildren;
        }

        /// <summary>
        ///     loads the form data from the http request, override this in your own widgets if you
        ///     have widgets that posts data to the server
        /// </summary>
        protected virtual void LoadFormData ()
        {
            if (this ["disabled"] == null) {
                if (!string.IsNullOrEmpty (this ["name"]) || ElementType == "option") {
                    switch (ElementType) {
                        case "input":
                            switch (this ["type"]) {
                                case "radio":
                                case "checkbox":
                                    if (Page.Request.Params [this ["name"]] != null) {
                                        var splits = Page.Request.Params [this ["name"]].Split (',');
                                        bool found = false;
                                        foreach (var idxSplit in splits) {
                                            if (idxSplit == this ["value"]) {
                                                found = true;
                                                break;
                                            }
                                        }
                                        if (found) {
                                            _attributes.SetAttributeFormData ("checked", null);
                                        } else {
                                            _attributes.RemoveAttribute ("checked");
                                        }
                                    } break;
                                default:
                                    _attributes.SetAttributeFormData ("value", Page.Request.Params [this ["name"]]);
                                    break;
                            }
                            break;
                        case "textarea":
                            _attributes.SetAttributeFormData ("innerValue", Page.Request.Params [this ["name"]]);
                            break;
                        case "option":
                            if (Page.Request.Params [(this.Parent as Widget) ["name"]] != null) {
                                var splits = Page.Request.Params [(this.Parent as Widget) ["name"]].Split (',');
                                bool found = false;
                                foreach (var idxSplit in splits) {
                                    if (idxSplit == this ["value"]) {
                                        found = true;
                                        break;
                                    }
                                }
                                if (found) {
                                    _attributes.SetAttributeFormData ("selected", null);
                                } else {
                                    _attributes.RemoveAttribute ("selected");
                                }
                            } break;
                    }
                }
            }
        }

        /// <summary>
        ///     invokes the given event handler. if the widget has an attribute with the 'eventName', then the value
        ///     of that attribute will be retrieved, and a method on the page, usercontrol or master page the control belongs
        ///     to will be expected to contains a method with that name. if the widget does not have an attribute with the
        ///     'eventName' name, then a method with the name of 'eventName' will be invoked searching through the page,
        ///     usercontrol or master page the control belongs to, in that order. all methods invoked this way must have
        ///     the <see cref="phosphorus.ajax.core.WebMethod" /> attribute. if you override this method, please call base
        ///     if you do not recognize the 'eventName'
        /// </summary>
        /// <param name="eventName">event name such as 'onclick', or name of c# method on page, usercontrol or masterpage</param>
        /// <exception cref="NotImplementedException"></exception>
        protected void InvokeEventHandler (string eventName)
        {
            var eventHandlerName = eventName; // defaulting to event name for WebMethod invocations from JavaScript
            if (HasAttribute (eventName)) {
                // probably "onclick" or other types of automatically generated mapping between server method and javascript handler
                eventHandlerName = this [eventName];
            }

            // finding out at what context to invoke the method within
            var owner = Parent;
            while (!(owner is UserControl) && !(owner is Page))
                owner = owner.Parent;

            // retrieving the method
            var method = owner.GetType ().GetMethod (eventHandlerName,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.FlattenHierarchy);
            if (method == null)
                throw new NotImplementedException ("method + '" + eventHandlerName + "' could not be found");

            // verifying method has the WebMethod attribute
            var atrs = method.GetCustomAttributes (typeof (WebMethod), false /* for security reasons we want method to be explicitly marked as WebMethod */);
            if (atrs == null || atrs.Length == 0)
                throw new AccessViolationException ("method + '" + eventHandlerName + "' is illegal to invoke over http");

            // invoking methods with the "this" widget and empty event args
            method.Invoke (owner, new object[] {this, new AjaxEventArgs (eventName)});
        }

        /// <summary>
        ///     renders all children as json update to be sent back to client. override this one if you wish
        ///     to create custom functionality as an alternative
        /// </summary>
        protected virtual void RenderChildrenWidgetsAsJson (HtmlTextWriter writer)
        {
            // re-rendering all children by default
            (Page as IAjaxPage).Manager.RegisterWidgetChanges (ClientID, "innerValue", GetChildrenHtml ());
        }

        public override void RenderControl (HtmlTextWriter writer)
        {
            if (AreAncestorsVisible ()) {
                var ancestorReRendering = IsAncestorRendering ();
                if (Visible) {
                    if (IsPhosphorusRequest && !ancestorReRendering) {
                        if (RenderMode == RenderingMode.ReRender) {
                            // re-rendering entire widget
                            (Page as IAjaxPage).Manager.RegisterWidgetChanges (ClientID, "outerHTML", GetWidgetHtml ());
                        } else if (RenderMode == RenderingMode.ReRenderChildren) {
                            // re-rendering all children controls
                            RenderChildrenWidgetsAsJson (writer);
                        } else {
                            // only pass changes back to client as json
                            _attributes.RegisterChanges ((Page as IAjaxPage).Manager, ClientID);
                            RenderChildren (writer);
                        }
                    } else {
                        // not ajax request, or ancestors are re-rendering
                        RenderHtmlResponse (writer);
                    }
                } else {
                    // invisible widget
                    if (IsPhosphorusRequest && RenderMode == RenderingMode.RenderInvisible && !ancestorReRendering) {
                        // re-rendering widget's invisible markup
                        (Page as IAjaxPage).Manager.RegisterWidgetChanges (ClientID, "outerHTML", GetWidgetInvisibleHtml ());
                    } else if (!IsPhosphorusRequest || ancestorReRendering) {
                        // rendering invisible markup
                        writer.Write (GetWidgetInvisibleHtml ());
                    } // else, nothing to render since widget is in-visible, and this was an ajaxx request
                }
            }
        }

        protected override void LoadViewState (object savedState)
        {
            var tmp = savedState as object[];
            base.LoadViewState (tmp [0]);
            _attributes.LoadFromViewState (tmp [1]);
            _attributes.LoadRemovedFromViewState (tmp [2]);
        }

        protected override object SaveViewState ()
        {
            var retVal = new object[3];
            retVal [0] = base.SaveViewState ();
            retVal [1] = _attributes.SaveToViewState ();
            retVal [2] = _attributes.SaveRemovedToViewState ();
            return retVal;
        }

        protected override void OnLoad (EventArgs e)
        {
            if (Page.IsPostBack)
                LoadFormData ();

            // making sure event handlers are being processed
            if (IsPhosphorusRequest) {
                if (Page.Request.Params ["__pf_widget"] == ClientID) {
                    Page.LoadComplete += delegate {
                        // event was raised for this widget
                        InvokeEventHandler (Page.Request.Params ["__pf_event"]);
                    };
                }
            }

            base.OnLoad (e);
        }

        // private methods for internal use in the class

        private string GetWidgetHtml ()
        {
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

        private string GetChildrenHtml ()
        {
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

        private string GetWidgetInvisibleHtml () { return string.Format (@"<{0} id=""{1}"" style=""display:none important!;""></{0}>", InvisibleElement, ClientID); }

        private bool AreAncestorsVisible ()
        {
            // if this control and all of its ancestors are visible, then return true, else false
            var idx = Parent;
            while (true) {
                if (!idx.Visible)
                    break;
                idx = idx.Parent;
                if (idx == null) // we traversed all the way up to beyond the Page, and found no in-visible ancestor controls
                    return true;
            }

            // in-visible control among ancestors was found!
            return false;
        }

        private bool IsAncestorRendering ()
        {
            // returns true if any of its ancestors are rendering html
            var idx = Parent;
            while (idx != null) {
                var wdg = idx as Widget;
                if (wdg != null && (wdg.RenderMode == RenderingMode.ReRender || wdg.RenderMode == RenderingMode.ReRenderChildren))
                    return true;
                idx = idx.Parent;
            }
            return false;
        }

        private void RenderHtmlResponse (HtmlTextWriter writer)
        {
            // render opening tag
            if (NoIdAttribute) {
                writer.Write (@"<{0}", ElementType);
            } else {
                writer.Write (@"<{0} id=""{1}""", ElementType, ClientID);
            }

            // render attributes
            _attributes.Render (writer, this);

            if (HasContent) {
                writer.Write (">");
                RenderChildren (writer);
                if (RenderType == RenderingType.Default)
                    writer.Write ("</{0}>", ElementType);
            } else {
                // no content in widget
                switch (RenderType) {
                    case RenderingType.SelfClosing:
                        writer.Write (" />");
                        break;
                    case RenderingType.NoClose:
                        writer.Write (">");
                        break;
                    case RenderingType.Default:
                        writer.Write ("></{0}>", ElementType);
                        break;
                }
            }
        }

        /// <summary>
        ///     wrapper for an ajax server-side event
        /// </summary>
        public class AjaxEventArgs : EventArgs
        {
            public AjaxEventArgs (string name) { Name = name; }

            /// <summary>
            ///     retrieves the name of the event raised
            /// </summary>
            /// <value>the name of the event raised on client side</value>
            public string Name { get; private set; }
        }

        // used to figure out if element just became visible, in-visible, should re-render children, and so on
        protected enum RenderingMode
        {
            Default,
            ReRender,
            ReRenderChildren,
            RenderInvisible
        };
    }
}