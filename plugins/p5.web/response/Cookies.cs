/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Web;
using p5.core;
using p5.web.ui.common;

/// <summary>
///     Main namespace for everything related to the current HTTP response.
/// 
///     Contains many helper classes and Active Events, for everything related to the current HTTP response, such as
///     setting headers, cookies, and returning MIME content to client, etc.
/// </summary>
namespace p5.web.ui.response
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
        [ActiveEvent (Name = "p5.web.set-response-cookie")]
        private static void p5_web_set_response_cookie (ApplicationContext context, ActiveEventArgs e)
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