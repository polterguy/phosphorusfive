/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Web;
using System.Linq;
using p5.exp;
using p5.core;

namespace p5.web.ui
{
    /// <summary>
    ///     Helper to retrieve and set Session values.
    /// </summary>
    public static class Session
    {
        /// <summary>
        ///     Sets one or more Session object(s).
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-session-value", Protection = EventProtection.LambdaClosed)]
        private static void set_session_value (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.Set (context, e.Args, delegate (string key, object value) {

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
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-session-value", Protection = EventProtection.LambdaClosed)]
        private static void get_session_value (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.Get (context, e.Args, key => HttpContext.Current.Session [key]);
        }

        /// <summary>
        ///     Lists all keys in the Session object.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-session-keys", Protection = EventProtection.LambdaClosed)]
        private static void list_session_keys (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.List (context, e.Args, HttpContext.Current.Session.Keys);
        }
    }
}
