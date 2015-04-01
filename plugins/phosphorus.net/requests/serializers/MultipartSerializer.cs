/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

namespace phosphorus.net.requests.serializers
{
    /// <summary>
    ///     Responsible for serializing MIME HTTP requests.
    /// 
    ///     This serializer is used when you create an HTTP/POST or PUT request, and you choose to send your request as a 'multipart' type
    ///     of request. Internally it uses MimeKit to create a MIME message, which then will be serialized over the HttpWebResponse.
    /// 
    ///     Supports all features from MimeKit, and allows for adding any MIME header as children nodes beneath every value you choose 
    ///     to serialize. If your MIME 'Content-Type' is 'multipart/something', and you have no value as your content, then it will traverse
    ///     all [children] nodes, expecting these to be MIME entities by themselves, wrapped inside the multipart they exists within. This
    ///     allows you to create MIME tree messages, where you can nest multipart messages inside of other multipart messages.
    /// 
    ///     If your MIME entities have the 'Content-Disposition' header set, with a 'filename' parameter, and no value in their main node,
    ///     then this file will be transferred without being loaded into memory as your MIME entity.
    /// 
    ///     All children nodes of your MIME entities that have a value, will be assumed to be a MIME header, and used as such.
    /// 
    ///     This serializer supports most features of MimeKit, such as serializing content encoded as base64, by setting the 
    ///     'Content-Transfer-Encoding' MIME header for your MIME entity, etc.
    /// </summary>
    public class MultipartSerializer : ISerializer
    {
        private ContentType _contentType;

        /*
         * we must store the ContentType, since Multipart's constructor will create a Content-Type itself, such that we can
         * pass in any arguments given, in addition to keeping our automatically generated boundary, unless an explicit boundary is given.
         */
        public MultipartSerializer (ContentType contentType)
        {
            _contentType = contentType;
        }

        public void Serialize (ApplicationContext context, Node node, HttpWebRequest request)
        {
            // we have to track all of our FileStream objects, such that we can dispose them when we're done
            List<Stream> streams = new List<Stream> ();
            try
            {
                // retreiving parameters
                var entities = new List<Node> (HttpRequest.GetParameters (node));
                if (entities.Count == 0)
                    throw new ArgumentException ("You must add at least one parameter to your MIME request");

                // creating Multipart
                var multipartNode = new Node ();
                multipartNode.Add ("ContentType", _contentType);
                multipartNode.Add ("entities", entities);
                multipartNode.Add ("streams", streams);
                multipartNode.Add ("main-node", node);
                context.Raise ("_pf.mime.create-multipart", multipartNode);
                
                // writing Multipart to request stream
                WriteMultipartToRequest (multipartNode.Get<Multipart> (context), request);
            }
            finally
            {
                // cleaning up, to make sure all open FileStreams are released
                foreach (var idxStream in streams) {
                    idxStream.Dispose ();
                }
            }
        }

        /*
         * writes the given MimeEntity to the Request stream
         */
        private void WriteMultipartToRequest (MimeEntity entity, HttpWebRequest request)
        {
            // updating request HTTP header 'Content-Type' to reflect "boundary"
            request.ContentType = entity.ContentType.MimeType + entity.ContentType.Parameters;

            // writing Multipart to HTTP request stream
            using (var stream = request.GetRequestStream ()) {
                entity.WriteTo (stream);
            }
        }
    }
}
