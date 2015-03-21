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

namespace phosphorus.web.helpers
{
    public class HttpGetRequest : HttpUriFormattedRequest
    {
        protected override void Decorate (ApplicationContext context, Node node, HttpWebRequest request, ContentType type)
        {
            // nothing to do here, since parameters are already handled in URL
            request.Method = "GET";
        }
    }
}
