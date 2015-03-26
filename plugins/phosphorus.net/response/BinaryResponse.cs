/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using phosphorus.core;

/// <summary>
///     Namespace wrapping response classes.
/// 
///     This namespace contains all HTTP response classes for parsing HTTP responses when creating HTTP requests.
/// </summary>
namespace phosphorus.net.response
{
    /// <summary>
    ///     Responsible for de-serializing a binary HTTP response.
    /// 
    ///     Will de-serialize an HTTP response as an array of bytes, byte[], and return as [content].
    /// </summary>
    public class BinaryResponse : HttpResponse
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.net.response.BinaryResponse"/> class.
        /// </summary>
        /// <param name="response">Wrapped response.</param>
        public BinaryResponse (HttpWebResponse response)
            : base (response)
        { }
        
        public override void Parse (ApplicationContext context, Node node)
        {
            base.Parse (context, node);

            // then response content
            using (MemoryStream stream = new MemoryStream ()) {
                Response.GetResponseStream ().CopyTo (stream);
                node.LastChild.Add ("content", stream.ToArray ());
            }
        }
    }
}
