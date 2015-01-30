
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Web;
using phosphorus.core;
using phosphorus.lambda;

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
            Expression.Iterate<string> (e.Args, false, 
            delegate (string idx) {

                // creating cookie
                HttpCookie cookie = CreateCookieFromNode (context, idx, e.Args);
                if (cookie != null) {

                    // returning cookie to client
                    HttpContext.Current.Response.Cookies.Add (cookie);
                } else if (HttpContext.Current.Response.Cookies.Get (idx) != null) {

                    // removing cookie
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

        /*
         * creates a cookie from given Node and returns back to caller. returns null if no cookie
         * values exists in node
         */
        private static HttpCookie CreateCookieFromNode (ApplicationContext context, string name, Node node)
        {
            HttpCookie retVal = null;

            if (node.Count > 0) {

                // converting value to Hyperlisp, and URL encoding it for our cookie
                // but removing "property nodes" such as [duration] before converting
                Node convert = new Node ().AddRange (node.Clone ().Children);
                convert.Remove ("duration");
                convert.Remove ("http-only");

                // in case there is no actual value, but only [duration] and "property nodes"
                if (convert.Count > 0) {

                    // this node structure actually have values to be stored in cookie
                    context.Raise ("lambda2code", convert);
                    string value = HttpUtility.UrlEncode (convert.Get<string> ());

                    // finding duration, defaulting to 365 if none
                    int duration = 365;
                    Node durationNode = node.Find ("duration");
                    if (durationNode != null)
                        duration = durationNode.Get<int> ();

                    // creating cookie to send back to caller
                    retVal = new HttpCookie (name, value);
                    retVal.Expires = DateTime.Now.Date.AddDays (duration);

                    // making sure cookie is "secured" before we send it back to client, unless
                    // caller explicitly tells us he does not want it secured
                    retVal.HttpOnly = true;
                    Node httpOnly = node.Find ("http-only");
                    if (httpOnly != null)
                        retVal.HttpOnly = httpOnly.Get<bool> ();
                }
            }

            // returning cookie (or null) back to caller
            return retVal;
        }
    }
}
