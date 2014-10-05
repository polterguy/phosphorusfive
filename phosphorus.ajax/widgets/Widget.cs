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
    /// general ajax html element
    /// </summary>
    public abstract class Widget : Control, IAttributeAccessor
    {
        private bool _render;

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
            get { return ViewState ["InvisibleTag"] == null ? "span" : ViewState["InvisibleTag"] as string; }
            set { ViewState ["InvisibleTag"] = value; }
        }

        /// <summary>
        /// gets or sets the named attribute for the widget
        /// </summary>
        /// <param name="attribute">attribute to retrieve or set</param>
        public string this [string attribute] {
            get { return GetAttribute (attribute); }
            set { SetAttribute (attribute, value); }
        }

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

        protected override void LoadViewState (object savedState)
        {
            object[] tmp = savedState as object[];
            base.LoadViewState (tmp [0]);
            Attribute[] atrs = tmp [1] as Attribute [];
            foreach (Attribute idx in atrs) {
                Attributes.RemoveAll (
                    delegate(Attribute idx2) {
                        return idx.Name == idx2.Name;
                    });
            }
            Attributes.AddRange (atrs);
        }

        protected override object SaveViewState ()
        {
            object[] retVal = new object [2];
            retVal [0] = base.SaveViewState ();
            List<Attribute> dirty = Attributes.FindAll (
                delegate(Attribute idx) {
                    return idx.Dirty;
                });
            retVal [1] = dirty.ToArray ();
            return retVal;
        }
        
        public override void RenderControl (HtmlTextWriter writer)
        {
            if (AreAncestorsVisible ()) {
                if (Visible) {
                    if (IsPhosphorusRequest && !IsAncestorRendering ()) {
                        if (_render) {
                            Node tmp = new Node (ClientID);
                            tmp ["outerHTML"].Value = GetControlHtml ();
                            (Page as IAjaxPage).Manager.RegisterWidgetChanges (tmp);
                        } else {
                            Node tmp = new Node (ClientID);
                            foreach (Attribute idx in Attributes) {
                                if (idx.Dirty) {
                                    tmp [idx.Name].Value = new Node (idx.OldValue, idx.Value);
                                }
                            }
                            if (tmp.Count > 0) {
                                (Page as IAjaxPage).Manager.RegisterWidgetChanges (tmp);
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

        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            if (Page.IsPostBack)
                LoadFormData ();

            if (IsPhosphorusRequest) {

                if (Page.Request.Params ["__pf_wdg"] == ClientID) {

                    Page.Load += delegate {
                        // event was raised for this widget
                        InvokeEvent (Page.Request.Params ["__pf_evt"]);
                    };
                }
            }
        }

        /// <summary>
        /// loads the form data from the http request, override this in your own widgets if you 
        /// have widgets that posts data to the server
        /// </summary>
        protected virtual void LoadFormData ()
        {
            // only elements which are not disabled should post data
            if (this ["disabled"] == null) {
                // only elements with a given name attribute should post data
                if (!string.IsNullOrEmpty (this ["name"])) {

                    // different types of html elements loads their data in different ways
                    switch (Tag.ToLower ()) {
                    case "input":
                        this ["value"] = Page.Request.Params [this ["name"]];
                        break;
                    case "textarea":
                        this ["innerHTML"] = Page.Request.Params [this ["name"]];
                        break;
                    case "select":
                        string value = Page.Request.Params [this ["name"]];
                        foreach (Control idx in Controls) {
                            Widget widget = idx as Widget;
                            if (widget != null) {
                                if (widget ["value"] == value) {
                                    widget ["selected"] = "true";
                                } else {
                                    widget.Attributes.RemoveAll (
                                        delegate(Attribute idxAtr) {
                                            return idxAtr.Name == "selected";
                                        });
                                }
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
        /// returns the html for the control, override if you need to create your own custom html in your widgets
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
        /// returns true if this request is an ajax request
        /// </summary>
        /// <value><c>true</c> if this instance is an ajax request; otherwise, <c>false</c></value>
        protected bool IsPhosphorusRequest {
            get { return Page.Request.Params ["__pf_ajax"] == "1"; }
        }
        
        private List<Attribute> Attributes {
            get {
                if (ViewState ["Attributes"] == null) {
                    ViewState ["Attributes"] = new List<Attribute> ();
                }
                return ViewState["Attributes"] as List<Attribute>;
            }
        }

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

        private bool AreAncestorsVisible ()
        {
            // if this control and all of its ancestors are visible, then return true, else false
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

        private void RenderHtmlResponse (HtmlTextWriter writer)
        {
            // opening tag
            writer.Write (string.Format (@"<{0} id=""{1}""", Tag, ClientID));
            if (Attributes.Count > 0) {
                writer.Write (" ");
                RenderAttributes (writer);
            }
            writer.Write (">");
            RenderChildren (writer);
            writer.Write (string.Format ("</{0}>", Tag));
        }

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

        public string GetAttribute (string key)
        {
            Attribute atr = Attributes.Find (
                delegate (Attribute idx) {
                return idx.Name == key;
            });
            if (atr == null)
                return null;
            return atr.Value ?? string.Empty;
        }

        public void SetAttribute (string name, string value)
        {
            Attribute atr = Attributes.Find (
                delegate (Attribute idx) {
                    return idx.Name == name;
                });
            if (atr != null) {
                if (IsTrackingViewState && atr.Value != value && atr.OldValue == null) {
                    atr.OldValue = atr.Value;
                    atr.Dirty = true;
                }
                atr.Value = value;
            } else {
                atr = new Attribute (name, value);
                atr.Dirty = IsTrackingViewState;
                Attributes.Add (atr);
            }
        }

        /// <summary>
        /// removes the attribute with the given name
        /// </summary>
        /// <param name="name">name of attribute to remove</param>
        public void RemoveAttribute (string name)
        {
            Attributes.RemoveAll (
                delegate(Attribute idx) {
                    return idx.Name == name;
                });
        }
    }
}

