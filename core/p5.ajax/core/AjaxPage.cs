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
using System.Linq;
using System.Web.UI;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using p5.ajax.core.filters;
using p5.ajax.core.internals;

/*
 * Making sure we include our "manager.js" JavaScript file as a WebResource.
 */
[assembly: WebResource ("p5.ajax.javascript.manager.min.js", "application/javascript")]

namespace p5.ajax.core
{
    /// <summary>
    ///     Base class for all pages you wish to use p5.ajax widgets within.
    ///     You must inherit all your pages, that includes p5.ajax widgets, from this class, either directly, or indirectly.
    /// </summary>
    public class AjaxPage : Page
    {
        // Contains all changes that needs to be serialized back to client.
        // Only relevant during an Ajax request, in a normal HTTP POST/GET request, it is not in use.
        private readonly OrderedDictionary _changes = new OrderedDictionary ();

        // Responsible for persisting the ViewState for page.
        private PageStatePersister _statePersister;

        // Contains the list of new JavaScript objects to include in current request.
        // If Item2 of Tuple is true, it means that it is a JavaScript file inclusion, otherwise it's an inline JavaScript content inclusion.
        private List<Tuple<string, bool>> _javaScriptObjectsForCurrentRequest = new List<Tuple<string, bool>> ();

        // List of new CSS files to include for the current request.
        private List<string> _cssFileForCurrentRequest = new List<string> ();

        /// <summary>
        ///    Initializes a new instance of the <see cref="T:p5.ajax.core.AjaxPage"/> class.
        /// </summary>
        public AjaxPage ()
        {
            // Making sure we initialize all the necessary stuff during Load event of page.
            Load += delegate {

                // Determining if we should create an Ajax filter or an HTML filter for rendering our response.
                if (IsAjaxRequest) {

                    // Rendering JSON changes back to the client, by making sure we use the correct Response filter.
                    Response.Filter = new JsonFilter (this);

                } else {

                    // Rendering HTML back to client, by making sure we use the correct Response filter.
                    Response.Filter = new HtmlFilter (this);

                    // Making sure we include "manager.js", which is the main client-side parts of p5.ajax.
                    IncludeJavaScriptFile (ClientScript.GetWebResourceUrl (typeof (AjaxPage), "p5.ajax.javascript.manager.min.js"));
                }
            };
        }

        /// <summary>
        ///     Maximum number of ViewState entries in Session.
        ///     Notice, if you set this to zero, then ViewState persisting on server will be completely bypassed, and all ViewState
        ///     will render back to the client, as in a default and normal ASP.NET Web Forms app.
        /// </summary>
        /// <value>The number of valid viewstate entries for each session. Zero turns off server-side ViewState storage.</value>
        public int ViewStateSessionEntries { get; private set; }

        /// <summary>
        ///     Returns true if this request is an Ajax request.
        /// </summary>
        /// <value><c>true</c> if this instance is an ajax request; otherwise, <c>false</c></value>
        public bool IsAjaxRequest
        {
            get { return !string.IsNullOrEmpty (Page.Request.Params ["_p5_event"]); }
        }

        /// <summary>
        ///     Registers CSS stylesheet file for inclusion on your page.
        /// 
        ///     Notice, if you wish, you can supply an absolute URL, to include files from for instance a CDN, or similar types of sources.
        ///     This inclusion is persistently done, such that a normal postback to the server, will remember the included file, and
        ///     re-include it automatically for you.
        ///     In addition, it will make sure the same CSS file is not included twice.
        ///     You can also use "ClientScript.GetWebResourceUrl" to include CSS files that are embedded resources in other projects.
        /// </summary>
        /// <param name="url">URL to stylesheet you wish to include</param>
        public void IncludeCSSFile (string url)
        {
            // Checking if we have previously included this file.
            if (!PersistentCSSInclusions.Contains (url)) {

                // File has not previously been included, making sure we include it, and persistently store that fact in ViewState.
                // In addition to registering it for "push" towards client.
                PersistentCSSInclusions.Add (url);
                _cssFileForCurrentRequest.Add (url);
            }
        }

        /// <summary>
        ///     Registers JavaScript files for inclusion on your page.
        /// 
        ///     Will automatically keep track of all previously included JavaScript files, to avoid redundant inclusions, 
        ///     and even include JavaScript files during Ajax requests automatically for you, making sure they're included on the client-side.
        ///     Notice, if you wish, you can supply an absolute URL, to include files from for instance a CDN, or similar types of sources.
        ///     You can also use "ClientScript.GetWebResourceUrl" to include JavaScript files that are embedded resources in other projects.
        /// </summary>
        /// <param name="url">URL to JavaScript to register</param>
        public void IncludeJavaScriptFile (string url)
        {
            // Checking if specified file is already included.
            if (!PersistentJSInclusions.Exists (ix => ix.Item2 && ix.Item1 == url)) {

                // File has not been previously included, making sure we store it in ViewState, to avoid redundant inclusions in the future,
                // before we register it for a "push" to client.
                var file = new Tuple<string, bool> (url, true /* Registering the fact that this is a file */);
                PersistentJSInclusions.Add (file);
                _javaScriptObjectsForCurrentRequest.Add (file);
            }
        }

        /// <summary>
        ///     Registers inline JavaScript "inclusion" for your page.
        /// 
        ///     Notice, a JavaScript "inclusion", is an inline, persistently included piece of JavaScript, which if in the rare occasion of a
        ///     normal PostBack should occur, will be automatically re-included for you, during the rendering of the HTML for your PostBack.
        ///     This allows you to "include" JavaScript, as if it was contained in a static file. 
        ///     In addition, it will make sure that the same JavaScript object, is not included twice.
        /// 
        ///     Notice, if you only want to "send" some JavaScript to the client, and not persistently "include" it, you should rather use the
        ///     "SendJavaScriptToClient" method. This is useful for sending JavaScript to the client, that only should be evaluated once, and not
        ///     persistently included. Examples include for instance to set focus to some widget, in response to some event occurring, etc.
        ///     The latter will not check to see if your JavaScript object has been previously sent to the client.
        /// </summary>
        /// <param name="script">JavaScript to register for inclusion and evaluation on client</param>
        public void IncludeJavaScriptObject (string script)
        {
            // Checking if specified JavaScript inline "inclusion" is already included on page.
            if (!PersistentJSInclusions.Exists (ix => !ix.Item2 && ix.Item1 == script)) {

                // JavaScript inclusion has not been previously included, making sure we store it in ViewState, to avoid redundant inclusions in 
                // the future, before we register it for a "push" to client.
                var jsObject = new Tuple<string, bool> (script, false /* Registering the fact that this is an object */);
                PersistentJSInclusions.Add (jsObject);
                _javaScriptObjectsForCurrentRequest.Add (jsObject);
            }
        }

        /// <summary>
        ///     Sends the given JavaScript to the client for execution.
        /// 
        ///     Notice, this will simply "send" the JavaScript once to the client, as a "burst", and not persistently include it, 
        ///     as a persistent JavaScript object on your page.
        ///     See the comments for "RegisterJavaScript" on why this difference is important to understand.
        /// </summary>
        /// <param name="script">JavaScript to evaluate on client</param>
        public void SendJavaScript (string script)
        {
            SendScripts.Add (script);
        }

        /// <summary>
        ///     Sends an object back to the client as JSON.
        /// 
        ///     Useful if you're having custom JavaScript raising Ajax server-side events, and you wish to return an arbitrary object to your client.
        ///     Allows you to return "custom data" back to the client.
        /// </summary>
        /// <param name="id">ID of object, must be unique for request</param>
        /// <param name="value">Object to serialize back as JSON</param>
        public void SendObject (string id, object value)
        {
            if (!IsAjaxRequest)
                throw new ApplicationException ("You cannot return an object to the client unless you're doign so from within an Ajax request.");
            _changes [id] = value;
        }

        /// <summary>
        ///     Handled to make sure we configure the number of ViewState entries to store in the Session object.
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected override void OnPreInit (EventArgs e)
        {
            // Retrieving viewstate entries per session from web.config, defaulting to 5 if no configuration is found.
            ViewStateSessionEntries = int.Parse (ConfigurationManager.AppSettings [".p5.webapp.viewstate-per-session-entries"] ?? "5");
            base.OnPreInit (e);
        }

        /// <summary>
        ///     Gets the page state persister.
        /// </summary>
        /// <value>The page state persister</value>
        protected override PageStatePersister PageStatePersister
        {
            get {
                // Notice, if consumer of class has declared in his web.config that he wishes to have zero "0" ViewState entries stored in session, 
                // we completely bypass our custom PageStatePersister, and simply returns the base implementation from System.Web.UI,
                // to facilitate for turning OFF server-side ViewState storage entirely.
                if (ViewStateSessionEntries == 0)
                    return base.PageStatePersister;
                return _statePersister ?? (_statePersister = new StatePersister (this, ViewStateSessionEntries));
            }
        }

        /*
         * Contains all changes, and other objects, that needs to be pushed as JSON to client during this request.
         * Has no effect unless we're in an Ajax request.
         */
        internal OrderedDictionary Changes
        {
            get { return _changes; }
        }

        /*
         * Returns the URLs of all CSS files we should persistently include.
         * This is normally only used when we're having a normal postback, or initial request, using e.g. the HtmlFilter for filtering our response.
         * It keeps track of all CSS files included on page, during page's entire lifetime.
         */
        internal List<string> PersistentCSSInclusions
        {
            get {
                if (ViewState ["__p5_css_files"] == null)
                    ViewState ["__p5_css_files"] = new List<string> ();
                return ViewState ["__p5_css_files"] as List<string>;
            }
        }

        /*
         * Returns the JavaScript file URL's we need to push to client for this request.
         * This is normally only used if we're in a JsonFilter type of request, and only returns the new CSS inclusions, that was added during the
         * current request.
         */
        internal IEnumerable<string> CSSInclusionsForCurrentRequest
        {
            get { return _cssFileForCurrentRequest; }
        }

        /*
         * Returns the JavaScript files and objects we need to push to client for this request.
         * This is normally only used if we're in a JsonFilter type of request, and only returns the new JS inclusions, that was added during the
         * current request.
         */
        internal IEnumerable<Tuple<string, bool>> JSInclusionsForCurrentRequest
        {
            get { return _javaScriptObjectsForCurrentRequest; }
        }

        /*
         * Returns all persistently included JavaScript files.
         */
        internal IEnumerable<string> PersistensJSFileInclusions
        {
            get {
                return PersistentJSInclusions.Where (ix => ix.Item2).Select (ix => ix.Item1);
            }
        }

        /*
         * Returns all persistently included JavaScript objects.
         */
        internal IEnumerable<string> PersistensJSObjectInclusions
        {
            get {
                return PersistentJSInclusions.Where (ix => !ix.Item2).Select (ix => ix.Item1);
            }
        }

        /*
         * Used internally to keep track of which JavaScript objects to "send" to client.
         * These are the JavaScript objects that user wants to send to the client, not in a persistent way, but simply as a single one-time send
         * operation.
         */
        internal List<string> SendScripts
        {
            get {
                if (!_changes.Contains ("__p5_scripts"))
                    _changes ["__p5_scripts"] = new List<string> ();
                return _changes ["__p5_scripts"] as List<string>;
            }
        }

        /*
         * Registers a change for a widget to be sent back to the client.
         */
        internal void RegisterWidgetChanges (string widgetID, string attributeName, string value)
        {
            // Making sure our dictionary contains the main root item for sending changes to widgets back to client, before we retrieve the root node.
            if (!_changes.Contains ("__p5_change"))
                _changes ["__p5_change"] = new Dictionary<string, Dictionary<string, object>> ();
            var widgets = _changes ["__p5_change"] as Dictionary<string, Dictionary<string, object>>;

            // Making sure the main root change node contains an entry for the current widget, before retrieving the node for widget.
            if (!widgets.ContainsKey (widgetID))
                widgets [widgetID] = new Dictionary<string, object> ();
            var widgetChanges = widgets [widgetID];

            // Setting the value for the change, such that it can be sent to client during JSON rendering.
            widgetChanges [attributeName] = value;
        }

        /*
         * Registers an attribute of a widget to be deleted.
         */
        internal void RegisterDeletedAttribute (string widgetID, string attributeName)
        {
            // Making sure our dictionary contains the main root item for sending changes to widgets back to client, before we retrieve the root node.
            if (!_changes.Contains ("__p5_change"))
                _changes ["__p5_change"] = new Dictionary<string, Dictionary<string, object>> ();
            var widgets = _changes ["__p5_change"] as Dictionary<string, Dictionary<string, object>>;

            // Making sure the main root change node contains an entry for the current widget, before retrieving the node for widget.
            if (!widgets.ContainsKey (widgetID))
                widgets [widgetID] = new Dictionary<string, object> ();
            var widgetChanges = widgets [widgetID];

            // Making sure the changes contains an entry for deleting attributes, such that we can append the specified attribute to it.
            if (!widgetChanges.ContainsKey ("_p5_del"))
                widgetChanges ["_p5_del"] = new List<string> ();
            var list = widgetChanges ["_p5_del"] as List<string>;
            list.Add (attributeName);
        }

        /*
         * Registers an entire widget for being deleted on the client side.
         */
        internal void RegisterDeletedWidget (string widgetID)
        {
            if (!_changes.Contains ("_p5_del"))
                _changes ["_p5_del"] = new List<string> ();
            var list = _changes ["_p5_del"] as List<string>;
            list.Add (widgetID);
        }

        /*
         * Returns the JavaScript files and objects we need to push to client for each postback.
         * This is normally only used when we're having a normal postback, or initial request, using e.g. the HtmlFilter for filtering our response.
         * It keeps track of all CSS files included on page, during page's entire lifetime.
         */
        private List<Tuple<string, bool>> PersistentJSInclusions
        {
            get {
                if (ViewState ["__p5_js_objects"] == null)
                    ViewState ["__p5_js_objects"] = new List<Tuple<string, bool>> ();
                return ViewState ["__p5_js_objects"] as List<Tuple<string, bool>>;
            }
        }
    }
}
