/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Web;
using phosphorus.core;
using phosphorus.web.ui.Common;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.web.ui
{
    /// <summary>
    ///     Helper to retrieve and set cookies
    /// </summary>
    public static class Cookies
    {
        /// <summary>
        ///     Creates one or more cookies to send back to client, where [duration] becomes number of days before it expires, and
        ///     [source], or [src], becomes the nodes that are stored in the cookie.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.cookie.set")]
        private static void pf_web_cookie_set (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.Set (e.Args, context, delegate (string key, object value) {
                if (value == null) {
                    // removing existing cookie
                    var httpCookie = HttpContext.Current.Response.Cookies [key];
                    if (httpCookie != null) httpCookie.Expires = DateTime.Now.Date.AddDays (-1);
                } else {
                    // creating cookie
                    HttpContext.Current.Response.Cookies.Add (CreateCookieFromNode (e.Args, context, key, value));
                }
            });
        }

        /// <summary>
        ///     Returns the cookie(s) given through the value(s) of the main node. Since cookies can only be stored
        ///     as strings, this method will always return the string representation of whatever was previously stored
        ///     in it using [pf.web.cookie.set]
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.cookie.get")]
        private static void pf_web_cookie_get (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.Get (e.Args, context, delegate (string key) {
                //fetching cookie
                var cookie = HttpContext.Current.Request.Cookies.Get (key);
                if (cookie != null && !string.IsNullOrEmpty (cookie.Value)) {
                    // adding key node, and values beneath key node
                    return Utilities.Convert<Node> (HttpUtility.UrlDecode (cookie.Value), context).Clone ().Children;
                }
                return null;
            });
        }

        /// <summary>
        ///     Lists all keys for all cookies
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.cookie.list")]
        private static void pf_web_cookie_list (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.List (e.Args, context, () => HttpContext.Current.Request.Cookies.AllKeys);
        }
        
        /*
         * creates a cookie from given Node and returns back to caller
         */
        private static HttpCookie CreateCookieFromNode (Node node, ApplicationContext context, string name, object nodes)
        {
            // creating cookie to send back to caller
            var retVal = new HttpCookie (name, HttpUtility.UrlEncode (Utilities.Convert<string> (nodes, context))) {
                Expires = DateTime.Now.Date.AddDays (node.GetChildValue ("duration", context, 365)),
                HttpOnly = node.GetChildValue ("http-only", context, true)
            };

            // making sure cookie is "secured" before we send it back to client, unless
            // caller explicitly tells us he or she does not want it secured

            // returning cookie (or null) back to caller
            return retVal;
        }
    }
}