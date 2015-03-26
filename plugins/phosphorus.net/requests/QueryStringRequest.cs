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

namespace phosphorus.net.requests
{
    /// <summary>
    ///     Query string request.
    /// 
    ///     Will serialize an HTTP GET or DELETE request by adding all parameters to the URL as URL encoded key/value pairs. All
    ///     parameters will be converted to string values before being serialized.
    /// </summary>
    public abstract class QueryStringRequest : HttpRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.net.requests.QueryStringRequest"/> class.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node wrapping request.</param>
        /// <param name="url">URL for request.</param>
        /// <param name="method">Method to use, normally either 'DELETE' or 'GET'.</param>
        public QueryStringRequest (ApplicationContext context, Node node, string url, string method)
            : base (context, node, url, method)
        { }

        protected override string GetURL (ApplicationContext context, Node node, string url)
        {
            StringBuilder builder = new StringBuilder (url);
            bool first = url.IndexOf ("?") == -1;

            // looping through everything that's neither [cookies], nor [headers] nor [method], and has a value, and using name of
            // node as name of parameter and value of node as value, constructing a URL-Encoded URL, returning to caller
            foreach (var idxArg in GetParameters (node)) {

                // making sure our first argument starts with a "?", and all other arguments have "&" prepended in front of them
                if (first) {
                    first = false;
                    builder.Append ("?");
                } else {
                    builder.Append ("&");
                }

                builder.Append (HttpUtility.UrlEncode (idxArg.Name));
                builder.Append ("=");
                builder.Append (HttpUtility.UrlEncode (idxArg.GetExValue<string> (context)));
            }

            // returning Url to caller
            return builder.ToString ();
        }
    }
}
