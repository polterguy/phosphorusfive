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
    /// <summary>
    ///     Multipart echo response.
    /// 
    ///     Will echo a multipart back to client over the current HTTP response.
    /// </summary>
    public class EchoResponseMultipart : EchoResponse, IEchoResponse
    {
        private ContentType _contentType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.web.ui.response.echo.EchoResponseMultipart"/> class.
        /// </summary>
        /// <param name="contentType">Content type.</param>
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
                // retreiving parameters
                var entities = GetParameters (node);

                // creating Multipart
                var multipartNode = new Node ();
                multipartNode.Add ("ContentType", _contentType);
                multipartNode.Add ("entities", entities);
                multipartNode.Add ("streams", streams);
                multipartNode.Add ("main-node", node);
                context.Raise ("_pf.mime.create-multipart", multipartNode);

                // writing Multipart to request stream
                WriteMultipartToResponse (multipartNode.Get<Multipart> (context), response);
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
         * writes the roor Multipart to the Request stream
         */
        private void WriteMultipartToResponse (MimeEntity multipart, HttpResponse response)
        {
            // updating request HTTP header 'Content-Type' to reflect "boundary"
            response.ContentType = multipart.ContentType.MimeType + multipart.ContentType.Parameters;

            // writing Multipart to HTTTP request stream
            multipart.WriteTo (response.OutputStream);
        }
    }
}
