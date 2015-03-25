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
    public class HttpDeleteRequest : QueryStringRequest
    {
        public HttpDeleteRequest (ApplicationContext context, Node node, string url)
            : base (context, node, url, "DELETE")
        { }
    }
}
