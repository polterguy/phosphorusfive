/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using p5.core;
using p5.exp;

/// <summary>
///     Namespace wrapping Active Events related to networking.
/// 
///     Contains useful helper Classes for creating HTTP REST requests.
/// </summary>
namespace p5.net
{
    /// <summary>
    ///     Class wrapping the [p5.net.http-get/post/put/delete] Active Events.
    /// 
    ///     Contains the Active Events necessary to create REST HTTP requests. Use either [p5.net.post], [p5.net.get], [p5.net.put] or
    ///     [p5.net.delete] to create request. Request type is determined according to what Active Event you're using.
    /// </summary>
    public static class HttpRequest
    {
        /// <summary>
        ///     Creates a new HTTP REST request of specified type.
        /// 
        ///     Active Events necessary to create REST HTTP requests. Use either [p5.net.post], [p5.net.get], [p5.net.put] or
        ///     [p5.net.delete] to create request. Request type is determined according to what Active Event you're using.
        /// </summary>
        [ActiveEvent (Name = "p5.net.http-get")]
        [ActiveEvent (Name = "p5.net.http-post")]
        [ActiveEvent (Name = "p5.net.http-put")]
        [ActiveEvent (Name = "p5.net.http-delete")]
        private static void p5_net_http_request (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here

            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover args = new Utilities.ArgsRemover (e.Args)) {

                // figuring out which method to use
                string method = e.Args.Name.Substring (e.Args.Name.IndexOf ("-") + 1).ToUpper ();

                // iterating through each request URL given
                foreach (var idxUrl in XUtil.Iterate<string> (e.Args, context)) {

                    if (string.IsNullOrEmpty (idxUrl))
                        continue; // nothing to do here, probably expression leading into oblivion

                    // Creating actual request
                    try {
                        HttpWebRequest request = WebRequest.Create (idxUrl) as HttpWebRequest;
                        if (request == null)
                            throw new ArgumentException (string.Format ("'{0}' did not create a valid HTTP request URL", idxUrl));

                        // setting HTTP method
                        request.Method = method;

                        // writing content to request, if any
                        RenderRequest (context, request, e.Args, method);

                        // returning response to caller
                        RenderResponse (context, request, e.Args);
                    } catch (Exception err) {
                        e.Args.Add (idxUrl, string.Format ("Something went wrong with request, error message was; '{0}'", err.Message));
                    }
                }
            }
        }

        /// <summary>
        ///     Posts or puts a file over an HTTP request.
        /// 
        ///     Identical to [p5.net.post] and [p5.net.put] except this posts or puts a file, without loading it into memory first.
        ///     Pass in file path as [file] instead of using [content].
        /// 
        ///     Default Content-Type HTTP header used is "application/octet-stream" unless explicitly overridden.
        /// </summary>
        [ActiveEvent (Name = "p5.net.http-post-file")]
        [ActiveEvent (Name = "p5.net.http-put-file")]
        private static void p5_net_http_post_put_file (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here
            
            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover args = new Utilities.ArgsRemover (e.Args)) {

                // figuring out which method to use
                string method = e.Args.Name.Substring (e.Args.Name.LastIndexOf (".") + 1).ToUpper ();
                method = method.Substring (0, method.IndexOf ("-"));

                // iterating through each request URL given
                foreach (var idxUrl in XUtil.Iterate<string> (e.Args, context)) {

                    if (string.IsNullOrEmpty (idxUrl))
                        continue; // nothing to do here, probably expression leading into oblivion

                    // Creating actual request
                    try {
                        HttpWebRequest request = WebRequest.Create (idxUrl) as HttpWebRequest;
                        if (request == null)
                            throw new ArgumentException (string.Format ("'{0}' did not create a valid HTTP request URL", idxUrl));

                        // setting request HTTP method
                        request.Method = method;

                        // writing file to request
                        RenderFileRequest (context, request, e.Args, method);

                        // returning response to caller
                        RenderResponse (context, request, e.Args);
                    } catch (Exception err) {
                        e.Args.Add (idxUrl, string.Format ("Something went wrong with request, error message was; '{0}'", err.Message));
                    }
                }
            }
        }
        
        /// <summary>
        ///     Gets a file from an HTTP request.
        /// 
        ///     Identical to [p5.net.get] except this retrieves a file, without loading it into memory first, and saves it
        ///     to a specified path. Pass in file path where you wish to save the response as [file]. No [content] is returned.
        /// </summary>
        [ActiveEvent (Name = "p5.net.http-get-file")]
        private static void p5_net_http_get_file (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here
            
            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover args = new Utilities.ArgsRemover (e.Args)) {

                // Figuring out URL to create request towards
                var url = XUtil.Single<string> (e.Args, context);
                if (string.IsNullOrEmpty (url))
                    return; // nothing to do here, probably expression leading into oblivion

                // checking that a valid file path is given
                if (e.Args ["file"] == null || string.IsNullOrEmpty (e.Args ["file"].Get<string> (context)))
                    throw new ArgumentException ("No valid [file] node given to [p5.net.http-get-file].");

                string fileName = XUtil.Single<string> (e.Args ["file"], context);
                if (string.IsNullOrEmpty (fileName))
                    throw new ArgumentException ("No valid [file] given to [p5.net.http-get-file], possibly an expression leading into oblivion.");

                // Creating actual request
                HttpWebRequest request = WebRequest.Create (url) as HttpWebRequest;
                if (request == null)
                    throw new ArgumentException (string.Format ("'{0}' did not create a valid HTTP request URL", url));

                // setting method
                request.Method = "GET";

                // writing file to request
                RenderRequest (context, request, e.Args, "GET");

                // returning response to caller
                RenderFileResponse (context, request, e.Args, fileName);
            }
        }

        /*
         * renders HTTP request
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
                    throw new ArgumentException ("You cannot have content with 'GET' and 'DELETE' types of requests.");

                // Retrieving actual content to post or put
                var content = GetRequestContent (args ["content"], context);

                // Checking to see if this is Hyperlisp content, since we're b y default setting Content-Type to application/Hyperlisp if it is
                bool isHyperlisp = args ["content"].Value == null && args ["content"].Count > 0;

                if (content != null) {

                    // caller supplied actual content in [content] node (as opposed to for instance an expression leading to oblivion)
                    using (Stream stream = request.GetRequestStream ()) {

                        // checking if this is binary content
                        byte[] byteContent = content as byte[];
                        if (byteContent != null) {

                            // setting our Content-Type header, defaulting to "application/octet-stream", in addition to other headers
                            request.ContentType = args.GetExChildValue (
                                "Content-Type", 
                                context, 
                                "application/octet-stream");

                            // setting other headers
                            SetRequestHeaders (request, context, args);

                            // binary content
                            stream.Write (byteContent, 0, byteContent.Length);
                        } else {

                            // some sort of "text" type of content, can also be Hyperlisp
                            // setting our Content-Type header, defaulting to "text/plain" unless Hyperlisp is given
                            request.ContentType = args.GetExChildValue (
                                "Content-Type", 
                                context, 
                                isHyperlisp ? "application/Hyperlisp" : "text/plain");

                            // setting other headers
                            SetRequestHeaders (request, context, args);

                            // any other type of content, such as string/integer/boolean etc. Converting to string beffore we write.
                            using (TextWriter writer = new StreamWriter (stream)) {
                                writer.Write (Utilities.Convert<string> (content, context, ""));
                            }
                        }
                    }
                } else {
                    
                    // Only setting headers and returning immediately since caller supplied empty [content] node, or expression leading into oblivion
                    SetRequestHeaders (request, context, args);
                }
            } else {

                // Only setting headers and returning immediately since caller didn't supply [content] node
                SetRequestHeaders (request, context, args);
            }
        }
        
        /*
         * renders HTTP post/put file request
         */
        private static void RenderFileRequest (
            ApplicationContext context, 
            HttpWebRequest request, 
            Node args, 
            string method)
        {
            // verifying syntax
            if (args ["file"] == null)
                throw new ArgumentException ("No [file] node given");

            // getting file to post or put
            var file = XUtil.Single<string> (args ["file"], context);
            if (file == null)
                throw new ArgumentException ("No file given, or expression leading into oblivion");

            using (Stream stream = request.GetRequestStream ()) {

                // Setting Content-Type to "application/octet-stream", unless file ends with ".hl", or Content-Type is explicitly supplied
                request.ContentType = args.GetExChildValue (
                    "Content-Type", 
                    context, 
                    file.EndsWith (".hl") ? "application/Hyperlisp" : "application/octet-stream");

                // seting other HTTP request headers
                SetRequestHeaders (request, context, args);

                // retrieving root node of web application
                var rootNode = new Node ();
                context.Raise ("p5.core.application-folder", rootNode);
                var rootFolder = rootNode.Get<string> (context);

                // copying FileStream to RequestStream, and pushing file to server end-point
                using (Stream fileStream = File.OpenRead (rootFolder + file)) {
                    fileStream.CopyTo (stream);
                }
            }
        }

        /*
         * returns content back to caller
         */
        private static object GetRequestContent (Node content, ApplicationContext context)
        {
            if (content.Value == null) {

                // Hyperlisp content
                return context.Raise ("lambda2lisp", content.Clone ()).Value;
            } else {

                // some sort of "value" content, either text or binary (byte[])
                return XUtil.Single<object> (content, context);
            }
        }

        /*
         * decorates all headers for request, except Content-Type which is handled in caller
         */
        private static void SetRequestHeaders (HttpWebRequest request, ApplicationContext context, Node args)
        {
            foreach (var idxHeader in 
                     args.Children.Where (idxArg => idxArg.Name != "content" && idxArg.Name != "Content-Type" && idxArg.Name != string.Empty)) {
                switch (idxHeader.Name) {
                case "Accept":
                    request.Accept = XUtil.Single<string> (idxHeader, idxHeader, context);
                    break;
                case "Connection":
                    request.Connection = XUtil.Single<string> (idxHeader, idxHeader, context);
                    break;
                case "Content-Length":
                    request.ContentLength = XUtil.Single<long> (idxHeader, idxHeader, context);
                    break;
                case "Content-Type":
                    request.ContentType = XUtil.Single<string> (idxHeader, idxHeader, context);
                    break;
                case "Date":
                    request.Date = XUtil.Single<DateTime> (idxHeader, idxHeader, context);
                    break;
                case "Expect":
                    request.Expect = XUtil.Single<string> (idxHeader, idxHeader, context);
                    break;
                case "Host":
                    request.Host = XUtil.Single<string> (idxHeader, idxHeader, context);
                    break;
                case "If-Modifies-Since":
                    request.IfModifiedSince = XUtil.Single<DateTime> (idxHeader, idxHeader, context);
                    break;
                case "Referer":
                    request.Referer = XUtil.Single<string> (idxHeader, idxHeader, context);
                    break;
                case "Transfer-Encoding":
                    request.TransferEncoding = XUtil.Single<string> (idxHeader, idxHeader, context);
                    break;
                case "User-Agent":
                    request.UserAgent = XUtil.Single<string> (idxHeader, idxHeader, context);
                    break;
                default:
                    request.Headers.Add (idxHeader.Name, XUtil.Single<string> (idxHeader, idxHeader, context));
                    break;
                }
            }
        }

        /*
         * renders response into given Node
         */
        private static void RenderResponse (ApplicationContext context, HttpWebRequest request, Node args)
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
            Node result = args.Add ("result", request.RequestUri.ToString ()).LastChild;

            // getting response HTTP headers
            GetResponseHeaders (context, response, result, request);

            // retrieving response stream, and parsing content
            using (Stream stream = response.GetResponseStream ()) {

                // checking type of response
                if (response.ContentType.StartsWith ("application/Hyperlisp")) {

                    // Hyperlisp, special treatment
                    using (TextReader reader = new StreamReader (stream)) {

                        // converting from Hyperlisp to p5.lambda
                        Node convert = context.Raise ("lisp2lambda", new Node ("content", reader.ReadToEnd ()));
                        convert.Value = null;
                        result.Add (convert);
                    }
                } else if (response.ContentType.StartsWith ("text")) {

                    // text response
                    using (TextReader reader = new StreamReader (stream)) {

                        // simply adding as text
                        result.Add ("content", reader.ReadToEnd ());
                    }
                } else {

                    // defaulting to binary
                    // TODO: check up which non-text MIME types are actually textually based
                    using (MemoryStream memStream = new MemoryStream ()) {

                        // simply adding as byte[]
                        stream.CopyTo (memStream);
                        result.Add ("content", memStream.GetBuffer ());
                    }
                }
            }
        }
        
        /*
         * saves response into filename given
         */
        private static void RenderFileResponse (ApplicationContext context, HttpWebRequest request, Node args, string fileName)
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
            Node result = args.Add ("result").LastChild;

            // getting HTTP response headers
            GetResponseHeaders (context, response, result, request);

            // retrieving response content stream, and parsing as expected by caller
            using (Stream stream = response.GetResponseStream ()) {

                // retrieving root folder of web application
                var rootFolder = context.Raise ("p5.core.application-folder").Get<string> (context);

                // copying response content stream to file stream encapsualting file caller requested to save content to
                using (Stream fileStream = File.Create (rootFolder + XUtil.Single<string> (args ["file"], context))) {

                    stream.CopyTo (fileStream);
                }
            }
        }

        /*
         * returns the HTTP response headers into node given
         */
        private static void GetResponseHeaders (ApplicationContext context, HttpWebResponse response, Node args, HttpWebRequest request)
        {
            // we only add [Status] node if status was NOT OK! At which point we also supply the error description to caller
            if (response.StatusCode != HttpStatusCode.OK) {

                args.Add ("status", response.StatusCode.ToString ());
                args.Add ("Status-Description", response.StatusDescription);
            }

            // checking to see if Content-Type is given, and if so, adding header to caller
            if (!string.IsNullOrEmpty (response.ContentType))
                args.Add ("Content-Type", response.ContentType);

            // Character-Set
            // TODO: Check up if this is actually a part of "Content-Type" (which I suspect)
            if (!string.IsNullOrEmpty (response.CharacterSet))
                args.Add ("Character-Set", response.CharacterSet);

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

            // the rest of the HTTP headers
            foreach (string idxHeader in response.Headers.Keys) {

                if (idxHeader != "Server" && idxHeader != "Content-Type")
                    args.Add (idxHeader, response.Headers [idxHeader]);
            }
        }
    }
}
