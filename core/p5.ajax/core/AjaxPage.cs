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
using System.Web.UI;
using System.Configuration;
using System.Collections.Generic;
using p5.ajax.core.internals;

/// <summary>
///     Contains all Ajax functionality in Phosphorus Five
/// </summary>
namespace p5.ajax
{
    /// <summary>
    ///     Contains all core functionality in Phosphorus Ajax
    /// </summary>
    namespace core
    {
        /// <summary>
        ///     Helper class for implementing core Ajax functionality on your page
        /// </summary>
        public class AjaxPage : Page, IAjaxPage
        {
            private PageStatePersister _statePersister;
            private List<Tuple<string, bool>> _newJavaScriptObjects = new List<Tuple<string, bool>> ();
            private List<string> _newCssFiles = new List<string> ();

            /// <summary>
            ///     Maximum number of ViewState entries in Session
            /// </summary>
            /// <value>The number of valid viewstate entries for each session</value>
            public int ViewStateSessionEntries { get; set; }

            /// <summary>
            ///     Gets the page state persister
            /// </summary>
            /// <value>The page state persister</value>
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
            ///     Returns the ajax manager for your page
            /// </summary>
            /// <value>the ajax manager</value>
            public Manager Manager
            {
                get;
                private set;
            }

            /// <summary>
            ///     Registers JavaScript files for your page
            /// </summary>
            /// <param name="url">url to JavaScript to register</param>
            public void RegisterJavaScriptFile (string url)
            {
                if (ViewState ["_p5_js_objects"] == null)
                    ViewState ["_p5_js_objects"] = new List<Tuple<string, bool>> ();
                var lst = ViewState ["_p5_js_objects"] as List<Tuple<string, bool>>;
                if (lst.Find (delegate (Tuple<string, bool> idx) { return idx.Item1 == url; }) == null) {
                    lst.Add (new Tuple<string, bool>(url, true));
                    _newJavaScriptObjects.Add (new Tuple<string, bool>(url, true));
                }
            }
            
            /// <summary>
            ///     Registers JavaScript for your page
            /// </summary>
            /// <param name="url">url to JavaScript to register</param>
            public void RegisterJavaScript (string script)
            {
                if (ViewState ["_p5_js_objects"] == null)
                    ViewState ["_p5_js_objects"] = new List<Tuple<string, bool>> ();
                var lst = ViewState ["_p5_js_objects"] as List<Tuple<string, bool>>;
                if (lst.Find (delegate (Tuple<string, bool> idx) { return idx.Item1 == script; }) == null) {
                    lst.Add (new Tuple<string, bool>(script, false));
                    _newJavaScriptObjects.Add (new Tuple<string, bool>(script, false));
                }
            }

            /// <summary>
            ///     Registers stylesheet file for your page
            /// </summary>
            /// <param name="url">url to stylesheet to register</param>
            public void RegisterStylesheetFile (string url)
            {
                var lst = (this as IAjaxPage).StylesheetFilesToPush;
                if (!lst.Contains (url)) {
                    lst.Add (url);
                    _newCssFiles.Add (url);
                }
            }

            /*
             * Returns the JavaScript file URL's we need to push to client for each postback
             */
            List<Tuple<string, bool>> IAjaxPage.JavaScriptToPush
            {
                get { return ViewState ["_p5_js_objects"] as List<Tuple<string, bool>>; }
            }

            /*
             * Returns the JavaScript file URL's we need to push to client for this request
             */
            List<Tuple<string, bool>> IAjaxPage.NewJavaScriptToPush
            {
                get { return _newJavaScriptObjects; }
            }

            /*
             * Returns the CSS file URL's we need to push to client for each postback
             */
            List<string> IAjaxPage.StylesheetFilesToPush
            {
                get {
                    if (ViewState["_p5_css_files"] == null)
                        ViewState["_p5_css_files"] = new List<string> ();
                    return ViewState ["_p5_css_files"] as List<string>;
                }
            }

            /*
             * Returns the JavaScript file URL's we need to push to client for this request
             */
            List<string> IAjaxPage.NewStylesheetFilesToPush
            {
                get { return _newCssFiles; }
            }

            protected override void OnPreInit (EventArgs e)
            {
                Manager = new Manager (this);

                // Retrieving viewstate entries per session
                ViewStateSessionEntries = int.Parse (ConfigurationManager.AppSettings [".p5.webapp.viewstate-per-session-entries"] ?? "5");

                base.OnPreInit (e);
            }
        }
    }
}
