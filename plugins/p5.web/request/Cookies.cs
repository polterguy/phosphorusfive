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

using System.Web;
using p5.exp;
using p5.core;

namespace p5.web.ui.request
{
    /// <summary>
    ///     Helper to retrieve and set Cookie values
    /// </summary>
    public static class Cookies
    {
        /// <summary>
        ///     Retrieves Cookie object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.cookie.get")]
        [ActiveEvent (Name = ".p5.web.cookie.get")]
        public static void p5_web_cookie_get (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.Get (context, e.Args, delegate (string key) {

                // Fetching cookie
                var cookie = HttpContext.Current.Request.Cookies.Get (key);
                if (cookie != null && !string.IsNullOrEmpty (cookie.Value)) {

                    // Adding key node, and values beneath key node
                    return HttpUtility.UrlDecode (cookie.Value);
                }
                return null;
            });
        }

        /// <summary>
        ///     Lists all keys in the Cookie object of client
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.cookie.list")]
        [ActiveEvent (Name = ".p5.web.cookie.list")]
        public static void p5_web_cookie_list (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.List (context, e.Args, HttpContext.Current.Request.Cookies.AllKeys);
        }
    }
}
