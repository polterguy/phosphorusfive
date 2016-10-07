/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.IO;
using System.Web.UI;
using System.Reflection;
using System.Collections.Generic;
using p5.ajax.core;
using p5.ajax.core.internals;

namespace p5.ajax.widgets
{
    /// <summary>
    ///     Abstract base class for all widgets in p5.ajax
    /// </summary>
    [ViewStateModeById]
    public abstract class Widget : Control, IAttributeAccessor
    {
        /// <summary>
        ///     Wrapper for an Ajax server-side event
        /// </summary>
        public class AjaxEventArgs : EventArgs
        {
            /// <summary>
            ///     Initializes a new instance of the AjaxEventArgs class
            /// </summary>
            /// <param name="name">Name</param>
            public AjaxEventArgs (string name)
            {
                Name = name;
            }

            /// <summary>
            ///     Retrieves the name of the event raised
            /// </summary>
            /// <value>The name of the event raised on the client-side</value>
            public string Name { get; private set; }
        }

        /// <summary>
        ///     Defines how to render the HTML element of your widget
        /// </summary>
        public enum RenderingType
        {
            /// <summary>
            ///     This is for elements that require both an opening element, and a closing element, such as "div" and "ul"
            /// </summary>
            normal,

            /// <summary>
            ///     This forces the element to close itself immediately
            /// </summary>
            immediate,

            /// <summary>
            ///     This is for elements that does not require a closing element at all
            /// </summary>
            open
        }

        /// <summary>
        ///     Defines how the widget is supposed to be rendered during the current request
        /// </summary>
        protected enum RenderingMode
        {
            /// <summary>
            ///     The default value
            /// </summary>
            Default,

            /// <summary>
            ///     Re-rendering mode
            /// </summary>
            ReRender,

            /// <summary>
            ///     Re-renders the children collection
            /// </summary>
            ReRenderChildren,

            /// <summary>
            ///     Renders the widget in invisible mode
            /// </summary>
            RenderInvisible
        }

        /// <summary>
        ///     Initializes a new instance of the widget class
        /// </summary>
        public Widget ()
        {
            RenderMode = RenderingMode.Default;
        }

        // Contains all attributes of widget
        private readonly AttributeStorage _attributes = new AttributeStorage ();

        /// <summary>
        ///     Gets or sets the rendering mode
        /// </summary>
        /// <value>The render mode</value>
        protected RenderingMode RenderMode {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the element type used to render your widget
        /// </summary>
        /// <value>The HTML element used to render widget</value>
        public virtual string Element
        {
            get { return this ["Element"]; }
            set
            {
                if (value.ToLower () != value)
                    throw new ArgumentException ("p5.ajax doesn't like uppercase element names", "value");
                this ["Element"] = value;
            }
        }

        /// <summary>
        ///     Gets or sets the rendering type
        /// </summary>
        /// <value>The render type</value>
        public RenderingType RenderType
        {
            get { return (RenderingType) (ViewState ["rt"] ?? RenderingType.normal); }
            set { ViewState ["rt"] = value; }
        }

        /// <summary>
        ///     Overridden to make it possible to change an element's ID during execution of page life cycle
        /// </summary>
        /// <value>The new ID</value>
        private string _oldId;
        public override string ID {
            get {
                return base.ID;
            }
            set {
                // Storing old ID of element, since this is the stuff that'll be rendered over the wire
                // to allow for retrieving the element on the client side
                if (IsTrackingViewState) {
                    if (value != base.ID) {
                        if (_oldId == null)
                            _oldId = base.ID;
                        _attributes.ChangeAttribute ("id", value);
                    }
                }
                base.ID = value;
            }
        }

        /// <summary>
        ///     Gets or sets an attribute value for your widget
        /// </summary>
        /// <param name="name">Name of attribute to retrieve or set value of</param>
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
        ///     Gets a value indicating whether this instance has content or not
        /// </summary>
        /// <value><c>true</c> if this instance has content; otherwise, <c>false</c></value>
        protected abstract bool HasContent
        {
            get;
        }

        // Overridden asp.net properties and methods
        public override bool Visible
        {
            get { return base.Visible; }
            set
            {
                if (value == Visible)
                    return; // Nothing to do here ...

                if (!base.Visible && value && IsTrackingViewState && IsPhosphorusRequest) {

                    // This control was made visible during this request and should be rendered as html
                    // unless any of its ancestors are invisible
                    RenderMode = RenderingMode.ReRender;
                } else if (base.Visible && !value && IsTrackingViewState && IsPhosphorusRequest) {

                    // This control was made invisible during this request and should be rendered 
                    // with its invisible html, unless any of its ancestors are invisible
                    RenderMode = RenderingMode.RenderInvisible;
                }
                base.Visible = value;
            }
        }

        private bool IsPhosphorusRequest
        {
            get { return !string.IsNullOrEmpty (Page.Request.Params ["_p5_event"]); }
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
        ///     Determines whether this instance has an attribute with the specified name
        /// </summary>
        /// <returns><c>true</c> if this instance has the attribute with the specified name; otherwise, <c>false</c></returns>
        /// <param name="name">Name of the attribute to check if exists</param>
        public virtual bool HasAttribute (string name)
        {
            return _attributes.HasAttribute (name);
        }

        /// <summary>
        ///     Removes the specified attribute
        /// </summary>
        /// <param name="name">Name of attribute you wish to remove</param>
        public void RemoveAttribute (string name)
        {
            if (HasAttribute (name))
                _attributes.RemoveAttribute (name);
        }

        /// <summary>
        ///     Returns all attributes for widget
        /// </summary>
        /// <value>All attribute keys for widget</value>
        public IEnumerable<string> AttributeKeys {
            get {
                return _attributes.Keys;
            }
        }

        /// <summary>
        ///     Forces a re-rendering of your widget
        /// </summary>
        public void ReRender ()
        {
            RenderMode = RenderingMode.ReRender;
        }

        /// <summary>
        ///     Forces a re-rendering of your widget's children controls
        /// </summary>
        public void ReRenderChildren ()
        {
            if (RenderMode != RenderingMode.ReRender)
                RenderMode = RenderingMode.ReRenderChildren;
        }

        /// <summary>
        ///     Loads the form data from the HTTP request object for the current widget
        /// </summary>
        protected virtual void LoadFormData ()
        {
            if (!HasAttribute ("disabled")) {
                if (!string.IsNullOrEmpty (this ["name"]) || Element == "option") {
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
        ///     Invokes the given event handler
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
            method.Invoke (owner, new object[] {this, new AjaxEventArgs (eventName)});
        }

        /// <summary>
        ///     Renders all children as JSON update to be sent back to client
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

                            // Re-rendering entire widget
                            (Page as IAjaxPage).Manager.RegisterWidgetChanges (_oldId ?? ClientID, "outerHTML", GetWidgetHtml ());
                        } else if (RenderMode == RenderingMode.ReRenderChildren) {

                            // Re-rendering all children controls, but also renders changes to widget ...
                            _attributes.RegisterChanges ((Page as IAjaxPage).Manager, _oldId ?? ClientID);
                            RenderChildrenWidgetsAsJson (writer);
                        } else {

                            // Only pass changes back to client as json
                            _attributes.RegisterChanges ((Page as IAjaxPage).Manager, _oldId ?? ClientID);
                            RenderChildren (writer);
                        }
                    } else {

                        // Not ajax request, or ancestors are re-rendering
                        RenderHtmlResponse (writer);
                    }
                } else {

                    // Invisible widget
                    if (IsPhosphorusRequest && RenderMode == RenderingMode.RenderInvisible && !ancestorReRendering) {

                        // Re-rendering widget's invisible markup
                        (Page as IAjaxPage).Manager.RegisterWidgetChanges (_oldId ?? ClientID, "outerHTML", GetWidgetInvisibleHtml ());
                    } else if (!IsPhosphorusRequest || ancestorReRendering) {

                        // Rendering invisible markup
                        writer.Write (GetWidgetInvisibleHtml ());
                    } // Else, nothing to render since widget is in-visible, and this was an ajax request
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

            // Making sure event handlers are being processed
            if (IsPhosphorusRequest) {
                if (Page.Request.Params ["_p5_widget"] == ClientID) {
                    Page.LoadComplete += delegate {

                        // Event was raised for this widget
                        InvokeEventHandler (Page.Request.Params ["_p5_event"]);
                    };
                }
            }
            base.OnLoad (e);
        }

        // Private methods for internal use in the class
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
            return string.Format (@"<{0} id=""{1}"" style=""display:none important!;""></{0}>", Element, ClientID);
        }

        public bool AreAncestorsVisible ()
        {
            // If this control and all of its ancestors are visible, then return true, else false
            var idx = Parent;
            while (true) {
                if (!idx.Visible)
                    break;
                idx = idx.Parent;
                if (idx == null) // We traversed all the way up to beyond the Page, and found no in-visible ancestor controls
                    return true;
            }

            // In-visible control among ancestors was found!
            return false;
        }

        private bool IsAncestorRendering ()
        {
            // Returns true if any of its ancestors are rendering HTML
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
            // Render opening tag
            writer.Write (@"<{0} id=""{1}""", Element, ClientID);

            // Render attributes
            _attributes.Render (writer, this);

            if (HasContent) {
                writer.Write (">");
                RenderChildren (writer);
                if (RenderType == RenderingType.normal)
                    writer.Write ("</{0}>", Element);
            } else {
                // No content in widget
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
    }
}