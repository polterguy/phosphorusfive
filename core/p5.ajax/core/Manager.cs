/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using p5.ajax.core.filters;
using p5 = p5.ajax.widgets;

/*
 * Making sure we include our "manager.js" JavaScript file as a WebResource
 */
[assembly: WebResource ("p5.ajax.javascript.manager.js", "application/javascript")]

namespace p5.ajax.core
{
    /// <summary>
    ///     Manages an IAjaxPage
    /// </summary>
    public class Manager
    {
        // Contains all changes that needs to be serialized back to client
        private readonly OrderedDictionary _changes = new OrderedDictionary ();

        /// <summary>
        ///     Initializes a new instance of the Manager class
        /// </summary>
        /// <param name="page">The page the manager is managing</param>
        public Manager (Page page)
        {
            Page = page;

            // Determining if we should create an ajax filter or a normal html filter for rendering the response
            if (IsPhosphorusAjaxRequest) {
                
                // Rendering ajax json back to the client
                Page.Response.Filter = new JsonFilter (this);
            } else {
                
                // Rendering plain html back to client
                Page.Response.Filter = new HtmlFilter (this);
            }

            // Including main p5.ajax javascript file
            Page.Load += delegate {

                // Retrieving JavaScript file as WebResource
                var coreScriptFileUrl = Page.ClientScript.GetWebResourceUrl (typeof (Manager), "p5.ajax.javascript.manager.js");
                (Page as IAjaxPage).RegisterJavaScriptFile (coreScriptFileUrl);
            };
        }

        /// <summary>
        ///     Returns the page for this instance
        /// </summary>
        /// <value>The page</value>
        public Page Page
        {
            get;
            private set;
        }

        /// <summary>
        ///     Returns true if this request is a Phosphorus Ajax request
        /// </summary>
        /// <value><c>true</c> if this instance is an ajax request; otherwise, <c>false</c></value>
        public bool IsPhosphorusAjaxRequest
        {
            get { return !string.IsNullOrEmpty (Page.Request.Params ["_p5_event"]); }
        }

        /*
         * Contains everything that needs to be pushed as JSON to client during this request
         */
        internal OrderedDictionary Changes
        {
            get { return _changes; }
        }

        /// <summary>
        ///     Sends the given JavaScript to client for execution
        /// </summary>
        /// <param name="script">JavaScript to execute on client-side</param>
        public void SendJavaScriptToClient (string script)
        {
            if (!_changes.Contains ("_p5_script")) {
                _changes ["_p5_script"] = new List<string> ();
            }
            var lst = _changes ["_p5_script"] as List<string>;
            lst.Add (script);
        }

        /// <summary>
        ///     Sends an object back to the client as JSON
        /// </summary>
        /// <param name="id">ID of object, must be unique for request</param>
        /// <param name="value">Object to serialize back as JSON</param>
        public void SendObject (string id, object value)
        {
            _changes [id] = value;
        }

        internal void RegisterWidgetChanges (string id, string name, string newValue, string oldValue = null)
        {
            var change = GetPropertyChanges (oldValue, newValue);
            if (!_changes.Contains ("__p5_change"))
                _changes ["__p5_change"] = new Dictionary<string, Dictionary<string, object>> ();
            var widgets = _changes ["__p5_change"] as Dictionary<string, Dictionary<string, object>>;
            if (!widgets.ContainsKey (id)) {
                widgets [id] = new Dictionary<string, object> ();
            }
            var widgetChanges = widgets [id];
            widgetChanges [name] = change;
        }

        internal void RegisterDeletedAttribute (string id, string name)
        {
            if (!_changes.Contains ("__p5_change"))
                _changes ["__p5_change"] = new Dictionary<string, Dictionary<string, object>> ();
            var widgets = _changes ["__p5_change"] as Dictionary<string, Dictionary<string, object>>;
            if (!widgets.ContainsKey (id)) {
                widgets [id] = new Dictionary<string, object> ();
            }
            var widgetChanges = widgets [id];
            if (!widgetChanges.ContainsKey ("_p5_del")) {
                widgetChanges ["_p5_del"] = new List<string> ();
            }
            var list = widgetChanges ["_p5_del"] as List<string>;
            list.Add (name);
        }

        internal void RegisterDeletedWidget (string id)
        {
            if (!_changes.Contains ("_p5_del"))
                _changes ["_p5_del"] = new List<string> ();
            var list = _changes ["_p5_del"] as List<string>;
            list.Add (id);
        }

        private object GetPropertyChanges (string oldValue, string newValue)
        {
            if (oldValue == null || oldValue.Length < 10 || newValue == null || newValue.Length < 10) {
                return newValue; // No need to reduce size
            }

            // Finding the position of where the changes start such that we can return 
            // as small amounts of changes back to client as possible, to conserve bandwidth and make response smaller
            if (oldValue == newValue) {
                return null;
            }
            var start = -1;
            string update = null;
            for (var idx = 0; idx < oldValue.Length && idx < newValue.Length; idx++) {
                if (oldValue [idx] != newValue [idx]) {
                    start = idx;
                    if (idx < newValue.Length) {
                        update = newValue.Substring (idx);
                    }
                    break;
                }
            }
            if (start == -1 && newValue.Length > oldValue.Length) {
                return new object[] {oldValue.Length, newValue.Substring (oldValue.Length)};
            }
            if (start == -1 && newValue.Length < oldValue.Length) {
                return new object[] {newValue.Length};
            }
            if (start < 5) {
                return newValue; // We cannot save anything here ...
            }
            return new object[] {start, update};
        }
    }
}
