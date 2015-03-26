/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

namespace phosphorus.web.ui.response.echo
{
    /// \todo put the stuff that's common between this class and MultipartSerializer from phosphorus.net into a shared common class
    /// which is actually almost EVERYTHING ...!
    /// possibly create a common "Parse Mime" class or something, which allows serializing a Multipart to any stream ...?
    public class EchoResponseMultipart : EchoResponse, IEchoResponse
    {
        private ContentType _contentType;

        public EchoResponseMultipart (ContentType contentType)
        {
            _contentType = contentType;
        }

        public void Echo (ApplicationContext context, Node node, HttpResponse response)
        {
            // we have to track all of our FileStream objects, such that we can dispose them when we're done
            List<Stream> streams = new List<Stream> ();
            try
            {
                // creating root Multipart, making sure sub-type and parameters from 'Content-Type' is passed on
                Multipart multipart = CreateRootMultipart ();

                // looping through all arguments, creating a MimeEntity, adding to Multipart
                foreach (var idxArg in GetParameters (node)) {
                    multipart.Add (CreateMimeEntity (context, idxArg, streams));
                }

                // writing Multipart to request stream
                WriteMultipartToResponse (multipart, response);
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
            } else if (cntType != null && cntType.MediaType == "multipart" && node.Value == null && node ["children"] != null) {

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
            Stream stream = File.OpenRead (GetBasePath (context) + cntDisp.FileName);
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
        private void WriteMultipartToResponse (Multipart multipart, HttpResponse response)
        {
            // updating request HTTP header 'Content-Type' to reflect "boundary"
            response.ContentType = multipart.ContentType.MimeType + multipart.ContentType.Parameters;

            // writing Multipart to HTTTP request stream
            multipart.WriteTo (response.OutputStream);
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
