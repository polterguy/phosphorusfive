/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.net.requests.serializers
{
    public class TextSerializer : Serializer, ISerializer
    {
        public void Serialize (
            ApplicationContext context, 
            Node node, 
            HttpWebRequest request)
        {
            // putting all parameters into body of request, as text
            using (var writer = new StreamWriter (request.GetRequestStream ())) {
                foreach (var idxArg in HttpRequest.GetArguments (node)) {
                    writer.Write (XUtil.Single<string> (idxArg.Value, idxArg, context));
                }
            }
        }
    }
}
