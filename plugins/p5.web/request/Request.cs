/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Collections.Generic;
using p5.exp;
using p5.core;

/// <summary>
///     Main namespace for everything related to the current HTTP request
/// </summary>
namespace p5.web.ui.request
{
    /// <summary>
    ///     Class wrapping Active Events related to the HTTP request
    /// </summary>
    public static class Request
    {
        /// <summary>
        ///     Retrieves type of request
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-http-method", Protection = EventProtection.LambdaClosed)]
        public static void get_http_method (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = HttpContext.Current.Request.HttpMethod;
        }

        /// <summary>
        ///     Returns the current HTTP request's body
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-request-body", Protection = EventProtection.LambdaClosed)]
        public static void get_request_body (ApplicationContext context, ActiveEventArgs e)
        {
            if (HttpContext.Current.Request.InputStream.Length == 0) {
                e.Args.Value = ""; // Defaulting to string.Empty!
                return; // Nothing to do here ...
            }

            if (RequestIsText (e.Args, context)) {
                
                // Some sort of "textual" based type of request
                StreamReader reader = new StreamReader (HttpContext.Current.Request.InputStream);
                e.Args.Value = reader.ReadToEnd ();
            } else {

                // Some sort of "binary" type of request, we assume
                var rawBytes = new byte [HttpContext.Current.Request.InputStream.Length];
                HttpContext.Current.Request.InputStream.Read (rawBytes, 0, rawBytes.Length);
                e.Args.Value = rawBytes;
            }
        }

        /// <summary>
        ///     Returns true if the user is coming in from a mobile device
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "request-is-mobile-device", Protection = EventProtection.LambdaClosed)]
        public static void request_is_mobile_device (ApplicationContext context, ActiveEventArgs e)
        {
            var userAgent = HttpContext.Current.Request.UserAgent.ToLower ();
            if (userAgent.Contains ("blackberry") || 
                userAgent.Contains ("iphone") || 
                userAgent.Contains ("ppc") || 
                userAgent.Contains ("windows ce") || 
                userAgent.Contains ("mobile") || 
                userAgent.Contains ("palm") || 
                userAgent.Contains ("portable") || 
                userAgent.Contains ("opera mobi") || 
                userAgent.Contains ("android")) {

                // This is a mobile device, probably not on .Net's list of devices
                e.Args.Value = true;
            } else {

                // This "might" be a mobile device, checking the .Net Framework's property
                e.Args.Value = HttpContext.Current.Request.Browser.IsMobileDevice;
            }
        }

        /*
         * Determines if current request is "text"
         */
        private static bool RequestIsText (Node node, ApplicationContext context)
        {
            // Checking if Content-Type starts with "text/" ...
            if (HttpContext.Current.Request.ContentType.StartsWith ("text/"))
                return true;

            // Not text type
            return false;
        }
    }
}
