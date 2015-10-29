/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Web;
using p5.core;
using p5.web.ui.common;

namespace p5.web.ui.request
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
        ///     Retrieves Cookie object(s).
        /// 
        ///     Supply one or more keys to which items you wish to retrieve as the value of your main node.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.get-request-cookie")]
        private static void p5_web_get_request_cookie (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.Get (e.Args, context, delegate (string key) {
                //fetching cookie
                var cookie = HttpContext.Current.Request.Cookies.Get (key);
                if (cookie != null && !string.IsNullOrEmpty (cookie.Value)) {
                    // adding key node, and values beneath key node
                    return HttpUtility.UrlDecode (cookie.Value);
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
        [ActiveEvent (Name = "p5.web.list-request-cookies")]
        private static void p5_web_list_request_cookies (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.List (e.Args, context, () => HttpContext.Current.Request.Cookies.AllKeys);
        }
    }
}
