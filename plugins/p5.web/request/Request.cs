/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Collections.Generic;
using p5.core;
using p5.exp;

/// <summary>
///     Main namespace for everything related to the current HTTP request.
/// 
///     Contains many helper classes and Active Events for retrieving information and/or data from the current HTTP request, such
///     as for instance retrieving cookies, headers, parameters, and parsing content as MIME, etc.
/// </summary>
namespace p5.web.ui.request
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
        [ActiveEvent (Name = "p5.web.get-request-method")]
        private static void p5_web_get_request_method (ApplicationContext context, ActiveEventArgs e)
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
        [ActiveEvent (Name = "p5.web.get-request-body")]
        private static void p5_web_get_request_body (ApplicationContext context, ActiveEventArgs e)
        {
            if (HttpContext.Current.Request.InputStream.Length == 0)
                return; // nothing to do here ...

            if (RequestIsText (e.Args, context)) {
                
                // some sort of "textual" based type of request
                StreamReader reader = new StreamReader (HttpContext.Current.Request.InputStream);
                e.Args.Value = reader.ReadToEnd ();
            } else {

                // some sort of "binary" type of request, we assume
                var rawBytes = new byte [HttpContext.Current.Request.InputStream.Length];
                HttpContext.Current.Request.InputStream.Read (rawBytes, 0, rawBytes.Length);
                e.Args.Value = rawBytes;
            }
        }

        /*
         * determines if current request is "text"
         */
        private static bool RequestIsText (Node node, ApplicationContext context)
        {
            if (HttpContext.Current.Request.ContentType.StartsWith ("text"))
                return true;
            return false;
        }
    }
}
