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
    public class UrlEncodedSerializer : Serializer, ISerializer
    {
        public void Serialize (
            ApplicationContext context, 
            Node node, 
            HttpWebRequest request)
        {
            // creating a stream writer wrapping the "request content stream"
            using (StreamWriter writer = new StreamWriter (request.GetRequestStream ())) {
                bool first = true;
                foreach (var idxArg in HttpRequest.GetArguments (node)) {
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
