/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.IO;
using System.Web;
using System.Text;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.web.ui.common;
using MimeKit;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.web.ui.request
{
    /// <summary>
    ///     Helper to retrieve POST parameters serialized as MIME message.
    /// 
    ///     This class allows you to retrieve HTTP POST parameters values that was serialized by the client 
    ///     as a MIME message in the body of the request.
    /// </summary>
    public static class ParseMime
    {
        /// <summary>
        ///     Parses all MIME parameters sent by client.
        /// 
        ///     Parses all MIME parameters sent by client, and returns to caller.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        // TODO: Change this such that it doesn't read from request, but from an expression, 
        // since [pf.web.request.get-body] should be enough for reading request, or ...?
        [ActiveEvent (Name = "pf.web.request.parse-mime")]
        private static void pf_web_request_parse_mime (ApplicationContext context, ActiveEventArgs e)
        {
            if (string.IsNullOrEmpty (HttpContext.Current.Request.ContentType) || 
                ContentType.Parse (HttpContext.Current.Request.ContentType).MediaType != "multipart")
                return; // nothing to do here ...

            // loading Multipart from body of request
            Multipart multipart = Multipart.Load (HttpContext.Current.Request.InputStream) as Multipart;

            // looping through each MimePart in Multipart, and returning to caller
            foreach (MimePart idxPart in multipart) {

                // figuring out "name" of node using "name" parameter, if ContentDisposition exists, and is of type "form-data"
                string name = "content";
                if (idxPart.ContentDisposition != null && 
                    idxPart.ContentDisposition.Disposition == "form-data" &&
                    idxPart.ContentDisposition.Parameters.Contains ("name"))
                    name = idxPart.ContentDisposition.Parameters ["name"];
                e.Args.Add (name);

                // adding all headers
                foreach (var idxHeader in idxPart.Headers) {
                    e.Args.LastChild.Add (idxHeader.Field, idxHeader.Value);
                }

                if (idxPart.ContentType.MediaType == "text") {
                    e.Args.LastChild.Add ("value", ((TextPart)idxPart).GetText (Encoding.UTF8));
                } else {
                    MemoryStream stream = new MemoryStream ();
                    idxPart.ContentObject.DecodeTo (stream);
                    stream.Position = 0;
                    byte [] content = new byte [stream.Length];
                    stream.Read (content, 0, content.Length);
                    e.Args.LastChild.Add ("value", content);
                }
            }
        }
    }
}
