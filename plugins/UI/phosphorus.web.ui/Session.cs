/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Linq;
using System.Web;
using phosphorus.core;
using phosphorus.web.ui.Common;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.web.ui
{
    /// <summary>
    ///     Helper to retrieve and set Session values.
    /// 
    ///     Allows for you to retrieve and set items in your Session object.
    /// 
    ///     The Session object, is an object that can store items locally on a session basis for each user of your web site.
    /// </summary>
    public static class Session
    {
        /// <summary>
        ///     Sets one or more Session object(s).
        /// 
        ///     Where [source], or [src], becomes the nodes that are stored in the session. The main node's value(s), becomes
        ///     the key your items are stored with.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.session.set")]
        private static void pf_web_session_set (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.Set (e.Args, context, delegate (string key, object value) {
                if (value == null) {
                    // removing object, if it exists
                    HttpContext.Current.Session.Remove (key);
                } else {
                    // adding object
                    HttpContext.Current.Session [key] = value;
                }
            });
        }

        /// <summary>
        ///     Retrieves Session object(s).
        /// 
        ///     Supply one or more keys to which items you wish to retrieve as the value of your main node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.session.get")]
        private static void pf_web_session_get (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.Get (e.Args, context, key => HttpContext.Current.Session [key]);
        }

        /// <summary>
        ///     Lists all keys in the Session object.
        /// 
        ///     Returns all keys for all items in your Session object.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.session.list")]
        private static void pf_web_session_list (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.List (e.Args, context, () => (from object idx in HttpContext.Current.Session.Keys select idx.ToString ()).ToList ());
        }
    }
}