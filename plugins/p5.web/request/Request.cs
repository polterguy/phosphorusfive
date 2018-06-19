/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.IO;
using System.Web;
using p5.exp;
using p5.core;

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
        [ActiveEvent (Name = "p5.web.request.get-method")]
        public static void p5_web_request_get_method (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = HttpContext.Current.Request.HttpMethod;
        }

        /// <summary>
        ///     Returns the current HTTP request's body
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.request.get-body")]
        public static void p5_web_request_get_body (ApplicationContext context, ActiveEventArgs e)
        {
            if (HttpContext.Current.Request.InputStream.Length == 0) {
                e.Args.Value = ""; // Defaulting to string.Empty!
                return; // Nothing to do here ...
            }

            // Checking type of request, and acting accordingly.
            if (RequestIsHyperlambda (context)) {

                // Some sort of "textual" based type of request
                var reader = new StreamReader (HttpContext.Current.Request.InputStream);
                var code = Utilities.Convert<Node> (context, reader.ReadToEnd ());
                e.Args.AddRange (code.Children);

            } else if (RequestIsText (context)) {

                // Some sort of "textual" based type of request
                var reader = new StreamReader (HttpContext.Current.Request.InputStream);
                e.Args.Value = reader.ReadToEnd ();

            } else {

                // Some sort of "binary" type of request, we assume
                var rawBytes = new byte [HttpContext.Current.Request.InputStream.Length];
                HttpContext.Current.Request.InputStream.Read (rawBytes, 0, rawBytes.Length);
                e.Args.Value = rawBytes;
            }
        }

        /// <summary>
        ///     Parser the current HTTP request's body as a MIME entity.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.request.parse-mime")]
        public static void p5_web_request_parse_mime (ApplicationContext context, ActiveEventArgs e)
        {
            /*
             * Checking if we should avoid retrieving "Content-Type" from HTTP request's headers, and rather
             * expect it to be found at top of body.
             */
            if (e.Args.GetExChildValue ("content-type-in-body", context, false)) {

                /* Deleting "content-type-in-body" argument, and not adding explicit "Content-Type" from HTTP
                 * header collection.
                 */
                e.Args ["content-type-in-body"].UnTie ();

            } else {

                // Finding "Content-Type" from HTTP request's HTTP headers collection.
                var contentType = HttpContext.Current.Request.ContentType;
                e.Args.Add ("Content-Type", contentType);
            }
            e.Args.Value = new Tuple<object, Stream> (e.Args.Value, HttpContext.Current.Request.InputStream);
            context.RaiseEvent (".p5.mime.load-from-stream", e.Args);

            // House cleaning.
            e.Args ["Content-Type"]?.UnTie ();
        }

        /// <summary>
        ///     Saves the current HTTP request's body to a specified file.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.request.save-body")]
        public static void p5_web_request_save_body (ApplicationContext context, ActiveEventArgs e)
        {
            // Getting filename.
            var filename = e.Args.GetExValue<string> (context);

            // Making sure we transform filename into actual filename in case it uses our "~" logic.
            filename = context.RaiseEvent (".p5.io.unroll-path", new Node ("", filename)).Get<string> (context);

            // Verifying user is authorized writing to the file.
            context.RaiseEvent (".p5.io.authorize.modify-file", new Node ("", filename).Add ("args", e.Args));

            // Retrieving root path for web application.
            var rootFolder = context.RaiseEvent (".p5.core.application-folder").Get<string> (context);

            // Creating a file stream, copying the request stream to our filestream.
            using (FileStream fs = File.Create (rootFolder + filename)) {
                HttpContext.Current.Request.InputStream.CopyTo (fs);
            }
        }

        /// <summary>
        ///     Returns true if the user is coming in from a mobile device.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.request.is-mobile")]
        public static void p5_web_request_is_mobile (ApplicationContext context, ActiveEventArgs e)
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

                // This *might* be a mobile device, checking the .Net Framework's property
                e.Args.Value = HttpContext.Current.Request.Browser.IsMobileDevice;
            }
        }

        /*
         * Determines if current request is Hyperlambda.
         */
        static bool RequestIsHyperlambda (ApplicationContext context)
        {
            // Checking if Content-Type is Hyperlambda MIME extension.
            if (HttpContext.Current.Request.ContentType == "application/x-hyperlambda")
                return true;

            // Not Hyperlambda.
            return false;
        }

        /*
         * Determines if current request is some sort of text.
         */
        static bool RequestIsText (ApplicationContext context)
        {
            // Checking if Content-Type starts with "text/".
            if (HttpContext.Current.Request.ContentType.StartsWithEx ("text/"))
                return true;

            // Not text.
            return false;
        }
    }
}
