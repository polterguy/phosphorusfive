/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using pf.ajax.core.filters;
using pf = pf.ajax.widgets;

// ReSharper disable PossibleNullReferenceException

/*
 * making sure we include our "manager.js" JavaScript file as a WebResource
 */
[assembly: WebResource ("pf.ajax.javascript.manager.js", "application/javascript")]

namespace pf.ajax.core
{
    /// <summary>
    ///     Manages an IAjaxPage.
    /// 
    ///     Manages an IAjaxPage by providing services to the page, and helps render the page correctly, by among other things
    ///     adding a filter stream in the filtering chain, that renders the page according to how it is supposed to be rendered,
    ///     depending upon what type of request the http request is.
    /// 
    ///     Also contains several useful helper methods for developers.
    /// </summary>
    public class Manager
    {
        // contains all changes that needs to be serialized back to client
        private readonly OrderedDictionary _changes = new OrderedDictionary ();

        /// <summary>
        ///     Initializes a new instance of the Manager class.
        /// </summary>
        /// <param name="page">The page the manager is managing.</param>
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
                var coreScriptFileUrl = Page.ClientScript.GetWebResourceUrl (typeof (Manager), "pf.ajax.javascript.manager.js");
                (Page as IAjaxPage).RegisterJavaScriptFile (coreScriptFileUrl);
            };
        }

        /// <summary>
        ///     Returns the page for this instance.
        /// </summary>
        /// <value>The page.</value>
        public Page Page { get; private set; }

        /// <summary>
        ///     Returns true if this request is a Phosphorus Ajax request.
        /// </summary>
        /// <value><c>true</c> if this instance is an ajax request; otherwise, <c>false</c>.</value>
        public bool IsPhosphorusRequest
        {
            get { return !string.IsNullOrEmpty (Page.Request.Params ["__pf_event"]); }
        }

        /*
         * contains all changes for all widgets for this HTTP request.
         */
        internal OrderedDictionary Changes
        {
            get { return _changes; }
        }

        /// <summary>
        ///     Sends the given JavaScript to client for execution.
        /// </summary>
        /// <param name="script">JavaScript to execute on client-side.</param>
        public void SendJavaScriptToClient (string script)
        {
            if (!_changes.Contains ("__pf_script")) {
                _changes ["__pf_script"] = new List<string> ();
            }
            var lst = _changes ["__pf_script"] as List<string>;
            lst.Add (script);
        }

        /// <summary>
        ///     Sends an object back to the client as JSON.
        /// </summary>
        /// <param name="id">ID of object, must be unique for request.</param>
        /// <param name="value">Object to serialize back as JSON.</param>
        public void SendObject (string id, object value) { _changes [id] = value; }

        internal void RegisterWidgetChanges (string id, string name, string newValue, string oldValue = null)
        {
            var change = GetPropertyChanges (oldValue, newValue);
            if (!_changes.Contains ("__pf_change"))
                _changes ["__pf_change"] = new Dictionary<string, Dictionary<string, object>> ();
            var widgets = _changes ["__pf_change"] as Dictionary<string, Dictionary<string, object>>;
            if (!widgets.ContainsKey (id)) {
                widgets [id] = new Dictionary<string, object> ();
            }
            var widgetChanges = widgets [id];
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
            var widgetChanges = widgets [id];
            if (!widgetChanges.ContainsKey ("__pf_del")) {
                widgetChanges ["__pf_del"] = new List<string> ();
            }
            var list = widgetChanges ["__pf_del"] as List<string>;
            list.Add (name);
        }

        internal void RegisterDeletedWidget (string id)
        {
            if (!_changes.Contains ("__pf_del"))
                _changes ["__pf_del"] = new List<string> ();
            var list = _changes ["__pf_del"] as List<string>;
            list.Add (id);
        }

        private object GetPropertyChanges (string oldValue, string newValue)
        {
            if (oldValue == null || oldValue.Length < 10 || newValue == null || newValue.Length < 10) {
                return newValue; // no need to reduce size
            }
            // finding the position of where the changes start such that we can return 
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
                return newValue; // we cannot save anything here ...
            }
            return new object[] {start, update};
        }
    }
}