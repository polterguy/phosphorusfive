/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Web;
using p5.exp;
using p5.core;

/// <summary>
///     Main namespace for everything related to the current HTTP response
/// </summary>
namespace p5.web.ui.response
{
    /// <summary>
    ///     Helper to retrieve and set Cookie values
    /// </summary>
    public static class Cookies
    {
        /// <summary>
        ///     Sets one or more Cookie object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-cookie", Protection = EventProtection.LambdaClosed)]
        public static void set_cookie (ApplicationContext context, ActiveEventArgs e)
        {
            Collection.Set (context, e.Args, delegate (string key, object value) {
                if (value == null) {

                    // Removing existing cookie
                    var httpCookie = HttpContext.Current.Response.Cookies [key];
                    if (httpCookie != null) httpCookie.Expires = DateTime.Now.Date.AddDays (-1);
                } else {

                    // Creating cookie
                    HttpContext.Current.Response.Cookies.Add (CreateCookieFromNode (e.Args, context, key, value));
                }
            }, e.NativeSource);
        }

        /*
         * Creates a cookie from given Node and returns back to caller
         */
        private static HttpCookie CreateCookieFromNode (Node node, ApplicationContext context, string name, object nodes)
        {
            // Creating cookie to send back to caller
            var retVal = new HttpCookie (name, HttpUtility.UrlEncode (Utilities.Convert<string> (context, nodes))) {
                Expires = DateTime.Now.Date.AddDays (node.GetChildValue ("duration", context, 365)),
                HttpOnly = node.GetChildValue ("http-only", context, true)
            };

            // Returning cookie (or null) back to caller
            return retVal;
        }
    }
}
