/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using phosphorus.core;

namespace phosphorus.web.helpers
{
    public class TextResponse : HttpResponse
    {
        public TextResponse (HttpWebResponse response)
            : base (response)
        { }

        public override void Parse (ApplicationContext context, Node node)
        {
            using (var reader = new StreamReader (Response.GetResponseStream ())) {
                Node current = node.Add ("result", Response.ResponseUri.ToString ()).LastChild;

                // HTTP headers and cookies
                ParseHeaders (context, current);
                ParseCookies (context, current);

                // then response text
                current.Add ("content", reader.ReadToEnd ());
            }
        }
    }
}
