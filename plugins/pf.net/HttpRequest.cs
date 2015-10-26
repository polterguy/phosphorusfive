/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Linq;
using pf.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

/// <summary>
///     Namespace wrapping Active Events related to networking.
/// 
///     Contains useful helper Classes for creating HTTP REST requests.
/// </summary>
namespace phosphorus.net
{
    /// <summary>
    ///     Class wrapping the [pf.net.get/post/put/delete] Active Events.
    /// 
    ///     Contains the Active Events necessary to create REST HTTP requests. Use either [pf.net.post], [pf.net.get], [pf.net.put] or
    ///     [pf.net.delete] to create request. Request type is determined according to what Active Event you're using.
    /// </summary>
    public static class HttpRequest
    {
        /// <summary>
        ///     Creates a new HTTP REST request of specified type.
        /// 
        ///     Active Events necessary to create REST HTTP requests. Use either [pf.net.post], [pf.net.get], [pf.net.put] or
        ///     [pf.net.delete] to create request. Request type is determined according to what Active Event you're using.
        /// 
        ///     If you choose post or put, you can optionally choose to have a [content] node, being the actual content you're posting or putting
        ///     into your request. By default, only the basic HTTP headers are returned back, beneath a [result] node, in addition to the [content]
        ///     from the other side. If you wish to have all headers show up for you, then set [all-headers] to true.
        /// 
        ///     [content] you send, can be any type, in addition to that you can optionally choose to set the [content] value to null, and
        ///     add up Hyperlisp beneath your [content] node. If you do, then your Hyperlisp will be automatically transmitted to
        ///     the server end-point.
        /// 
        ///     All other child nodes of your main Active Event node, except [content] and [all-headers] will be automatically treated as 
        ///     HTTP headers, and passed in as such to the server end-point.
        /// 
        ///     Example of posting some Hyperlisp to a web service;
        /// 
        ///     <pre>pf.net.post:"http://127.0.0.1:8080/web-service-1"
        ///   content
        ///     foo:foo-value
        ///     bar:bar-value</pre>
        /// 
        /// In the above sample, your content will be automatically determined to be of type Hyperlisp, and Content-Type of your request will be set
        ///     to <em>"application/Hyperlisp"</em>. Below is an example that will transmit some arbitrary piece of text with Content-Type header
        ///     being explicitly set to "text/mumbo", in addition to a custom header of <em>"foo"</em> having the value of <em>"bar"</em>;
        /// 
        ///     <pre>pf.net.post:"http://127.0.0.1:8080/web-service-1"
        ///   Content-Type:text/mumbo
        ///   foo:bar
        ///   content:foo, bar</pre>
        /// 
        /// You can also transmit binary content using these Active Events. By default the Content-Type header of your request will be set to "text/plain"
        ///     if it is text, and you do not explicitly set the header. If you choose to transmit binary content, the default Content-Type used will
        ///     be "application/octet-stream". The default Content-Type for Hyperlisp as children of [content] node is "application/Hyperlisp".
        /// 
        ///     When the server returns, you will by default find a [Status] node, hopefully having the value of <em>"OK"</em>. Unless it has, you
        ///     will find a [Status-Description] telling you what went wrong from the other side of your request. In addition you will have a [Content-Type]
        ///     node, informing you what type of content was being returned. The last node will be [content], containing your actual content returned
        ///     from the other side.
        /// 
        ///     As previously said, if you add [all-headers] and set its value to true, then all HTTP headers will be returned after execution is
        ///     done.
        /// </summary>
        [ActiveEvent (Name = "pf.net.get")]
        [ActiveEvent (Name = "pf.net.post")]
        [ActiveEvent (Name = "pf.net.put")]
        [ActiveEvent (Name = "pf.net.delete")]
        private static void pf_net_http_request (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here

            // figuring out which method to use
            string method = e.Args.Name.Substring (e.Args.Name.LastIndexOf (".") + 1).ToUpper ();

            // Figuring out URL to create request towards
            var url = XUtil.Single<string> (e.Args, context);
            if (string.IsNullOrEmpty (url))
                return; // nothing to do here, probably expression leading into oblivion

            // Creating actual request
            HttpWebRequest request = WebRequest.Create (url) as HttpWebRequest;
            if (request == null)
                throw new ArgumentException (string.Format ("'{0}' did not create a valid HTTP request URL", url));
            request.Method = method;

            // writing content to request, if any
            RenderRequest (context, request, e.Args, method);

            // returning response to caller
            RenderResponse (context, request, e.Args);
        }

        /// <summary>
        ///     Posts or puts a file over an HTTP request.
        /// 
        ///     Identical to [pf.net.post] and [pf.net.put] except this posts or puts a file, without loading it into memory first.
        ///     Pass in file path as [file] instead of using [content].
        /// 
        ///     Default Content-Type HTTP header used is "application/octet-stream" unless explicitly overridden.
        /// </summary>
        [ActiveEvent (Name = "pf.net.post-file")]
        [ActiveEvent (Name = "pf.net.put-file")]
        private static void pf_net_post_put_file (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here

            // figuring out which method to use
            string method = e.Args.Name.Substring (e.Args.Name.LastIndexOf (".") + 1).ToUpper ();
            method = method.Substring (0, method.IndexOf ("-"));

            // Figuring out URL to create request towards
            var url = XUtil.Single<string> (e.Args, context);
            if (string.IsNullOrEmpty (url))
                return; // nothing to do here, probably expression leading into oblivion

            // Creating actual request
            HttpWebRequest request = WebRequest.Create (url) as HttpWebRequest;
            if (request == null)
                throw new ArgumentException (string.Format ("'{0}' did not create a valid HTTP request URL", url));
            request.Method = method;

            // writing file to request
            RenderFileRequest (context, request, e.Args, method);

            // returning response to caller
            RenderResponse (context, request, e.Args);
        }
        
        /// <summary>
        ///     Gets a file from an HTTP request.
        /// 
        ///     Identical to [pf.net.get] except this retrieves a file, without loading it into memory first, and saves it
        ///     to a specified path. Pass in file path where you wish to save the response as [file]. No [content] is returned.
        /// </summary>
        [ActiveEvent (Name = "pf.net.get-file")]
        private static void pf_net_get_file (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here

            // Figuring out URL to create request towards
            var url = XUtil.Single<string> (e.Args, context);
            if (string.IsNullOrEmpty (url))
                return; // nothing to do here, probably expression leading into oblivion

            // checking that a valid file path is given
            if (e.Args ["file"] == null || string.IsNullOrEmpty (e.Args ["file"].Get<string> (context)))
                throw new ArgumentException ("No valid [file] node given.");
            string fileName = XUtil.Single<string> (e.Args ["file"], context);
            if (string.IsNullOrEmpty (fileName))
                throw new ArgumentException ("No valid [file] given, possibly an expression leading into oblivion.");

            // Creating actual request
            HttpWebRequest request = WebRequest.Create (url) as HttpWebRequest;
            if (request == null)
                throw new ArgumentException (string.Format ("'{0}' did not create a valid HTTP request URL", url));
            request.Method = "GET";

            // writing file to request
            RenderRequest (context, request, e.Args, "GET");

            // returning response to caller
            RenderFileResponse (context, request, e.Args, fileName);
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
                if (method != "PUT" && method != "POST")
                    throw new ArgumentException ("You cannot have content with 'GET' and 'DELETE' types of requests.");

                var content = GetRequestContent (args ["content"], context);
                bool isHyperlisp = args ["content"].Value == null && args ["content"].Count > 0;
                if (content != null) {
                    using (Stream stream = request.GetRequestStream ()) {
                        byte[] byteContent = content as byte[];
                        if (byteContent != null) {

                            // setting our Content-Type header, defaulting to "application/octet-stream", in addition to other headers
                            request.ContentType = args.GetExChildValue ("Content-Type", context, "application/octet-stream");
                            SetRequestHeaders (request, context, args);

                            // binary content
                            stream.Write (byteContent, 0, byteContent.Length);
                        } else {

                            // setting our Content-Type header, defaulting to "text/plain" unless Hyperlisp is given, in addition to other headers
                            request.ContentType = args.GetExChildValue ("Content-Type", context, isHyperlisp ? "application/Hyperlisp" : "text/plain");
                            SetRequestHeaders (request, context, args);

                            // any other type of content, such as string/integer/boolean etc. Converting to string beffore we write.
                            using (TextWriter writer = new StreamWriter (stream)) {
                                writer.Write (Utilities.Convert<string> (content, context, ""));
                            }
                        }
                    }
                }
            } else {
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
            if (args ["file"] == null)
                throw new ArgumentException ("No [file] node given");

            var file = XUtil.Single<object> (args ["file"], context);
            if (file == null)
                throw new ArgumentException ("No file given, or expression leading into oblivion");

            using (Stream stream = request.GetRequestStream ()) {

                // Setting request HTTP headers, defaulting Content-Type to "application/octet-stream"
                request.ContentType = args.GetExChildValue ("Content-Type", context, "application/octet-stream");
                SetRequestHeaders (request, context, args);

                // retrieving root node of web application
                var rootNode = new Node ();
                context.Raise ("pf.core.application-folder", rootNode);
                var rootFolder = rootNode.Get<string> (context);

                // making sure we normalize folder separators, to have uniform folder structure
                // for both Linux and Windows
                rootFolder = rootFolder.Replace ("\\", "/");
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
            if (content.Value != null) {
                return XUtil.Single<object> (content, context);
            } else {
                var cloned = content.Clone ();
                context.Raise ("pf.hyperlisp.lambda2hyperlisp", cloned);
                return cloned.Value;
            }
        }

        /*
         * decorates all headers for request, except Content-Type which is handled in caller
         */
        private static void SetRequestHeaders (HttpWebRequest request, ApplicationContext context, Node args)
        {
            foreach (var idxHeader in 
                     args.Children.Where (idxArg => idxArg.Name != "content" && idxArg.Name != "Content-Type" && idxArg.Name != "all-headers")) {
                switch (idxHeader.Name) {
                case "Accept":
                    request.Accept = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Connection":
                    request.Connection = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Content-Length":
                    request.ContentLength = XUtil.Single<long> (idxHeader.Value, idxHeader, context);
                    break;
                case "Content-Type":
                    request.ContentType = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Date":
                    request.Date = XUtil.Single<DateTime> (idxHeader.Value, idxHeader, context);
                    break;
                case "Expect":
                    request.Expect = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Host":
                    request.Host = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "If-Modifies-Since":
                    request.IfModifiedSince = XUtil.Single<DateTime> (idxHeader.Value, idxHeader, context);
                    break;
                case "Referer":
                    request.Referer = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Transfer-Encoding":
                    request.TransferEncoding = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "User-Agent":
                    request.UserAgent = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                default:
                    request.Headers.Add (idxHeader.Name, XUtil.Single<string> (idxHeader.Value, idxHeader, context));
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
            Node result = args.Add ("result").LastChild;
            GetResponseHeaders (context, response, result);
            using (Stream stream = response.GetResponseStream ()) {
                if (response.ContentType.StartsWith ("application/Hyperlisp")) {

                    // Hyperlisp, special treatment
                    using (TextReader reader = new StreamReader (stream)) {
                        Node convert = new Node ("content", reader.ReadToEnd ());
                        context.Raise ("pf.hyperlisp.hyperlisp2lambda", convert);
                        convert.Value = null;
                        result.Add (convert);
                    }
                } else if (response.ContentType.StartsWith ("text")) {

                    // text response
                    using (TextReader reader = new StreamReader (stream)) {
                        result.Add ("content", reader.ReadToEnd ());
                    }
                } else {

                    // defaulting to binary
                    // TODO: check up which non-text MIME types are actually textually based
                    using (MemoryStream memStream = new MemoryStream ()) {
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
            GetResponseHeaders (context, response, result);
            using (Stream stream = response.GetResponseStream ()) {

                // retrieving root node of web application
                var rootNode = new Node ();
                context.Raise ("pf.core.application-folder", rootNode);
                var rootFolder = rootNode.Get<string> (context);

                // making sure we normalize folder separators, to have uniform folder structure
                // for both Linux and Windows
                rootFolder = rootFolder.Replace ("\\", "/");

                using (Stream fileStream = File.Create (rootFolder + XUtil.Single<string> (args ["file"], context))) {
                    stream.CopyTo (fileStream);
                }
            }
        }

        /*
         * returns the HTTP response headers into node given
         */
        private static void GetResponseHeaders (ApplicationContext context, HttpWebResponse response, Node args)
        {
            args.Add ("Status", response.StatusCode.ToString ());
            if (response.StatusCode != HttpStatusCode.OK)
                args.Add ("Status-Description", response.StatusDescription);
            if (!string.IsNullOrEmpty (response.ContentType))
                args.Add ("Content-Type", response.ContentType);

            // Adding the rest of the headers is caller requested such
            if (args.Parent.GetExChildValue ("all-headers", context, false)) {
                if (!string.IsNullOrEmpty (response.CharacterSet))
                    args.Add ("Character-Set", response.CharacterSet);
                if (!string.IsNullOrEmpty (response.ContentEncoding))
                    args.Add ("Content-Encoding", response.ContentEncoding);
                args.Add ("Last-Modified", response.LastModified);
                args.Add ("Response-Uri", response.ResponseUri.ToString ());
                args.Add ("Server", response.Server);
                foreach (string idxHeader in response.Headers.Keys) {
                    if (idxHeader != "Server" && idxHeader != "Content-Type")
                        args.Add (idxHeader, response.Headers [idxHeader]);
                }
            }
        }
    }
}
