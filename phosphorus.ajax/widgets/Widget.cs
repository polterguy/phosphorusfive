/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web.UI;
using System.Reflection;
using System.Collections.Generic;
using phosphorus.types;
using internals = phosphorus.ajax.core.internals;

namespace phosphorus.ajax.widgets
{
    /// <summary>
    /// general ajax html element
    /// </summary>
    [ViewStateModeById]
    public abstract class Widget : Control, IAttributeAccessor
    {
        // helps determine how we should render the widget
        protected enum RenderMode
        {
            // the engine takes care of how to render automatically
            Default,

            // re-render entire widget, regardless of what type of request this is
            RenderVisible,

            // re-render children of widget, regardless of what type of request this is
            RenderChildren,

            // re-render invisible markup, regardless of what type of request this is
            RenderInvisible
        };

        // contains all attributes of widget
        private internals.AttributeStorage _attributes = new internals.AttributeStorage ();

        // how to render the widget. normally this is automatically determined, but sometimes it needs to be overridden explicitly
        private RenderMode _renderMode = RenderMode.Default;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.widgets.Widget"/> class
        /// </summary>
        public Widget ()
        { }

        /// <summary>
        /// gets or sets the tag name used to render the html element
        /// </summary>
        /// <value>the tag name</value>
        public string Tag {
            get { return ViewState["Tag"] as string; }
            set {
                if (value.ToLower () != value)
                    throw new ApplicationException ("phosphorus.ajax doesn't like uppercase tags, you tried to supply; '" + value + "' as tagname");
                ViewState ["Tag"] = value;
            }
        }

        /// <summary>
        /// gets or sets the tag name used to render the html element when it is invisible
        /// </summary>
        /// <value>the tag name</value>
        public string InvisibleTag {
            get { return ViewState ["InvisibleTag"] == null ? "span" : ViewState["InvisibleTag"] as string; }
            set { ViewState ["InvisibleTag"] = value; }
        }

        /// <summary>
        /// gets or sets a value indicating whether this <see cref="phosphorus.ajax.widgets.Widget"/> should close an empty tag immediately.
        /// the default value for this property is false, which mean that if the element has no content, either as children controls, or 
        /// as innerHTML, then the tag to render the element will still create an end tag element. sometimes this is not what you wish, 
        /// though for most elements this is according to the standard. if you have a "void element", meaning an element that cannot have 
        /// any content at all, such as img, br, hr and such, then you can set this value to true, and the tag will be "self-closed". please 
        /// also be aware of the HasEndTag property, which often can be set to false for the same type of html elements that are void elements
        /// </summary>
        /// <value><c>true</c> if it closes an empty tag immediately; otherwise, <c>false</c></value>
        public bool SelfClosed {
            get { return ViewState ["SelfClosed"] == null ? false : (bool)ViewState["SelfClosed"]; }
            set { ViewState ["SelfClosed"] = value; }
        }

        /// <summary>
        /// gets or sets a value indicating whether this instance has an end tag or not. some elements does not require an end tag 
        /// at all, examples here are p, td, li, and so on. for these tags you can choose to explicitly turn off the rendering of the 
        /// end tag, and such save a couple of bytes of http traffic. the default value of this property is true
        /// </summary>
        /// <value><c>true</c> if this instance has an end tag; otherwise, <c>false</c></value>
        public bool HasEndTag {
            get { return ViewState ["HasEndTag"] == null ? true : (bool)ViewState["HasEndTag"]; }
            set { ViewState ["HasEndTag"] = value; }
        }

        /// <summary>
        /// gets or sets the named attribute for the widget. notice that attribute might exist, even if 
        /// return value is null, since attributes can have "null values", such as for instance "controls" 
        /// for the html5 video element, or the "disabled" attribute on form elements. if you wish to 
        /// check for the existence of an attribute, then use the <see cref="phosphorus.ajax.widgets.Widget.HasAttribute"/>. 
        /// if you wish to remove an attribute, use the <see cref="phosphorus.ajax.widgets.Widget.RemoveAttribute"/>
        /// </summary>
        /// <param name="name">attribute to retrieve or set</param>
        public virtual string this [string name] {
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
        /// determines whether this instance has an attribute aith the specified naame
        /// </summary>
        /// <returns><c>true</c> if this instance has the attribute with the specified name; otherwise, <c>false</c></returns>
        /// <param name="name">name of attribute to check for existence of</param>
        public virtual bool HasAttribute (string name)
        {
            return _attributes.HasAttribute (name);
        }

        /// <summary>
        /// removes an attribute
        /// </summary>
        /// <param name="name">name of attribute to remove</param>
        public virtual void RemoveAttribute (string name)
        {
            _attributes.RemoveAttribute (name);
        }

        /// <summary>
        /// gets a value indicating whether this instance has content or not
        /// </summary>
        /// <value><c>true</c> if this instance has content; otherwise, <c>false</c></value>
        protected abstract bool HasContent {
            get;
        }

        protected RenderMode RenderingMode {
            get { return _renderMode; }
            set { _renderMode = value; }
        }

        /// <summary>
        /// loads the form data from the http request, override this in your own widgets if you 
        /// have widgets that posts data to the server
        /// </summary>
        protected virtual void LoadFormData ()
        {
            if (this ["disabled"] == null) {
                if (!string.IsNullOrEmpty (this ["name"]) || Tag == "option") {
                    switch (Tag.ToLower ()) {
                        case "input":
                            switch (this ["type"]) {
                                case "radio":
                                case "checkbox":
                                    if (Page.Request.Params [this ["name"]] == "on") {
                                        _attributes.SetAttributeFormData ("checked", null);
                                    } else {
                                        _attributes.RemoveAttribute ("checked");
                                    }
                                    break;
                                default:
                                    _attributes.SetAttributeFormData ("value", Page.Request.Params [this ["name"]]);
                                    break;
                            }
                            break;
                        case "textarea":
                            _attributes.SetAttributeFormData ("innerHTML", Page.Request.Params [this ["name"]]);
                            break;
                        case "option":
                            Widget parent = Parent as Widget;
                            if (parent != null && !parent.HasAttribute ("disabled") && !string.IsNullOrEmpty (parent ["name"])) {
                                if (Page.Request.Params [parent ["name"]] == this ["value"]) {
                                    _attributes.SetAttributeFormData ("selected", null);
                                } else {
                                    _attributes.RemoveAttribute ("selected");
                                }
                            }
                            break;
                    }
                }
            }
        }
        
        /// <summary>
        /// invokes the given event handler. if the widget has an attribute with the 'eventName', then the value
        /// of that attribute will be retrieved, and a method on the page, usercontrol or master page the control belongs 
        /// to will be expected to contains a method with that name. if the widget does not have an attribute with the 
        /// 'eventName' name, then a method with the name of 'eventName' will be invoked searching through the page, 
        /// usercontrol or master page the control belongs to, in that order. all methods invoked this way must have 
        /// the <see cref="phosphorus.ajax.core.WebMethod"/> attribute. if you override this method, please call base 
        /// if you do not recognize the 'eventName'
        /// </summary>
        /// <param name="eventName">event name such as 'onclick', or name of c# method on page, usercontrol or masterpage</param>
        protected virtual void InvokeEventHandler (string eventName)
        {
            string eventHandlerName = null;
            if (HasAttribute (eventName)) {
                // probably "onclick" or other types of automatically generated mapping between server method and javascript handler
                eventHandlerName = this [eventName];
            } else {
                // WebMethod invocation
                eventHandlerName = eventName;
            }

            // finding out at what context to invoke the method within
            Control owner = this.Parent;
            while (!(owner is UserControl) && !(owner is Page) && !(owner is MasterPage))
                owner = owner.Parent;

            // retrieving the method
            MethodInfo method = owner.GetType ().GetMethod (eventHandlerName, 
                                                            BindingFlags.Instance | 
                                                            BindingFlags.Public | 
                                                            BindingFlags.NonPublic | 
                                                            BindingFlags.FlattenHierarchy);
            if (method == null)
                throw new NotImplementedException ("method + '" + eventHandlerName + "' could not be found");

            // verifying method has the WebMethod attribute
            object[] atrs = method.GetCustomAttributes (typeof (core.WebMethod), false /* for security reasons we want method to be explicitly marked as WebMethod */);
            if (atrs == null || atrs.Length == 0)
                throw new AccessViolationException ("method + '" + eventHandlerName + "' is illegal to invoke over http");

            // invoking methods with the "this" widget and empty event args
            method.Invoke (owner, new object[] { this, new EventArgs() });
        }

        public override bool Visible {
            get {
                return base.Visible;
            }
            set {
                if (!base.Visible && value && IsTrackingViewState && IsPhosphorusRequest) {
                    // this control was made visible during this request and should be rendered as html
                    // unless any of its ancestors are invisible
                    _renderMode = RenderMode.RenderVisible;
                } else if (base.Visible && !value && IsTrackingViewState && IsPhosphorusRequest) {
                    // this control was made invisible during this request and should be rendered 
                    // with its invisible html, unless any of its ancestors are invisible
                    _renderMode = RenderMode.RenderInvisible;
                }
                base.Visible = value;
            }
        }
        
        public override void RenderControl (HtmlTextWriter writer)
        {
            if (AreAncestorsVisible ()) {
                bool ancestorReRendering = IsAncestorRendering ();
                if (Visible) {
                    if (IsPhosphorusRequest && !ancestorReRendering) {
                        if (_renderMode == RenderMode.RenderVisible) {

                            // re-rendering entire widget
                            Node tmp = new Node (ClientID);
                            tmp ["outerHTML"].Value = GetWidgetHtml ();
                            (Page as core.IAjaxPage).Manager.RegisterWidgetChanges (tmp);
                        } else if (_renderMode == RenderMode.RenderChildren) {

                            // re-rendering all children
                            Node tmp = new Node (ClientID);
                            tmp ["innerHTML"].Value = GetChildrenHtml ();
                            (Page as core.IAjaxPage).Manager.RegisterWidgetChanges (tmp);
                        } else {

                            // only pass changes back to client as json
                            Node tmp = GetJsonChanges ();
                            if (tmp.Count > 0) {
                                (Page as core.IAjaxPage).Manager.RegisterWidgetChanges (tmp);
                            }
                            RenderChildren (writer);
                        }
                    } else {

                        // not ajax request, or ancestors are re-rendering
                        RenderHtmlResponse (writer);
                    }
                } else {

                    // invisible widget
                    if (IsPhosphorusRequest && _renderMode == RenderMode.RenderInvisible && !ancestorReRendering) {

                        // re-rendering widget's invisible markup
                        Node tmp = new Node (ClientID);
                        tmp ["outerHTML"].Value = GetWidgetInvisibleHtml ();
                        (Page as core.IAjaxPage).Manager.RegisterWidgetChanges (tmp);
                    } else if (!IsPhosphorusRequest || ancestorReRendering) {

                        // rendering invisible markup
                        writer.Write (GetWidgetInvisibleHtml ());
                    } // else, nothing to render since widget is in-visible, and this was an ajaxx request
                }
            }
        }

        protected override void LoadControlState (object state)
        {
            object[] obj = state as object[];
            base.Visible = (bool)obj [0];
            base.LoadControlState (obj [1]);
        }

        protected override object SaveControlState ()
        {
            object[] obj = new object [2];
            obj [0] = base.Visible;
            obj [1] = base.SaveControlState ();
            return obj;
        }

        protected override void LoadViewState (object savedState)
        {
            if (savedState != null) {
                object[] tmp = savedState as object[];
                base.LoadViewState (tmp [0]);
                _attributes.LoadFromViewState (tmp [1]);
                _attributes.LoadRemovedFromViewState (tmp [2]);
            }
        }

        protected override object SaveViewState ()
        {
            object[] retVal = new object [3];
            retVal [0] = base.SaveViewState ();
            retVal [1] = _attributes.SaveToViewState ();
            retVal [2] = _attributes.SaveRemovedToViewState ();
            return retVal;
        }

        protected override void OnInit (EventArgs e)
        {
            Page.RegisterRequiresControlState (this);
            if (Page.IsPostBack)
                LoadFormData ();
            base.OnInit (e);
        }

        protected override void OnLoad (EventArgs e)
        {
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

        private Node GetJsonChanges ()
        {
            Node tmp = new Node (ClientID);
            _attributes.RenderJsonChanges (tmp);

            // checking to see if we should update tagName of element
            if (ViewState.IsItemDirty ("Tag")) {
                tmp ["tagName"].Value = Tag;
            }
            return tmp;
        }

        private string GetWidgetHtml ()
        {
            using (MemoryStream stream = new MemoryStream ()) {
                using (HtmlTextWriter txt = new HtmlTextWriter (new StreamWriter (stream))) {
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
            using (MemoryStream stream = new MemoryStream ()) {
                using (HtmlTextWriter txt = new HtmlTextWriter (new StreamWriter (stream))) {
                    var oldRender = _renderMode;
                    _renderMode = RenderMode.RenderVisible;
                    RenderChildren (txt);
                    _renderMode = oldRender;
                    txt.Flush ();
                }
                stream.Seek (0, SeekOrigin.Begin);
                using (TextReader reader = new StreamReader (stream)) {
                    return reader.ReadToEnd ();
                }
            }
        }

        private string GetWidgetInvisibleHtml ()
        {
            return string.Format (@"<{0} id=""{1}"" style=""display:none important!;""></{0}>", InvisibleTag, ClientID);
        }

        private bool IsPhosphorusRequest {
            get { return !string.IsNullOrEmpty (Page.Request.Params ["__pf_event"]); }
        }

        private bool AreAncestorsVisible ()
        {
            // if this control and all of its ancestors are visible, then return true, else false
            Control idx = this.Parent;
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
            Control idx = this.Parent;
            while (idx != null) {
                Widget wdg = idx as Widget;
                if (wdg != null && (wdg._renderMode == RenderMode.RenderVisible || wdg._renderMode == RenderMode.RenderChildren))
                    return true;
                idx = idx.Parent;
            }
            return false;
        }

        private void RenderHtmlResponse (HtmlTextWriter writer)
        {
            // render opening tag
            writer.Write (string.Format (@"<{0} id=""{1}""", Tag, ClientID));

            // render attributes
            _attributes.Render (writer);

            if (HasContent) {
                writer.Write (">");
                RenderChildren (writer);
                if (HasEndTag) {
                    writer.Write (string.Format ("</{0}>", Tag));
                }
            } else {

                // no content in widget
                if (HasEndTag) {
                    if (SelfClosed) {

                        // xhtml style of syntax
                        writer.Write (" />");
                    } else {

                        // tag must have its ending tag, even though it is empty
                        writer.Write (">");
                        writer.Write (string.Format ("</{0}>", Tag));
                    }
                } else {

                    // widget doesn't need ending html tag
                    writer.Write (">");
                }
            }
        }

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

