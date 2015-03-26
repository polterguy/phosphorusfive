/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Net;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

namespace phosphorus.net.requests
{
    /// <summary>
    ///     Wraps an HTTP DELETE request.
    /// </summary>
    public class HttpDeleteRequest : QueryStringRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.net.requests.HttpDeleteRequest"/> class.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node wrapping the request parameters.</param>
        /// <param name="url">URL for request.</param>
        public HttpDeleteRequest (ApplicationContext context, Node node, string url)
            : base (context, node, url, "DELETE")
        { }
    }
}
