
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Web;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.web.ui
{
    /// <summary>
    /// helper to retrieve and set cookies
    /// </summary>
    public static class cookie
    {
        /// <summary>
        /// creates one or more cookies to send back to client, where [duration] becomes number of days before it expires, and
        /// [source], or [src], becomes the nodes that are stored in the cookie
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.cookie.set")]
        private static void pf_web_cookie_set (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.Set (e.Args, context, delegate (string key, object value) {
                if (value == null) {
                    
                    // removing existing cookie
                    HttpContext.Current.Response.Cookies [key].Expires = DateTime.Now.Date.AddDays (-1);
                } else {
                    
                    // creating cookie
                    HttpContext.Current.Response.Cookies.Add (CreateCookieFromNode (e.Args, context, key, value));

                }});
        }

        /// <summary>
        /// retrieves one or more cookies from client, and converts to pf.lambda, returning the name
        /// of the cookie as a child of the main node, containing all children nodes from cookie. cookie(s)
        /// to retrieve are given as value of main node
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
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

        /*
         * creates a cookie from given Node and returns back to caller
         */
        private static HttpCookie CreateCookieFromNode (Node node, ApplicationContext context, string name, object nodes)
        {
            // creating cookie to send back to caller
            var retVal = new HttpCookie (name, HttpUtility.UrlEncode (Utilities.Convert<string> (nodes, context)));
            retVal.Expires = DateTime.Now.Date.AddDays (node.GetChildValue ("duration", context, 365));

            // making sure cookie is "secured" before we send it back to client, unless
            // caller explicitly tells us he or she does not want it secured
            retVal.HttpOnly = node.GetChildValue ("http-only", context, true);

            // returning cookie (or null) back to caller
            return retVal;
        }
        
        /// <summary>
        /// lists all cookies keys in request
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.cookie.list")]
        private static void pf_web_cookie_list (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.List (e.Args, context, delegate { return HttpContext.Current.Request.Cookies.AllKeys; });
        }
    }
}
