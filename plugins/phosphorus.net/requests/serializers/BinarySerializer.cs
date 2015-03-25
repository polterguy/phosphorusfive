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
    public class BinarySerializer : Serializer
    {
        public override void Serialize (
            ApplicationContext context, 
            Node node, 
            Stream stream,
            HttpWebRequest request)
        {
            // putting all parameters into body of request, as binary content
            foreach (var idxArg in node.FindAll (ix => ix.Name != "headers" && ix.Name != "cookies" && ix.Name != "method")) {

                // getting Content-Disposition, if there is any
                var cntDisp = GetDisposition (context, idxArg);

                if (cntDisp != null && !string.IsNullOrEmpty (cntDisp.FileName) && idxArg.Value == null) {

                    // this is a file attachment
                    using (var fileStream = File.OpenRead (GetBasePath (context) + cntDisp.FileName)) {
                        fileStream.CopyTo (stream);
                    }
                } else if (idxArg.Value != null) {

                    // content is (supposed to be) in value of node, somehow
                    var byteValue = idxArg.Value as byte [];
                    if (byteValue != null) {

                        // content is byte array
                        stream.Write (byteValue, 0, byteValue.Length);
                    } else {
                        var strValue = idxArg.Value as string;
                        if (strValue != null) {

                            // content is string, making sure we support expressions
                            StreamWriter writer = new StreamWriter (stream);
                            writer.Write (XUtil.Single <string> (strValue, idxArg, context)); 
                        } else {

                            // defaulting to binary formatter
                            BinaryFormatter formatter = new BinaryFormatter ();
                            formatter.Serialize (stream, idxArg.Value);
                        }
                    }
                }
            }
        }
    }
}
