/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using System.Collections;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.web.ui.Common;

// ReSharper disable UnusedMember.Local

namespace phosphorus.web.ui
{
    /// <summary>
    ///     Helper to retrieve and set cache values
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public static class Cache
    {
        /// <summary>
        ///     Sets one or more cache object(s) where [source], or [src], becomes the nodes that are stored in the cache.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.cache.set")]
        private static void pf_web_cache_set (ApplicationContext context, ActiveEventArgs e)
        {
            phosphorus.web.ui.Common.CollectionBase.Set (e.Args, context, delegate (string key, object value) {
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
        ///     Returns the cache object(s) given through the value(s) of the main node.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.cache.get")]
        private static void pf_web_cache_get (ApplicationContext context, ActiveEventArgs e)
        {
            phosphorus.web.ui.Common.CollectionBase.Get (e.Args, context, key => HttpContext.Current.Cache [key]);
        }

        /// <summary>
        ///     Lists all keys in the cache.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.cache.list")]
        private static void pf_web_cache_list (ApplicationContext context, ActiveEventArgs e)
        {
            List<string> retVal = new List<string> ();
            foreach (IDictionaryEnumerator idx in HttpContext.Current.Cache) {
                retVal.Add (idx.Key.ToString ());
            }
            phosphorus.web.ui.Common.CollectionBase.List (e.Args, context, () => retVal);
        }
    }
}