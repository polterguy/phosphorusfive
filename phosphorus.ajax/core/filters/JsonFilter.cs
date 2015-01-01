/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using phosphorus.ajax.core;
using pf = phosphorus.ajax.widgets;

namespace phosphorus.ajax.core.filters
{
    /// <summary>
    /// the http response filter for rendering json back to client in ajax requests
    /// </summary>
    public class JsonFilter : Filter
    {
        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.core.filters.JsonFilter"/> class
        /// </summary>
        /// <param name="manager">the manager this instance is rendering for</param>
        public JsonFilter (Manager manager)
            : base (manager)
        { }

        /// <summary>
        /// renders the response
        /// </summary>
        /// <returns>the response returned back to client</returns>
        protected override string RenderResponse ()
        {
            Manager.Page.Response.Headers ["Content-Type"] = "application/json";

            // TODO: pass in ContentEncoding directly in ctor, since Response is not necessarily available in this context (.net)
            TextReader reader = new StreamReader (this, Manager.Page.Response.ContentEncoding);
            string content = reader.ReadToEnd ();

            // registering viewstate for change
            string viewstate = GetViewState (content);
            if (!string.IsNullOrEmpty (viewstate)) {
                string oldViewstate = Manager.Page.Request.Params ["__VIEWSTATE"];
                if (viewstate != oldViewstate) {
                    Manager.RegisterWidgetChanges ("__VIEWSTATE", "value", viewstate, oldViewstate);
                }
            }

            if ((Manager.Page as IAjaxPage).JavaScriptFilesToPush.Count > 0) {
                Manager.SendObject ("__pf_js_files", (Manager.Page as IAjaxPage).JavaScriptFilesToPush);
            }

            // returning json
            return new JavaScriptSerializer ().Serialize (Manager.Changes);
        }

        private string GetViewState (string html)
        {
            Regex regex = new Regex (@"<input[\s\n]+type=""hidden""[\s\n]+name=""__VIEWSTATE""[\s\n]+id=""__VIEWSTATE""[\s\n]+value=""(.+[^""])""[\s\n]*/>", RegexOptions.Compiled);
            Match match = regex.Match (html);
            return match.Groups[1].Value;
        }
    }
}

