/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using p5.core;
using p5.exp;

namespace p5.web.ui.request
{
    /// <summary>
    ///     Helper to retrieve HTTP headers
    /// </summary>
    public static class Headers
    {
        /// <summary>
        ///     Returns one or more HTTP request header(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-http-header", Protection = EventProtection.LambdaClosed)]
        public static void get_http_header (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.Get (context, e.Args, key => HttpContext.Current.Request.Headers [key], e.NativeSource);
        }

        /// <summary>
        ///     Lists all keys for our HTTP headers
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-http-headers", Protection = EventProtection.LambdaClosed)]
        public static void list_http_headers (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.List (context, e.Args, HttpContext.Current.Request.Headers.AllKeys, e.NativeSource);
        }
    }
}
