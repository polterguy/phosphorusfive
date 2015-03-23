/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

namespace phosphorus.web.helpers
{
    public class HttpComplexRequest : HttpRequest
    {
        protected override void Decorate (ApplicationContext context, Node node, HttpWebRequest request, ContentType type)
        {
            switch (type.MediaType) {
            case "multipart":
                WriteMultipart (context, node, request, type);
                break;
            case "text":
                WriteText (context, node, request);
                break;
            default:
                WriteBinary (context, node, request);
                break;
            }
        }

        private void WriteMultipart (ApplicationContext context, Node node, HttpWebRequest request, ContentType type)
        {
            List<Stream> streams = new List<Stream> ();
            try {

                // creating root Multipart and iterating through children adding MimeEntities
                Multipart multipart = new Multipart (type.MediaSubtype);
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
                retVal = CreateMultipartEntity (context, node, entityType, streams);
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
                    continue; // header already set other placees
                retVal.Headers.Replace (idxHeader.Name, XUtil.Single<string> (idxHeader.Value, idxHeader, context));
            }
            return retVal;
        }

        private Multipart CreateMultipartEntity (
            ApplicationContext context, 
            Node node, 
            ContentType type, 
            List<Stream> streams)
        {
            var value = XUtil.Single<object> (node.Value, node, context, null);
            if (value == null) {

                // individual parts of Multipart are probably children of current node
                Multipart multipart = new Multipart (type.MediaSubtype);
                foreach (var idxParam in node.FindAll (idx => idx.Name == "content" || idx.Name == "file")) {

                    // creating our MIME entity, and adding to root Multipart
                    MimeEntity entity = CreateMimeEntity (context, idxParam, streams);
                    if (entity != null)
                        multipart.Add (entity);
                }
                return multipart;
            } else {

                // somehow, multipart is contained in "value" of node
                Stream stream;
                if (value is byte[]) {

                    // multipart in binary format
                    stream = new MemoryStream ((byte[])value);
                } else if (value is string) {
                    if (node.Name == "content") {

                        // multipart in text format
                        stream = new MemoryStream (Encoding.UTF8.GetBytes ((string)value));
                    } else {

                        // multipart exists in file on disc, loading file, making sure we store stream, such that it can be disposed
                        stream = File.OpenRead (GetBasePath (context) + value);
                        streams.Add (stream);
                    }
                } else {

                    // only byte[] and strings can create Multiparts
                    throw new ArgumentException ("Sorry, I don't know how to create a Multipart from the given argument");
                }

                // loading Multipart from stream, which is now wrapping its contents, and returning to caller
                return (Multipart)Multipart.Load (type, stream);
            }
        }
        
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

        private void WriteText (ApplicationContext context, Node node, HttpWebRequest request)
        {
            // putting all parameters into body of request, as text, with CR/LF between all entities
            using (StreamWriter writer = new StreamWriter (request.GetRequestStream ()) { AutoFlush = true }) {
                bool first = true;
                foreach (var idxParam in node.FindAll (idx => idx.Name == "content" || idx.Name == "file")) {
                    var value = XUtil.Single<object> (idxParam.Value, idxParam, context);
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
        
        private void WriteBinary (ApplicationContext context, Node node, HttpWebRequest request)
        {
            // putting all parameters into body of request, as binary
            using (Stream stream = request.GetRequestStream ()) {
                foreach (var idxParam in node.FindAll (idx => idx.Name == "content" || idx.Name == "file")) {
                    var value = XUtil.Single<object> (idxParam.Value, idxParam, context);
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
