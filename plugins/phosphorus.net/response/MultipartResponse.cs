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
            base.Parse (context, node);
            var stream = Response.GetResponseStream ();
            ParseMultipart (context, (Multipart)Multipart.Load (stream), node.LastChild);
        }

        /*
         * parses one Multipart
         */
        private void ParseMultipart (ApplicationContext context, Multipart multipart, Node node)
        {
            // checking to see if 
            multipart = PreProcessMultipart (context, multipart, node);

            // looping through each MIME part, trying to figure out a nice name for it
            foreach (var idxEntityNode in multipart) {
                Node current = node.Add (GetName (idxEntityNode)).LastChild;

                // MIME headers
                foreach (var idxHeader in idxEntityNode.Headers) {
                    current.Add (idxHeader.Field, idxHeader.Value);
                }

                // actual MIME part, which depend upon what type of part we're talking about
                if (idxEntityNode is TextPart) {

                    // text part
                    current.Value = ((TextPart)idxEntityNode).GetText (Encoding.UTF8);
                } else if (idxEntityNode is Multipart) {

                    // nested Multipart
                    current.Add ("children");
                    ParseMultipart (context, (Multipart)idxEntityNode, current.LastChild);
                } else if (idxEntityNode is MimePart) {

                    // "anything else", which we're treating as binary content
                    using (MemoryStream stream = new MemoryStream ()) {
                        ((MimePart)idxEntityNode).ContentObject.DecodeTo (stream);
                        current.Value = stream.ToArray ();
                    }
                }
            }
        }

        /*
         * responsible for checking to see if multipart is encrypted, and if it is, and if it is, and caller supplied
         * a [decrypt] parameter, we will decrypt the multipart, and return the decrypted version. In addition, it will
         * check to see if multipart was signed, and if it was, it will validate the signature, and make sure caller
         * gets to know the state about the signature, such that it can validate the multipart's integrity.
         */
        private Multipart PreProcessMultipart (ApplicationContext context, Multipart entity, Node node)
        {
            if (entity is MultipartEncrypted && node.Parent ["decryption-password"] != null) {

                // multipart was encrypted, and caller supplied a [decryption-password] parameter
                var encrNode = new Node (string.Empty, entity);
                encrNode.Add ("password", node.Parent.GetExChildValue<string> ("decryption-password", context));
                entity = context.Raise ("_pf.crypto.pgp.decrypt", encrNode).Get<Multipart> (context);
                node.Add ("encrypted", true);
            }
            if (entity is MultipartSigned) {

                // entity is signed, verifying signature and returning signature data to caller
                var signatureResult = context.Raise ("_pf.crypto.pgp.verify-signature", new Node (string.Empty, entity)).Get<Node> (context);
                node.Add ("signed", signatureResult.Get<bool> (context));
                node ["signed"].AddRange (signatureResult.Children);
            }
            return entity;
        }

        /*
         * returns the name to use for the node wrapping one MIME entity
         */
        private string GetName (MimeEntity entity)
        {
            if (entity.ContentDisposition != null && entity.ContentDisposition.Parameters ["name"] != null)
                return entity.ContentDisposition.Parameters ["name"];
            return "content";
        }
    }
}
