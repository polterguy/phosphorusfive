/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using p5.exp;
using p5.core;

namespace p5.web.ui.request
{
    /// <summary>
    ///     Helper to retrieve and set Cookie values
    /// </summary>
    public static class Cookies
    {
        /// <summary>
        ///     Retrieves Cookie object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-cookie-value")]
        [ActiveEvent (Name = ".get-cookie-value")]
        public static void get_cookie_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.GetCollection (context, e.Args, delegate (string key) {

                // Fetching cookie
                var cookie = HttpContext.Current.Request.Cookies.Get (key);
                if (cookie != null && !string.IsNullOrEmpty (cookie.Value)) {

                    // Adding key node, and values beneath key node
                    return HttpUtility.UrlDecode (cookie.Value);
                }
                return null;
            }, e.Name.StartsWith ("."));
        }

        /// <summary>
        ///     Lists all keys in the Cookie object of client
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-cookie-keys")]
        [ActiveEvent (Name = ".list-cookie-keys")]
        public static void list_cookie_keys (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.ListCollection (context, e.Args, HttpContext.Current.Request.Cookies.AllKeys, e.Name.StartsWith ("."));
        }
    }
}
