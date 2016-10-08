/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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

using System.Web;
using p5.exp;
using p5.core;

namespace p5.web.ui.response
{
    /// <summary>
    ///     Helper class to manipulate the HTTP response headers
    /// </summary>
    public static class Headers
    {
        /// <summary>
        ///     Changes the HTTP headers for the current response
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-http-header")]
        public static void set_http_header (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.SetCollection (context, e.Args, delegate (string key, object value) {
                if (value == null) {

                    // Removal
                    HttpContext.Current.Response.Headers.Remove (key);
                } else {

                    // Setting or updating
                    HttpContext.Current.Response.AddHeader (key, Utilities.Convert<string> (context, value));
                }
            }, true);
        }
    }
}
