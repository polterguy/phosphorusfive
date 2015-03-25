/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.net.requests;
using MimeKit;

namespace phosphorus.net.requests
{
    public class HttpPostRequest : HttpRequest
    {
        private ISerializer _serializer;

        public HttpPostRequest (ApplicationContext context, Node node, string url, ISerializer serializer)
            : base (context, node, url, "POST")
        {
            if (serializer == null)
                throw new ArgumentNullException ("serializer");
            _serializer = serializer;
        }

        public override IResponse Execute (ApplicationContext context, Node node)
        {
            // making sure our stream is disposed when done ...
            using (var str = Request.GetRequestStream ()) {
                _serializer.Serialize (context, node, Request);
            }
            return base.Execute (context, node);
        }
    }
}
