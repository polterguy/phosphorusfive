/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using System.Collections;
using System.Collections.Generic;
using p5.exp;
using p5.core;

namespace p5.web.ui
{
    /// <summary>
    ///     Helper to retrieve and set Cache values.
    /// 
    ///     Allows for you to retrieve and set items in your Cache object.
    /// 
    ///     The Cache object is a "global shared" object between all sessions, visitors and user of your web site, 
    ///     and allowss for you to share and cache information between different users of your web site.
    /// </summary>
    public static class Cache
    {
        /// <summary>
        ///     Sets one or more Cache object(s).
        /// 
        ///     Where [source], or [src], becomes the nodes that are stored in the cache. The main node's value(s), becomes
        ///     the key your items are stored with.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-cache-value", Protection = EventProtection.LambdaClosed)]
        private static void set_cache_value (ApplicationContext context, ActiveEventArgs e)
        {
            p5.exp.CollectionBase.Set (context, e.Args, delegate (string key, object value) {
                if (value == null) {
                    // removing object, if it exists
                    HttpContext.Current.Cache.Remove (key);
                } else {
                    // adding object
                    HttpContext.Current.Cache [key] = value;
                }
            });
        }

        /// <summary>
        ///     Retrieves Cache object(s).
        /// 
        ///     Supply one or more keys to which items you wish to retrieve as the value of your main node.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-cache-value", Protection = EventProtection.LambdaClosed)]
        private static void get_cache_value (ApplicationContext context, ActiveEventArgs e)
        {
            p5.exp.CollectionBase.Get (context, e.Args, key => HttpContext.Current.Cache [key]);
        }

        /// <summary>
        ///     Lists all keys in the Cache object.
        /// 
        ///     Returns all keys for all items in your Cache object.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-cache-keys", Protection = EventProtection.LambdaClosed)]
        private static void list_cache_keys (ApplicationContext context, ActiveEventArgs e)
        {
            List<string> retVal = new List<string> ();
            foreach (IDictionaryEnumerator idx in HttpContext.Current.Cache) {
                retVal.Add (idx.Key.ToString ());
            }
            p5.exp.CollectionBase.List (context, e.Args, () => retVal);
        }
    }
}
