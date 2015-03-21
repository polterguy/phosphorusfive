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

namespace phosphorus.web.helpers
{
    public class MultipartResponse : HttpResponse
    {
        public MultipartResponse (HttpWebResponse response)
            : base (response)
        { }

        public override void Parse (ApplicationContext context, Node node)
        {
            using (var stream = Response.GetResponseStream ()) {
                var multipart = (Multipart)Multipart.Load (stream);
                if (multipart.Count > 0) {
                    Node current = node.Add ("result", Response.ResponseUri.ToString ()).LastChild;

                    // HTTP headers and cookies
                    ParseHeaders (context, current);
                    ParseCookies (context, current);

                    // parsing actual Multipart
                    ParseMultipart (multipart, current);
                }
            }
        }

        private void ParseMultipart (Multipart multipart, Node node)
        {
            // looping through each MIME part, trying to figure out a nice name for it
            foreach (var idxEntity in multipart) {
                Node current = node.Add (GetPartName (idxEntity)).LastChild;

                // MIME headers
                current.Add ("headers");
                foreach (var idxHeader in idxEntity.Headers) {
                    current.LastChild.Add (idxHeader.Field, idxHeader.Value);
                }

                // actual MIME part, which depend upon what type of part we're talking about
                if (idxEntity is TextPart) {
                    current.Add ("value", ((TextPart)idxEntity).GetText (Encoding.UTF8));
                } else if (idxEntity is Multipart) {
                    ParseMultipart ((Multipart)idxEntity, current);
                } else if (idxEntity is MimePart) {
                    MemoryStream stream = new MemoryStream ();
                    ((MimePart)idxEntity).ContentObject.DecodeTo (stream);
                    current.Add ("value", stream.ToArray ());
                }
            }
        }

        private string GetPartName (MimeEntity entity)
        {
            if (entity.ContentDisposition != null && 
                entity.ContentDisposition.Parameters.Contains ("name"))
                return entity.ContentDisposition.Parameters ["name"];
            return "part";
        }
    }
}
