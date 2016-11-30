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

using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace p5.ajax.core.filters
{
    /// <summary>
    ///     Http response filter for rendering json back to client during ajax requests
    /// </summary>
    public class JsonFilter : Filter
    {
        private readonly string _oldViewState;

        /// <summary>
        ///     Initializes a new instance of the JsonFilter class
        /// </summary>
        /// <param name="manager">The manager this instance is rendering for</param>
        public JsonFilter (Manager manager)
            : base (manager)
        {
            Manager.Page.Response.Headers ["Content-Type"] = "application/json";
            _oldViewState = Manager.Page.Request.Params ["__VIEWSTATE"];
        }

        /// <summary>
        ///     Renders the JSON response back to the client
        /// </summary>
        /// <returns>The response returned back to client</returns>
        protected override string RenderResponse ()
        {
            TextReader reader = new StreamReader (this, ContentEncoding);
            var content = reader.ReadToEnd ();

            // Registering viewstate for change
            var viewstate = GetViewState (content);
            if (!string.IsNullOrEmpty (viewstate)) {
                if (viewstate != _oldViewState) {
                    Manager.RegisterWidgetChanges ("__VIEWSTATE", "value", viewstate, _oldViewState);
                }
            }

            // JavaScript files
            if ((Manager.Page as IAjaxPage).NewJavaScriptToPush.Count > 0) {
                Manager.SendObject ("_p5_js_objects", (Manager.Page as IAjaxPage).NewJavaScriptToPush);
            }

            // Stylesheet files
            if ((Manager.Page as IAjaxPage).NewStylesheetFilesToPush.Count > 0) {
                Manager.SendObject ("_p5_css_files", (Manager.Page as IAjaxPage).NewStylesheetFilesToPush);
            }

            // Returning json
            return new JavaScriptSerializer ().Serialize (Manager.Changes);
        }

        /*
         * Helper for above
         */
        private string GetViewState (string html)
        {
            var regex = new Regex (@"<input[\s\n]+type=""hidden""[\s\n]+name=""__VIEWSTATE""[\s\n]+id=""__VIEWSTATE""[\s\n]+value=""(.+[^""])""[\s\n]*/>", RegexOptions.Compiled);
            var match = regex.Match (html);
            return match.Groups [1].Value;
        }
    }
}