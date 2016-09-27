/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
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
        public static void set_cache_value (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving for how long the value should be set in cache, before removed
            var minutes = e.Args.GetExChildValue ("minutes", context, 30);

            // Settings cache value
            XUtil.SetCollection (context, e.Args, delegate (string key, object value) {
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
            }, new string[] { "minutes" }.ToList ());
        }

        /// <summary>
        ///     Retrieves Cache object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-cache-value")]
        public static void get_cache_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.GetCollection (context, e.Args, key => HttpContext.Current.Cache [key]);
        }

        /// <summary>
        ///     Lists all keys in the Cache object
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-cache-keys")]
        public static void list_cache_keys (ApplicationContext context, ActiveEventArgs e)
        {
            var retVal = new List<string> ();
            foreach (DictionaryEntry idx in HttpContext.Current.Cache) {
                retVal.Add (idx.Key.ToString ());
            }
            XUtil.ListCollection (context, e.Args, retVal);
        }
    }
}
