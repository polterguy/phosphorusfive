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
using MimeKit;

namespace p5.mime
{
    /// <summary>
    ///     Class wrapping MIME Active Events related to the HTTP request
    /// </summary>
    public static class Request
    {
        /// <summary>
        ///     Returns the current HTTP request's body as parsed MIME
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.request.parse-mime", Protection = EventProtection.LambdaClosed)]
        public static void p5_web_request_parse_mime (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Loading MimeEntity from request stream
                var entity = MimeEntity.Load (
                    ContentType.Parse (HttpContext.Current.Request.ContentType), 
                    HttpContext.Current.Request.InputStream);
                e.Args.Value = entity;
                context.RaiseNative ("p5.mime.parse-native", e.Args);
            }
        }
    }
}
