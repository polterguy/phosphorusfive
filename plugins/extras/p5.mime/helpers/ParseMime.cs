/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MimeKit;
using MimeKit.Cryptography;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.mime.helpers
{
    /// <summary>
    ///     Helper to parse a MimeEntity
    /// </summary>
    public static class ParseMime
    {
        /// <summary>
        ///     Parses a MIME entity and returns as node structure
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="itemNode">Node where to put parsed MimeEntity</param>
        /// <param name="entity">MIME entity to parse</param>
        public static void ParseMimeEntity (
            ApplicationContext context, 
            Node itemNode,
            MimeEntity entity,
            bool parentIsCryptoMultipart = false,
            string pgpPrivateKey = null,
            string pgpPassword = null)
        {
            // Checking if entity was encrypted, and user supplied a PGP private key and password, and if so,
            // automatically decrypting entity
            if (pgpPrivateKey != null && entity is MultipartEncrypted) {

                // Decrypting and invoking "self"
                var plainTextEntity = DecryptMultipart (
                    context, 
                    entity as MultipartEncrypted, 
                    pgpPrivateKey, 
                    pgpPassword);
                ParseMimeEntity (
                    context, 
                    itemNode, 
                    plainTextEntity, 
                    parentIsCryptoMultipart, 
                    pgpPrivateKey, 
                    pgpPassword);
            } else {

                // Content-Type and wrapper for current MimeEntity
                Node curNode = itemNode.Add (entity.ContentType.MediaType, entity.ContentType.MediaSubtype).LastChild;

                // Then all other headers
                foreach (var idxHeader in entity.Headers) {

                    // Adding header as child node of main MimeEntity node
                    curNode.Add (idxHeader.Field, idxHeader.Value);
                }

                // Checking if entity is Multipart, and if so, traversing all children entities
                Multipart multipart = entity as Multipart;
                if (multipart != null) {

                    // Process Multipart
                    ParseMultipart (context, curNode, multipart, pgpPrivateKey, pgpPassword);
                } else {

                    // Entity is some sort of "leaf" entity
                    ParseLeafEntity (context, curNode, entity as MimePart, parentIsCryptoMultipart);
                }
            }
        }

        /*
         * Parses a Multipart recursively
         */
        private static void ParseMultipart (
            ApplicationContext context,
            Node curNode,
            Multipart multipart,
            string pgpPrivateKey,
            string pgpPassword)
        {
            // Adding preamble, if there is any
            if (!string.IsNullOrEmpty ((multipart.Preamble ?? "").Trim ()))
                curNode.Add ("preamble", multipart.Preamble.Trim ());

            // Traversing all children, invoking "self" for each entity
            foreach (var idxEntity in multipart) {

                // Processing currently iterated MimeEntity child of Multipart
                ParseMimeEntity (
                    context, 
                    curNode, 
                    idxEntity, 
                    multipart is MultipartEncrypted, 
                    pgpPrivateKey, 
                    pgpPassword);
            }

            // Adding epilogue, if there is any
            if (!string.IsNullOrEmpty ((multipart.Epilogue ?? "").Trim ()))
                curNode.Add ("epilogue", multipart.Epilogue.Trim ());
        }

        /*
         * Decrypts a MIME entity and returns result
         */
        private static MimeEntity DecryptMultipart (
            ApplicationContext context,
            MultipartEncrypted encryptedEntity,
            string pgpPrivateKey,
            string pgpPassword)
        {
            using (var ctx = new GnuPrivacyContext ()) {
                ctx.Password = pgpPassword;
                var result = encryptedEntity.Decrypt (ctx);
                return result;
            }
        }

        /*
         * Parses a "leaf" MimePart
         */
        private static void ParseLeafEntity (
            ApplicationContext context,
            Node curNode,
            MimePart part,
            bool parentIsCryptoMultipart)
        {
            using (MemoryStream stream = new MemoryStream ()) {

                // Decoding content to memory
                part.ContentObject.DecodeTo (stream);

                // Resetting position
                stream.Position = 0;

                // Setting up buffer to hold actual content
                object buffer = null;

                // Checking how to handle content, either binary or text
                bool isText = false;
                switch (part.ContentType.MediaType + "/" + part.ContentType.MediaSubtype) {

                    // Some "application" types are actually text, and should be handled as such
                    // Make sure we handle the most common text types as text, to save coders from a roundtrip through conversion
                    case "application/x-hyperlisp":
                    case "application/javascript":
                    case "application/x-javascript":
                    case "application/ecmascript":
                    case "application/json":
                    case "application/pgp-signature":
                        isText = true;
                        break;
                    default:
                    if (parentIsCryptoMultipart || part.ContentType.MediaType == "text") {
                            isText = true;
                        }
                        break;
                }

                // Now we know how to handle content, which is as text or as binary content
                if (isText) {

                    // Content is text of some kind, decoding to text through StringReader
                    StreamReader reader = new StreamReader (stream);
                    buffer = reader.ReadToEnd ();
                } else {

                    // Content is binary, simply returning byte[] value raw
                    buffer = stream.ToArray ();
                }

                // Putting content into return node for MimeEntity
                curNode.Add ("content", buffer);
            }
        }
    }
}

