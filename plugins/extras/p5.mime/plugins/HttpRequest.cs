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
        [ActiveEvent (Name = "p5.net.http-post-mime", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "p5.net.http-put-mime", Protection = EventProtection.LambdaClosed)]
        public static void p5_net_http_post_put_mime (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Storing decryption keys to be able to decrypt result
                var decryptionKeys = e.Args ["decryption-keys"];
                if (decryptionKeys != null)
                    decryptionKeys.UnTie (); // Don't want passwords to leave method in case of exceptions

                // Figuring out which HTTP method to use
                string method = e.Args.Name.Substring (e.Args.Name.IndexOf ("-") + 1);
                method = method.Substring (0, method.IndexOf ("-"));

                // Creating request MimeEntity
                var requestEntity = CreateEntity (context, e.Args);

                // Creating HTTP request(s), and retrieving result
                var result = CreateHttpRequest (context, e.Args, requestEntity, method);

                // Parsing response as MIME and returning to caller
                ParseResponse (context, e.Args, result, decryptionKeys);
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
                createMimeNode.AddRange (args.Children.Where (ix => ix.Name != "decryption-keys"));

                // Returning MimeEntity to caller
                return context.RaiseNative ("p5.mime.create-native", createMimeNode).Get<MimeEntity> (context);
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
         * Creates HTTP request, passing in MimeEntity
         */
        private static Node CreateHttpRequest (
            ApplicationContext context, 
            Node args, 
            MimeEntity entity, 
            string method)
        {
            // Figuring out Content-Type HTTP header to use, and creating Node to use for HTTP request invocation Active Event
            var contentType = entity.ContentType.MediaType + "/" + entity.ContentType.MediaSubtype + entity.ContentType.Parameters;
            var activeEventName = string.Format ("p5.net.http-{0}", method);
            var requestNode = new Node (activeEventName, args.Value);

            // Streaming MimeEntity to memory, before invoking Active Event that creates HTTP request
            using (var stream = new MemoryStream()) {
                entity.WriteTo (stream, true);
                requestNode.Add ("Content-Type", contentType);
                requestNode.Add ("content", stream.ToArray ());
            }

            // Invoking Active Event that create HTTP request
            return context.RaiseNative(activeEventName, requestNode);
        }

        /*
         * Parses results, and puts into args
         */
        private static void ParseResponse (
            ApplicationContext context, 
            Node args, 
            Node result, 
            Node decryptionKeys)
        {
            // Moving entire result into args
            args.AddRange (result.Children);

            // Looping through each [result] node, parsing [content] as MIME if it is multipart, and returning parsed content
            foreach (var idxResult in args.Children.Where (ix => ix.Name == "result")) {

                // Making sure we only handle multipart types
                if (idxResult.GetChildValue ("Content-Type", context, "").StartsWith ("multipart")) {

                    // Creating parse mime node, retrieving [content] and using as MIME value for Active Event invocation
                    var parseMimeNode = new Node ("", idxResult.GetChildValue ("content", context, ""));

                    // Making sure we pass in decryption keys, if there are any
                    if (decryptionKeys != null)
                        parseMimeNode.Add (decryptionKeys);

                    // Making sure pass in Content-Type header from response
                    parseMimeNode.Add ("Content-Type", idxResult["Content-Type"].Value);

                    // Raising Active Event that will parse MIME content
                    context.RaiseNative ("p5.mime.parse", parseMimeNode);

                    // Removing [content] value, and adding result from parsing mime as children of [content]
                    idxResult["content"].Value = null;
                    idxResult ["content"].AddRange (parseMimeNode.Children);
                }
            }
        }
    }
}
