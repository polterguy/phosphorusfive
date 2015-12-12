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

namespace p5.mime
{
    /// <summary>
    ///     Class wrapping the [p5.net.http-post-mime/put-mime] Active Events
    /// </summary>
    public static class HttpRequest
    {
        // Specialized delegate functors for rendering request and response
        private delegate void RenderRequestFunctor (ApplicationContext context, HttpWebRequest request, Node args, string method);
        private delegate void RenderResponseFunctor (ApplicationContext context, HttpWebRequest request, Node args);

        /// <summary>
        ///     Posts or puts a MIME message over an HTTP request
        /// </summary>
        [ActiveEvent (Name = "p5.net.http-post-mime", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "p5.net.http-put-mime", Protection = EventProtection.LambdaClosed)]
        private static void p5_net_http_post_put_mime (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args["content"] == null)
                throw new LambdaException (
                    "No [content] node found to post or put to server end-point",
                    e.Args,
                    context);

            // List of streams created during creation of MimeEntity.
            // We need to keep track of this, such that we can close and dispose all streams after we're done with them
            var streams = new List<Stream> ();

            // Making sure we can clean up after ourselves
            try {
                // Creating MIME entity, making sure we pass in a list of streams, such that we can clean up after ourselves
                e.Args ["content"].Value = streams;
                var entity = context.RaiseNative ("p5.mime.create-native", e.Args ["content"]).UnTie ().Get<MimeEntity> (context);

                // Creating request, with delegate writing MimeEntity acquired above
                CreateRequest (context, e.Args, delegate (
                    ApplicationContext ctx, 
                    HttpWebRequest request, 
                    Node args, 
                    string method) {
                    using (Stream stream = request.GetRequestStream ()) {

                        // Setting our Content-Type header, defaulting to "application/octet-stream", in addition to other headers
                        request.ContentType = args.GetExChildValue (
                            "Content-Type", 
                            context, 
                            "multipart/mixed");

                        // Setting other headers
                        SetRequestHeaders (context, request, args);

                        // Writing MIME entity to request stream
                        entity.WriteTo (stream);
                    }

                }, RenderResponse);
            } finally {

                // Closing and disposing all streams created during creation of MimeEntity
                foreach (var idxStream in streams) {

                    // Closing and disposing currently iterated stream
                    idxStream.Close ();
                    idxStream.Dispose ();
                }
            }
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
                try
                {
                    // Iterating through each request URL given
                    foreach (var idxUrl in XUtil.Iterate<string> (context, args, true)) {

                        // Creating request
                        HttpWebRequest request = WebRequest.Create (idxUrl) as HttpWebRequest;

                        // Setting HTTP method
                        request.Method = method;

                        // Writing content to request, if any
                        renderRequest (context, request, args, method);

                        // Returning response to caller
                        renderResponse (context, request, args);
                    }
                }
                catch (Exception err)
                {
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
         * Renders response into given Node
         */
        private static void RenderResponse (
            ApplicationContext context, 
            HttpWebRequest request, 
            Node args)
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
            Node result = args.Add ("result", request.RequestUri.ToString ()).LastChild;

            // Getting response HTTP headers
            GetResponseHeaders (context, response, result, request);

            // Retrieving response stream, and parsing content
            using (Stream stream = response.GetResponseStream ()) {

                // Checking type of response
                if (response.ContentType.StartsWith ("application/x-hyperlisp")) {

                    // Hyperlisp, possibly special treatment
                    using (TextReader reader = new StreamReader (stream, Encoding.GetEncoding (response.CharacterSet ?? "UTF8"))) {

                        // Checking if caller wants to automatically convert to p5 lambda, which is default behavior
                        if (args.GetExChildValue ("convert", context, true)) {

                            // Converting from Hyperlisp to p5 lambda
                            Node convert = context.RaiseNative ("lisp2lambda", new Node ("content", reader.ReadToEnd ()));
                            convert.Value = null;
                            result.Add (convert);
                        } else {

                            // Caller explicitly said he did NOT want to convert
                            result.Add ("content", reader.ReadToEnd());
                        }
                    }
                } else if (response.ContentType.StartsWith ("text")) {

                    // Text response
                    using (TextReader reader = new StreamReader (stream, Encoding.GetEncoding (response.CharacterSet ?? "UTF8"))) {

                        // Simply adding as text
                        result.Add ("content", reader.ReadToEnd ());
                    }
                } else {

                    // Defaulting to binary
                    using (MemoryStream memStream = new MemoryStream ()) {

                        // Simply adding as byte[]
                        stream.CopyTo (memStream);
                        result.Add ("content", memStream.GetBuffer ());
                    }
                }
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
