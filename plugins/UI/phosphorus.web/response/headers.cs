
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Text;
using phosphorus.core;
using phosphorus.lambda;

namespace phosphorus.web
{
    /// <summary>
    /// helper to manipulate the HTTP response
    /// </summary>
    public static class headers
    {
        /// <summary>
        /// changes an existing or adds a new HTTP header to response
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.headers.set")]
        private static void pf_web_headers_set (ApplicationContext context, ActiveEventArgs e)
        {
            string name = Expression.Single<string> (e.Args, true);
            string value = Expression.Single<string> (e.Args.LastChild, true);
            HttpContext.Current.Response.AddHeader (name, value);
        }
    }
}
