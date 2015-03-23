/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using phosphorus.core;

namespace phosphorus.net.helpers
{
    /// <summary>
    ///     Class encapsulating a 'binary' HTTP response.
    /// 
    ///     Class encapsulating anything not 'text/xxx', 'multipart/xxx', and so on. Basically, defaults response to 'binary', returning raw bytes.
    /// </summary>
    public class BinaryResponse : HttpResponse
    {
        public BinaryResponse (HttpWebResponse response)
            : base (response)
        { }
        
        public override void Parse (ApplicationContext context, Node node)
        {
            base.Parse (context, node);

            // then response content
            MemoryStream stream = new MemoryStream ();
            Response.GetResponseStream ().CopyTo (stream);
            node.LastChild.Add ("content", stream.ToArray ());
        }
    }
}
