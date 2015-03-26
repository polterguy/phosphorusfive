/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Web;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

namespace phosphorus.net.requests
{
    /// <summary>
    ///     Wraps an HTTP GET request.
    /// </summary>
    public class HttpGetRequest : QueryStringRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.net.requests.HttpGetRequest"/> class.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node wrapping the request parameters.</param>
        /// <param name="url">URL for request.</param>
        public HttpGetRequest (ApplicationContext context, Node node, string url)
            : base (context, node, url, "GET")
        { }
    }
}
