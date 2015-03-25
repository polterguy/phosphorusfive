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
    public class HttpGetRequest : QueryStringRequest
    {
        public HttpGetRequest (ApplicationContext context, Node node, string url)
            : base (context, node, url, "GET")
        { }
    }
}
