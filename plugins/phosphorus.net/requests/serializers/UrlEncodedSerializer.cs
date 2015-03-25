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
            Stream stream,
            HttpWebRequest request)
        {
            // creating a stream writer wrapping the "request content stream"
            StreamWriter writer = new StreamWriter (stream);
            bool first = true;
            foreach (var idxArg in node.FindAll (ix => ix.Name != "headers" && ix.Name != "cookies" && ix.Name != "method")) {
                if (first)
                    first = false; // first parameter
                else
                    writer.Write ("&"); // second, third, or fourth, etc, parameter, making sure we separate our parameters correctly
                
                // getting Content-Disposition, if there is any
                var cntDisp = GetDisposition (context, idxArg);

                writer.Write (
                    string.Format ("{0}={1}", 
                               GetName (context, idxArg, cntDisp), 
                               HttpUtility.UrlEncode (GetContent (context, idxArg, cntDisp))));
            }
        }
        
        /*
         * returns the name of our parameter, which unless there's a ContentDisposition with a "name" parameter, 
         * will default to the node's name
         */
        private string GetName (ApplicationContext context, Node node, ContentDisposition cntDisp)
        {
            if (cntDisp != null && cntDisp.Parameters ["name"] != null)
                return cntDisp.Parameters ["name"];
            return node.Name;
        }

        /*
         * retrieves content of parameter, which if this is a file attachment, will be the content of that file, otherwise
         * it will be the value of the parameter node given
         */
        private byte[] GetContent (ApplicationContext context, Node node, ContentDisposition cntDisp)
        {
            if (cntDisp != null && !string.IsNullOrEmpty (cntDisp.FileName)) {

                // this is a file attachment, which is weird for this type of request, but who are we to judge ...
                if (node.Value != null)
                    throw new ArgumentException ("Sorry, I got confused, but a 'value' and a 'filename' was given, please decide where your content is.");
                return File.ReadAllBytes (GetBasePath (context) + cntDisp.FileName);
            }

            // this is not a file attachment, converting content to byte array if necessary, and returning to caller
            var content = XUtil.Single<object> (node.Value, node, context, null);
            if (content == null)
                return new byte [] { }; // no value

            var byteContent = content as byte [];
            if (byteContent != null)
                return byteContent; // content is already byte array

            // converting value to string if necessary, before retrieving byte [], and returning to caller
            return Encoding.UTF8.GetBytes (Utilities.Convert<string> (content, context));
        }
    }
}
