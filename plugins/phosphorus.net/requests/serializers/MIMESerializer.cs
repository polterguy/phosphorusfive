/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

namespace phosphorus.net.requests.serializers
{
    public class MIMESerializer : Serializer
    {
        private ContentType _contentType;

        public MIMESerializer (ContentType contentType)
        {
            _contentType = contentType;
        }

        public override void Serialize (
            ApplicationContext context, 
            Node node, 
            HttpWebRequest request)
        {
            List<Stream> streams = new List<Stream> ();
            try {
                // creating root Multipart, making sure sub-type is passed on
                Multipart multipart = CreateMultipart ();

                // looping through all arguments, creating a MimeEntity, adding to Multipart
                foreach (var idxArg in GetArguments (node)) {

                    // creating our MIME entity, and adding to root Multipart
                    multipart.Add (CreateMimeEntity (context, idxArg, streams));
                }

                // updating request HTTTP header 'Content-Type' to reflect "boundary"
                request.ContentType = multipart.ContentType.MimeType + multipart.ContentType.Parameters;

                // writing Multipart to HTTTP request stream
                multipart.WriteTo (request.GetRequestStream ());
            } finally {

                // cleaning up
                foreach (var idxStream in streams) {
                    idxStream.Dispose ();
                }
            }
        }

        private Multipart CreateMultipart ()
        {
            // making sure we pass in the MediaSubtype
            Multipart multipart = new Multipart (_contentType.MediaSubtype);

            // adding all existing parameters from 'Content-Type'
            foreach (var idxHeader in _contentType.Parameters) {
                multipart.ContentType.Parameters [idxHeader.Name] = idxHeader.Value;
            }
            return multipart;
        }

        /*
         * creates a single MimeEntity from the given node
         */
        private MimeEntity CreateMimeEntity (ApplicationContext context, Node node, List<Stream> streams)
        {
            // figuring out content's disposition
            ContentDisposition cntDisp = GetDisposition (context, node);

            // figuring out Content-Type
            ContentType cntType = GetContentType (context, node);

            MimePart part = new MimePart ();
            Stream stream;
            if (cntDisp != null && !string.IsNullOrEmpty (cntDisp.FileName) && node.Value == null) {

                // part's content is in a file
                stream = File.OpenRead (GetBasePath (context) + cntDisp.FileName);
                streams.Add (stream); // adding stream to list of streams to dispose when we're done
            } else if (cntType.MediaType == "multipart" && node.Value == null && node ["children"] != null && node ["children"].Count > 0) {

                // part is a Multipart in itself, and individual items are in children nodes
                Multipart multipart = new Multipart (cntType.MediaSubtype);
                foreach (var idxChild in node ["children"].Children) {
                    MimeEntity entity = CreateMimeEntity (context, idxChild, streams);
                    if (entity != null)
                        multipart.Add (entity);
                }
                // decorating MimeEntity with headers, making sure we don't get "child" mime entities
                foreach (var idxHeader in node.FindAll (ix => ix.Name != "children")) {
                    if (idxHeader.Name == "Content-Type")
                        continue; // already set
                    multipart.Headers.Replace (idxHeader.Name, XUtil.Single<string> (idxHeader.Value, idxHeader, context));
                }
                return multipart.Count == 0 ? null : multipart;
            } else {

                // parts content is in its value somehow
                var objValue = XUtil.Single<object> (node.Value, node, context, null);
                if (objValue == null)
                    return null;
                var byteValue = objValue as byte [];
                if (byteValue != null) {

                    // value is already byte array
                    stream = new MemoryStream (byteValue);
                } else {

                    if (cntType.MediaType == "text" || objValue is string) {

                        // value is string value, or Content-Type is "text" something,
                        // stuffing contents of string as byte array into MemoryStream for ContentObject, posssibly converting to string
                        // before we create byte array
                        stream = new MemoryStream (Encoding.UTF8.GetBytes (Utilities.Convert<string> (objValue, context, "")));
                    } else {

                        // defaulting to BinaryFormatter
                        stream = new MemoryStream ();
                        BinaryFormatter formatter = new BinaryFormatter ();
                        formatter.Serialize (stream, objValue);
                    }
                }
                if (stream.Length == 0)
                    return null;
            }
            part.ContentObject = new ContentObject (stream);

            // decorating MimeEntity with headers
            foreach (var idxHeader in node.Children) {
                part.Headers.Replace (idxHeader.Name, XUtil.Single<string> (idxHeader.Value, idxHeader, context));
            }
            return part;
        }
        
        /*
         * returns the ContentDisposition for the given node, if there is any
         */
        protected static ContentDisposition GetDisposition (ApplicationContext context, Node node)
        {
            var cntNode = node ["Content-Disposition"];
            if (cntNode != null)
                return ContentDisposition.Parse (XUtil.Single<string> (cntNode.Value, cntNode, (context)));
            return null;
        }

        /*
         * returns the ContentDisposition for the given node, if there is any
         */
        protected static ContentType GetContentType (ApplicationContext context, Node node)
        {
            var cntNode = node ["Content-Type"];
            if (cntNode != null)
                return ContentType.Parse (XUtil.Single<string> (cntNode.Value, cntNode, (context)));
            return null;
        }
    }
}
