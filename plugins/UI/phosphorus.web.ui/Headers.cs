/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.web.ui.common;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.web.ui
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
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.headers.get")]
        private static void pf_web_headers_get (ApplicationContext context, ActiveEventArgs e)
        {
            // looping through each parameter requested by caller
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                // adding parameter's name/value as Node return value
                if (HttpContext.Current.Request.Headers [idx] != null)
                    e.Args.Add (idx, HttpContext.Current.Request.Headers [idx]);
            }
        }

        /// <summary>
        ///     Lists all keys for our HTTP headers.
        /// 
        ///     Returns all keys for all HTTP headers in current request.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.headers.list")]
        private static void pf_web_headers_list (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.List (e.Args, context, () => HttpContext.Current.Request.Headers.AllKeys);
        }
    }
}
