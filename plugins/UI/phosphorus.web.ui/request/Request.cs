/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

/// <summary>
///     Main namespace for everything related to the current HTTP request.
/// 
///     Contains many helper classes and Active Events for retrieving information and/or data from the current HTTP request, such
///     as for instance retrieving cookies, headers, parameters, and parsing content as MIME, etc.
/// </summary>
namespace phosphorus.web.ui.request
{
    /// <summary>
    /// </summary>
    public static class Request
    {
        /// <summary>
        ///     Retrieves type of request.
        /// 
        ///     Will return the type of request, such as for instance "GET" or "POST" as value of main node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.request.method")]
        private static void pf_web_request_method (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = HttpContext.Current.Request.HttpMethod;
        }
    }
}
