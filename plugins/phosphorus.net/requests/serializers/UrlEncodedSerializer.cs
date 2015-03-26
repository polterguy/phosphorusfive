/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Net;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.net.requests.serializers
{
    /// <summary>
    ///     Responsible for serializing 'application/x-www-form-urlencode' types of requests.
    /// 
    ///     This is the default serializer used, if no 'Content-Type' is declared for your HTTP POST or PUT requests. Basically it just
    ///     URL-encodes the HTTP request, the same way a browser would for its form elements.
    /// </summary>
    public class UrlEncodedSerializer : ISerializer
    {
        public void Serialize (
            ApplicationContext context, 
            Node node, 
            HttpWebRequest request)
        {
            // creating a stream writer wrapping the "request content stream"
            using (StreamWriter writer = new StreamWriter (request.GetRequestStream ())) {
                bool first = true;
                foreach (var idxArg in HttpRequest.GetParameters (node)) {
                    if (first)
                        first = false; // first parameter
                    else
                        writer.Write ("&"); // second, third, or fourth, etc, parameter, making sure we separate our parameters correctly
                    writer.Write (string.Format ("{0}=", idxArg.Name));
                    writer.Write (HttpUtility.UrlEncode (XUtil.Single<string> (idxArg.Value, idxArg, context)));
                }
            }
        }
    }
}
