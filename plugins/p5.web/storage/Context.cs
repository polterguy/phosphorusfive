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

namespace p5.web.storage {
    /// <summary>
    ///     Helper to retrieve and set HttpContext values
    /// </summary>
    public static class Context
    {
        /// <summary>
        ///     Sets one or more HttpContext object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-context-value")]
        [ActiveEvent (Name = ".set-context-value")]
        public static void set_context_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.SetCollection (context, e.Args, delegate (string key, object value) {
                if (value == null) {

                    // Removal
                    HttpContext.Current.Items.Remove (key);
                } else {

                    // Setting or updating
                    HttpContext.Current.Items[key] = value;
                }
            }, e.Name.StartsWith ("."));
        }

        /// <summary>
        ///     Retrieves HttpContext object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-context-value")]
        [ActiveEvent (Name = ".get-context-value")]
        public static void get_context_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.GetCollection (context, e.Args, key => HttpContext.Current.Items [key], e.Name.StartsWith ("."));
        }

        /// <summary>
        ///     Lists all keys in the HttpContext object
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-context-keys")]
        [ActiveEvent (Name = ".list-context-keys")]
        public static void list_context_keys (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.ListCollection (context, e.Args, HttpContext.Current.Items.Keys, e.Name.StartsWith ("."));
        }
    }
}
