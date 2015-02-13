
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Web.UI;
using System.Collections.Generic;
using System.Collections.Specialized;
using pf = phosphorus.ajax.widgets;
using phosphorus.ajax.core.filters;

[assembly: WebResource("phosphorus.ajax.javascript.manager.js", "application/javascript")]

namespace phosphorus.ajax.core
{
    /// <summary>
    /// manages an IAjaxPage by providing services to the page, and helps render the page correctly, by 
    /// adding a filter stream in the filtering chain that renders the page according to how it is supposed to be rendered, 
    /// depending upon what type of request the http request is
    /// </summary>
    public class Manager
    {
        // contains all changes that needs to be serialized back to client
        private OrderedDictionary _changes = new OrderedDictionary ();

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.core.Manager"/> class
        /// </summary>
        /// <param name="page">the page the manager is managing</param>
        public Manager (Page page)
        {
            Page = page;

            // determining if we should create an ajax filter or a normal html filter for rendering the response
            if (IsPhosphorusRequest) {
                // rendering ajax json back to the client
                Page.Response.Filter = new JsonFilter (this);
            } else {
                // rendering plain html back to client
                Page.Response.Filter = new HtmlFilter (this);
            }

            // including main javascript file
            Page.Load += delegate {
                string coreScriptFileUrl = Page.ClientScript.GetWebResourceUrl (typeof(Manager), "phosphorus.ajax.javascript.manager.js");
                (Page as IAjaxPage).RegisterJavaScriptFile (coreScriptFileUrl);
            };
        }

        /// <summary>
        /// returns the page for this instance
        /// </summary>
        /// <value>the page</value>
        public Page Page {
            get;
            private set;
        }

        /// <summary>
        /// returns true if this request is an ajax request
        /// </summary>
        /// <value><c>true</c> if this instance is an ajax request; otherwise, <c>false</c></value>
        public bool IsPhosphorusRequest {
            get { return !string.IsNullOrEmpty (Page.Request.Params ["__pf_event"]); }
        }

        /// <summary>
        /// sends the given javascript to client for execution
        /// </summary>
        /// <param name="script">javascript to execute on client</param>
        public void SendJavaScriptToClient (string script)
        {
            if (!_changes.Contains ("__pf_script")) {
                _changes ["__pf_script"] = new List<string> ();
            }
            List<string> lst = _changes ["__pf_script"] as List<string>;
            lst.Add (script);
        }

        /// <summary>
        /// send an object back to the client as json
        /// </summary>
        /// <param name="id">id of object, must be unique for request</param>
        /// <param name="value">object to serialize back as json</param>
        public void SendObject (string id, object value)
        {
            _changes [id] = value;
        }

        internal void RegisterWidgetChanges (string id, string name, string newValue, string oldValue = null)
        {
            var change = GetPropertyChanges (oldValue, newValue);
            if (!_changes.Contains ("__pf_change"))
                _changes ["__pf_change"] = new Dictionary<string, Dictionary<string, object>> ();
            var widgets = _changes ["__pf_change"] as Dictionary<string, Dictionary<string, object>>;
            if (!widgets.ContainsKey (id)) {
                widgets [id] = new Dictionary<string, object> ();
            }
            var widgetChanges = widgets [id] as Dictionary<string, object>;
            widgetChanges [name] = change;
        }

        internal void RegisterDeletedAttribute (string id, string name)
        {
            if (!_changes.Contains ("__pf_change"))
                _changes ["__pf_change"] = new Dictionary<string, Dictionary<string, object>> ();
            var widgets = _changes ["__pf_change"] as Dictionary<string, Dictionary<string, object>>;
            if (!widgets.ContainsKey (id)) {
                widgets [id] = new Dictionary<string, object> ();
            }
            var widgetChanges = widgets [id] as Dictionary<string, object>;
            if (!widgetChanges.ContainsKey ("__pf_del")) {
                widgetChanges ["__pf_del"] = new List<string> ();
            }
            List<string> list = widgetChanges ["__pf_del"] as List<string>;
            list.Add (name);
        }

        internal void RegisterDeletedWidget (string id)
        {
            if (!_changes.Contains ("__pf_del"))
                _changes ["__pf_del"] = new List<string> ();
            List<string> list = _changes ["__pf_del"] as List<string>;
            list.Add (id);
        }

        internal OrderedDictionary Changes {
            get { return _changes; }
        }

        private object GetPropertyChanges (string oldValue, string newValue)
        {
            if (oldValue == null || oldValue.Length < 10 || newValue == null || newValue.Length < 10) {
                return newValue; // no need to reduce size
            } else {
                // finding the position of where the changes start such that we can return 
                // as small amounts of changes back to client as possible, to conserve bandwidth and make response smaller
                if (oldValue == newValue) {
                    return null;
                }
                int start = -1;
                string update = null;
                for (int idx = 0; idx < oldValue.Length && idx < newValue.Length; idx++) {
                    if (oldValue [idx] != newValue [idx]) {
                        start = idx;
                        if (idx < newValue.Length) {
                            update = newValue.Substring (idx);
                        }
                        break;
                    }
                }
                if (start == -1 && newValue.Length > oldValue.Length) {
                    return new object[] { oldValue.Length, newValue.Substring (oldValue.Length) };
                } else if (start == -1 && newValue.Length < oldValue.Length) {
                    return new object[] { newValue.Length };
                } else if (start < 5) {
                    return newValue; // we cannot save anything here ...
                } else {
                    return new object[] { start, update };
                }
            }
        }
    }
}
