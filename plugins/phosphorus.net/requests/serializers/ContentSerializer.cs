/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using phosphorus.core;
using phosphorus.expressions;

/// <summary>
///     Namespace wrapping all HTTP request serializers.
/// 
///     Namespace contains all HTTP request serializers, responsible for serializing content over HTTP POST and PUT requests.
/// </summary>
namespace phosphorus.net.requests.serializers
{
    /// <summary>
    ///     Fallback serializer for HTTP POST and PUT requests.
    /// 
    ///     Serializer used as fallback serializer when creating HTTP/POST or PUT web requests, when no other match can be found, due to a
    ///     'Content-Type' header that is not recognized by any of the other serializers, such as 'multipart/xxx' or
    ///     'application/x-www-form-urlencode'.
    /// 
    ///     Can serialize both binary types, and string-based request objects. If your item has an [is-file] node, with the value 
    ///     of 'true', then the file will be serialized, without being loaded into memory.
    /// </summary>
    public class ContentSerializer : ISerializer
    {
        public void Serialize (ApplicationContext context, Node node, HttpWebRequest request)
        {
            if (node ["sign"] != null || node ["encrypt"] != null)
                throw new ArgumentException ("You can only sign and/or encrypt a multipart request. Set the 'Content-Type' header to 'multipart/xxx' to allow for signing and/or encryption");

            // putting all parameters into body of request, as a single object
            using (var stream = request.GetRequestStream ()) {
                foreach (var idxArg in HttpRequest.GetParameters (node)) {
                    if (idxArg.GetExChildValue ("is-file", context, false)) {

                        // item is a file
                        using (FileStream fileStream = File.OpenRead (HttpRequest.GetBasePath (context) + idxArg.GetExValue<string> (context))) {
                            fileStream.CopyTo (stream);
                        }
                    } else {

                        // serializing value of node
                        var byteValue =  idxArg.GetExValue<byte[]> (context);
                        stream.Write (byteValue, 0, byteValue.Length);
                    }
                }
            }
        }
    }
}
