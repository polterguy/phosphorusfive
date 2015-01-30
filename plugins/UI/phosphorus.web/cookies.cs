
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Globalization;
using System.Security.Cryptography;
using phosphorus.core;
using phosphorus.lambda;
using phosphorus.ajax.widgets;

namespace phosphorus.web
{
    /// <summary>
    /// helper to retrieve and set cookies
    /// </summary>
    public static class cookie
    {
        /// <summary>
        /// sends on or more cookies back to client where [duration] becomes number of days before it expires,
        /// [value] becomes the nodes that are stored in the cookie, and the value of the main node becomes
        /// the name(s) of the cookie(s)
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.cookie.set")]
        private static void pf_web_cookie_set (ApplicationContext context, ActiveEventArgs e)
        {
            // finding value to set as cookie, if any, and converting to code syntax
            string value = null;
            int duration = 365;
            Node convert = e.Args.Find ("value");
            if (convert != null) {

                // converting value to Hyperlisp, and URL encoding it for our cookie
                convert = convert.Clone ();
                context.Raise ("lambda2code", convert);
                value = HttpUtility.UrlEncode (convert.Get<string> ());

                // finding duration, defaulting to 365 if none
                Node durationNode = e.Args.Find ("duration");
                if (durationNode != null)
                    duration = durationNode.Get<int> ();
            }

            Expression.Iterate<string> (e.Args, false, 
            delegate (string idx) {
                if (value != null) {

                    // creating cookie to send back to client
                    HttpCookie cookie = new HttpCookie (idx);
                    cookie.Value = value;
                    cookie.Expires = DateTime.Now.Date.AddDays (duration);

                    // making sure cookie is "secured" before we send it back to client
                    cookie.HttpOnly = true;
                    HttpContext.Current.Response.Cookies.Add (cookie);
                } else {

                    // removing cookie, if it exists
                    if (HttpContext.Current.Response.Cookies.Get (idx) != null)
                        HttpContext.Current.Response.Cookies [idx].Expires = DateTime.Now.Date.AddDays (-1);
                }
            });
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
            Expression.Iterate<string> (e.Args, false, 
            delegate (string idx) {

                // checking to see if this cookie exists
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get (idx);
                if (cookie != null && !string.IsNullOrEmpty (cookie.Value)) {

                    // URL decoding cookie value, before converting to pf.lambda
                    string cookieValue = HttpUtility.UrlDecode (cookie.Value);
                    Node convertNode = new Node (string.Empty, cookieValue);
                    context.Raise ("code2lambda", convertNode);

                    if (Expression.IsExpression (e.Args.Value)) {

                        // adding key node, and values beneath key node
                        e.Args.Add (new Node (idx));
                        e.Args.LastChild.AddRange ((convertNode as Node).Clone ().Children);
                    } else {

                        // since this is not an expression, we simply append values into main node
                        e.Args.AddRange ((convertNode as Node).Clone ().Children);
                    }
                }
            });
        }
    }
}
