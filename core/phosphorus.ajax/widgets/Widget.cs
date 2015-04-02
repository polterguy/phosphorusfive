/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Web.UI;
using System.Reflection;
using System.Collections.Generic;
using phosphorus.ajax.core;
using phosphorus.ajax.core.internals;

// ReSharper disable PossibleNullReferenceException
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace phosphorus.ajax.widgets
{
    /// <summary>
    ///     Abstract base class for all widgets in Phosphorus.Ajax.
    /// 
    ///     This is the abstract base class for all widgets in Phosphorus.Ajax, and is what allows Phosphorus.Ajax to update
    ///     properties of widgets automatically during Ajax requests.
    /// 
    ///     All changes to your widgets will be automatically rendered back to client as JSON, and only the changes will be sent. This means
    ///     that Phosphorus.Ajax consumes very little bandwidth. This fact combined with the fact that Phosphorus.Ajax only contains ~5KB of
    ///     JavaScript in release builds, makes your websites extremely small and responsive.
    /// 
    ///     This class automatically maps up all properties and attributes of your widgets, and also automatically creates Ajax server-side event
    ///     handlers for your DOM events, and sends back the changes to the client when you're done handling your HTTP postbacks and requests.
    /// 
    ///     This widget can render any HTML you wish, and dynamically change, set, or remove, any attributes and/or server-side Ajax Events you 
    ///     wish, in addition to have your client-side JavaScript DOM events automatically mapped for you.
    /// 
    ///     Basically; You get to create your code exclusively in C#, and this class maps your C# to JavaScript and client-side logic for you. Though,
    ///     if you wish, you can of course combine widgets created through this class, with any amounts of client-side logic, and JavaScript you wish.
    /// </summary>
    [ViewStateModeById]
    public abstract class Widget : Control, IAttributeAccessor
    {
        /// <summary>
        ///     Wrapper for an Ajax server-side event.
        /// 
        ///     EventArgs for your Ajax event handlers. All your Ajax event handlers on your server-side, will be sent an object of this instance.
        /// </summary>
        public class AjaxEventArgs : EventArgs
        {
            /// <summary>
            ///     Initializes a new instance of the AjaxEventArgs class.
            /// </summary>
            /// <param name="name">Name.</param>
            public AjaxEventArgs (string name)
            {
                Name = name;
            }

            /// <summary>
            ///     Retrieves the name of the event raised.
            /// 
            ///     This is the name of the Ajax event that was raised.
            /// </summary>
            /// <value>The name of the event raised on the client-side.</value>
            public string Name { get; private set; }
        }

        /// <summary>
        ///     Defines how to render the HTML element of your widget.
        /// 
        ///     This defines how to render your widget's HTML. You can choose to end your HTML element immediately, keep
        ///     it open, which is useful for for instance "p" HTML element, or use the default rendering here.
        /// 
        ///     Notice, if you wish to be XHTML compliant, you cannot use the "open" property value, but must use either
        ///     of the other two values from here.
        /// </summary>
        public enum RenderingType
        {
            /// <summary>
            ///     This is for elements that require both an opening element, and a closing element, such as "div" and "ul".
            /// 
            ///     Using this property, will close the HTML element wrapping your widget, with an explicit closing tag.
            /// </summary>
            normal,

            /// <summary>
            ///     This forces the element to close itself immediately.
            /// 
            ///     This property value, will close your element immediately, if it can, meaning it won't render a closing
            ///     HTML tag for you, but instead close the tag immediately, with a slash / appended at the end of your element,
            ///     before the &gt; at the end of your element's opening HTML tag.
            /// </summary>
            immediate,

            /// <summary>
            ///     This is for elements that does not require a closing element at all.
            /// 
            ///     Unless you want to render your website as XHTML, then you can use this property value on some of your widgets, 
            ///     such as for instance the "p" (paragraph) HTML element, to save some few bytes, and create more beautiful HTML.
            /// 
            ///     This property means that your HTML element, wrapping your Ajax widget will not be closed at all, and there will
            ///     be no end HTML element, nor will the tag close immediately.
            /// </summary>
            open
        }

        /// <summary>
        ///     Defines how the widget is supposed to be rendered during the current request.
        /// 
        ///     Allows you to override the rendering mode of your widget, to re-render it, re-render its children collection,
        ///     render it in invisible mode, and so on. Normally you don't have to fiddle with this, unless you have added and/or removed
        ///     controls from your widget's Control collection.
        /// </summary>
        protected enum RenderingMode
        {
            /// <summary>
            ///     The default value.
            /// 
            ///     The framework will take care of the rendering automatically for you.
            /// </summary>
            Default,

            /// <summary>
            ///     Re-rendering mode.
            /// 
            ///     This mode means that your widget will be re-rendered entirely, and have its entire HTML replaces on the client-side.
            ///     This is useful if you have tampered with properties of your widget that cannot be changed automatically during Ajax
            ///     requests using the default JSON serialization logic.
            /// </summary>
            ReRender,

            /// <summary>
            ///     Re-renders the children collection.
            /// 
            ///     Will re-render the children of the widget, which is useful if you have tampered with the children collection
            ///     of your widget, and you need to re-render the children.
            /// </summary>
            ReRenderChildren,

            /// <summary>
            ///     Renders the widget in invisible mode.
            /// 
            ///     This is the rendered mode that your widget will be rendered in, when it is set to invisible through its Visible property.
            /// </summary>
            RenderInvisible
        }

        /// <summary>
        ///     Initializes a new instance of the widget class.
        /// 
        ///     Creates a new instance of your widget.
        /// </summary>
        public Widget ()
        {
            RenderMode = RenderingMode.Default;
        }

        // contains all attributes of widget
        private readonly AttributeStorage _attributes = new AttributeStorage ();

        /// <summary>
        ///     Gets or sets the rendering mode.
        /// 
        ///     The rendering mode defines how your widget is rendered during the current request, and allows you to set its
        ///     state to "re-render", "re-render children", and so on, which is useful when you create your own widgets, and
        ///     make changes to them, that requires them to either be re-rendered themselves, or have their children collection
        ///     re-rendered.
        /// 
        ///     Normally you do not need to fiddle with this property yourself, but can allow the framework to take care of this automatically
        ///     for you.
        /// </summary>
        /// <value>The render mode.</value>
        protected RenderingMode RenderMode {
            get; set;
        }

        /// <summary>
        ///     Gets or sets the element type used to render your widget.
        /// 
        ///     Set this value to whatever HTML element you wish to use for your widget, such as "p", "div", "ul" etc.
        /// </summary>
        /// <value>The HTML element used to render widget.</value>
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
        ///     Gets or sets the tag name used to render the html element when it is invisible.
        /// 
        ///     This is sometimes useful, since the default tag rendered when widget is invisible, is a span tag. This is not 
        ///     necessarily compatible with the position in the DOM you're rendering it. For instance, if you have a "ul" tag, 
        ///     or widget, which has an invisible "li" widget, then rendering a span tag as the child HTML element of your "ul"
        ///     is illegal according to the html standard. For such cases, you must change the invisible tag rendered to become 
        ///     an "li" element.
        /// </summary>
        /// <value>The invisible element name.</value>
        public string InvisibleElement
        {
            get { return ViewState ["ie"] == null ? "span" : ViewState ["ie"] as string; }
            set { ViewState ["ie"] = value; }
        }

        /// <summary>
        ///     Gets or sets the rendering type.
        /// 
        ///     The rendering type of your widget, determines how it is rendered. See 
        ///     <see cref="phosphorus.ajax.widgets.Widget.RenderingType">RenderingType</see>, to understand how this affects your rendered HTML.
        /// </summary>
        /// <value>The render type.</value>
        public RenderingType RenderType
        {
            get { return (RenderingType) (ViewState ["rt"] ?? RenderingType.normal); }
            set { ViewState ["rt"] = value; }
        }

        /// <summary>
        ///     Sets or gets whether your widget has its ID attribute rendered to the client or not.
        /// 
        ///     If this property is true, then no "id" attribute will be rendered back to the client for the widget. This is useful 
        ///     for widgets that does not require an id attribute, where the id attribute is not supposed to be rendered to the
        ///     client.
        /// 
        ///     Examples includes for instance "option" HTML elements, where the id attribute is not always necessary, and
        ///     not rendering the id attribute, creates "better" and more beautiful HTML.
        /// 
        ///     The widget must still have a unique id, but this id will only be used on the server-side, and never rendered back to the client.
        /// 
        ///     If you do not render the id attribute, then you cannot change any of the widget's attributes or properties during server-side
        ///     Ajax callbacks, since the id attribute is used on the client-side to find the widget in the DOM, to update its properties and
        ///     attributes.
        /// </summary>
        /// <value>Whether an "id" attribute is rendered to the client or not.</value>
        public bool NoIdAttribute
        {
            get { return ViewState ["noid"] != null && (bool) ViewState ["noid"]; }
            set { ViewState ["noid"] = value; }
        }

        /// <summary>
        ///     Gets or sets an attribute value for your widget.
        /// 
        ///     This allows you to dynamically set and get any attribute value you wish from your widget. Notice that the attribute 
        ///     might still exist, even if the return value is null, since attributes can have "null values", such as for instance 
        ///     the "controls" attribute for the html5 video element, or the "disabled" attribute on form elements.
        /// 
        ///     If you wish to check for the existence of an attribute, then use HasAttribute method. If you wish to remove an attribute, 
        ///     use the RemoveAttribute method.
        /// 
        ///     If you however wish to change an attribute, or add a "null" attribute, then you can use the [ ] operator to accomplish just that.
        /// </summary>
        /// <param name="name">Name of attribute to retrieve or set value of.</param>
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
        ///     Gets a value indicating whether this instance has content or not.
        /// 
        ///     This is used during rendering of the widget, to determine how the widget should be rendered. If you create
        ///     your own widget classes, you should return false in an override from this method, if your widget has no content.
        /// 
        ///     This is necessary to determine how to render the widget's main HTML element if your widget is being rendered in
        ///     for instance <em>"immediate mode"</em>.
        /// </summary>
        /// <value><c>true</c> if this instance has content; otherwise, <c>false</c>.</value>
        protected abstract bool HasContent
        {
            get;
        }

        // overridden asp.net properties and methods
        public override bool Visible
        {
            get { return base.Visible; }
            set
            {
                if (value == Visible)
                    return; // nothing to do here ...

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

        string IAttributeAccessor.GetAttribute (string key)
        {
            return _attributes.GetAttribute (key);
        }

        void IAttributeAccessor.SetAttribute (string key, string value)
        {
            _attributes.SetAttributePreViewState (key, value);
        }

        /// <summary>
        ///     Determines whether this instance has an attribute with the specified name.
        /// 
        ///     Will return true, if an attribute with the specified name exists for the widget.
        /// </summary>
        /// <returns><c>true</c> if this instance has the attribute with the specified name; otherwise, <c>false</c>.</returns>
        /// <param name="name">Name of the attribute to check if exists.</param>
        public virtual bool HasAttribute (string name)
        {
            return _attributes.HasAttribute (name);
        }

        /// <summary>
        ///     Removes the specified attribute.
        /// 
        ///     Allows you to remove the specified attribute. Notice, that you cannot use the [ ] operator to remove attributes,
        ///     since even passing in "null" as the value, will just eliminate the attribute's value, and not the attribute itself.
        ///     If you wish to remove an attribute, you must use this method instead.
        /// </summary>
        /// <param name="name">Name of attribute you wish to remove.</param>
        public void RemoveAttribute (string name)
        {
            if (HasAttribute (name))
                _attributes.RemoveAttribute (name);
        }

        /// <summary>
        ///     Returns all attributes for widget.
        /// 
        ///     Sometimes it may be useful to see all the attributes that exists for a specific widget. For such cases, you can
        ///     use this property, to retrieve all the keys of the attributes that exists for your widget, for then to use the
        ///     returned values as input to the [ ] operator, to retrieve the values of your attributes.
        /// </summary>
        /// <value>All attribute keys for widget.</value>
        public IEnumerable<string> AttributeKeys {
            get {
                return _attributes.Keys;
            }
        }

        /// <summary>
        ///     Forces a re-rendering of your widget.
        /// 
        ///     Normally, this is not something you should have to mess with yourself, but something the framework itself will take care of. 
        ///     However, if you wish to force the control to re-render itself entirely as HTML back to the client, you can invoke this method.
        /// 
        ///     Sometimes this might be necessary, if you for instance have added widgets to the widget's Controls collection, without having used
        ///     the <em>"persistence"</em> features of the Container widget.
        /// </summary>
        public void ReRender ()
        {
            RenderMode = RenderingMode.ReRender;
        }

        /// <summary>
        ///     Forces a re-rendering of your widget's children controls.
        /// 
        ///     Normally, this is not something you should have to mess with yourself, but something the framework itself will take care of. 
        ///     However, if you wish to force the control to re-render its children entirely as HTML back to the client, you can invoke this method.
        /// 
        ///     Sometimes this might be necessary, if you for instance have added widgets to the widget's Controls collection, without having used
        ///     the <em>"persistence"</em> features of the Container widget.
        /// </summary>
        public void ReRenderChildren ()
        {
            if (RenderMode != RenderingMode.ReRender)
                RenderMode = RenderingMode.ReRenderChildren;
        }

        /// <summary>
        ///     Loads the form data from the HTTP request object for the current widget.
        /// 
        ///     The default implementation of this method, will automatically de-serialize all basic HTML form elements, such as
        ///     "input", "select", "option" and "textarea" elements.
        /// 
        ///     If you create your own custom types of widgets, that transmits data to the server, then you must override this method.
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
                        case "select":
                            if (!AllChildrenHasIds ()) {
                                if (Page.Request.Params [this ["name"]] != null)
                                    _attributes.SetAttributeFormData ("value", Page.Request.Params [this ["name"]]);
                                else
                                    _attributes.RemoveAttribute ("value");
                            }
                            break;
                        case "textarea":
                            if (Page.Request.Params [this ["name"]] != null)
                                _attributes.SetAttributeFormData ("innerValue", Page.Request.Params [this ["name"]]);
                            else
                                _attributes.RemoveAttribute ("innerValue");
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
                                    _attributes.RemoveAttribute ("selected", false);
                                }
                            } else {
                                _attributes.RemoveAttribute ("selected", false);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Invokes the given event handler.
        /// 
        ///     If the widget has an attribute with the given 'eventName' argument, then the value of that attribute will be retrieved, 
        ///     and the Page object, UserControl widget belongs to, or MasterPage the control is embedded inside of, will be expected to contain 
        ///     a method with that name.
        /// 
        ///     If the widget does not have an attribute with the 'eventName' name, then a method 
        ///     with the name of 'eventName' will be invoked searching through the page, usercontrol, or master page, the 
        ///     control belongs to, in that order. All methods invoked this way must have the WebMethod attribute on them.
        ///     If you override this method, you should call base if you do not recognize the 'eventName'.
        /// 
        ///     This allows you to explicitly invoke server-side WebMethods in your Page, UserControl or MasterPage, within the context of
        ///     a specific widget. While at the same time, allowing you to automatically map client-side DOM events to server-side WebMethods.
        /// </summary>
        /// <param name="eventName">Event name such as 'onclick', or name of C# method on Page, UserControl, or MasterPage.</param>
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
        ///     Renders all children as JSON update to be sent back to client.
        /// 
        ///     Override this method if you wish to create custom functionality as an alternative.
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

        /// <summary>
        ///     Returns true if all children controls have IDs.
        /// 
        ///     If one of the children widgets of your widget does not render its ID attribute to the client,
        ///     then this method will return false. This is useful for widgets of type "select" for instance,
        ///     when we need to determine how to access the POST parameter passed in from the client, to set
        ///     the "value" of a "select" HTML element.
        /// </summary>
        /// <returns><c>true</c>, if all children have ID attributes rendered, <c>false</c> otherwise.</returns>
        protected bool AllChildrenHasIds ()
        {
            foreach (Control idxCtrl in Controls) {
                var idxWidget = idxCtrl as Widget;
                if (idxWidget != null) {
                    if (idxWidget.NoIdAttribute)
                        return false;
                }
            }
            return true;
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

        private string GetWidgetInvisibleHtml ()
        {
            return string.Format (@"<{0} id=""{1}"" style=""display:none important!;""></{0}>", InvisibleElement, ClientID);
        }

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
                if (RenderType == RenderingType.normal)
                    writer.Write ("</{0}>", ElementType);
            } else {
                // no content in widget
                switch (RenderType) {
                    case RenderingType.immediate:
                        writer.Write (" />");
                        break;
                    case RenderingType.open:
                        writer.Write (">");
                        break;
                    case RenderingType.normal:
                        writer.Write ("></{0}>", ElementType);
                        break;
                }
            }
        }
    }
}