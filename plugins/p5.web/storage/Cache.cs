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

using System;
using System.Web;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using p5.exp;
using p5.core;

namespace p5.web.storage
{
    /// <summary>
    ///     Helper to retrieve and set Cache values
    /// </summary>
    public static class Cache
    {
        /// <summary>
        ///     Sets one or more Cache object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-cache-value")]
        [ActiveEvent (Name = ".set-cache-value")]
        public static void set_cache_value (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving for how long the value should be set in cache, before removed
            var minutes = e.Args.GetExChildValue ("minutes", context, 30);

            // Settings cache value
            XUtil.Set (context, e.Args, delegate (string key, object value) {
                if (value == null) {

                    // Removal
                    HttpContext.Current.Cache.Remove (key);
                } else {

                    // Setting or updating
                    HttpContext.Current.Cache.Insert (
                        key,
                        value,
                        null,
                        DateTime.Now.AddMinutes (minutes),
                        System.Web.Caching.Cache.NoSlidingExpiration);
                }
            }, "minutes");
        }

        /// <summary>
        ///     Retrieves Cache object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-cache-value")]
        [ActiveEvent (Name = ".get-cache-value")]
        public static void get_cache_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.Get (context, e.Args, key => HttpContext.Current.Cache [key]);
        }

        /// <summary>
        ///     Lists all keys in the Cache object
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-cache-keys")]
        [ActiveEvent (Name = ".list-cache-keys")]
        public static void list_cache_keys (ApplicationContext context, ActiveEventArgs e)
        {
            var retVal = new List<string> ();
            foreach (DictionaryEntry idx in HttpContext.Current.Cache) {
                retVal.Add (idx.Key.ToString ());
            }
            XUtil.List (context, e.Args, retVal);
        }
    }
}
