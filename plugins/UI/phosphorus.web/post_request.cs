
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Web;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.web
{
    /// <summary>
    /// helper to create an HTTP POST request
    /// </summary>
    public static class post_request
    {
        /// <summary>
        /// creates an HTTP POST request
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.post")]
        private static void pf_web_post (ApplicationContext context, ActiveEventArgs e)
        {
        }
    }
}
