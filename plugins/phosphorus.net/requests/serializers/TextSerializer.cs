/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

namespace phosphorus.net.requests.serializers
{
    public class TextSerializer : Serializer
    {
        public override void Serialize (
            ApplicationContext context, 
            Node node, 
            Stream stream,
            HttpWebRequest request)
        {
            // putting all parameters into body of request, as text, with CR/LF between all entities
            StreamWriter writer = new StreamWriter (stream) { AutoFlush = true };
            bool first = true;
            foreach (var idxArg in node.FindAll (ix => ix.Name != "headers" && ix.Name != "cookies" && ix.Name != "method")) {

                // making sure we have a CR/LF between all entities
                if (first)
                    first = false;
                else
                    writer.Write ("\r\n");
                
                // getting Content-Disposition, if there is any
                var cntDisp = GetDisposition (context, idxArg);
                
                if (cntDisp != null && !string.IsNullOrEmpty (cntDisp.FileName) && idxArg.Value == null) {

                    // this is a file attachment, assuming file is text
                    using (var fileStream = File.OpenRead (GetBasePath (context) + cntDisp.FileName)) {
                        fileStream.CopyTo (stream);
                    }
                } else {
                    
                    // content is (supposed to be) in value of node, somehow
                    writer.Write (idxArg.Get<string> (context, ""));
                }
            }
        }
    }
}
