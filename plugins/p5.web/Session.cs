/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Web;
using System.Linq;
using p5.core;
using p5.web.ui.common;

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
        [ActiveEvent (Name = "set-session", Protection = EventProtection.LambdaClosed)]
        private static void set_session (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.Set (e.Args, context, delegate (string key, object value) {

                // Verifying this is not a "protected session object"
                if (key.StartsWith ("_"))
                    throw new ApplicationException (string.Format ("You cannot set session value '{0}' since it is protected", key));

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
        [ActiveEvent (Name = "get-session", Protection = EventProtection.LambdaClosed)]
        private static void get_session (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.Get (e.Args, context, key => HttpContext.Current.Session [key]);
            e.Args.RemoveAll (ix => ix.Name.StartsWith ("_"));
        }

        /// <summary>
        ///     Lists all keys in the Session object.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-session", Protection = EventProtection.LambdaClosed)]
        private static void list_session (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.List (e.Args, context, () => (from object idx in HttpContext.Current.Session.Keys select idx.ToString ()).ToList ());
            e.Args.RemoveAll (ix => ix.Name.StartsWith ("_"));
        }
    }
}
