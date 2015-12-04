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

/// <summary>
///     Namespace wrapping Active Events related to networking
/// </summary>
namespace p5.net
{
    /// <summary>
    ///     Class wrapping the [p5.net.http-get/post/put/delete] Active Events
    /// </summary>
    public static class HttpRequest
    {
        // Specialized delegate functors for rendering request and response
        private delegate void RenderRequestFunctor (ApplicationContext context, HttpWebRequest request, Node args, string method);
        private delegate void RenderResponseFunctor (ApplicationContext context, HttpWebRequest request, Node args);

        /// <summary>
        ///     Creates a new HTTP REST request of specified type
        /// </summary>
        [ActiveEvent (Name = "p5.net.http-get", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "p5.net.http-post", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "p5.net.http-put", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "p5.net.http-delete", Protection = EventProtection.LambdaClosed)]
        private static void p5_net_http_request (ApplicationContext context, ActiveEventArgs e)
        {
            CreateRequest (context, e.Args, RenderRequest, RenderResponse);
        }

        /// <summary>
        ///     Posts or puts a file over an HTTP request
        /// </summary>
        [ActiveEvent (Name = "p5.net.http-post-file", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "p5.net.http-put-file", Protection = EventProtection.LambdaClosed)]
        private static void p5_net_http_post_put_file (ApplicationContext context, ActiveEventArgs e)
        {
            CreateRequest (context, e.Args, RenderFileRequest, RenderResponse);
        }
        
        /// <summary>
        ///     Gets a file from an HTTP request
        /// </summary>
        [ActiveEvent (Name = "p5.net.http-get-file", Protection = EventProtection.LambdaClosed)]
        private static void p5_net_http_get_file (ApplicationContext context, ActiveEventArgs e)
        {
            CreateRequest (context, e.Args, RenderRequest, RenderFileResponse);
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
         * Renders normal HTTP request
         */
        private static void RenderRequest (
            ApplicationContext context, 
            HttpWebRequest request, 
            Node args, 
            string method)
        {
            if (args ["content"] != null) {

                // We've got content to post or put, making sure caller is not trying to submit content over HTTP get or delete requests
                if (method != "PUT" && method != "POST")
                    throw new LambdaException ("You cannot have content with 'GET' and 'DELETE' types of requests", args, context);

                // Retrieving actual content to post or put
                var content = GetRequestContent (context, args ["content"]);

                // Checking to see if this is Hyperlisp content, since we're b y default setting Content-Type to application/x-hyperlisp if it is
                bool isHyperlisp = args ["content"].Value == null && args ["content"].Count > 0;

                if (content != null) {

                    // Caller supplied actual content in [content] node (as opposed to for instance an expression leading to oblivion)
                    using (Stream stream = request.GetRequestStream ()) {

                        // Checking if this is binary content
                        byte[] byteContent = content as byte[];
                        if (byteContent != null) {

                            // Setting our Content-Type header, defaulting to "application/octet-stream", in addition to other headers
                            request.ContentType = args.GetExChildValue (
                                "Content-Type", 
                                context, 
                                "application/octet-stream");

                            // Setting other headers
                            SetRequestHeaders (context, request, args);

                            // Binary content
                            stream.Write (byteContent, 0, byteContent.Length);
                        } else {

                            // Some sort of "text" type of content, can also be Hyperlisp
                            // Setting our Content-Type header, defaulting to "text/plain", unless Hyperlisp is given
                            request.ContentType = args.GetExChildValue (
                                "Content-Type", 
                                context, 
                                isHyperlisp ? "application/x-hyperlisp" : "text/plain");

                            // Setting other headers
                            SetRequestHeaders (context, request, args);

                            // Any other type of content, such as string/integer/boolean etc. Converting to string before we write.
                            using (TextWriter writer = new StreamWriter (stream)) {
                                writer.Write (Utilities.Convert<string> (context, content, ""));
                            }
                        }
                    }
                } else {

                    // Checking if this is a POST request, at which case not supplying content is a bug
                    if (method == "POST" || method == "PUT")
                        throw new LambdaException ("No content supplied with '" + method + "' request", args, context);
                    
                    // Only setting headers and returning immediately, since caller supplied empty [content] node, or expression leading into oblivion
                    SetRequestHeaders (context, request, args);
                }
            } else {

                // Checking if this is a POST request, at which case not supplying content is a bug
                if (method == "POST" || method == "PUT")
                    throw new LambdaException ("No content supplied with '" + method + "' request", args, context);

                // Only setting headers and returning immediately, since caller didn't supply [content] node
                SetRequestHeaders (context, request, args);
            }
        }
        
        /*
         * Renders HTTP post/put file request
         */
        private static void RenderFileRequest (
            ApplicationContext context, 
            HttpWebRequest request, 
            Node args, 
            string method)
        {
            // Verifying caller supplied [file] node
            if (args["file"] == null)
                throw new LambdaException ("No [file] node given", args, context);

            // Getting file to post or put, verifying expression does not lead into oblivion
            var file = XUtil.Single<string> (context, args ["file"]);
            if (file == null)
                throw new LambdaException ("No file given, probably an expression leading into oblivion", args, context);

            // Making sure user is authorized to read the file request should send
            context.RaiseNative ("p5.io.authorize.load-file", new Node ("p5.io.authorize.load-file", file).Add ("args", args));

            // Opening request stream, and render file as content of request
            using (Stream stream = request.GetRequestStream ()) {

                // Setting Content-Type to "application/octet-stream", unless file ends with ".hl", or Content-Type is explicitly supplied
                request.ContentType = args.GetExChildValue (
                    "Content-Type", 
                    context, 
                    file.EndsWith (".hl") ? "application/x-hyperlisp" : "application/octet-stream");

                // Setting other HTTP request headers
                SetRequestHeaders (context, request, args);

                // Retrieving root node of web application
                var rootFolder = context.RaiseNative ("p5.core.application-folder").Get<string> (context);

                // Copying FileStream to RequestStream
                using (Stream fileStream = File.OpenRead (rootFolder + file.TrimStart ('/'))) {

                    // Sending file to server end-point
                    fileStream.CopyTo (stream);
                }
            }
        }

        /*
         * Returns content back to caller
         */
        private static object GetRequestContent (
            ApplicationContext context, 
            Node content)
        {
            if (content.Value == null && content.Count > 0) {

                // Hyperlisp content
                return context.RaiseNative ("lambda2lisp", content.Clone ()).Value;
            } else {

                // Some sort of "value" content, either text or binary (byte[])
                return XUtil.Single<object> (context, content, false, null);
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
                     args.Children.Where (idxArg => idxArg.Name != "content" && idxArg.Name != "Content-Type" && idxArg.Name != string.Empty)) {
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

                        // Checking if caller wants to automatically convert to p5.lambda, which is default behavior
                        if (args.GetExChildValue ("convert", context, true)) {

                            // Converting from Hyperlisp to p5.lambda
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
         * Saves response into filename given
         */
        private static void RenderFileResponse (
            ApplicationContext context, 
            HttpWebRequest request, 
            Node args)
        {
            // Getting filename user wants to save response as
            var filename = XUtil.Single<string> (context, args ["file"]);

            // Making sure user is authorized to write/overwrite the file response should be saved to
            context.RaiseNative ("p5.io.authorize.save-file", new Node ("p5.io.authorize.save-file", filename).Add ("args", args));

            // Retrieving HTTP response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
            Node result = args.Add ("result").LastChild;

            // Getting HTTP response headers
            GetResponseHeaders (context, response, result, request);

            // Retrieving response content stream, and parsing as expected by caller
            using (Stream stream = response.GetResponseStream ()) {

                // Retrieving root folder of web application
                var rootFolder = context.RaiseNative ("p5.core.application-folder").Get<string> (context);

                // Copying response content stream to file stream encapsualting file caller requested to save content to
                using (Stream fileStream = File.Create (rootFolder + filename)) {

                    // Copy response stream to file stream
                    stream.CopyTo (fileStream);
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
