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
using MimeKit;

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
    ///     Class wrapping Active Events related to the HTTP request.
    /// 
    ///     Contains helper Active Events to retrieve information and meta-information about the current HTTP request.
    /// </summary>
    public static class Request
    {
        /// <summary>
        ///     Retrieves type of request.
        /// 
        ///     Will return the type of request, such as for instance "GET", "POST", "DELETE" or "PUT" as value of main node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.request.method")]
        private static void pf_web_request_method (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = HttpContext.Current.Request.HttpMethod;
        }

        /// <summary>
        ///     Returns the current HTTP request's body.
        /// 
        ///     Will return the current HTTP request body. If the 'Content-Type' is "text/something", then the request will
        ///     be returned as text, otherwise as raw bytes. If no 'Content-Type' header exists, the request will be assumed
        ///     to be of some sort of binary type.
        /// 
        ///     The body of the request will be returned as the value of [result] node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.request.get-body")]
        private static void pf_web_request_get_body (ApplicationContext context, ActiveEventArgs e)
        {
            if (RequestIsText (e.Args, context)) {
                
                // some sort of "textual" based type of request
                StreamReader reader = new StreamReader (HttpContext.Current.Request.InputStream);
                e.Args.Add ("result", reader.ReadToEnd ());
            } else {
                
                // some sort of "binary" type of request, we assume
                MemoryStream stream = new MemoryStream ();
                HttpContext.Current.Request.InputStream.CopyTo (stream);
                e.Args.Add ("result", stream.GetBuffer ());
            }
        }

        /*
         * determines if current request is "text"
         */
        private static bool RequestIsText (Node node, ApplicationContext context)
        {
            if (node.GetExChildValue ("force-text", context, false))
                return true;
            if (node.GetExChildValue ("force-binary", context, false))
                return false;
            if (ContentType.Parse (HttpContext.Current.Request.ContentType).MediaType == "text")
                return true;
            return false;
        }
    }
}
