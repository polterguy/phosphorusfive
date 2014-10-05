/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed license.txt file for details
 */

using System;
using System.IO;
using System.Web.UI;
using System.Reflection;
using System.Collections.Generic;
using phosphorus.types;
using phosphorus.ajax.core;

namespace phosphorus.ajax.widgets
{
    /// <summary>
    /// generic ajax element
    /// </summary>
    public abstract class Widget : Control, IAttributeAccessor
    {
        /// <summary>
        /// contains changes
        /// </summary>
        Node _changes;

        /// <summary>
        /// true if we should render html for widget, regardless of whether or not this is a phosphorus.ajax request
        /// </summary>
        private bool _render;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.widgets.Widget"/> class
        /// </summary>
        public Widget ()
        {
            AttributesExcludedFromViewstate = new List<string> ();
        }

        /// <summary>
        /// gets or sets the tag name used to render the html element
        /// </summary>
        /// <value>the tag name</value>
        public string Tag {
            get { return ViewState["Tag"] as string; }
            set { ViewState ["Tag"] = value; }
        }

        /// <summary>
        /// gets or sets the tag name used to render the html element when it is invisible
        /// </summary>
        /// <value>the tag name</value>
        public string InvisibleTag {
            get { return ViewState["InvisibleTag"] == null ? "span" : ViewState["InvisibleTag"] as string; }
            set { ViewState ["InvisibleTag"] = value; }
        }

        /// <summary>
        /// gets or sets the named attribute for the widget
        /// </summary>
        /// <param name="attribute">attribute to retrieve or set</param>
        public string this [string attribute] {
            get { return ((IAttributeAccessor)this).GetAttribute (attribute); }
            set { ((IAttributeAccessor)this).SetAttribute (attribute, value); }
        }
        
        /// <summary>
        /// gets <see cref="phosphorus.widget.Attribute"/>  collection for widget
        /// </summary>
        /// <value>the attributes for the widget</value>
        public List<Attribute> Attributes {
            get { return ViewState["Attributes"] as List<Attribute>; }
        }

        /// <summary>
        /// determines whether this instance has the attribute with the specified name
        /// </summary>
        /// <returns><c>true</c> if this instance has the attribute with the specified name; otherwise, <c>false</c></returns>
        /// <param name="name">Name.</param>
        public bool HasAttribute (string name)
        {
            return Attributes != null && Attributes.Find (
                delegate (Attribute idx) {
                    return idx.Name == name;
                }) != null;
        }

        /// <summary>
        /// gets or sets a value indicating whether this <see cref="phosphorus.ajax.widgets.Widget"/> is visible or not
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c></value>
        public override bool Visible {
            get {
                return base.Visible;
            }
            set {
                if (!base.Visible && value && IsTrackingViewState && IsPhosphorusRequest) {
                    _render = true;
                }
                base.Visible = value;
            }
        }
        
        /// <summary>
        /// renders the control to the given writer
        /// </summary>
        /// <param name="writer">writer</param>
        public override void RenderControl (HtmlTextWriter writer)
        {
            if (AreAncestorsVisible ()) {
                if (Visible) {
                    if (IsPhosphorusRequest && !IsAncestorRendering ()) {
                        if (_render) {
                            _changes.Clear ();
                            _changes ["outerHTML"].Value = GetControlHtml ();
                            (Page as IAjaxPage).Manager.RegisterWidgetChanges (_changes);
                        } else {
                            if (_changes.Count > 0) {
                                (Page as IAjaxPage).Manager.RegisterWidgetChanges (_changes);
                            }
                            RenderChildren (writer);
                        }
                    } else {
                        RenderHtmlResponse (writer);
                    }
                } else {
                    writer.Write (string.Format (@"<{0} id=""{1}"" style=""display:none important!;""></{0}>", InvisibleTag, ClientID));
                }
            }
        }

        /// <summary>
        /// list of attributes to remove when serializing viewstate
        /// </summary>
        /// <value>The attributes excluded from viewstate.</value>
        protected List<string> AttributesExcludedFromViewstate {
            get;
            private set;
        }

        /// <summary>
        /// raises the init event, overridden to make sure we wire up events correctly
        /// </summary>
        /// <param name="e">E.</param>
        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            if (Page.IsPostBack)
                LoadFormData ();
            if (IsPhosphorusRequest) {
                _changes = new Node (ClientID);
                Page.Load += delegate {
                    string evt = Page.Request.Params ["__pf_evt"];
                    string widgetId = Page.Request.Params ["__pf_wdg"];
                    if (widgetId == ClientID) {
                        // event was raised for this widget
                        InvokeEvent (evt);
                    }
               };
            }
        }

        /// <summary>
        /// loads the form data from the http request
        /// </summary>
        protected virtual void LoadFormData ()
        {
            // only elements which are not disabled should post data
            if (!HasAttribute ("disabled")) {
                // only elements with a given name attribute should post data
                if (this ["name"] != null) {

                    // different types of html elements loads their data in different ways
                    switch (Tag.ToLower ()) {
                    case "input":
                        this ["value"] = Page.Request.Params [this ["name"]];
                        AttributesExcludedFromViewstate.Add ("value");
                        break;
                    case "textarea":
                        this ["innerHTML"] = Page.Request.Params [this ["name"]];
                        AttributesExcludedFromViewstate.Add ("innerHTML");
                        break;
                    case "select":
                        string value = Page.Request.Params [this ["name"]];
                        foreach (Control idx in Controls) {
                            Widget widget = idx as Widget;
                            if (widget != null) {
                                if (widget ["value"] == value) {
                                    widget ["selected"] = "true";
                                } else {
                                    widget ["selected"] = null;
                                }
                                widget.AttributesExcludedFromViewstate.Add ("value");
                            }
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// raise given event, override to raise your own custom events, but please call base if the event is not yours
        /// </summary>
        /// <param name="evt">name of event to raise</param>
        protected virtual void InvokeEvent (string evt)
        {
            string eventHandlerName = this [evt];
            InvokeEventHandler (eventHandlerName);
        }

        /// <summary>
        /// saves the viewstate of the control. overridden to remove excluded attributes
        /// </summary>
        /// <returns>the saved viewstate</returns>
        protected override object SaveViewState ()
        {
            if (Attributes != null) {
                foreach (string idx in AttributesExcludedFromViewstate) {
                    Attributes.RemoveAll (
                        delegate (Attribute idxA) {
                        return idxA.Name == idx;
                    });
                }
            }
            return base.SaveViewState ();
        }

        /// <summary>
        /// returns the html for the control
        /// </summary>
        /// <returns>the control's html</returns>
        protected virtual string GetControlHtml()
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

        /// <summary>
        /// tracks changes done to the widget
        /// </summary>
        /// <param name="name">name of attribute that has changed</param>
        /// <param name="value">new value of attribute</param>
        protected void TrackChanges (string name, string oldValue, string newValue)
        {
            if (IsTrackingViewState && IsPhosphorusRequest && oldValue != newValue) {

                // storing oldValue as Name and newValue as Value of Node
                Node tmp = _changes [name].Get<Node> ();
                if (tmp == null) {
                    tmp = new Node (oldValue);
                }
                tmp.Value = newValue;
                _changes [name].Value = tmp;
            }
        }

        /// <summary>
        /// returns true if this request is an ajax request
        /// </summary>
        /// <value><c>true</c> if this instance is an ajax request; otherwise, <c>false</c></value>
        protected bool IsPhosphorusRequest {
            get { return Page.Request.Params ["__pf_ajax"] == "1"; }
        }

        /// <summary>
        /// executes the given method on the given owner passing in the given widget
        /// </summary>
        /// <param name="owner">owner where we expect to find method</param>
        /// <param name="eventHandlerName">name of event handler method</param>
        private void InvokeEventHandler (string eventHandlerName)
        {
            Control owner = this.Parent;
            while (!(owner is Page) && !(owner is UserControl) && !(owner is MasterPage))
                owner = owner.Parent;
            MethodInfo method = owner.GetType ().GetMethod (eventHandlerName, 
                                                            BindingFlags.Instance | 
                                                            BindingFlags.Public | 
                                                            BindingFlags.NonPublic | 
                                                            BindingFlags.FlattenHierarchy);
            if (method == null)
                throw new NotImplementedException ();
            method.Invoke (owner, new object[] { this, new EventArgs() });
        }

        /// <summary>
        /// determines whether all ancestors are visible
        /// </summary>
        /// <returns><c>true</c> if all ancestors are visible; otherwise, <c>false</c></returns>
        private bool AreAncestorsVisible ()
        {
            // if this control and all of its ancestors are visible, then return true, else return false
            Control idx = this.Parent;
            while (idx != null) {
                if (!idx.Visible)
                    break;
                idx = idx.Parent;
                if (idx == null)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// returns whether or not any of its ancestors are rendering html or not
        /// </summary>
        /// <returns><c>true</c> if this instance is ancestor rendering; otherwise, <c>false</c>.</returns>
        private bool IsAncestorRendering()
        {
            // returns true if any of its ancestors are rendering html
            Control idx = this.Parent;
            while (idx != null) {
                Widget wdg = idx as Widget;
                if (wdg != null && wdg._render)
                    return true;
                idx = idx.Parent;
            }
            return false;
        }

        /// <summary>
        /// renders html response to the given writer
        /// </summary>
        /// <param name="writer">html text writer to render response into</param>
        private void RenderHtmlResponse (HtmlTextWriter writer)
        {
            // opening tag
            writer.Write (string.Format (@"<{0} id=""{1}""", Tag, ClientID));
            if (Attributes != null && Attributes.Count > 0) {
                writer.Write (" ");
                RenderAttributes (writer);
            }
            writer.Write (">");
            RenderChildren (writer);
            writer.Write (string.Format ("</{0}>", Tag));
        }

        /// <summary>
        /// renders the attributes for the widget
        /// </summary>
        /// <param name="writer">html writer</param>
        private void RenderAttributes (HtmlTextWriter writer)
        {
            bool isFirst = true;
            foreach (Attribute idx in Attributes) {
                string name = idx.Name;
                string value;
                if (idx.Name.StartsWith ("on") && Utilities.IsLegalMethodName (idx.Value)) {
                    value = "pf.e(event)";
                } else {
                    value = idx.Value;
                }
                if (isFirst) {
                    isFirst = false;
                } else {
                    writer.Write (" ");
                }
                if (value == null) {
                    writer.Write (string.Format (@"{0}", name));
                } else {
                    writer.Write (string.Format (@"{0}=""{1}""", name, value.Replace ("\"", "\\\"")));
                }
            }
        }

        /// <summary>
        /// returns attribute for widget
        /// </summary>
        /// <returns>the attribute value</returns>
        /// <param name="key">name of attribute</param>
        string IAttributeAccessor.GetAttribute (string key)
        {
            if (Attributes == null)
                return null;
            Attribute atr = Attributes.Find (
                delegate (Attribute idx) {
                return idx.Name == key;
            });
            if (atr == null)
                return null;
            return atr.Value;
        }

        /// <summary>
        /// sets attribute for widget
        /// </summary>
        /// <param name="name">name of attribute</param>
        /// <param name="value">value of attribute</param>
        void IAttributeAccessor.SetAttribute (string name, string value)
        {
            if (Attributes == null) {
                ViewState ["Attributes"] = new List<Attribute> ();
            }
            Attribute atr = Attributes.Find (
                delegate (Attribute idx) {
                return idx.Name == name;
            });
            string oldValue = null;
            if (atr != null) {
                oldValue = atr.Value;
                atr.Value = value; // already existing, changing existing
            } else {
                // not existing, creating new
                Attributes.Add (new Attribute (name, value));
            }
            TrackChanges (name, oldValue, value);
        }
    }
}

