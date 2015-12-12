/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using MimeKit;

namespace p5.mime.plugins
{
    /// <summary>
    ///     Class wrapping the [p5.net.http-post-mime/put-mime] Active Events for creating HTTP requests that posts/puts MIME
    /// </summary>
    public static class HttpRequest
    {
        // Specialized delegate functors for rendering request and response
        private delegate void RenderRequestFunctor (ApplicationContext context, MimeEntity entity, HttpWebRequest request, Node args, string method);
        private delegate void RenderResponseFunctor (ApplicationContext context, HttpWebRequest request, Node args);

        /// <summary>
        ///     Posts or puts a MIME message over an HTTP request
        /// </summary>
        [ActiveEvent (Name = "p5.net.http-post-mime", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "p5.net.http-put-mime", Protection = EventProtection.LambdaClosed)]
        private static void p5_net_http_post_put_mime (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Count != 1)
                throw new LambdaException (
                    "You can only post and put one MIME entity when creating HTTP requests, to post multiple values, use [multipart] as your root",
                    e.Args,
                    context);

            // Creating request, with delegate writing MimeEntity
            CreateRequest (context, e.Args, RenderRequest, RenderResponse);
        }

        /*
         * Actual implementation of creation of HTTP request
         */
        private static void CreateRequest (
            ApplicationContext context, 
            Node args, 
            RenderRequestFunctor renderRequest, 
            RenderResponseFunctor renderResponse)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (args, true)) {

                // Figuring out which HTTP method to use
                string method = args.Name.Substring (args.Name.IndexOf ("-") + 1).ToUpper ();
                if (method.Contains ("-"))
                    method = method.Substring (0, method.IndexOf ("-"));
                try {

                    // Iterating through each request URL given
                    foreach (var idxUrl in XUtil.Iterate<string> (context, args, true)) {

                        // List of streams created during creation of MimeEntity.
                        // We need to keep track of this, such that we can close and dispose all streams after we're done with them
                        var streams = new List<Stream> ();

                        // Creating MIME entity, making sure we pass in a list of streams, such that we can clean up after ourselves
                        args.Value = streams;

                        try {
                            var entity = context.RaiseNative ("p5.mime.create-native", args).Get<MimeEntity> (context);

                            // Creating request
                            HttpWebRequest request = WebRequest.Create (idxUrl) as HttpWebRequest;

                            // Setting HTTP method
                            request.Method = method;

                            // Writing content to request, if any
                            renderRequest (context, entity, request, args, method);

                            // Returning response to caller
                            renderResponse (context, request, args);
                        } finally {

                            // Closing and disposing all streams created during creation of MimeEntity
                            foreach (var idxStream in streams) {

                                // Closing and disposing currently iterated stream
                                idxStream.Close ();
                                idxStream.Dispose ();
                            }

                        }
                    }
                } catch (Exception err) {

                    // Trying to avoid throwing a new exception, unless we have to
                    if (err is LambdaException)
                        throw;

                    // Making sure we re-throw as LambdaException, to get more detailed information about what went wrong ...
                    throw new LambdaException (
                        string.Format ("Something went wrong with request, error message was; '{0}'", err.Message), 
                        args, 
                        context, 
                        err);
                }
            }
        }

        /*
         * Decorates all headers for request, except Content-Type, which should be handled by caller
         */
        private static void SetRequestHeaders (
            ApplicationContext context, 
            HttpWebRequest request, 
            Node args)
        {
            // Redmond, this is ridiculous! Why can't we set headers in a uniform way ...?
            foreach (var idxHeader in 
                     args.Children.Where (idxArg => idxArg.Name != "content" && idxArg.Name != "Content-Type" && idxArg.Name != "")) {
                switch (idxHeader.Name) {
                case "Accept":
                    request.Accept = XUtil.Single<string> (context, idxHeader, idxHeader);
                    break;
                case "Connection":
                    request.Connection = XUtil.Single<string> (context, idxHeader, idxHeader);
                    break;
                case "Content-Length":
                    request.ContentLength = XUtil.Single<long> (context, idxHeader, idxHeader);
                    break;
                case "Content-Type":
                    request.ContentType = XUtil.Single<string> (context, idxHeader, idxHeader);
                    break;
                case "Date":
                    request.Date = XUtil.Single<DateTime> (context, idxHeader, idxHeader);
                    break;
                case "Expect":
                    request.Expect = XUtil.Single<string> (context, idxHeader, idxHeader);
                    break;
                case "Host":
                    request.Host = XUtil.Single<string> (context, idxHeader, idxHeader);
                    break;
                case "If-Modifies-Since":
                    request.IfModifiedSince = XUtil.Single<DateTime> (context, idxHeader, idxHeader);
                    break;
                case "Referer":
                    request.Referer = XUtil.Single<string> (context, idxHeader, idxHeader);
                    break;
                case "Transfer-Encoding":
                    request.TransferEncoding = XUtil.Single<string> (context, idxHeader, idxHeader);
                    break;
                case "User-Agent":
                    request.UserAgent = XUtil.Single<string> (context, idxHeader, idxHeader);
                    break;
                default:
                    request.Headers.Add (idxHeader.Name, XUtil.Single<string> (context, idxHeader, idxHeader));
                    break;
                }
            }
        }

        /*
         * Renders MIME request
         */
        private static void RenderRequest (
            ApplicationContext context, 
            MimeEntity entity,
            HttpWebRequest request, 
            Node args, 
            string method)
        {
            using (Stream stream = request.GetRequestStream ()) {

                // Setting our Content-Type header, defaulting to Entity's Content-Type, in addition to other headers
                request.ContentType = args.GetExChildValue (
                    "Content-Type", 
                    context, 
                    entity.ContentType.ToString ());

                // Setting other headers
                SetRequestHeaders (context, request, args);

                // Writing MIME entity to request stream
                entity.WriteTo (stream);
            }
        }

        /*
         * Renders response into given Node
         */
        private static void RenderResponse (
            ApplicationContext context, 
            HttpWebRequest request, 
            Node args)
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
            Node result = args.Add ("result").LastChild;

            // Getting response HTTP headers
            GetResponseHeaders (context, response, result, request);

            // Retrieving response stream, and parsing content, making sure MimeEntity never leaves this method
            try {
                using (Stream stream = response.GetResponseStream ()) {

                    // Retrieving response by reading stream and creating MimeEntity
                    result.Value = MimeEntity.Load (ContentType.Parse (response.ContentType), stream);
                    context.RaiseNative ("p5.mime.parse-native", result);
                }
            } finally {

                // Making sure [result] node's value is URL of response
                result.Value = request.RequestUri.ToString ();
            }
        }

        /*
         * Returns the HTTP response headers into node given
         */
        private static void GetResponseHeaders (
            ApplicationContext context, 
            HttpWebResponse response, 
            Node args, 
            HttpWebRequest request)
        {
            // We only add [Status] node if status was NOT OK! At which point we also supply the error description to caller
            if (response.StatusCode != HttpStatusCode.OK) {

                args.Add ("status", response.StatusCode.ToString ());
                args.Add ("Status-Description", response.StatusDescription);
            }

            // Checking to see if Content-Type is given, and if so, adding header to caller
            if (!string.IsNullOrEmpty (response.ContentType))
                args.Add ("Content-Type", response.ContentType);

            // Content-Encoding
            if (!string.IsNullOrEmpty (response.ContentEncoding))
                args.Add ("Content-Encoding", response.ContentEncoding);

            // Last-Modified
            if (response.LastModified != DateTime.MinValue)
                args.Add ("Last-Modified", response.LastModified);

            // Response-Uri
            if (response.ResponseUri.ToString () != request.RequestUri.ToString ())
                args.Add ("Response-Uri", response.ResponseUri.ToString ());

            // Server
            args.Add ("Server", response.Server);

            // The rest of the HTTP headers
            foreach (string idxHeader in response.Headers.Keys) {

                // Checking if header is not one of those already handled, and if not, handling it
                if (idxHeader != "Server" && idxHeader != "Content-Type")
                    args.Add (idxHeader, response.Headers [idxHeader]);
            }
        }
    }
}
