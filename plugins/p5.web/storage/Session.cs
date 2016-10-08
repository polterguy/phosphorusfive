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

using System.Web;
using p5.exp;
using p5.core;

namespace p5.web.storage
{
    /// <summary>
    ///     Helper to retrieve and set Session values
    /// </summary>
    public static class Session
    {
        /// <summary>
        ///     Sets one or more Session object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-session-value")]
        [ActiveEvent (Name = ".set-session-value")]
        public static void set_session_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.SetCollection (context, e.Args, delegate (string key, object value) {
                if (value == null) {

                    // Removal
                    HttpContext.Current.Session.Remove (key);
                } else {

                    // Setting or updating
                    HttpContext.Current.Session[key] = value;
                }
            }, e.Name.StartsWith ("."));
        }

        /// <summary>
        ///     Retrieves Session object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-session-value")]
        [ActiveEvent (Name = ".get-session-value")]
        public static void get_session_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.GetCollection (context, e.Args, key => HttpContext.Current.Session [key], e.Name.StartsWith ("."));
        }

        /// <summary>
        ///     Lists all keys in the Session object
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-session-keys")]
        [ActiveEvent (Name = ".list-session-keys")]
        public static void list_session_keys (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.ListCollection (context, e.Args, HttpContext.Current.Session.Keys, e.Name.StartsWith ("."));
        }
    }
}
