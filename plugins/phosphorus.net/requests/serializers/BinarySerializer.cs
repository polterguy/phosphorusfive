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
            HttpWebRequest request)
        {
            List<Node> args = new List<Node> (GetArguments (node));
            if (args.Count != 1)
                throw new ArgumentException ("The binary serializer can only handle exactly one argument.");

            var objValue = XUtil.Single<object> (args [0].Value, args [0], context, null);
            if (objValue == null)
                throw new ArgumentException ("The binary serializer was given a void expression, or a null argument.");

            var byteValue = objValue as byte [];
            if (byteValue != null) {

                // content is byte array
                request.GetRequestStream ().Write (byteValue, 0, byteValue.Length);
            } else {
                var strValue = objValue as string;
                if (strValue != null) {

                    // content is string
                    StreamWriter writer = new StreamWriter (request.GetRequestStream ());
                    writer.Write (strValue); 
                } else {

                    // defaulting to binary formatter
                    BinaryFormatter formatter = new BinaryFormatter ();
                    formatter.Serialize (request.GetRequestStream (), objValue);
                }
            }
        }
    }
}
