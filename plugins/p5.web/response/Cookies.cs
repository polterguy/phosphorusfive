/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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
using System.Web;
using System.Linq;
using p5.exp;
using p5.core;

/// <summary>
///     Main namespace for everything related to the current HTTP response
/// </summary>
namespace p5.web.ui.response
{
    /// <summary>
    ///     Helper to retrieve and set Cookie values
    /// </summary>
    public static class Cookies
    {
        /// <summary>
        ///     Sets one or more Cookie object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-cookie-value")]
        [ActiveEvent (Name = ".set-cookie-value")]
        public static void set_cookie_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.SetCollection (context, e.Args, delegate (string key, object value) {
                if (value == null) {

                    // Removal
                    var httpCookie = HttpContext.Current.Response.Cookies[key];
                    if (httpCookie != null) httpCookie.Expires = DateTime.Now.Date.AddDays (-1);
                } else {

                    // Setting or updating
                    HttpContext.Current.Response.Cookies.Add (CreateCookieFromNode (e.Args, context, key, value));
                }
            }, e.Name.StartsWith ("."), new string[] { "duration", "http-only" }.ToList ());
        }

        /*
         * Creates a cookie from given Node and returns back to caller
         */
        private static HttpCookie CreateCookieFromNode (Node node, ApplicationContext context, string name, object value)
        {
            // Creating cookie to send back to caller
            var retVal = new HttpCookie (name, HttpUtility.UrlEncode (Utilities.Convert<string> (context, value))) {
                Expires = DateTime.Now.Date.AddDays (node.GetChildValue ("duration", context, 365)),
                HttpOnly = node.GetChildValue ("http-only", context, true)
            };

            // Returning cookie (or null) back to caller
            return retVal;
        }
    }
}
