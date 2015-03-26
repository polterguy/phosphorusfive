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

namespace phosphorus.net.response
{
    public class MultipartResponse : HttpResponse
    {
        public MultipartResponse (HttpWebResponse response)
            : base (response)
        { }

        public override void Parse (ApplicationContext context, Node node)
        {
            base.Parse (context, node);
            var stream = Response.GetResponseStream ();
            ParseMultipart ((Multipart)Multipart.Load (stream), node.LastChild);
        }

        private void ParseMultipart (Multipart multipart, Node node)
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
                    ParseMultipart ((Multipart)idxEntityNode, current);
                } else if (idxEntityNode is MimePart) {

                    // "anything else", which we're treating as binary content
                    using (MemoryStream stream = new MemoryStream ()) {
                        ((MimePart)idxEntityNode).ContentObject.DecodeTo (stream);
                        current.Value = stream.ToArray ();
                    }
                }
            }
        }

        private string GetName (MimeEntity entity)
        {
            if (entity.ContentDisposition != null && entity.ContentDisposition.Parameters ["name"] != null)
                return entity.ContentDisposition.Parameters ["name"];
            return "content";
        }
    }
}
