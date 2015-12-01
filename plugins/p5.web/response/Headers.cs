
/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using p5.core;
using p5.exp;

namespace p5.web.ui.response
{
    /// <summary>
    ///     Helper class to manipulate the HTTP response headers.
    /// </summary>
    public static class Headers
    {
        /// <summary>
        ///     Changes the HTTP headers for the current response.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "set-http-header")]
        private static void set_http_header (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idxMatch in XUtil.Iterate<Node> (context, e.Args)) {
                if (idxMatch.Value == null) {

                    // removing specific header
                    HttpContext.Current.Response.Headers.Remove (idxMatch.Name);
                } else {

                    // adding header
                    HttpContext.Current.Response.AddHeader (idxMatch.Name, XUtil.Single<string> (context, idxMatch));
                }
            }
        }
    }
}
