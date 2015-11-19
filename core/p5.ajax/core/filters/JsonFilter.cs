/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using p5 = p5.ajax.widgets;

namespace p5.ajax.core.filters
{
    /// <summary>
    ///     Http response filter for rendering json back to client during ajax requests.
    /// 
    ///     Normally you do not have to fiddle
    ///     with this class, since the framework takes care of initializing instances of this type automatically for you.
    /// </summary>
    public class JsonFilter : Filter
    {
        private readonly string _oldViewState;

        /// <summary>
        ///     Initializes a new instance of the JsonFilter class.
        /// </summary>
        /// <param name="manager">The manager this instance is rendering for.</param>
        public JsonFilter (Manager manager)
            : base (manager)
        {
            Manager.Page.Response.Headers ["Content-Type"] = "application/json";
            _oldViewState = Manager.Page.Request.Params ["__VIEWSTATE"];
        }

        /// <summary>
        ///     Renders the JSON response back to the client.
        /// </summary>
        /// <returns>The response returned back to client.</returns>
        protected override string RenderResponse ()
        {
            TextReader reader = new StreamReader (this, Encoding);
            var content = reader.ReadToEnd ();

            // registering viewstate for change
            var viewstate = GetViewState (content);
            if (!string.IsNullOrEmpty (viewstate)) {
                if (viewstate != _oldViewState) {
                    Manager.RegisterWidgetChanges ("__VIEWSTATE", "value", viewstate, _oldViewState);
                }
            }

            // JavaScript files
            if ((Manager.Page as IAjaxPage).NewJavaScriptToPush.Count > 0) {
                Manager.SendObject ("__p5_js_objects", (Manager.Page as IAjaxPage).JavaScriptToPush);
            }

            // stylesheet files
            if ((Manager.Page as IAjaxPage).NewStylesheetFilesToPush.Count > 0) {
                Manager.SendObject ("__p5_css_files", (Manager.Page as IAjaxPage).NewStylesheetFilesToPush);
            }

            // returning json
            return new JavaScriptSerializer ().Serialize (Manager.Changes);
        }

        /*
         * helper for above
         */
        private string GetViewState (string html)
        {
            var regex = new Regex (@"<input[\s\n]+type=""hidden""[\s\n]+name=""__VIEWSTATE""[\s\n]+id=""__VIEWSTATE""[\s\n]+value=""(.+[^""])""[\s\n]*/>", RegexOptions.Compiled);
            var match = regex.Match (html);
            return match.Groups [1].Value;
        }
    }
}