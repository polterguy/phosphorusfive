/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

/// <summary>
///     Namespace wrapping Active Events related to HTTP REST requests.
/// 
///     Contains useful helper Classes for creating HTTP REST requests.
/// </summary>
namespace phosphorus.web
{
    /// <summary>
    ///     Class wrapping [pf.net.create-request] Active Event.
    /// 
    ///     Contains the [pf.net.create-request] Active Event, and its associated helper methods.
    /// </summary>
    public static class RequestOld
    {
        /// <summary>
        ///     Creates a new HTTP REST request.
        /// 
        ///     Choose which type of request you wish to create as [method], which can be either 'GET', 'PUT', 'POST' or 'DELETE'. You can 
        ///     add HTTP headers to your request as a key/value collection beneath [headers]. In addition you can transfer cookies to the server
        ///     through [cookie] as key/value pair nodes.
        /// 
        ///     The content or arguments you wish to transfer can be given through [content], which is a collection of key/value arguments, which will
        ///     be transferred to the server end-point. You can also pass in files through [files]. Both files and arguments passed in this way, 
        ///     will be transferred as a MIME message, if you choose to send the request as either a 'POST' or 'PUT' request, and you set the 
        ///     'Content-Type' header to 'multipart/xxx', where 'xxx' can be freely chosen by you.
        /// 
        ///     Any files passed in through [files], will not be loaded into memory, but directly transfered over the HTTP request, preserving 
        ///     memory on your client.
        /// 
        ///     If you do not set the 'Content-Type' header, and your request is either a 'POST' or 'PUT' type of request, then its default value will be 
        ///     'application/x-www-form-urlencoded', and the request create will be url-encoded. If your request is of type 'GET' or 'DELETE', 
        ///     then all arguments, and files passed in, will be sent in using the HTTP URL, meaning they will be a part of the URL of your request.
        /// 
        ///     If you create a 'POST' or 'PUT' request, and you are setting the 'Content-Type' header to 'multipart/anything', then you can
        ///     supply MIME headers for each object, [files] and/or [content], as children nodes of your argument(s) or file(s). All MIME headers set this
        ///     way, will be correctly handled by this Active Event. This means that you can for instance choose a transfer encoding for your argument(s) 
        ///     and/or file(s) as 'Content-Transfer-Encoding', and content type as 'Content-Type', etc. You can for instance set the 'Content-Transfer-Encoding' 
        ///     to 'binary', 'base64', or any of the other transfer encoding values supported by MimeKit. If you do not set the 'Content-Disposition' 
        ///     header, then for a file transfered through [files], it will default to "form-data; name='node-name'; filename='filepath/name.ext'", 
        ///     where 'node-name' is the name of the node where you supply your file, and 'filepath/name.ext' is the path and name of your file. If 
        ///     you do not supply a 'Content-Disposition' header for an argument transferred through [content], then its default value will be 
        ///     'form-data; name=node-name'.
        /// 
        ///     If you have multiple arguments and/or files, then it is probably wise of you to choose 'multipart/mixed' or 'multipart/form-data' as
        ///     your 'Content-Type' header of your request, since some web servers will reject the request, if the URL used is too long.
        /// 
        ///     Example that will create a MIME multipart HTTP request;
        /// 
        ///     <pre>pf.net.create-request:"http://127.0.0.1:8080/echo"
        ///   method:post
        ///   headers
        ///     Content-Type:multipart/mixed
        ///     foo-header:foo header value
        ///   cookies
        ///     foo-cookie:foo cookie value
        ///   content
        ///     foo-arg:Howdy world
        ///       Content-Type:text/plain
        ///   files
        ///     file1:foo-file.hl
        ///       Content-Type:text/Hyperlisp
        ///     file2:image.png
        ///       Content-Type:image/png
        ///       Content-Transfer-Encoding:base64</pre>
        /// 
        ///     Example that will create a 'application/x-www-form-urlencoded' type of request;
        /// 
        ///     <pre>pf.net.create-request:"http://127.0.0.1:8080/echo"
        ///   method:post
        ///   content
        ///     foo-arg:Howdy world
        ///       this-bugger-will:be ignored!!!
        ///     bar-arg:Yo Dude!</pre>
        /// 
        ///     Please notice that in a 'application/x-www-form-urlencoded' type of request, any children nodes of your arguments will be ignored. 
        ///     You can still supply [cookies] and/or [headers]. The same is true for any type of 'GET' or 'DELETE' request.
        /// 
        ///     Also notice, that this Active Event will automatically parse any multipart (MIME) messages returned from the server end point,
        ///     unless you set the [parse-mime] parameter to 'false', at which point the response from the server, will be one single return value.
        /// 
        ///     After execution, this Active Event will create one [result] node for each URL you gave as input, adding all headers, cookies, and
        ///     contents returned from server as [headers], [cookies] and [content]. Beneath the [content] node, any MIME headers for that particular
        ///     piece of return value, will be appended as key/value children, and the actual contents of that MIME part can be found in [value].
        /// 
        ///     If the server returns anything but MIME, or you set [parse-mime] to false, then this Active Event will attempt to figure out whether or 
        ///     not the returned value is binary or text, by inspecting the response headers, and return it accordingly. This behavior can be overridden 
        ///     by adding either [force-text] or [force-binary] as parameters, setting their values to 'true', which will override the default attempt 
        ///     at trying to figure out what type of response the server returned, and force the Active Event to treat the returned value as either 
        ///     'text' or 'binary' when returning the result to caller.
        /// 
        ///     If the server endpoint sets the 'Content-Disposition' header for a MIME part, supplying a 'name' parameter, then the name of the
        ///     [content] node, will be set to the 'name' parameters from the Content-Disposition header of that MIME part. This allows for 
        ///     returning 'named arguments' from your server end-points easily.
        /// 
        ///     If you create a request with the HTTP header 'Content-Type' being some sort of 'text/xxx' type, then this Active Event will append a CR/LF
        ///     sequence between all your arguments and/or files, but still pass them in as 'one object'. This allows you to for instance, create complex
        ///     Hyperlisp graph objects, by combining [files], and/or [content] together, having them concatenated into one piece of Hyperlisp, before sending
        ///     the combined result to your server end-point.
        /// 
        ///     Regardless of which type of request you create, you can repeat the [content] node, and/or the [files] nodes, as many times as you wish, and
        ///     all [content], and/or [files], will be appended into your request, in the order they are declared.
        /// 
        ///     Since this Active Event does not necessarily rely upon encoding binary objects, and/or binary files as base64, in addition to that it does neither
        ///     temporary store files in memory when transmitting files to the server end-point, while still being able to send multiple documents
        ///     to its server end-point, it should significantly outperform most alternative ways of invoking Web Services, such as for instance SOAP, while
        ///     still being 100% SOAP compatible, since you can just as well transfer XML. At the same time, it is not in any ways 'breaking' the philosophy
        ///     of REST in any ways, since it 100% perfectly relies upon REST at its core. All other things set aside, this should outperform traditional
        ///     Web Services significantly. Both in bandwidth usage, and in resource usage, for both the client, and the server.
        /// 
        ///     By combining this Active Event with the <see cref="phosphorus.web.ui.response.Echo.pf_web_response_echo">[pf.web.response.echo]</see> and 
        ///     <see cref="phosphorus.web.ui.request.ParseMime.pf_web_request_parse_mime">[pf.web.request.parse-mime]</see>/<see cref="phosphorus.web.ui.request.Parameters.pf_web_request_parameters_get">[pf.web.request.parameters.get]</see>
        ///     Active Events on the server-side, you can easily create and consume Web Services in your applications.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.net.create-request-OLD")]
        private static void pf_web_create_request (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here

            // iterating through every URL requested by caller
            foreach (var idxUrl in XUtil.Iterate<string> (e.Args.Value, e.Args, context)) {

                // creates, decorates, and executes request
                HttpWebResponse response = ExecuteRequest (idxUrl, e.Args, context);
                if (response == null)
                    return; // possibly an HTTP verb we do not support

                // adding "result node" for current request
                e.Args.Add ("result", idxUrl);

                // returning result of request, headers, cookies and content. First headers
                ParseHeaders (response, e.Args.LastChild);

                // then cookies
                ParseCookies (response, e.Args.LastChild);

                // then content
                ParseContent (
                    response, 
                    e.Args.LastChild, 
                    context, 
                    XUtil.Single<bool> (e.Args.GetChildValue<object> ("parse-mime", context, null), e.Args ["parse-mime"], context, true),
                    XUtil.Single<bool> (e.Args.GetChildValue<object> ("force-text", context, null), e.Args ["force-text"], context, false),
                    XUtil.Single<bool> (e.Args.GetChildValue<object> ("force-binary", context, null), e.Args ["force-binary"], context, false));
            }
        }

        /*
         * creates one request
         */
        private static HttpWebResponse ExecuteRequest (string url, Node node, ApplicationContext context)
        {
            // creating request, and adding headers and cookies, setting some properties, and returning response to caller
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create (CreateUrl (url, node, context));
            
            // setting properties
            request.Timeout = XUtil.Single (node.GetChildValue<object> ("timeout", context, null), node ["timeout"], context, 100000);
            request.Method = XUtil.Single (node.GetChildValue<object> ("method", context, null), node ["method"], context, "GET").ToUpper ();

            // adding headers
            AddHeadersToRequest (request, node, context);

            // adding cookies
            AddCookiesToRequest (request, node, context);

            // retrieving response, and returning to caller
            return ExecuteRequest (request, node, context);
        }
        
        /*
         * creates the URL for the current request
         * if this is a "GET" method request, then all [content] and [files] nodes will be a part of the URL, in encoded format
         */
        private static string CreateUrl (string url, Node node, ApplicationContext context)
        {
            // figuring out what type of request this is, defaulting to GET unless [method] is explicitly given
            string method = XUtil.Single (node.GetChildValue<string> ("method", context, null), node, context, "GET").ToUpper ();

            if (method == "GET" || method == "DELETE") {

                // this is either a "GET" or a "DELETE" method type of request, hence passing in the [content] and [files] as part of URL
                bool first = url.IndexOf ("?") == -1;
                foreach (var idxNode in node.FindAll (delegate (Node idxArgNode) {
                    return idxArgNode.Name == "content" || idxArgNode.Name == "files";
                })) {
                    if (idxNode.Name == "content") {

                        // looping through each argument in current segment
                        foreach (var idxArg in XUtil.Iterate <Node> (idxNode, context)) {

                            // making sure our first argument starts with a "?", and all consecutive arguments have "&" prepended in front of them
                            if (first) {
                                first = false;
                                url += "?" + idxArg.Name + "=" + HttpUtility.UrlEncode (XUtil.Single<string> (idxArg.Value, idxArg, (context)));
                            } else {
                                url += "&" + idxArg.Name + "=" + HttpUtility.UrlEncode (XUtil.Single<string> (idxArg.Value, idxArg, (context)));
                            }
                        }
                    } else  {
                        
                        // looping through each file in current segment
                        foreach (var idxFile in XUtil.Iterate <Node> (idxNode, context)) {

                            // making sure our first argument starts with a "?", and all consecutive arguments have "&" prepended in front of them
                            if (first) {
                                first = false;
                                url += "?" + idxFile.Name + "=";
                            } else {
                                url += "&" + idxFile.Name + "=";
                            }
                            using (StreamReader reader = new StreamReader (File.OpenRead (GetBasePath (context) + XUtil.Single<string> (idxFile.Value, idxFile, context)))) {
                                url += HttpUtility.UrlEncode (reader.ReadToEnd ());
                            }
                        }
                    }
                }
            }

            // returning updated URL to caller
            return url;
        }

        /*
         * adding headers to request
         */
        private static void AddHeadersToRequest (HttpWebRequest request, Node node, ApplicationContext context)
        {
            // looping through each header in our [headers] collection, and adding to request
            foreach (var idxHeader in XUtil.Iterate<Node> (node ["headers"], context)) {

                // %&$@ing MSFT HttpWebRequest needs special treatment of all sorts of weird headers ...!!
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
         * adding cookies to request
         */
        private static void AddCookiesToRequest (HttpWebRequest request, Node node, ApplicationContext context)
        {
            // making sure we've got our Cookie Container
            request.CookieContainer = new CookieContainer ();

            // looping through all request cookies passed in by caller
            foreach (var idxCookie in XUtil.Iterate<Node> (node ["cookies"], context)) {
                request.CookieContainer.Add (
                    new Cookie (idxCookie.Name, HttpUtility.UrlEncode (XUtil.Single<string> (idxCookie, context))) { 
                    Domain = request.RequestUri.Host
                });
            }
        }

        /*
         * executes response, and returns to caller
         */
        private static HttpWebResponse ExecuteRequest (HttpWebRequest request, Node node, ApplicationContext context)
        {
            switch (request.Method) {
            case "GET":
            case "DELETE":

                // nothing to do here, [content] was passed in as a part of URL in encoded form
                return (HttpWebResponse)request.GetResponse ();
            case "POST":
            case "PUT":

                // need to handle [content] in some intelligent manner
                return CreateComplexRequest (request, node, context);
            default:

                // we only support the 4 basic methods; GET, DELETE, POST and PUT
                return null; // hence, we do nothing, but allow for other Active Event handlers to still execute without any exceptions.
            }
        }

        /*
         * creates a "complex" response, from either "PUT" or "POST" method type of request
         */
        private static HttpWebResponse CreateComplexRequest (HttpWebRequest request, Node node, ApplicationContext context)
        {
            // retrieving Content-Type, defaulting to "application/x-www-form-urlencoded", unless explicitly overridden
            request.ContentType = node ["headers"] == null ? 
                "application/x-www-form-urlencoded" : 
                XUtil.Single<string> (
                        node ["headers"].GetChildValue<object> ("Content-Type", context, null), 
                        node ["headers"] ["Content-Type"], 
                        context, 
                        "application/x-www-form-urlencoded");

            // checking what type of request this is, and acting accordingly
            if (request.ContentType.StartsWith ("application/x-www-form-urlencoded")) {

                // creating a simple URL encoded request
                return CreateUrlEncodedRequest (request, node, context);
            } else if (request.ContentType.StartsWith ("multipart")) {

                // using MimeKit to create a complex "multipart/xxx" request
                return CreateMultipartRequest (request, node, context);
            } else if (request.ContentType.StartsWith ("text")) {

                // creating a "text" type of request, simply streaming all [content] and [files] as one big text-content, appending CR/LF between each object
                return CreateTextContentRequest (request, node, context);
            } else {
                
                // creating "anything else"
                return CreateGenericContentRequest (request, node, context);
            }
        }

        /*
         * creates a response from a "application/x-www-form-urlencoded" type of POST request
         */
        private static HttpWebResponse CreateUrlEncodedRequest (HttpWebRequest request, Node node, ApplicationContext context)
        {
            // creating a stream writer wrapping the "request content stream"
            using (StreamWriter writer = new StreamWriter (request.GetRequestStream ())) {
                bool first = true;

                foreach (var idxNode in node.FindAll (delegate (Node idxArgNode) {
                    return idxArgNode.Name == "content" || idxArgNode.Name == "files";
                })) {
                    if (idxNode.Name == "content") {

                        // looping through each [content] children node in current segment, and URL encoding it into request stream writer
                        foreach (var idxArg in XUtil.Iterate<Node> (idxNode, context)) {
                            if (first)
                                first = false; // first parameter
                            else
                                writer.Write ("&"); // second, third, or fourth, etc, parameter, making sure we separate our parameters correctly
                            string value = XUtil.Single<string> (idxArg.Value, idxArg, context);
                            writer.Write (string.Format ("{0}={1}", idxArg.Name, HttpUtility.UrlEncode (value)));
                        }
                    } else  {

                        // looping through each [files] children node in current segment, and URL encoding the file into request stream writer
                        foreach (var idxArg in XUtil.Iterate<Node> (idxNode, context)) {
                            if (first)
                                first = false; // first parameter
                            else
                                writer.Write ("&"); // second, third, or fourth, etc, parameter, making sure we separate our parameters correctly
                            string fileName = XUtil.Single<string> (idxArg.Value, idxArg, context);
                            using (FileStream stream = File.OpenRead (GetBasePath (context) + fileName)) {
                                byte[] value = new byte [stream.Length];
                                stream.Read (value, 0, value.Length);
                                writer.Write (string.Format ("{0}={1}", idxArg.Name, HttpUtility.UrlEncode (value)));
                            }
                        }
                    }
                }
            }

            // returning response to caller
            return (HttpWebResponse)request.GetResponse ();
        }
        
        /*
         * creates a response from a "multipart/something" type of POST request
         */
        private static HttpWebResponse CreateMultipartRequest (HttpWebRequest request, Node node, ApplicationContext context)
        {
            // creating root Multipart to hold all [content] and [files]
            Multipart root = new Multipart (ContentType.Parse (request.ContentType).MediaSubtype);

            List<Stream> streams = new List<Stream> ();
            try {
                foreach (var idxNode in node.FindAll (delegate (Node idxArgNode) {
                    return idxArgNode.Name == "content" || idxArgNode.Name == "files";
                })) {
                    if (idxNode.Name == "content") {

                        // adding [content] from current segment to request
                        AddContentToMultipart (request, idxNode, context, root);
                    } else {

                        // adding [files] from current segment to request, while storing streams, such that we can dispose them when done
                        AddFilesToMultipart (request, idxNode, context, root, streams);
                    }
                }
                
                // writing multipart to request stream
                root.WriteTo (request.GetRequestStream ());
            } finally {

                // cleaning up
                foreach (var idxStream in streams) {
                    idxStream.Dispose ();
                }
            }

            // returning HttpWebResponse to caller
            return (HttpWebResponse)request.GetResponse ();
        }
        
        /*
         * adding up [content] given to Multipart
         */
        private static void AddContentToMultipart (
            HttpWebRequest request, 
            Node node, 
            ApplicationContext context, 
            Multipart root)
        {
            // looping through each child of [content], building a new MimePart
            foreach (var idxArg in XUtil.Iterate<Node> (node, context)) {

                // creating our MimePart, and adding the headers
                MimePart part = new MimePart ();
                foreach (var idxHeader in idxArg.Children) {
                    part.Headers.Replace (idxHeader.Name, XUtil.Single<string> (idxHeader.Value, idxHeader, context));
                }

                // unless Content-Disposition is explicitly defined, we default it to "form-data; name='node-name'"
                if (part.ContentDisposition == null) {
                    part.Headers.Add ("Content-Disposition", "form-data"); // fix this when MimeKit comes with new release!
                    part.ContentDisposition.Parameters.Add ("name", idxArg.Name);
                }

                // creating our ContentObject for our MimePart, and adding MimePart to root Multipart
                MemoryStream stream = new MemoryStream (
                    idxArg.Value is byte[] ? 
                    (byte[])idxArg.Value : 
                    Encoding.UTF8.GetBytes (XUtil.Single<string> (idxArg.Value, idxArg, context)));
                part.ContentObject = new ContentObject (stream);
                root.Add (part);
            }
        }

        /*
         * adding up [files] given to Multipart
         */
        private static void AddFilesToMultipart (
            HttpWebRequest request, 
            Node node, 
            ApplicationContext context, 
            Multipart root,
            List<Stream> streams)
        {
            // looping through each child of [files], building a new MimePart
            foreach (var idxArg in XUtil.Iterate<Node> (node, context)) {

                // creating our MimePart, and adding the headers
                MimePart part = new MimePart ();
                foreach (var idxHeader in idxArg.Children) {
                    part.Headers.Replace (idxHeader.Name, XUtil.Single<string> (idxHeader.Value, idxHeader, context));
                }

                // retrieving relative path and name of file
                string fileName = XUtil.Single<string> (idxArg.Value, idxArg, context);

                // unless Content-Disposition is explicitly defined, we default it to "form-data; name='node-name'; filename='filepath/name.ext'"
                if (part.ContentDisposition == null) {
                    part.Headers.Add ("Content-Disposition", "form-data"); // fix this when MimeKit comes with new release ...
                    part.ContentDisposition.Parameters.Add ("name", idxArg.Name);
                    part.ContentDisposition.Parameters.Add ("filename", fileName);
                }

                // creating our ContentObject for our MimePart, and adding MimePart to root Multipart
                FileStream stream = new FileStream (GetBasePath (context) + fileName, FileMode.Open, FileAccess.Read);
                streams.Add (stream);
                part.ContentObject = new ContentObject (stream);
                root.Add (part);
            }
        }

        /*
         * creates a response from a "text/something" type of POST request
         */
        private static HttpWebResponse CreateTextContentRequest (HttpWebRequest request, Node node, ApplicationContext context)
        {
            // creating a stream writer wrapping the "request content stream"
            using (StreamWriter writer = new StreamWriter (request.GetRequestStream ()) { AutoFlush = true }) {

                // looping through each [content] and [files] sequentially, in order declared
                bool first = true;
                foreach (var idxNode in node.FindAll (delegate (Node idxArgNode) {
                    return idxArgNode.Name == "content" || idxArgNode.Name == "files";
                })) {

                    // looping through each [content] children node given, and pushing it into request stream, making sure that CR/LF is
                    // put between each arg/file
                    if (idxNode.Name == "content") {

                        // looping through each [content] in current segment
                        foreach (var idxArg in XUtil.Iterate<Node> (idxNode, context)) {
                            if (first)
                                first = false;
                            else
                                writer.Write ("\r\n");
                            string value = XUtil.Single<string> (idxArg.Value, idxArg, context, "", "\r\n"/*in case Hyperlisp is transferred with expression yielding multiple results*/);
                            writer.Write (value);
                        }
                    } else {
                        
                        // looping through each [files] in current segment
                        foreach (var idxFile in XUtil.Iterate<Node> (idxNode, context)) {
                            if (first)
                                first = false;
                            else
                                writer.Write ("\r\n");
                            string fileName = XUtil.Single<string> (idxFile.Value, idxFile, context);
                            using (FileStream stream = File.OpenRead (GetBasePath (context) + fileName)) {
                                stream.CopyTo (writer.BaseStream); // assuming file is text file ...
                            }
                        }
                    }
                }
            }

            // returning response to caller
            return (HttpWebResponse)request.GetResponse ();
        }
        
        /*
         * creates a generic type of POST request, from "anything/whatever" Content-Type
         */
        private static HttpWebResponse CreateGenericContentRequest (HttpWebRequest request, Node node, ApplicationContext context)
        {
            // creating a stream writer wrapping the "request content stream"
            using (StreamWriter writer = new StreamWriter (request.GetRequestStream ()) { AutoFlush = true }) {

                foreach (var idxNode in node.FindAll (delegate (Node idxArgNode) {
                    return idxArgNode.Name == "content" || idxArgNode.Name == "files";
                })) {
                    if (idxNode.Name == "content") {

                        // looping through each [content] children node in current segment, and pushing it into request stream
                        foreach (var idxArg in XUtil.Iterate<Node> (idxNode, context)) {
                            object value = XUtil.Single<object> (idxArg.Value, idxArg, context);
                            writer.Write (value);
                        }
                    } else {

                        // looping through each [files] children node in current segment, and streaming the file into request stream writer,
                        // assuming all files are text files
                        foreach (var idxArg in XUtil.Iterate<Node> (idxNode, context)) {
                            string fileName = XUtil.Single<string> (idxArg.Value, idxArg, context);
                            using (FileStream stream = File.OpenRead (GetBasePath (context) + fileName)) {
                                stream.CopyTo (writer.BaseStream);
                            }
                        }
                    }
                }
            }

            // returning response to caller
            return (HttpWebResponse)request.GetResponse ();
        }

        /*
         * parses headers of response
         */
        private static void ParseHeaders (HttpWebResponse response, Node node)
        {
            // checking to see if we've got any headers, and if so, adding a root [headers] node
            if (response.Headers.Count > 0) {
                node.Add ("headers");

                // looping through each header, and adding into [headers] node
                foreach (var idxHeader in response.Headers.AllKeys) {
                    node.LastChild.Add (idxHeader, response.Headers [idxHeader]);
                }
            }
        }

        /*
         * parses cookies from response
         */
        private static void ParseCookies (HttpWebResponse response, Node node)
        {
            // looping through each cookie
            foreach (Cookie idxCookie in response.Cookies) {
                if (!idxCookie.Expired) {

                    // cookie was not expired, making sure we've got our [cookies] wrapper node
                    if (node.LastChild.Name != "cookies")
                        node.Add ("cookies");

                    node.LastChild.Add (idxCookie.Name);
                    node.LastChild.LastChild.Add ("expires", idxCookie.Expires);
                    node.LastChild.LastChild.Add ("value", HttpUtility.UrlDecode (idxCookie.Value));
                }
            }
        }

        /*
         * responsible for parsing content returned from HTTP request
         */
        private static void ParseContent (
            HttpWebResponse response, 
            Node node, 
            ApplicationContext context, 
            bool parseMime,
            bool forceText,
            bool forceBinary)
        {
            // sanity check
            if (forceText && forceBinary)
                throw new ArgumentException ("You cannot force both text and binary return value");

            // figuring out what type of response we were given, and acting accordingly
            var contentType = ContentType.Parse (response.ContentType);

            switch (contentType.MediaType) {
            case "multipart":

                // server returned "multipart", checking to see if we should use MimeKit to parse, or simply return MIME message "raw"
                if (parseMime) {

                    // using MimeKit to parse response
                    ParseMultiPartContent (response, node, context);
                } else {

                    // no parsing of MIME should be done, even though return value from server is "multipart"
                    ParseSinglePartContent (response, node, context, forceText, forceBinary);
                }
                break;
            default:

                // server returned a "single object", no need to parse this bugger
                ParseSinglePartContent (response, node, context, forceText, forceBinary);
                break;
            }
        }

        /*
         * parses "multipart" content using MimeKit
         */
        private static void ParseMultiPartContent (
            HttpWebResponse response, 
            Node node, 
            ApplicationContext context)
        {
            // creating and looping through each MimePart in response
            var rootMultiPart = Multipart.Load (response.GetResponseStream ()) as Multipart;
            foreach (MimeEntity idxEntity in rootMultiPart) {

                // setting value of [content] node to "name" from ContentDisposition, but only if Disposition equals "form-data", and "name" parameter exists
                string name = "content";
                if (idxEntity.ContentDisposition != null && 
                    idxEntity.ContentDisposition.Disposition == "form-data" && 
                    idxEntity.ContentDisposition.Parameters.Contains ("name"))
                    name = idxEntity.ContentDisposition.Parameters ["name"];
                node.Add (name);

                // making sure we also return all MIME headers
                foreach (var idxHeader in idxEntity.Headers) {
                    node.LastChild.Add (idxHeader.Field, idxHeader.Value);
                }

                var idxPart = idxEntity as MimePart;
                if (idxPart == null) {

                    // possibly another "inner" multipart. Just returning it "raw" back to caller
                    MemoryStream stream = new MemoryStream ();

                    // writing entire MimeEntity to MemoryStream, and stuffing raw bytes into [value] node
                    idxEntity.WriteTo (stream);
                    node.LastChild.Add ("value", stream.GetBuffer ());
                } else {
                    if (idxPart.ContentType.MediaType == "text") {

                        // text Content-Type, putting text value into [value] node
                        using (StreamReader reader = new StreamReader (idxPart.ContentObject.Open ())) {
                            node.LastChild.Add ("value", reader.ReadToEnd ());
                        }
                    } else {

                        // some sort of binary or "non-text" Content-Type
                        MemoryStream stream = new MemoryStream ();

                        // decoding to MemoryStream, and stuffing raw bytes into [value] node
                        idxPart.ContentObject.DecodeTo (stream);
                        node.LastChild.Add ("value", stream.GetBuffer ());
                    }
                }
            }
        }

        /*
         * parsing a "single object" return value from HttpWebResponse
         */
        private static void ParseSinglePartContent (
            HttpWebResponse response, 
            Node node, 
            ApplicationContext context,
            bool forceText,
            bool forceBinary)
        {
            // adding root [content] node, and then [value] beneath that, to be consistent in how we build
            // our tree node structure, regardless of what server returns
            node.Add ("content");

            // checking what type of response this is
            if (forceText || (!forceBinary && ContentType.Parse (response.ContentType).MediaType == "text")) {

                // some sort of text Content-Type
                using (StreamReader reader = new StreamReader (response.GetResponseStream ())) {
                    node.LastChild.Add ("value", reader.ReadToEnd ());
                }
            } else {

                // some sort of binary or "non-text" Content-Type
                using (MemoryStream stream = new MemoryStream ()) {
                    response.GetResponseStream ().CopyTo (stream);
                    stream.Position = 0;
                    byte [] buffer = new byte [stream.Length];
                    stream.Read (buffer, 0, buffer.Length);
                    node.LastChild.Add ("value", buffer);
                }
            }
        }

        /*
         * returns base path of application
         */
        private static string _basePath;
        private static string GetBasePath (ApplicationContext context)
        {
            if (_basePath == null) {
                Node node = new Node ();
                context.Raise ("pf.core.application-folder", node);
                _basePath = node.Get<string> (context);
            }
            return _basePath;
        }
    }
}
