/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using p5.exp;
using p5.core;

namespace p5.web.storage {
    /// <summary>
    ///     Helper to retrieve and set HttpContext values
    /// </summary>
    public static class Context
    {
        /// <summary>
        ///     Sets one or more HttpContext object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-context-value")]
        public static void set_context_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.SetCollection (context, e.Args, delegate (string key, object value) {
                if (value == null) {

                    // Removal
                    HttpContext.Current.Items.Remove (key);
                } else {

                    // Setting or updating
                    HttpContext.Current.Items[key] = value;
                }
            });
        }

        /// <summary>
        ///     Retrieves HttpContext object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-context-value")]
        public static void get_context_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.GetCollection (context, e.Args, key => HttpContext.Current.Items [key]);
        }

        /// <summary>
        ///     Lists all keys in the HttpContext object
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-context-keys")]
        public static void list_context_keys (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.ListCollection (context, e.Args, HttpContext.Current.Items.Keys);
        }
    }
}
