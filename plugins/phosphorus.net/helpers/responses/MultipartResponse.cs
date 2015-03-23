/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using phosphorus.core;
using MimeKit;

namespace phosphorus.net.helpers
{
    /// <summary>
    ///     Class wrapping a multipart/xxx type of HTTP response.
    /// </summary>
    public class MultipartResponse : HttpResponse
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.net.helpers.MultipartResponse"/> class.
        /// </summary>
        /// <param name="response">Wrapped HTTP response.</param>
        public MultipartResponse (HttpWebResponse response)
            : base (response)
        { }

        public override void Parse (ApplicationContext context, Node node)
        {
            base.Parse (context, node);
            using (var stream = Response.GetResponseStream ()) {
                var multipart = (Multipart)Multipart.Load (stream);
                if (multipart.Count > 0) {

                    // parsing actual Multipart
                    ParseMultipart (multipart, node.LastChild);
                }
            }
        }

        private void ParseMultipart (Multipart multipart, Node node)
        {
            // looping through each MIME part, trying to figure out a nice name for it
            foreach (var idxEntity in multipart) {
                Node current = node.Add ("content").LastChild;

                // MIME headers
                foreach (var idxHeader in idxEntity.Headers) {
                    current.Add (idxHeader.Field, idxHeader.Value);
                }

                // actual MIME part, which depend upon what type of part we're talking about
                if (idxEntity is TextPart) {
                    current.Value = ((TextPart)idxEntity).GetText (Encoding.UTF8);
                } else if (idxEntity is Multipart) {
                    ParseMultipart ((Multipart)idxEntity, current);
                } else if (idxEntity is MimePart) {
                    MemoryStream stream = new MemoryStream ();
                    ((MimePart)idxEntity).ContentObject.DecodeTo (stream);
                    current.Value = stream.ToArray ();
                }
            }
        }
    }
}
