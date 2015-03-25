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
            HttpWebRequest request)
        {
            // putting all parameters into body of request, as text
            using (StreamWriter writer = new StreamWriter (request.GetRequestStream ())) {
                foreach (var idxArg in GetArguments (node)) {
                    writer.Write (XUtil.Single<string> (idxArg.Value, idxArg, context));
                }
            }
        }
    }
}
