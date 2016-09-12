/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using p5.exp;
using p5.core;

namespace p5.web.storage {
    /// <summary>
    ///     Helper to retrieve and set Session values
    /// </summary>
    public static class Session
    {
        /// <summary>
        ///     Sets one or more Session object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-session-value", Protection = EventProtection.LambdaClosed)]
        public static void set_session_value (ApplicationContext context, ActiveEventArgs e)
        {
            Collection.Set (context, e.Args, delegate (string key, object value) {

                if (value == null) {

                    // Removing object, if it exists
                    HttpContext.Current.Session.Remove (key);
                } else {

                    // Adding object
                    HttpContext.Current.Session [key] = value;
                }
            }, e.NativeSource);
        }

        /// <summary>
        ///     Retrieves Session object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-session-value", Protection = EventProtection.LambdaClosed)]
        public static void get_session_value (ApplicationContext context, ActiveEventArgs e)
        {
            Collection.Get (context, e.Args, key => HttpContext.Current.Session [key], e.NativeSource);
        }

        /// <summary>
        ///     Lists all keys in the Session object
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-session-keys", Protection = EventProtection.LambdaClosed)]
        public static void list_session_keys (ApplicationContext context, ActiveEventArgs e)
        {
            Collection.List (context, e.Args, HttpContext.Current.Session.Keys, e.NativeSource);
        }
    }
}
