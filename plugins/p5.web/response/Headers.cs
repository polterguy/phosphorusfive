
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
    ///     Helper class to manipulate the HTTP response headers
    /// </summary>
    public static class Headers
    {
        /// <summary>
        ///     Changes the HTTP headers for the current response
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-http-header", Protection = EventProtection.LambdaClosed)]
        private static void set_http_header (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idxMatch in XUtil.Iterate<Node> (context, e.Args)) {
                if (idxMatch.Value == null) {

                    // Removing specific header
                    HttpContext.Current.Response.Headers.Remove (idxMatch.Name);
                } else {

                    // Adding or modifying existing header
                    HttpContext.Current.Response.AddHeader (idxMatch.Name, XUtil.Single<string> (context, idxMatch));
                }
            }
        }
    }
}
