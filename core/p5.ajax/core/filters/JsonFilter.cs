/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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

using System.Linq;
using System.Web.Script.Serialization;

namespace p5.ajax.core.filters
{
    /// <summary>
    ///     HTTP response filter for rendering JSON back to client during Ajax requests.
    /// </summary>
    public class JsonFilter : Filter
    {
        /// <summary>
        ///     Initializes a new instance of the JsonFilter class.
        /// </summary>
        /// <param name="page">The AjaxPage this instance is rendering for</param>
        public JsonFilter (AjaxPage page)
            : base (page)
        {
            Page.Response.Headers ["Content-Type"] = "application/json";
        }

        /// <summary>
        ///     Renders the JSON response back to the client.
        /// </summary>
        /// <returns>The response returned back to client</returns>
        protected override string RenderResponse ()
        {
            // JavaScript files.
            if (Page.JSInclusionsForCurrentRequest.Any ()) {
                Page.Changes ["__p5_js_objects"] = Page.JSInclusionsForCurrentRequest;
            }

            // Stylesheet files.
            if (Page.CSSInclusionsForCurrentRequest.Any ()) {
                Page.Changes ["__p5_css_files"] = Page.CSSInclusionsForCurrentRequest;
            }

            // Returning JSON.
            return new JavaScriptSerializer ().Serialize (Page.Changes);
        }
    }
}