/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
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
        [ActiveEvent (Name = "set-cache-value", Protection = EventProtection.LambdaClosed)]
        public static void set_cache_value (ApplicationContext context, ActiveEventArgs e)
        {
            p5.exp.CollectionBase.Set (context, e.Args, delegate (string key, object value) {
                if (value == null) {

                    // Removing object, if it exists
                    HttpContext.Current.Cache.Remove (key);
                } else {

                    // Adding object
                    HttpContext.Current.Cache [key] = value;
                }
            });
        }

        /// <summary>
        ///     Retrieves Cache object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-cache-value", Protection = EventProtection.LambdaClosed)]
        public static void get_cache_value (ApplicationContext context, ActiveEventArgs e)
        {
            p5.exp.CollectionBase.Get (context, e.Args, key => HttpContext.Current.Cache [key]);
        }

        /// <summary>
        ///     Lists all keys in the Cache object
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-cache-keys", Protection = EventProtection.LambdaClosed)]
        public static void list_cache_keys (ApplicationContext context, ActiveEventArgs e)
        {
            var retVal = new List<string> ();
            foreach (DictionaryEntry idx in HttpContext.Current.Cache) {
                retVal.Add (idx.Key.ToString ());
            }
            p5.exp.CollectionBase.List (context, e.Args, retVal);
        }
    }
}
