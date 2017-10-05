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

namespace p5.web.storage
{
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
        [ActiveEvent (Name = "p5.web.context.set")]
        [ActiveEvent (Name = ".p5.web.context.set")]
        public static void p5_web_context_set (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.Set (context, e.Args, delegate (string key, object value) {
                if (value == null) {

                    // Removal
                    HttpContext.Current.Items.Remove (key);

                } else {

                    // Setting or updating
                    HttpContext.Current.Items [key] = value;
                }
            });
        }

        /// <summary>
        ///     Retrieves HttpContext object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.context.get")]
        [ActiveEvent (Name = ".p5.web.context.get")]
        public static void p5_web_context_get (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.Get (context, e.Args, key => HttpContext.Current.Items [key]);
        }

        /// <summary>
        ///     Lists all keys in the HttpContext object
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.context.list")]
        [ActiveEvent (Name = ".p5.web.context.list")]
        public static void p5_web_context_list (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.List (context, e.Args, HttpContext.Current.Items.Keys);
        }
    }
}
