/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Net;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.net.requests.serializers
{
    public class BinarySerializer : Serializer, ISerializer
    {
        public void Serialize (
            ApplicationContext context, 
            Node node, 
            HttpWebRequest request)
        {
            // putting all parameters into body of request, as binary object
            using (var stream = request.GetRequestStream ()) {
                foreach (var idxArg in HttpRequest.GetArguments (node)) {
                    var value = XUtil.Single<byte[]> (idxArg.Value, idxArg, context);
                    stream.Write (value, 0, value.Length);
                }
            }
        }
    }
}
