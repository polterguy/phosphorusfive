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
    ///     Class wrapping an HTTP/POST request.
    /// 
    ///     Class encapsulating an HTTP/POST type of request. Nothing to really see here, most of the heavy loading is done in base class.
    /// </summary>
    public class HttpPostRequest : HttpComplexRequest
    {
        protected override void Decorate (ApplicationContext context, Node node, HttpWebRequest request, ContentType type)
        {
            request.Method = "POST";
            base.Decorate (context, node, request, type);
        }
    }
}
