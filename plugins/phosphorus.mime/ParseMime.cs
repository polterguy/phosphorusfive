/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Text;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;
using MimeKit.Cryptography;

/// <summary>
///     Main namespace for everything related to MIME.
/// 
///     Contains helper classes and Active Events necessary to create and parse MIME messages.
/// </summary>
namespace phosphorus.mime
{
    /// <summary>
    ///     Class wrapping Active Event(s) necessary to parse a MIME message.
    /// 
    ///     Contains Active Event(s) necessary to parse MIME messages. Uses MimeKit internally.
    /// </summary>
    public static class ParseMime
    {
        /// <summary>
        ///     Allows for parsing a MIME message.
        /// 
        ///     Parses zero or more MIME message(s) from a byte array or string, and returns a node hierarchy back to caller
        ///     representing the MIME entity.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.mime.parse-mime")]
        private static void pf_mime_parse_mime (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idxByteValue in XUtil.Iterate<byte[]> (e.Args, context)) {
                using (var memStream = new MemoryStream (idxByteValue)) {
                    ParseMultipart (context, (Multipart)Multipart.Load (memStream), e.Args);
                }
            }
        }
        
        /*
         * parses one Multipart
         */
        private static void ParseMultipart (ApplicationContext context, Multipart multipart, Node node)
        {
            // checking to see if multipart is signed and encrypted, and if caller wants to decrypt it
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
         * responsible for checking to see if multipart is encrypted, and if it is, and caller supplied
         * a [decryption-password] parameter, we will decrypt the multipart, and return the decrypted version. In addition, it will
         * check to see if multipart was signed, and if it was, it will validate the signature, and make sure caller
         * gets to know the state about the signature, such that it can validate the multipart's integrity.
         */
        private static Multipart PreProcessMultipart (ApplicationContext context, Multipart multipart, Node node)
        {
            // adding headers from given multipart
            node.Add ("headers");
            foreach (var idxHeader in multipart.Headers) {
                node.LastChild.Add (idxHeader.Field, idxHeader.Value);
            }

            if (multipart is MultipartEncrypted) {

                // checking to see if caller supplied a decryption-password. Notice that there are two different types of consumers of
                // this method, one has the encryption password in its parent node [pf.net.create-request], and the other in its child collection
                // which is [pf.mime.parse-mime] invoked directly
                if (node.Parent != null && node.Parent ["decryption-password"] != null) {

                    // multipart was encrypted, and caller supplied a [decryption-password] parameter
                    var encrNode = new Node (string.Empty, multipart);
                    encrNode.Add ("password", node.Parent.GetExChildValue<string> ("decryption-password", context));
                    multipart = context.Raise ("_pf.crypto.pgp.decrypt", encrNode).Get<Multipart> (context);
                }
                else if (node ["decryption-password"] != null) {

                    // multipart was encrypted, and caller supplied a [decryption-password] parameter
                    var encrNode = new Node (string.Empty, multipart);
                    encrNode.Add ("password", node.GetExChildValue<string> ("decryption-password", context));
                    multipart = context.Raise ("_pf.crypto.pgp.decrypt", encrNode).Get<Multipart> (context);
                }
                node.Add ("encrypted", true);
            }
            if (multipart is MultipartSigned) {

                // entity is signed, verifying signature and returning signature data to caller
                var signatureResult = context.Raise ("_pf.crypto.pgp.verify-signature", new Node (string.Empty, multipart)).Get<Node> (context);
                node.Add ("signed", signatureResult.Get<bool> (context));
                node ["signed"].AddRange (signatureResult.Children);
            }
            return multipart;
        }

        /*
         * returns the name to use for the node wrapping one MIME entity
         */
        private static string GetName (MimeEntity entity)
        {
            if (entity.ContentDisposition != null && entity.ContentDisposition.Parameters ["name"] != null)
                return entity.ContentDisposition.Parameters ["name"];
            return "content";
        }
    }
}
