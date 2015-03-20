/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.IO;
using System.Web;
using System.Text;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.web.ui.response
{
    /// <summary>
    ///     Class encapsulating the [pf.web.response.echo] Active Event
    /// 
    ///     Class wrapping the Active Events necessary to echo, or write, a specific piece of text, or nodes
    ///     back to client over the HTTP response.
    /// </summary>
    public static class Echo
    {
        /// <summary>
        ///     Echoes content and/or files back to client.
        /// 
        ///     Discards the current response, and writes all given [content] nodes, and/or [file] nodes, back to client.
        /// 
        ///     If more than one [content] nodes, and/or [file] nodes, are returned this way, then a MIME message will be composed, and returned as
        ///     a multipart. Any children nodes beneath [file] nodes and [content] nodes, will be used as headers, either HTTP header if there's only
        ///     one return value, or as MIME headers if there are multiple objects returned like this. After invocation of this Active Event,
        ///     then the session is abandoned, the session cookie cleared, and no additional rendering back to the client will have any effect.
        /// 
        ///     You can optionally supply a [headers] node, which if you do, will clear the existing HTTP headers collection, and be used as the
        ///     HTTP headers for the response. If there is only one object returned this way, then its children nodes will override whatever you
        ///     put into the [headers] node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.response.echo")]
        private static void pf_web_response_echo (ApplicationContext context, ActiveEventArgs e)
        {
            // checking to see if we should keep session around, defaulting to false
            bool keepSession = XUtil.Single<bool> (e.Args.GetChildValue<object> ("keep-session", context, null), e.Args ["keep-session"], context, false);

            // discarding current response, and removing session cookie
            HttpContext.Current.Response.Filter = null;
            HttpContext.Current.Response.ClearContent ();
            if (!keepSession)
                HttpContext.Current.Response.Cookies.Remove ("ASP.NET_SessionId");

            // retrieving all objects to render
            List<Node> retObjects = new List<Node> (e.Args.FindAll (
                delegate(Node idxNode) {
                    return idxNode.Name == "content" || idxNode.Name == "file";
            }));

            // adding headers to response, if caller explicitly set them
            if (e.Args ["headers"] != null)
                AddHeadersToResponse (e.Args ["headers"], context);

            // actually rendering of objects
            RenderObjects (retObjects, context);

            // flushing response, and making sure default content is never rendered
            HttpContext.Current.Response.OutputStream.Flush ();
            HttpContext.Current.Response.Flush ();
            HttpContext.Current.Response.SuppressContent = true;

            // abandoning session
            if (!keepSession)
                HttpContext.Current.Session.Abandon ();
        }

        /*
         * renders the given nodes back to client
         */
        private static void RenderObjects (List<Node> nodes, ApplicationContext context)
        {
            // verifying there's anything to actually render here
            if (nodes.Count == 0)
                return;

            if (nodes.Count == 1 && ContentType.Parse (HttpContext.Current.Response.ContentType).MediaType != "multipart") {

                // one single object, rendering it, putting headers into HTTP Header collection of HTTP Response
                RenderSingleObject (nodes [0], context);
            } else {

                // multiple objects, rendering as Multipart
                RenderMultipleObjects (nodes, context);
            }
        }

        /*
         * renders a single object back to client, putting headers into HTTP Header collection
         */
        private static void RenderSingleObject (Node node, ApplicationContext context)
        {
            // adding headers to response header collection
            // notice that this will overwrite the [headers] sent in as parameter to main Active Event
            AddHeadersToResponse (node, context);

            object value = XUtil.Single<object> (node.Value, node, context, null);
            if (value == null)
                return; // nothing to do here ...

            // checking type of object to render
            if (value is byte[]) {

                // binary response, ignoring whether or not this is a [file] or [content] node
                byte [] buffer = node.Get<byte []> (context);
                HttpContext.Current.Response.OutputStream.Write (buffer, 0, buffer.Length);
            } else {

                // not a binary response
                if (node.Name == "file") {

                    // rendering file back to client
                    using (FileStream stream = 
                           new FileStream (
                               GetBasePath (context) + Utilities.Convert<string> (value, context), 
                               FileMode.Open, 
                               FileAccess.Read)) {
                               stream.CopyTo (HttpContext.Current.Response.OutputStream);
                    }
                } else {

                    // rendering some sort of text back to client, making sure we add CR/LF between all entities returned by expression
                    // notice that we have to re-run expression here, to make sure we get CR/LF between all entities returned by expression
                    // but only if this is a "text/something" type of response
                    if (ContentType.Parse (HttpContext.Current.Response.ContentType).MediaType == "text")
                        HttpContext.Current.Response.Write (XUtil.Single<string> (node.Value, node, context, "", "\r\n"));
                    else
                        HttpContext.Current.Response.Write (Utilities.Convert<string> (value, context));
                }
            }
        }

        /*
         * renders multiple objects back to client as multipart
         */
        private static void RenderMultipleObjects (List<Node> nodes, ApplicationContext context)
        {
            // multiple return objects, rendering them as multipart back to client, but first making sure we set Content-Type header, unless it's already set
            if (string.IsNullOrEmpty (HttpContext.Current.Response.ContentType))
                HttpContext.Current.Response.ContentType = "multipart/mixed";

            // getting Content-Type from response headers
            ContentType contentType = ContentType.Parse (HttpContext.Current.Response.ContentType);

            if (contentType.MediaType == "multipart") {

                // supposed to be rendered as a "multipart/something" type of response
                RenderMultipart (nodes, context);
            } else {

                // rendering all nodes, one after the other
                foreach (var idxNode in nodes) {
                    object value = XUtil.Single<object> (idxNode.Value, idxNode, context);
                    if (value is byte[]) {

                        // rendering raw bytes
                        byte [] valueAsBytes = (byte [])value;
                        HttpContext.Current.Response.OutputStream.Write (valueAsBytes, 0, valueAsBytes.Length);
                    } else {

                        // some sort of file object, text object, or object that needs to be converted into text before rendered
                        if (idxNode.Name == "file") {

                            // file content
                            using (FileStream stream = new FileStream (GetBasePath (context) + Utilities.Convert<string> (value, context), FileMode.Open, FileAccess.Read)) {
                                stream.CopyTo (HttpContext.Current.Response.OutputStream);
                            }
                        } else {

                            // some sort of text object, making sure we inject CR/LF between values, if value is an expressions
                            // notice that we have to re-run our expression here, to make sure we inject CR/LF between all entities in expression
                            // but only if HTTP Content-Type header is "text/something"
                            string actualValue;
                            if (contentType.MediaType == "text")
                                actualValue = XUtil.Single<string> (idxNode.Value, idxNode, context, "", "\r\n");
                            else
                                actualValue = Utilities.Convert<string> (value, context);
                            MemoryStream stream = new MemoryStream (Encoding.UTF8.GetBytes (actualValue));
                            stream.CopyTo (HttpContext.Current.Response.OutputStream);
                        }
                    }
                }
            }
        }

        /*
         * Renders response as "multipart/something"
         */
        private static void RenderMultipart (List<Node> nodes, ApplicationContext context)
        {
            // creating Multipart to render back over response
            Multipart multipart = new Multipart (ContentType.Parse (HttpContext.Current.Response.ContentType).MediaSubtype);

            // looping through each object to render, storing references to any FileStreams, such that we can dispose them later
            List<Stream> streams = new List<Stream> ();
            try {

                // loping through each [content] and/or [file] node given
                foreach (var idxNode in nodes) {

                    // creating MimePart and settings its headers
                    // TODO: support inner Multipart object here ...!!
                    // if Content-Type == "multipart/something", we create Multipart here by parsing content or something, and not MimePart, or ...?
                    MimePart part = new MimePart ();
                    foreach (var idxHeader in idxNode.Children) {
                        part.Headers.Replace (idxHeader.Name, XUtil.Single<string> (idxHeader.Value, idxHeader, context));
                    }
                    object value = XUtil.Single<object> (idxNode.Value, idxNode, context);
                    if (value is byte[]) {

                        // binary content, ignoring whether or not this is a [file] or [content]
                        part.ContentObject = new ContentObject (new MemoryStream ((byte [])idxNode.Value));
                    } else {

                        // some sort of text object
                        if (idxNode.Name == "file") {

                            // file content
                            FileStream stream = new FileStream (GetBasePath (context) + Utilities.Convert<string> (value, context), FileMode.Open, FileAccess.Read);
                            streams.Add (stream);
                            part.ContentObject = new ContentObject (stream);
                        } else {

                            // some sort of text object, making sure we inject CR/LF between values, if value is an expressions
                            // notice that we have to re-run our expression here, to make sure we inject CR/LF between all entities in expression
                            // but only if our current MimePart is a "text/something" type of part
                            string actualValue;
                            if (part.ContentType.MediaType == "text")
                                actualValue = XUtil.Single<string> (idxNode.Value, idxNode, context, "", "\r\n");
                            else
                                actualValue = Utilities.Convert<string> (value, context);
                            MemoryStream stream = new MemoryStream (Encoding.UTF8.GetBytes (actualValue));
                            part.ContentObject = new ContentObject (stream);
                        }
                    }

                    // adding MimePart to Multipart
                    multipart.Add (part);
                }

                // rendering Multipart back to client
                multipart.WriteTo (HttpContext.Current.Response.OutputStream);
            } finally {
                foreach (Stream idxStream in streams) {
                    idxStream.Dispose ();
                }
            }
        }

        /*
         * adding headers to request. Used when only one object is returned
         */
        private static void AddHeadersToResponse (Node node, ApplicationContext context)
        {
            foreach (var idxHeader in node.Children) {
                switch (idxHeader.Name) {
                case "Content-Type":
                    HttpContext.Current.Response.ContentType = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Buffer":
                    HttpContext.Current.Response.Buffer = XUtil.Single<bool> (idxHeader.Value, idxHeader, context);
                    break;
                case "Buffer-Output":
                    HttpContext.Current.Response.BufferOutput = XUtil.Single<bool> (idxHeader.Value, idxHeader, context);
                    break;
                case "Cache-Control":
                    HttpContext.Current.Response.CacheControl = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Charset":
                    HttpContext.Current.Response.Charset = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Content-Encoding":
                    HttpContext.Current.Response.ContentEncoding = Encoding.GetEncoding (XUtil.Single<string> (idxHeader.Value, idxHeader, context));
                    break;
                case "Expires":
                    HttpContext.Current.Response.Expires = XUtil.Single<int> (idxHeader.Value, idxHeader, context);
                    break;
                case "Expires-Absolute":
                    HttpContext.Current.Response.ExpiresAbsolute = XUtil.Single<System.DateTime> (idxHeader.Value, idxHeader, context);
                    break;
                case "Header-Encoding":
                    HttpContext.Current.Response.HeaderEncoding = Encoding.GetEncoding (XUtil.Single<string> (idxHeader.Value, idxHeader, context));
                    break;
                case "Redirect-Location":
                    HttpContext.Current.Response.RedirectLocation = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Status":
                    HttpContext.Current.Response.Status = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Status-Code":
                    HttpContext.Current.Response.StatusCode = XUtil.Single<int> (idxHeader.Value, idxHeader, context);
                    break;
                case "Status-Description":
                    HttpContext.Current.Response.StatusDescription = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Sub-Status-Code":
                    HttpContext.Current.Response.SubStatusCode = XUtil.Single<int> (idxHeader.Value, idxHeader, context);
                    break;
                default:
                    HttpContext.Current.Response.Headers.Add (idxHeader.Name, XUtil.Single<string> (idxHeader.Value, idxHeader, context));
                    break;
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
