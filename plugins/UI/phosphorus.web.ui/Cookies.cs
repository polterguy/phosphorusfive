/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Web;
using phosphorus.core;
using phosphorus.web.ui.common;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.web.ui
{
    /// <summary>
    ///     Helper to retrieve and set Cookie values.
    /// 
    ///     Allows for you to retrieve and set items in your user's cookie storage.
    /// 
    ///     Cookies allows you to transfer data back to the client's web-browser, that will be transfered back to the server,
    ///     for consecutive requests.
    /// </summary>
    public static class Cookies
    {
        /// <summary>
        ///     Sets one or more Cookie object(s).
        /// 
        ///     Where [source], or [src], becomes the nodes that are stored in the cookie. The main node's value(s), becomes
        ///     the key your items are stored with. Pass in [duration] as number of days before cookie expires from cookie storage
        ///     on client.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
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
        ///     Retrieves Cookie object(s).
        /// 
        ///     Supply one or more keys to which items you wish to retrieve as the value of your main node.
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
        ///     Lists all keys in the Cookie object of client.
        /// 
        ///     Returns all keys for all items in your user's Cookie object.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
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