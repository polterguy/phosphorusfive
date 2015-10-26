
/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using pf.core;
using phosphorus.expressions;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.web.ui.response
{
    /// <summary>
    ///     Helper class to manipulate the HTTP response headers.
    /// 
    ///     Class encapsulating Active Events requires to change or set HTTP headers for the HTTTP response.
    /// </summary>
    public static class Headers
    {
        /// <summary>
        ///     Changes the HTTP headers for the current response.
        /// 
        ///     Allows changing, removing or adding the HTTP headers returned by the current HTTP response.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.set-response-header")]
        private static void pf_web_set_response_header (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idxMatch in XUtil.Iterate<Node> (e.Args, context)) {
                if (idxMatch.Value == null) {

                    // removing specific header
                    HttpContext.Current.Response.Headers.Remove (idxMatch.Name);
                } else {

                    // adding header
                    HttpContext.Current.Response.AddHeader (idxMatch.Name, XUtil.Single<string> (idxMatch, context));
                }
            }
        }
    }
}
