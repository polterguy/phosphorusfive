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
    /// <summary>
    ///     Wraps an HTTP PUT request.
    /// 
    ///     Class needs an ISerializer instance, to serialize its content over the HTTP request stream.
    /// </summary>
    public class HttpPutRequest : HttpRequest
    {
        private ISerializer _serializer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.net.requests.HttpPutRequest"/> class.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node wrapping the request parameters.</param>
        /// <param name="url">URL for request.</param>
        /// <param name="serializer">Serializer to use to serialize content over request stream.</param>
        public HttpPutRequest (ApplicationContext context, Node node, string url, ISerializer serializer)
            : base (context, node, url, "PUT")
        {
            if (serializer == null)
                throw new ArgumentNullException ("serializer");
            _serializer = serializer;
        }

        public override IResponse Execute (ApplicationContext context, Node node)
        {
            _serializer.Serialize (context, node, Request);
            return base.Execute (context, node);
        }
    }
}
