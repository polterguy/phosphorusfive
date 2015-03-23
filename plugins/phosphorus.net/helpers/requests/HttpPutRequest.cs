/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Net;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

namespace phosphorus.web.helpers
{
    public class HttpPutRequest : HttpComplexRequest
    {
        protected override void Decorate (ApplicationContext context, Node node, HttpWebRequest request, ContentType type)
        {
            request.Method = "PUT";
            base.Decorate (context, node, request, type);
        }
    }
}
