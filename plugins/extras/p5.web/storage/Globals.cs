/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using p5.exp;
using p5.core;

/// <summary>
///     Main namespace for everything related to Web User Interface
/// </summary>
namespace p5.web.storage
{
    /// <summary>
    ///     Helper to retrieve and set global application wide values
    /// </summary>
    public static class Globals
    {
        /// <summary>
        ///     Sets one or more global application wide object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-global-value")]
        public static void set_global_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.SetCollection (context, e.Args, delegate (string key, object value) {
                if (value == null) {

                    // Removal
                    HttpContext.Current.Application.Remove (key);
                } else {

                    // Setting or updating
                    HttpContext.Current.Application[key] = value;
                }
            });
        }

        /// <summary>
        ///     Retrieves global application wide object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-global-value")]
        public static void get_global_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.GetCollection (context, e.Args, key => HttpContext.Current.Application [key]);
        }

        /// <summary>
        ///     Lists all keys in the global application wide object storage
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-global-keys")]
        public static void list_global_keys (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.ListCollection (context, e.Args, HttpContext.Current.Application.AllKeys);
        }
    }
}