/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using p5.core;
using p5.exp;
using p5.web.ui.common;

namespace p5.web.ui.request
{
    /// <summary>
    ///     Helper to retrieve HTTP headers.
    /// 
    ///     This class allows you to retrieve HTTP headers values for the current request.
    /// </summary>
    public static class Headers
    {
        /// <summary>
        ///     Returns one or more HTTP header(s).
        /// 
        ///     The name of the header you wish to retrieve, is given as the value(s) of the main node.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-http-header", Protection = EntranceProtection.Lambda)]
        private static void get_http_header (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // looping through each parameter requested by caller
                foreach (var idx in XUtil.Iterate<string> (context, e.Args)) {

                    // adding parameter's name/value as Node return value
                    if (HttpContext.Current.Request.Headers [idx] != null)
                        e.Args.Add (idx, HttpContext.Current.Request.Headers [idx]);
                }
            }
        }

        /// <summary>
        ///     Lists all keys for our HTTP headers.
        /// 
        ///     Returns all keys for all HTTP headers in current request.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-http-headers", Protection = EntranceProtection.Lambda)]
        private static void list_http_headers (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.List (e.Args, context, () => HttpContext.Current.Request.Headers.AllKeys);
        }
    }
}
