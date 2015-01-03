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
        /// stores a cookie on disc, containing all the child nodes from underneath [value] underneath [pf.web.cookie.set],
        /// with the name of the cookie being the value of [pf.web.cookie.set]. the cookie will last for [duration] number
        /// of days, which is expected to be an integer. if no [duration] is given, the default value of 365 will be used
        /// value
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.cookie.set")]
        private static void pf_web_cookie_set (ApplicationContext context, ActiveEventArgs e)
        {
            // setting value of cookie
            Node convert = e.Args.Find (
                delegate (Node idx) {
                return idx.Name == "value";
            });

            if (convert != null) {

                // creating persistent cookie
                HttpCookie cookie = new HttpCookie (e.Args.Get<string> ());
                convert = convert.Clone ();
                context.Raise ("pf.nodes-2-code", convert);
                cookie.Value = HttpUtility.UrlEncode (convert.Get<string> ());

                // setting date of cookie
                Node durationNode = e.Args.Find (
                    delegate (Node idx) {
                    return idx.Name == "duration";
                });
                int duration = durationNode != null ? durationNode.Get<int> () : 365;
                cookie.Expires = DateTime.Now.Date.AddDays (duration);

                // making sure cookie is "secured" before we send it back to client
                cookie.HttpOnly = true;
                HttpContext.Current.Response.Cookies.Add (cookie);
            } else {

                // destroying existing persistent cookie, if it exists
                if (HttpContext.Current.Response.Cookies.Get (e.Args.Get<string> ()) != null)
                    HttpContext.Current.Response.Cookies [e.Args.Get<string> ()].Expires = DateTime.Now.Date.AddDays (-1);
            }
        }
        
        /// <summary>
        /// retrieves a cookie with the name given through the value of [pf.web.cookie.get], and returns it as [value]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.cookie.get")]
        private static void pf_web_cookie_get (ApplicationContext context, ActiveEventArgs e)
        {
            string cookieName = e.Args.Get<string> ();
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get (cookieName);
            if (cookie != null) {
                string cookieValue = HttpUtility.UrlDecode (cookie.Value);

                if (!string.IsNullOrEmpty (cookieValue)) {

                    Node convertNode = new Node (string.Empty, cookieValue);
                    context.Raise ("pf.code-2-nodes", convertNode);
                    if (convertNode.Count > 0) {
                        Node valueNode = new Node ("value");
                        e.Args.Add (valueNode);
                        foreach (Node idx in convertNode.Children) {
                            valueNode.Add (idx);
                        }
                    }
                }
            }
        }
    }
}
