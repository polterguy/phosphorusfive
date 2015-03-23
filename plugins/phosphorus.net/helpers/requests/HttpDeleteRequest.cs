/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Net;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

namespace phosphorus.net.helpers
{
    /// <summary>
    ///     Class wrapping an HTTP/DELETE type of request.
    /// 
    ///     Wraps a DELETE HTTP request. Nothing to really see here, most of the heavy loading is done in base class.
    /// </summary>
    public class HttpDeleteRequest : HttpUrlFormattedRequest
    {
        protected override void Decorate (ApplicationContext context, Node node, HttpWebRequest request, ContentType type)
        {
            // nothing to do here, since parameters are already handled in URL
            request.Method = "DELETE";
            base.Decorate (context, node, request, type);
        }
    }
}
