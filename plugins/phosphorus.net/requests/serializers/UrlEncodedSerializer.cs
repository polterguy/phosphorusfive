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
    public class UrlEncodedSerializer : Serializer
    {
        public override void Serialize (
            ApplicationContext context, 
            Node node, 
            HttpWebRequest request)
        {
            // creating a stream writer wrapping the "request content stream"
            StreamWriter writer = new StreamWriter (request.GetRequestStream ());
            bool first = true;
            foreach (var idxArg in GetArguments (node)) {
                if (first)
                    first = false; // first parameter
                else
                    writer.Write ("&"); // second, third, or fourth, etc, parameter, making sure we separate our parameters correctly
                writer.Write (string.Format ("{0}={1}", idxArg.Name, HttpUtility.UrlEncode (GetContent (context, idxArg))));
            }
        }

        /*
         * retrieves content of parameter
         */
        private byte[] GetContent (ApplicationContext context, Node node)
        {
            // this is not a file attachment, converting content to byte array if necessary, and returning to caller
            var content = XUtil.Single<object> (node.Value, node, context, null);
            if (content == null)
                return new byte [] { }; // no value, avoid returning null

            var byteContent = content as byte [];
            if (byteContent != null)
                return byteContent; // content is already byte array

            // converting value to string if necessary, before retrieving byte [], and returning to caller
            return Encoding.UTF8.GetBytes (Utilities.Convert<string> (content, context));
        }
    }
}
