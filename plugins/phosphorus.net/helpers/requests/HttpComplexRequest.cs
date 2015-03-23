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

/// <summary>
///     Namespace wrapping helpers for creating requests.
/// 
///     Primary helpers for creating HTTP requests can be found in this namespace.
/// </summary>
namespace phosphorus.net.helpers
{
    /// <summary>
    ///     Class responsible for creating a POST or PUT HTTP request.
    /// 
    ///     Encapsulates the necessary methods and functionality for creating HTTP POST and PUT requests.
    /// </summary>
    public class HttpComplexRequest : HttpRequest
    {
        /*
         * override of base class method that creates either a multipart, text part, URL-encoded or binary http POST or PUT request.
         */
        protected override void Decorate (ApplicationContext context, Node node, HttpWebRequest request, ContentType type)
        {
            switch (type.MediaType) {
            case "multipart":
                TransmitMultipart (context, node, request, type);
                break;
            case "text":
                TransmitText (context, node, request);
                break;
            default:
                if (type.MediaType == "application" && type.MediaSubtype == "x-www-form-urlencoded") {
                    TransmitUrlEncodedRequest (context, node, request);
                } else {
                    WriteBinary (context, node, request);
                }
                break;
            }
        }

        /*
         * creates a multipart HTTP request, and transmits over the given HttpWebRequest
         */
        private void TransmitMultipart (
            ApplicationContext context, 
            Node node, 
            HttpWebRequest request, 
            ContentType type)
        {
            if (node.CountWhere (ix => ix.Name == "content" || ix.Name == "file") == 1 && node ["file"] != null) {

                // loads a Multipart from disc, and transmits over the wire
                TransmitMultipartFromFile (context, node ["file"].Get<string> (context), request, type);
            } else {
                
                // parses a Multipart from the given args, and transmits over the wire
                TransmitMultipartFromArgs (context, node, request, type);
            }
        }

        /*
         * loads a Multipart from disc, and transmits over the given HttpWebRequest
         */
        private void TransmitMultipartFromFile (
            ApplicationContext context, 
            string filename, 
            HttpWebRequest request, 
            ContentType type)
        {
            using (Stream stream = File.OpenRead (GetBasePath (context) + filename)) {
                Multipart multipart = (Multipart)Multipart.Load (stream);

                // making sure we update the HTTTP header to contain the boundary, before we write our Multipart
                multipart.ContentType.MediaSubtype = type.MediaSubtype;
                request.ContentType = multipart.ContentType.MimeType + multipart.ContentType.Parameters;
                multipart.WriteTo (request.GetRequestStream ());
            }
        }

        /*
         * traverses the given arguments, and creates a Multipart from it, which it transmits over the given HttpWebRequest
         */
        private void TransmitMultipartFromArgs (
            ApplicationContext context,
            Node node,
            HttpWebRequest request,
            ContentType type)
        {
            List<Stream> streams = new List<Stream> ();
            try {
                // creating root Multipart and iterating through children adding MimeEntities
                Multipart multipart = new Multipart (type.MediaSubtype);
                if (!string.IsNullOrEmpty (type.Boundary))
                    multipart.Boundary = type.Boundary;
                foreach (var idxParam in node.FindAll (idx => idx.Name == "content" || idx.Name == "file")) {

                    // creating our MIME entity, and adding to root Multipart
                    MimeEntity entity = CreateMimeEntity (context, idxParam, streams);
                    if (entity != null)
                        multipart.Add (entity);
                }

                // making sure we update the HTTTP header to contain the boundary, before we write our Multipart
                request.ContentType = multipart.ContentType.MimeType + multipart.ContentType.Parameters;
                multipart.WriteTo (request.GetRequestStream ());
            } finally {

                // house cleaning
                foreach (var idxStream in streams) {
                    idxStream.Dispose ();
                }
            }
        }

        /*
         * creates a single MimeEntity from the given node
         */
        private MimeEntity CreateMimeEntity (ApplicationContext context, Node node, List<Stream> streams)
        {
            var entityType = ContentType.Parse (
                XUtil.Single<string> (
                    node.GetChildValue<object> ("Content-Type", context, null), 
                    node ["Content-Type"], 
                    context, 
                    "text/plain"));

            MimeEntity retVal;
            switch (entityType.MediaType) {
            case "multipart":
                retVal = CreateMultipart (context, node, entityType, streams);
                break;
            case "text":
                retVal = CreateTextEntity (context, node, entityType);
                break;
            default:
                retVal = CreateBinaryEntity (context, node, entityType, streams);
                break;
            }
            
            // decorating MimeEntity with headers
            foreach (var idxHeader in node.Children) {
                if (idxHeader.Name == "content" || idxHeader.Name == "file")
                    continue; // probably a Multipart child content node
                if (idxHeader.Name == "Content-Type" && retVal is Multipart)
                    continue; // header already set
                retVal.Headers.Replace (idxHeader.Name, XUtil.Single<string> (idxHeader.Value, idxHeader, context));
            }
            
            // defaulting Content-Disposition header, unless explicitly given, if entity is a "file type" of entity
            if (node.Name == "file" && retVal.ContentDisposition == null) {
                retVal.ContentDisposition = new ContentDisposition ("attachment");
                retVal.ContentDisposition.Parameters.Add ("filename", node.Get<string> (context));
            }
            return retVal;
        }

        /*
         * creates a Multipart from the given node
         */
        private Multipart CreateMultipart (
            ApplicationContext context, 
            Node node, 
            ContentType type, 
            List<Stream> streams)
        {
            var value = XUtil.Single<object> (node.Value, node, context, null);
            if (value == null) {

                // individual parts of Multipart are children of current node
                return CreateMultipartFromChildren (context, node, type, streams);
            } else if (node.Name == "content") {

                // individual parts of Multipart are somehow in the node's value
                return CreateMultipartFromValue (context, value, type, streams);
            } else if (node.Name == "file") {

                // individual parts of Multipart are somehow in the node's value
                return LoadMultipartFromValue (context, (string)value, type, streams);
            } else {
                throw new ArgumentException ("Don't know how to create a Multipart from the given argument.");
            }
        }
        
        /*
         * creates a Multipart from the given node's childre
         */
        private Multipart CreateMultipartFromChildren (
            ApplicationContext context, 
            Node node, 
            ContentType type, 
            List<Stream> streams)
        {
            Multipart multipart = new Multipart (type.MediaSubtype);
            if (!string.IsNullOrEmpty (type.Boundary))
                multipart.ContentType.Boundary = type.Parameters ["boundary"];
            foreach (var idxParam in node.FindAll (idx => idx.Name == "content" || idx.Name == "file")) {

                // creating our MIME entity, and adding to root Multipart
                MimeEntity entity = CreateMimeEntity (context, idxParam, streams);
                if (entity != null)
                    multipart.Add (entity);
            }
            return multipart;
        }

        /*
         * creates a Multipart from value of node, somehow
         */
        private Multipart CreateMultipartFromValue (
            ApplicationContext context,
            object value,
            ContentType type,
            List<Stream>streams)
        {
            Stream stream;
            if (value is byte[]) {

                // multipart in binary format
                stream = new MemoryStream ((byte[])value);
            } else if (value is string) {

                // multipart in text format, or somehow something that can be converted into text (hopefully!)
                stream = new MemoryStream (Encoding.UTF8.GetBytes (Utilities.Convert<string> (value, context)));
            } else {
                throw new ArgumentException ("Sorry, I don't know how to create a Multipart from that argument.");
            }

            // loading Multipart from stream, which is now wrapping its contents, and returning to caller
            if (type == null)
                return (Multipart)Multipart.Load (stream);
            else
                return (Multipart)Multipart.Load (type, stream);
        }
        
        /*
         * creates a Multipart from value of node, somehow
         */
        private Multipart LoadMultipartFromValue (
            ApplicationContext context,
            string filename,
            ContentType type,
            List<Stream>streams)
        {
            // multipart exists in file on disc, loading file, making sure we store stream, such that it can be disposed
            Stream stream = File.OpenRead (GetBasePath (context) + filename);
            streams.Add (stream);
            if (string.IsNullOrEmpty (type.Boundary)) {
                return (Multipart)Multipart.Load (stream);
            } else {
                return (Multipart)Multipart.Load (type, stream);
            }
        }

        /*
         * creates a TextPart by traversing the given node
         */
        private TextPart CreateTextEntity (
            ApplicationContext context, 
            Node node, 
            ContentType type)
        {
            TextPart retVal = new TextPart (type.MediaSubtype);
            var value = XUtil.Single<object> (node.Value, node, context);
            if (value == null)
                return null;
            if (value is byte[]) {

                // converting from byte[] to string
                retVal.SetText (Encoding.UTF8, Encoding.UTF8.GetString ((byte[])value));
            } else if (value is string) {
                if (node.Name == "content") {

                    // content is already string
                    retVal.SetText (Encoding.UTF8, (string)value);
                } else {

                    // content is on disc, assuming file is text file
                    using (StreamReader reader = File.OpenText (GetBasePath (context) + value)) {
                        retVal.SetText (Encoding.UTF8, reader.ReadToEnd ());
                    }
                }
            } else {

                // contents can be anything, Guid, Booleans or Integers. Converting to string before setting content ...
                retVal.SetText (Encoding.UTF8, Utilities.Convert<string> (value, context));
            }
            return retVal;
        }

        /*
         * creates a MimePart that is neither a TextPart nor a Multipart, but "anything else"
         */
        private MimePart CreateBinaryEntity (
            ApplicationContext context, 
            Node node, 
            ContentType type,
            List<Stream> streams)
        {
            MimePart retVal = new MimePart ();
            var value = XUtil.Single<object> (node.Value, node, context);
            if (value == null)
                return null;

            Stream stream;
            if (value is byte[]) {

                // wrapping byte[] in memory stream
                stream = new MemoryStream ((byte[])value);
            } else if (value is string) {
                if (node.Name == "content") {

                    // content is string, wrapping in memory stream, converting to bytes
                    stream = new MemoryStream (Encoding.UTF8.GetBytes ((string)value));
                } else {

                    // content is on disc, assuming file is text file
                    stream = File.OpenRead (GetBasePath (context) + value);
                    streams.Add (stream);
                }
            } else {

                // contents can be anything, Guid, Booleans or Integers. Converting to string before setting content ...
                BinaryFormatter formatter = new BinaryFormatter ();
                stream = new MemoryStream ();
                formatter.Serialize (stream, value);
            }
            retVal.ContentObject = new ContentObject (stream);
            return retVal;
        }

        /*
         * transmits a "text/xxx" type of request. Notice that this guy will make sure each text part are separated by CR/LF.
         */
        private void TransmitText (ApplicationContext context, Node node, HttpWebRequest request)
        {
            // putting all parameters into body of request, as text, with CR/LF between all entities
            using (StreamWriter writer = new StreamWriter (request.GetRequestStream ()) { AutoFlush = true }) {
                bool first = true;
                foreach (var idxParam in node.FindAll (idx => idx.Name == "content" || idx.Name == "file")) {
                    var value = XUtil.Single<object> (idxParam.Value, idxParam, context, null);
                    if (value == null)
                        continue;
                    if (first)
                        first = false;
                    else
                        writer.Write ("\r\n");
                    if (idxParam.Name == "file") {
                        using (FileStream stream = File.OpenRead (GetBasePath (context) + Utilities.Convert<string> (value, context))) {
                            // assuming file is "text file"
                            stream.CopyTo (writer.BaseStream);
                        }
                    } else {
                        writer.Write (Utilities.Convert<string> (value, context));
                    }
                }
            }
        }

        /*
         * transmits an "application/x-www-form-urlencoded" type of request
         */
        private static void TransmitUrlEncodedRequest (ApplicationContext context, Node node, HttpWebRequest request)
        {
            // creating a stream writer wrapping the "request content stream"
            using (StreamWriter writer = new StreamWriter (request.GetRequestStream ())) {
                bool first = true;
                foreach (var idxArg in node.FindAll (ix => ix.Name != "headers" && ix.Name != "cookies" && ix.Name != "method" && ix.Value != null)) {
                    if (first)
                        first = false; // first parameter
                    else
                        writer.Write ("&"); // second, third, or fourth, etc, parameter, making sure we separate our parameters correctly
                    string value = XUtil.Single<string> (idxArg.Value, idxArg, context);
                    writer.Write (string.Format ("{0}={1}", idxArg.Name, HttpUtility.UrlEncode (value)));
                }
            }
        }

        /*
         * transmits a request that is neither x-www-form-urlencoded, multipart nor text/xxx type of request. this is our default fallback
         */
        private void WriteBinary (ApplicationContext context, Node node, HttpWebRequest request)
        {
            // putting all parameters into body of request, as binary
            using (Stream stream = request.GetRequestStream ()) {
                foreach (var idxParam in node.FindAll (idx => idx.Name == "content" || idx.Name == "file")) {
                    if (idxParam.Count != 0)
                        throw new ArgumentException ("You cannot supply parameters to content when creating a text request");
                    var value = XUtil.Single<object> (idxParam.Value, idxParam, context, null);
                    if (value == null)
                        continue;
                    if (value is byte[]) {
                        byte[] byteValue = (byte[])value;
                        stream.Write (byteValue, 0, byteValue.Length);
                    } else if (value is string) {
                        if (idxParam.Name == "file") {
                            using (FileStream fileStream = File.OpenRead (GetBasePath (context) + Utilities.Convert<string> (value, context))) {
                                fileStream.CopyTo (stream);
                            }
                        } else {
                            byte[] byteValue = Encoding.UTF8.GetBytes ((string)value);
                            stream.Write (byteValue, 0, byteValue.Length);
                        }
                    } else {
                        // defaulting to Binary formatter
                        BinaryFormatter formatter = new BinaryFormatter ();
                        formatter.Serialize (stream, value);
                    }
                }
            }
        }
    }
}
