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
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.http
{
    /// <summary>
    ///     Class wrapping the [p5.http.get/post/put/delete] Active Events
    /// </summary>
    public static class HttpRequest
    {
        // Specialized delegate functors for rendering request and response
        delegate void RenderRequestFunctor (ApplicationContext context, HttpWebRequest request, Node args, string method);
        delegate void RenderResponseFunctor (ApplicationContext context, HttpWebRequest request, Node args);

        /// <summary>
        ///     Creates a new HTTP REST request of specified type
        /// </summary>
        [ActiveEvent (Name = "p5.http.get")]
        [ActiveEvent (Name = "p5.http.post")]
        [ActiveEvent (Name = "p5.http.put")]
        [ActiveEvent (Name = "p5.http.delete")]
        public static void p5_net_http_get_post_put_delete (ApplicationContext context, ActiveEventArgs e)
        {
            CreateRequest (context, e.Args, RenderRequest, RenderResponse);
        }

        /// <summary>
        ///     Posts or puts a file over an HTTP request
        /// </summary>
        [ActiveEvent (Name = "p5.http.post-file")]
        [ActiveEvent (Name = "p5.http.put-file")]
        public static void p5_http_post_put_file (ApplicationContext context, ActiveEventArgs e)
        {
            CreateRequest (context, e.Args, RenderFileRequest, RenderResponse);
        }

        /// <summary>
        ///     Gets a file from an HTTP request
        /// </summary>
        [ActiveEvent (Name = "p5.http.get-file")]
        public static void p5_http_get_file (ApplicationContext context, ActiveEventArgs e)
        {
            CreateRequest (context, e.Args, RenderRequest, RenderFileResponse);
        }

        /*
         * Actual implementation of creation of HTTP request
         */
        static void CreateRequest (
            ApplicationContext context, 
            Node args, 
            RenderRequestFunctor renderRequest, 
            RenderResponseFunctor renderResponse)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new ArgsRemover (args, true)) {

                // Figuring out which HTTP method to use
                string method = args.Name.Split ('.').Last ().ToUpper ();
                if (method.Contains ("-"))
                    method = method.Substring (0, method.IndexOfEx ("-"));
                try
                {
                    // Iterating through each request URL given
                    foreach (var idxUrl in XUtil.Iterate<string> (context, args)) {

                        // Creating request
                        var request = WebRequest.Create (idxUrl) as HttpWebRequest;

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
        static void RenderRequest (
            ApplicationContext context, 
            HttpWebRequest request, 
            Node args, 
            string method)
        {
            // Setting request headers
            SetRequestHeaders (context, request, args);

            // Checking if this is a "content" request, which also might be the case if it is an HTTP MIME request.
            // Notice we must ignore previously created results here, in addition to formatting expression.
            // We are also ignoring everything that has the structure of an HTTP header.
            var contentNode = args.Children.FirstOrDefault (ix => ix.Name != "" && ix.Name != "result" && ix.Name.ToLower () == ix.Name);

            // Checking if there exists content for request.
            if (contentNode != null) {

                // We've got content to post or put, making sure caller is not trying to submit content over HTTP get or delete requests
                if (method != "PUT" && method != "POST")
                    throw new LambdaException (
                        "You cannot have content with 'GET' and 'DELETE' types of requests", 
                        args, 
                        context);

                using (var stream = request.GetRequestStream ()) {

                    // Serializing request into stream.
                    SerializeRequest (context, contentNode, stream);
                }
			} else {

                // Checking if this is a POST request, at which case not supplying content is a bug
                if (method == "POST" || method == "PUT")
                    throw new LambdaException (
                        "No content supplied with '" + method + "' request", 
                        args, 
                        context);
            }
        }

        /*
         * Helper for above.
         */
        static void SerializeRequest (ApplicationContext context, Node contentNode, Stream stream)
        {
            // Checking type of content.
            if (contentNode.Name == "content") {

                // Hyperlambda or plain text content.
                var content = GetRequestContent (context, contentNode);
				var byteContent = content as byte [];
				if (byteContent != null) {

					// Binary content.
					stream.Write (byteContent, 0, byteContent.Length);

				} else {

					// Some sort of "text" type of content.
					using (TextWriter writer = new StreamWriter (stream)) {

						// Converting to string before we write.
						writer.Write (Utilities.Convert (context, content, ""));
					}
				}

			} else {

				// Attempting to create MIME envelope out of content, and serialize directly into Stream.
				var mimeNode = new Node ("", stream);
				mimeNode.Add ("", contentNode);
				context.RaiseEvent (".p5.mime.save2stream", mimeNode);
			}
        }

        /*
         * Renders HTTP post/put file request
         */
        static void RenderFileRequest (
            ApplicationContext context, 
            HttpWebRequest request, 
            Node args, 
            string method)
        {
            // Verifying caller supplied [filename] node.
            if (args["filename"] == null || args["filename"].Value == null)
                throw new LambdaException (
                    "No [filename] node given", 
                    args, 
                    context);

            // Getting file to post or put, verifying expression does not lead into oblivion
            var filename = context.RaiseEvent (".p5.io.unroll-path", new Node ("", XUtil.Single<string> (context, args["filename"]))).Get<string> (context);

            // Making sure user is authorized to read the file request should send
            context.RaiseEvent (".p5.io.authorize.read-file", new Node ("", filename).Add ("args", args));

            // Opening request stream, and render file as content of request
            using (Stream stream = request.GetRequestStream ()) {

                // Setting other HTTP request headers
                SetRequestHeaders (context, request, args);

                // Retrieving root node of web application
                var rootFolder = context.RaiseEvent (".p5.core.application-folder").Get<string> (context);

                // Copying FileStream to RequestStream
                using (Stream fileStream = File.OpenRead (rootFolder + filename)) {

                    // Sending file to server end-point
                    fileStream.CopyTo (stream);
                }
            }
        }

        /*
         * Returns content back to caller
         */
        static object GetRequestContent (
            ApplicationContext context, 
            Node content)
        {
            // Checking for Hyperlambda.
            if (content.Value == null && content.Count > 0) {

                // Hyperlambda content.
                return context.RaiseEvent ("lambda2hyper", content.Clone ()).Value;
            }

            // Some sort of "value" content, either text content, or binary blob (byte[])
            return XUtil.Single<object> (context, content);
        }

        /*
         * Decorates all headers for request, except Content-Type, which should be handled by caller
         */
        static void SetRequestHeaders (
            ApplicationContext context, 
            HttpWebRequest request, 
            Node args)
        {
            // Redmond, this is ridiculous! Why can't we set headers in a uniform way ...?
            foreach (var idxHeader in args.Children.Where (idxArg => idxArg.Name.ToLower () != idxArg.Name)) {
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
        static void RenderResponse (
            ApplicationContext context, 
            HttpWebRequest request, 
            Node args)
        {
            var response = request.GetResponseNoException ();
            Node result = args.Add ("result", request.RequestUri.ToString ()).LastChild;

            // Getting response HTTP headers
            GetResponseHeaders (context, response, result, request);

            // Retrieving response stream, and parsing content
            using (Stream stream = response.GetResponseStream ()) {

                // Checking type of response
                if (response.ContentType.StartsWithEx ("application/x-hyperlambda")) {

                    // Hyperlambda, possibly special treatment
                    using (TextReader reader = new StreamReader (stream, Encoding.GetEncoding (response.CharacterSet ?? "UTF8"))) {

                        // Checking if caller wants to automatically convert to p5 lambda, which is default behavior
                        if (args.GetExChildValue ("convert", context, true)) {

                            // Converting from Hyperlambda to p5 lambda
                            Node convert = context.RaiseEvent ("hyper2lambda", new Node ("content", reader.ReadToEnd ()));
                            convert.Value = null;
                            result.Add (convert);

                        } else {

                            // Caller explicitly said he did NOT want to convert
                            result.Add ("content", reader.ReadToEnd());
                        }
                    }
                } else if (response.ContentType.StartsWithEx ("text") || 
                    response.ContentType.StartsWithEx ("application/rss+xml") ||
                    response.ContentType.StartsWithEx ("application/xml")) {

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
                        result.Add ("content", memStream.ToArray ());
                    }
                }
            }
        }

        /*
         * Saves response into filename given
         */
        static void RenderFileResponse (
            ApplicationContext context, 
            HttpWebRequest request, 
            Node args)
        {
            // Getting filename user wants to save response as
            var filename = context.RaiseEvent (".p5.io.unroll-path", new Node ("", XUtil.Single<string> (context, args ["filename"]))).Get<string>(context);

            // Making sure user is authorized to write/overwrite the file response should be saved to
            context.RaiseEvent (".p5.io.authorize.modify-file", new Node ("", filename).Add ("args", args));

            // Retrieving HTTP response
            var response = request.GetResponseNoException ();
            Node result = args.Add ("result").LastChild;

            // Getting HTTP response headers
            GetResponseHeaders (context, response, result, request);

            // Retrieving response content stream, and parsing as expected by caller
            using (Stream stream = response.GetResponseStream ()) {

                // Retrieving root folder of web application
                var rootFolder = context.RaiseEvent (".p5.core.application-folder").Get<string> (context);

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
        static void GetResponseHeaders (
            ApplicationContext context, 
            HttpWebResponse response, 
            Node args, 
            HttpWebRequest request)
        {
            // Adding status code
            args.Add ("status", response.StatusCode.ToString ());
            args.Add ("Status-Description", response.StatusDescription);

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

        /*
         * Helper to retrieve response without exception, if possible
         */
        static HttpWebResponse GetResponseNoException (this HttpWebRequest req)
        {
            try
            {
                return (HttpWebResponse)req.GetResponse();
            }
            catch (WebException we)
            {
                var resp = we.Response as HttpWebResponse;
                if (resp == null)
                    throw;
                return resp;
            }
        }
    }
}
