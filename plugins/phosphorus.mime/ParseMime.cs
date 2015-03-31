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
                    ParseMultipart ((Multipart)Multipart.Load (memStream), e.Args);
                }
            }
        }
        
        /*
         * parses one Multipart
         */
        private static void ParseMultipart (Multipart multipart, Node node)
        {
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
                    ParseMultipart ((Multipart)idxEntityNode, current.LastChild);
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
