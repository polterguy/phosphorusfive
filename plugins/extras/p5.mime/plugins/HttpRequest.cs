/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using p5.core;
using MimeKit;

namespace p5.mime.plugins
{
    /// <summary>
    ///     Class wrapping the [p5.net.http-post-mime/put-mime] Active Events for creating HTTP requests that posts/puts MIME
    /// </summary>
    public static class HttpRequest
    {
        /// <summary>
        ///     Posts or puts a MIME message over an HTTP request
        /// </summary>
        [ActiveEvent (Name = "p5.net.http-post-mime")]
        [ActiveEvent (Name = "p5.net.http-put-mime")]
        [ActiveEvent (Name = "p5.net.http-get-mime")]
        public static void p5_net_http_post_put_mime (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Storing decryption keys to be able to decrypt result
                var decryptionKeys = e.Args ["decryption-keys"];
                if (decryptionKeys != null)
                    decryptionKeys.UnTie (); // Don't want passwords to leave method in case of exceptions

                // Storing attachment folder, to know where to serialise returned attachments, of there is one
                var attachmentFolder = e.Args ["attachment-folder"];
                if (attachmentFolder != null)
                    attachmentFolder.UnTie (); // Don't want to confuse creation of MimeEntity

                // Retrieving all HTTP headers, which are defined as children with Capital letters, and untying them,
                // to not confuse creation of MimeEntity
                var headerList = e.Args.Children.Where (ix => ix.Name != ix.Name.ToLower ()).ToList ();
                foreach (var idxHeader in headerList) {
                    idxHeader.UnTie ();
                }

                // Figuring out which HTTP method to use
                string method = e.Args.Name.Substring (e.Args.Name.IndexOf ("-") + 1);
                method = method.Substring (0, method.IndexOf ("-"));

                // Creating request MimeEntity, but not if method is GET
                var requestEntity = method == "get" ? null : CreateEntity (context, e.Args);

                // Creating HTTP request(s), and retrieving result
                CreateHttpRequest (context, e.Args, requestEntity, method, headerList, decryptionKeys, attachmentFolder);
            }
        }

        /*
         * Creates a MimeEntity acccording to given args
         */
        private static MimeEntity CreateEntity (ApplicationContext context, Node args)
        {
            // List of streams created during creation of MimeEntity.
            // We need to keep track of this, such that we can close and dispose all streams after we're done with them
            var bufferStreams = new List<Stream> ();

            try {
                // Creating MIME entity, making sure we pass in a list of streams, such that we can clean up after ourselves
                var createMimeNode = new Node ("", bufferStreams);

                // Removing [decryption-keys] to not confuse mime creator
                createMimeNode.AddRange (args.Children);

                // Returning MimeEntity to caller
                return context.Raise (".p5.mime.create-native", createMimeNode).Get<MimeEntity> (context);
            } finally {

                // Closing and disposing all streams created during creation of MimeEntity
                foreach (var idxStream in bufferStreams) {

                    // Closing and disposing currently iterated stream
                    idxStream.Close ();
                    idxStream.Dispose ();
                }
            }
        }

        /*
         * Creates HTTP request, passing in delegates for rendering request and handling response
         */
        private static void CreateHttpRequest (
            ApplicationContext context, 
            Node args, 
            MimeEntity entity, 
            string method,
            IEnumerable<Node> headerList,
            Node decryptionKeys,
            Node attachmentFolder)
        {
            // Figuring out Content-Type HTTP header to use, and creating Node to use for HTTP request invocation Active Event
            var activeEventName = string.Format (".p5.net.http-{0}-native", method);
            var requestNode = new Node (activeEventName, args.Value);

            // Streaming MimeEntity to memory, if we have one, before invoking Active Event that creates HTTP request
            if (entity != null) {

                // We have an entity to post in our HTTP request, making sure we get the Content-Type right
                var contentType = entity.ContentType.MediaType + "/" + entity.ContentType.MediaSubtype + entity.ContentType.Parameters;
                requestNode.Add ("Content-Type", contentType);

                // Supplying [request-native] for request node as delegate serialising entity
                requestNode.Add ("request-native", (EventHandler)delegate (object sender, EventArgs exp) {
                    HttpWebRequest request = sender as HttpWebRequest;
                    request.ContentType = contentType;
                    using (Stream requestStream = request.GetRequestStream ()) {
                        entity.WriteTo (requestStream, true);
                    }
                });
            }

            // Adding all other headers, but dropping Content-Type, since it is declared in MimeEntity
            requestNode.AddRange (headerList.Where (ix => ix.Name != "Content-Type"));

            // Adding [response] node as delegate de-serialising response
            requestNode.Add ("response-native", (EventHandler)delegate (object sender, EventArgs exp) {

                // Parsing response as MIME and returning to caller
                ParseResponse (context, args, sender as Node, decryptionKeys, attachmentFolder);
            });

            // Invoking Active Event that create HTTP request
            context.Raise(activeEventName, requestNode);
        }

        /*
         * Parses results, and puts into args
         */
        private static void ParseResponse (
            ApplicationContext context, 
            Node args, 
            Node result, 
            Node decryptionKeys,
            Node attachmentFolder)
        {
            // Retrieving response
            HttpWebResponse response = result ["response-native"].UnTie ().Value as HttpWebResponse;

            // Retrieving MimeEntity from stream
            using (Stream responseStream = response.GetResponseStream ()) {
                MimeEntity entity = MimeEntity.Load (
                    ContentType.Parse (response.ContentType), 
                    responseStream);

                // Moving entire result into args
                args.Add (result);

                // Raising Active Event that will parse MIME content
                var oldValue = result.Value;
                result.Value = entity;
                try {
                    result.Add (decryptionKeys);
                    result.Add (attachmentFolder);
                    context.Raise (".p5.mime.parse-native", result);
                } finally {
                    result.Value = oldValue;
                }
            }
        }
    }
}
