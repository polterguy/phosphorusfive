/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

namespace phosphorus.net.helpers
{
    /// <summary>
    ///     Class encapsulating an URL formatted HTTP request.
    /// 
    ///     Examples of an URL formatted request are HTTP/GET and DELETE.
    /// </summary>
    public class HttpUrlFormattedRequest : HttpRequest
    {
        protected override string GetURL (ApplicationContext context, Node node, string url)
        {
            StringBuilder builder = new StringBuilder (url);
            bool first = url.IndexOf ("?") == -1;

            // looping through everything that's neither [cookies], nor [headers] nor [method], and has a value, and using name of
            // node as name of parameter and value of node as value, constructing a URL-Encoded URL, returning to caller
            foreach (var idxArg in node.FindAll (ix => ix.Name != "headers" && ix.Name != "cookies" && ix.Name != "method" && ix.Value != null)) {

                // making sure our first argument starts with a "?", and all other arguments have "&" prepended in front of them
                if (first) {
                    first = false;
                    builder.Append ("?");
                } else {
                    builder.Append ("&");
                }
                builder.Append (HttpUtility.UrlEncode (idxArg.Name) + "=");
                builder.Append (HttpUtility.UrlEncode (XUtil.Single<string> (idxArg.Value, idxArg, (context))));
            }

            // returning Url to caller
            return builder.ToString ();
        }

        protected override void Decorate (ApplicationContext context, Node node, HttpWebRequest request, ContentType type)
        {
            // nothing to do here, since parameters are already handled in URL
            request.Method = "GET";
        }
    }
}
