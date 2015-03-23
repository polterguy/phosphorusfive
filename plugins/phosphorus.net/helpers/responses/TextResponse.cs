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
    ///     Class encapsulating a text/xxx type of response.
    /// </summary>
    public class TextResponse : HttpResponse
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.net.helpers.TextResponse"/> class.
        /// </summary>
        /// <param name="response">The wrapped HTTP response.</param>
        public TextResponse (HttpWebResponse response)
            : base (response)
        { }

        public override void Parse (ApplicationContext context, Node node)
        {
            base.Parse (context, node);
            using (var reader = new StreamReader (Response.GetResponseStream ())) {

                // then response text
                node.LastChild.Add ("content", reader.ReadToEnd ());
            }
        }
    }
}
