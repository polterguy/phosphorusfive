
/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using phosphorus.core;
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
        [ActiveEvent (Name = "pf.web.response.headers.set")]
        private static void pf_web_response_headers_set (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Count == 0) {

                // "remove headers" invocation, looping through all headers user wish to remove
                foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                    HttpContext.Current.Response.Headers.Remove (idx);
                }
            } else {

                // adding header(s) invocation
                var value = e.Args.LastChild.Get<string> (context);
                foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                    HttpContext.Current.Response.AddHeader (idx, value);
                }
            }
        }
    }
}
