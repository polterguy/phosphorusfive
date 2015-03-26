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
                // creating root Multipart, making sure sub-type and parameters from 'Content-Type' is passed on
                Multipart multipart = CreateRootMultipart ();

                // looping through all arguments, creating a MimeEntity, adding to Multipart
                foreach (var idxArg in HttpRequest.GetParameters (node)) {
                    multipart.Add (CreateMimeEntity (context, idxArg, streams));
                }

                // writing Multipart to request stream
                WriteMultipartToRequest (multipart, request);

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
         * creates the "root" Multipart MimeEntity
         */
        private Multipart CreateRootMultipart ()
        {
            // making sure we pass in the MediaSubtype
            Multipart multipart = new Multipart (_contentType.MediaSubtype);

            // adding all existing parameters from 'Content-Type'
            foreach (var idxHeader in _contentType.Parameters) {
                multipart.ContentType.Parameters [idxHeader.Name] = idxHeader.Value;
            }

            // returning a Multipart that now should have the exact same 'Content-Type' as the HTTP request header
            // except that it might have an automatically generated 'boundary' parameter though of course ...
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

            MimeEntity part;
            if (cntDisp != null && !string.IsNullOrEmpty (cntDisp.FileName) && node.Value == null) {

                // part's content is in a file
                part = CreateMimeEntityFromFile (context, cntDisp, streams);
            } else if (cntType.MediaType == "multipart" && node.Value == null && node ["children"] != null) {

                // part is a nested Multipart, with MimeEntity items in [children] node
                part = CreateNestedMultipart (context, node, cntType, streams);
            } else if (node.Value != null) {

                // part is in value of node, somehow
                part = CreateMimeEntityFromValue (context, node, streams);
            } else {
                throw new ArgumentException ("Don't know how to create a MimeEntity from the given arguments");
            }

            // decorating MimeEntity with headers, making sure we only use children node's with a value, to avoid nodes
            // such as [children]
            foreach (var idxHeader in node.FindAll (ix => ix.Value != null)) {
                part.Headers.Replace (idxHeader.Name, XUtil.Single<string> (idxHeader.Value, idxHeader, context));
            }
            return part;
        }

        /*
         * creates a MIME entity from file
         */
        private MimeEntity CreateMimeEntityFromFile (ApplicationContext context, ContentDisposition cntDisp, List<Stream> streams)
        {
            Stream stream = File.OpenRead (HttpRequest.GetBasePath (context) + cntDisp.FileName);
            streams.Add (stream); // adding stream to list of streams to dispose when we're done
            MimePart retVal = new MimePart ();
            retVal.ContentObject = new ContentObject (stream);
            return retVal;
        }

        /*
         * creates a nested Multipart MIME entity
         */
        private MimeEntity CreateNestedMultipart (ApplicationContext context, Node node, ContentType cntType, List<Stream> streams)
        {
            Multipart multipart = new Multipart (cntType.MediaSubtype);
            foreach (var idxChild in node ["children"].Children) {
                MimeEntity entity = CreateMimeEntity (context, idxChild, streams);
                multipart.Add (entity);
            }
            return multipart;
        }

        /*
         * creates a MIME entity from the value of the node
         */
        private MimeEntity CreateMimeEntityFromValue (ApplicationContext context, Node node, List<Stream> streams)
        {
            // parts content is in its value somehow
            var byteValue = XUtil.Single<byte[]> (node.Value, node, context, null);
            Stream stream = new MemoryStream (byteValue);
            streams.Add (stream);
            MimePart retVal = new MimePart ();
            retVal.ContentObject = new ContentObject (stream);
            return retVal;
        }

        /*
         * writes the roor Multipart to the Request stream
         */
        private void WriteMultipartToRequest (Multipart multipart, HttpWebRequest request)
        {
            // updating request HTTTP header 'Content-Type' to reflect "boundary"
            request.ContentType = multipart.ContentType.MimeType + multipart.ContentType.Parameters;

            // writing Multipart to HTTTP request stream
            using (var stream = request.GetRequestStream ()) {
                multipart.WriteTo (stream);
            }
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
