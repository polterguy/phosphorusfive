/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Web.UI;
using phosphorus.ajax.core.internals;

// ReSharper disable PossibleNullReferenceException

namespace phosphorus.ajax.core
{
    /// <summary>
    ///     helper class for inheriting your page from. if you do not wish to inherit from this class,
    ///     you can implement the <see cref="phosphorus.ajax.core.IAjaxPage" /> interface on your page instead,
    ///     and create an instance of the <see cref="phosphorus.ajax.core.Manager" /> for instance during the
    ///     initialization of your page
    /// </summary>
    public class AjaxPage : Page, IAjaxPage
    {
        private readonly List<string> _javaScriptFilesToPush = new List<string> ();
        private readonly List<string> _stylesheetFilesToPush = new List<string> ();
        private PageStatePersister _statePersister;

        /// <summary>
        ///     maximum number of ViewState entries in Session. if this is zero, ViewState will not be stored in Session,
        ///     but sent back and forth between client and browser as usual
        /// </summary>
        /// <value>the number of valid viewstate entries for each session</value>
        public int ViewStateSessionEntries { get;
            // ReSharper disable once MemberCanBeProtected.Global
            set; }

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                if (ViewStateSessionEntries == 0)
                    return base.PageStatePersister;
                return _statePersister ?? (_statePersister = new StatePersister (this, ViewStateSessionEntries));
            }
        }

        /// <summary>
        ///     returns the ajax manager for your page
        /// </summary>
        /// <value>the ajax manager</value>
        public Manager Manager { get; private set; }

        /// <summary>
        ///     registers JavaScript for page
        /// </summary>
        /// <param name="url">url to JavaScript to register</param>
        public void RegisterJavaScriptFile (string url)
        {
            if (ViewState ["__pf_js_files"] == null)
                ViewState ["__pf_js_files"] = new List<string> ();
            var lst = ViewState ["__pf_js_files"] as List<string>;
            if (!lst.Contains (url)) {
                lst.Add (url);
                _javaScriptFilesToPush.Add (url);
            }
        }

        /*
         * returns the JavaScript file URL's we need to push to client during this request
         */

        List<string> IAjaxPage.JavaScriptFilesToPush
        {
            get { return _javaScriptFilesToPush; }
        }

        /*
         * returns the JavaScript file URL's we need to push to client during this request
         */

        List<string> IAjaxPage.StylesheetFilesToPush
        {
            get { return _stylesheetFilesToPush; }
        }

        protected override void OnPreInit (EventArgs e)
        {
            Manager = new Manager (this);
            base.OnPreInit (e);
        }

        /// <summary>
        ///     registers stylesheet for page
        /// </summary>
        /// <param name="url">url to stylesheet to register</param>
        protected void RegisterStylesheetFile (string url)
        {
            if (ViewState ["__pf_css_files"] == null)
                ViewState ["__pf_css_files"] = new List<string> ();
            var lst = ViewState ["__pf_css_files"] as List<string>;
            if (!lst.Contains (url)) {
                lst.Add (url);
                _stylesheetFilesToPush.Add (url);
            }
        }
    }
}