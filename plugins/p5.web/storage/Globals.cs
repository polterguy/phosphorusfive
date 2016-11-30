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

using System.Web;
using p5.exp;
using p5.core;

/// <summary>
///     Main namespace for everything related to Web User Interface
/// </summary>
namespace p5.web.storage
{
    /// <summary>
    ///     Helper to retrieve and set global application wide values
    /// </summary>
    public static class Globals
    {
        /// <summary>
        ///     Sets one or more global application wide object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-global-value")]
        [ActiveEvent (Name = ".set-global-value")]
        public static void set_global_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.SetCollection (context, e.Args, delegate (string key, object value) {
                if (value == null) {

                    // Removal
                    HttpContext.Current.Application.Remove (key);
                } else {

                    // Setting or updating
                    HttpContext.Current.Application[key] = value;
                }
            }, e.Name.StartsWith ("."));
        }

        /// <summary>
        ///     Retrieves global application wide object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-global-value")]
        [ActiveEvent (Name = ".get-global-value")]
        public static void get_global_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.GetCollection (context, e.Args, key => HttpContext.Current.Application [key], e.Name.StartsWith ("."));
        }

        /// <summary>
        ///     Lists all keys in the global application wide object storage
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-global-keys")]
        [ActiveEvent (Name = ".list-global-keys")]
        public static void list_global_keys (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.ListCollection (context, e.Args, HttpContext.Current.Application.AllKeys, e.Name.StartsWith ("."));
        }
    }
}