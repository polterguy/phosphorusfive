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

/// <summary>
///     Main namespace for Active Events that fiddles with the HTTP response.
/// 
///     Wraps all Active Events that manipulates the HTTP response, such as changing HTTP headers, and such.
/// </summary>
namespace phosphorus.web.ui.response
{
    /// <summary>
    ///     Class encapsulating the [pf.web.echo] Active Event
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
        [ActiveEvent (Name = "pf.web.echo")]
        private static void pf_web_echo (ApplicationContext context, ActiveEventArgs e)
        {
            // discarding current response, and removing session cookie
            HttpContext.Current.Response.Filter = null;
            HttpContext.Current.Response.ClearContent ();
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

            if (nodes.Count == 1) {

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
            AddHeadersToResponse (node, context);

            // checking type of object to render
            if (node.Value is byte[]) {

                // binary response, ignoring whether or not this is a [file] or [content] node
                byte [] buffer = node.Get<byte []> (context);
                HttpContext.Current.Response.OutputStream.Write (buffer, 0, buffer.Length);
            } else {

                // not a binary response
                if (node.Name == "file") {

                    // rendering file back to client
                    using (FileStream stream = 
                           new FileStream (
                               GetBasePath (context) + XUtil.Single<string> (node.Value, node, context), 
                               FileMode.Open, 
                               FileAccess.Read)) {
                               stream.CopyTo (HttpContext.Current.Response.OutputStream);
                    }
                } else {

                    // rendering some sort of text back to client
                    HttpContext.Current.Response.Write (XUtil.Single<string> (node.Value, node, context, "", "\r\n" /* in case this is Hyperlisp with * iterator at end */));
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
            Multipart multipart = new Multipart (ContentType.Parse (HttpContext.Current.Response.ContentType).MediaSubtype);

            // looping through each object to render, storing references to any FileStreams such that we can dispose them later
            List<Stream> streams = new List<Stream> ();
            try {
                foreach (var idxNode in nodes) {

                    // creating our MimePart, and setting its headers, defaulting Content-Type to "text/plain"
                    ContentType contentType = new ContentType ("text", "plain");
                    HeaderList headers = new HeaderList ();
                    foreach (var idxHeader in idxNode.Children) {
                        if (idxHeader.Name == "Content-Type")
                            contentType = ContentType.Parse (XUtil.Single<string> (idxHeader.Value, idxHeader, context));
                        else
                            headers.Add (idxHeader.Name, XUtil.Single<string> (idxHeader.Value, idxHeader, context));
                    }

                    // creating MimePart and settings its headers
                    MimePart part = new MimePart (contentType);
                    foreach (var idxHeader in headers) {
                        part.Headers.Add (idxHeader);
                    }
                    if (idxNode.Value is byte []) {

                        // binary content, ignoring whether or not this is a [file] or [content]
                        part.ContentObject = new ContentObject (new MemoryStream ((byte [])idxNode.Value));
                    } else {

                        // some sort of text object
                        if (idxNode.Name == "file") {

                            // file content
                            FileStream stream = new FileStream (GetBasePath (context) + XUtil.Single<string> (idxNode.Value, idxNode, context), FileMode.Open, FileAccess.Read);
                            streams.Add (stream);
                            part.ContentObject = new ContentObject (stream);
                        } else {

                            // some sort of text object
                            MemoryStream stream = new MemoryStream (Encoding.UTF8.GetBytes (XUtil.Single<string> (idxNode.Value, idxNode, context)));
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
