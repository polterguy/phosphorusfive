/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;
using MimeKit.Cryptography;

namespace phosphorus.net.response
{
    /// <summary>
    ///     Responsible for de-serializing a multipart HTTP response.
    /// 
    ///     Will de-serialize an HTTP response as a multipart, using MimeKit, and put each MIME entity into
    ///     its own child node beneath the main [result] node, wrapping one server response. The name of the node
    ///     will be taken from the 'Content-Disposition' MIME header, and its 'name' parameter. If no 'name' parameter, 
    ///     or 'Content-Disposition' exists, for the given MIME entity, then the name of the node will be [content].
    /// 
    ///     The actual value of the MIME entity will be put in the value of each node wrapping one MIME entity. All MIME 
    ///     headers of the returned response from the server, will be children nodes of each MIME entity. If a MIME entity
    ///     is a multipart in itself, then each part of that multipart will become parsed too, and put inside of the outer
    ///     multipart's child node called [children], to avoid confusion between MIME headers for the multipart and its child
    ///     MIME entities.
    /// </summary>
    public class MultipartResponse : HttpResponse
    {
        public MultipartResponse (HttpWebResponse response)
            : base (response)
        { }

        /// <summary>
        ///     Parses the wrapped HTTP response as a multipart.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node to put the results into.</param>
        public override void Parse (ApplicationContext context, Node node)
        {
            // calling base, and reading in raw bytes from response stream
            base.Parse (context, node);
            var responseStream = Response.GetResponseStream ();
            using (MemoryStream stream = new MemoryStream ()) {

                // parsing raw bytes as Multipart, and putting result into given node structure
                // making sure we set value of node back to what it was before Active Event invocation
                responseStream.CopyTo (stream);
                var oldValue = node.Value;
                try
                {
                    node.LastChild.Value = stream.ToArray ();
                    context.Raise ("pf.mime.parse-mime", node.LastChild);
                }
                finally
                {
                    node.LastChild.Value = oldValue;
                }
            }
        }
    }
}
