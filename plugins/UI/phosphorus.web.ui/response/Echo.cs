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
        /*
         * class encapsulating a single content part, to transfer through echo
         */
        private class Part
        {
            /*
             * describes which type of part we're dealing with
             */
            public enum PartType
            {
                // file part, meaning a file is supposed to be transferred
                File,

                // simple text part
                Text
            };

            /*
             * creates a new Part for tranferring through echo
             */
            public Part (PartType typeOfPart, string value, string contentType, string partName)
            {
                TypeOfPart = typeOfPart;
                Value = value;
                ContentType = contentType;
                PartName = partName;
            }

            /*
             * contains name of part, if any
             */
            public string PartName {
                get;
                private set;
            }

            /*
             * contains "Content-Type" for part
             */
            public string ContentType {
                get;
                private set;
            }

            /*
             * contains type of part, meaning "File" or "Text", and so on
             */
            public PartType TypeOfPart {
                get;
                private set;
            }

            /*
             * contains "value", which is filename for files, content for text, and so on
             */
            public string Value {
                get;
                private set;
            }
        }

        /// <summary>
        ///     Returns the given text or object back to client.
        /// 
        ///     Discards the current response, and writes the given piece of text, nodes or objects(s)
        ///     back to client over the HTTP response.
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

            // retrieving parts caller wants to transfer
            var parts = GetParts (context, e.Args);

            // rendering parts back to client
            RenderParts (context, parts, e.Args);

            // flushing response and making sure default content is never rendered
            HttpContext.Current.Response.OutputStream.Flush ();
            HttpContext.Current.Response.Flush ();
            HttpContext.Current.Response.SuppressContent = true;

            // abandoning session
            HttpContext.Current.Session.Abandon ();
        }

        /*
         * returns all parts that echo should transfer
         */
        private static List<Part> GetParts (ApplicationContext context, Node node)
        {
            List<Part> retVal = new List<Part> ();

            // first adding all [text] parts
            foreach (var idxTextNode in node.FindAll ("text")) {
                retVal.Add (
                    new Part (
                        Part.PartType.Text, 
                        XUtil.Single (idxTextNode.Value, idxTextNode, context, "", "\r\n"), 
                        XUtil.Single (idxTextNode.GetChildValue<string> ("sub-type", context, null), idxTextNode ["sub-type"], context, "Hyperlisp"),
                        XUtil.Single<string> (idxTextNode.GetChildValue<string> ("name", context, null), idxTextNode ["name"], context, null)));
            }

            // then adding all [file] parts
            foreach (var idxTextNode in node.FindAll ("file")) {
                retVal.Add (
                    new Part (
                        Part.PartType.File, 
                        XUtil.Single<string> (idxTextNode.Value, idxTextNode, context), 
                        XUtil.Single<string> (idxTextNode.GetChildValue<string> ("content-type", context, null), idxTextNode ["content-type"], context, "text/Hyperlisp"),
                        XUtil.Single<string> (idxTextNode.GetChildValue<string> ("name", context, null), idxTextNode ["name"], context, null)));
            }
            return retVal;
        }
        
        /*
         * responsible for rendering content back to client over HttpContext.Current.Response
         */
        private static void RenderParts (ApplicationContext context, List<Part> parts, Node node)
        {
            if (parts.Count == 0)
                return; // nothing to do here
            if (parts.Count == 1) {

                // rendering simple content back to client
                RenderSinglePart (context, parts [0]);
            } else {

                // rendering "multipart/mixed" to client
                RenderMultiPart (context, parts, node);
            }
        }
        
        /*
         * renders multiple parts back to client
         */
        private static void RenderMultiPart (ApplicationContext context, List<Part> parts, Node node)
        {
            // creating Multipart to render
            Multipart multipart = new Multipart (node.GetChildValue ("sub-type", context, "mixed"));

            List<Stream> streams = new List<Stream> ();
            try {
                // looping through all parts
                foreach (var idxPart in parts) {
                    if (idxPart.TypeOfPart == Part.PartType.Text) {

                        // simple text part
                        var textPart = new TextPart (idxPart.ContentType);
                        if (!string.IsNullOrEmpty (idxPart.PartName))
                            textPart.Headers.Add ("Content-Disposition", string.Format ("form-data; name={0}", idxPart.PartName));
                        textPart.SetText (Encoding.UTF8, idxPart.Value);
                        multipart.Add (textPart);
                    } else {

                        // file type
                        var filePart = new MimePart (idxPart.ContentType);
                        filePart.FileName = idxPart.Value;
                        if (!string.IsNullOrEmpty (idxPart.PartName))
                            filePart.Headers.Add ("Content-Disposition", string.Format ("form-data; name={0}", idxPart.PartName));
                        FileStream stream = new FileStream (idxPart.Value, FileMode.Open);
                        streams.Add (stream); // to make sure we can dispose bugger ...
                        filePart.ContentObject = new ContentObject (stream);
                        if (filePart.ContentType.MediaType != "text") {
                            filePart.ContentTransferEncoding = ContentEncoding.Base64;
                        }
                        multipart.Add (filePart);
                    }
                }

                // rendering back to client
                // we do get some repetition of Content-Type here, since we're repeating it in both headers and body of
                // message, however, it has some advantages, like the ability to serialize an entire response, without its headers, 
                // keeping the entire MIME message intact, plus some proxies might fuck with headers, and so on
                HttpContext.Current.Response.ContentType = multipart.ContentType.MimeType + multipart.ContentType.Parameters;
                multipart.WriteTo (HttpContext.Current.Response.OutputStream);
            }
            finally {
                foreach (var idx in streams) {
                    idx.Dispose ();
                }
            }
        }

        /*
         * renders one single part back to client
         */
        private static void RenderSinglePart (ApplicationContext context, Part part)
        {
            if (part.TypeOfPart == Part.PartType.Text) {

                // simple text type
                HttpContext.Current.Response.ContentType = "text/" + part.ContentType + "; charset=utf-8";
                if (!string.IsNullOrEmpty (part.PartName))
                    HttpContext.Current.Response.Headers.Add ("Content-Disposition", "form-data; " + "name=" + part.PartName);
                HttpContext.Current.Response.Write (part.Value);
            } else {

                // file type
                HttpContext.Current.Response.ContentType = 
                    part.ContentType + (part.ContentType.Contains ("charset") ? "" : "; charset=utf-8");
                if (!string.IsNullOrEmpty (part.PartName))
                    HttpContext.Current.Response.Headers.Add ("Content-Disposition", "form-data; " + "name=" + part.PartName);
                using (Stream stream = new FileStream (GetBasePath (context) + part.Value, FileMode.Open)) {
                    stream.CopyTo (HttpContext.Current.Response.OutputStream);
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
