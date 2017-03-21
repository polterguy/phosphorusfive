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
    ///     Helper to retrieve and set global application wide values
    /// </summary>
    public static class Globals
    {
        /// <summary>
        ///     Sets one or more global application wide object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.application.set")]
        [ActiveEvent (Name = ".p5.web.application.set")]
        public static void p5_web_application_set (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.Set (context, e.Args, delegate (string key, object value) {
                if (value == null) {

                    // Removal
                    HttpContext.Current.Application.Remove (key);
                } else {

                    // Setting or updating
                    HttpContext.Current.Application[key] = value;
                }
            });
        }

        /// <summary>
        ///     Retrieves global application wide object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.application.get")]
        [ActiveEvent (Name = ".p5.web.application.get")]
        public static void p5_web_application_get (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.Get (context, e.Args, key => HttpContext.Current.Application [key]);
        }

        /// <summary>
        ///     Lists all keys in the global application wide object storage
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.application.list")]
        [ActiveEvent (Name = ".p5.web.application.list")]
        public static void p5_web_application_list (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.List (context, e.Args, HttpContext.Current.Application.AllKeys);
        }
    }
}