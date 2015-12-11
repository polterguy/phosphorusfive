/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MimeKit;
using MailKit;
using MimeKit.Cryptography;
using p5.exp;
using p5.core;
using p5.mail.helpers;
using p5.exp.exceptions;

namespace p5.mail.mime
{
    /// <summary>
    ///     Helper to parse a MimeEntity
    /// </summary>
    public static class ParseMime
    {
        /*
         * Processes one MimeEntity and put parsed content into itemNode
         */
        public static void ParseMimeEntity (
            ApplicationContext context, 
            Node itemNode,
            MimeEntity entity)
        {
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
                ParseMultipart (context, curNode, multipart);
            } else {

                // Entity is some sort of "leaf" entity
                ParseLeafEntity (context, curNode, entity as MimePart);
            }
        }

        /*
         * Processes a Multipart recursively
         */
        private static void ParseMultipart (
            ApplicationContext context,
            Node curNode,
            Multipart multipart)
        {
            // Adding preamble, if there is any
            if (!string.IsNullOrEmpty (multipart.Preamble.Trim ()))
                curNode.Add ("preamble", multipart.Preamble.Trim ());

            // Traversing all children, invoking "self" for each entity
            foreach (var idxEntity in multipart) {

                // Processing currently iterated MimeEntity child of Multipart
                ParseMimeEntity (context, curNode, idxEntity);
            }

            // Adding epilogue, if there is any
            if (!string.IsNullOrEmpty (multipart.Epilogue.Trim ()))
                curNode.Add ("epilogue", multipart.Epilogue.Trim ());
        }

        /*
         * Processes a "leaf" MimePart
         */
        private static void ParseLeafEntity (
            ApplicationContext context,
            Node curNode,
            MimePart part)
        {
            using (MemoryStream stream = new MemoryStream ()) {

                // Decoding content to memory
                part.ContentObject.DecodeTo (stream);

                // Resetting position
                stream.Position = 0;

                // Setting up buffer to hold actual content
                object buffer = null;

                // Checking how to handle content, either binary or text
                if (part.ContentType.MediaType == "text") {

                    // Decoding to string through StreamReader
                    StreamReader reader = new StreamReader (stream);
                    buffer = reader.ReadToEnd ();
                } else {

                    // Simply putting raw bytes into buffer
                    buffer = stream.ToArray ();
                }

                // Putting content into return node for MimeEntity
                curNode.Add ("content", buffer);
            }
        }
    }
}

