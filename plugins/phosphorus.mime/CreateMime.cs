/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

namespace phosphorus.mime
{
    public static class CreateMime
    {
        /// <summary>
        ///     Creates a MIME Multipart from the given arguments.
        /// 
        ///     Allows you to create a MIME Multipart from the arguments given. You can set any MIME header as [headers], which is a key/value collection,
        ///     where the name of your child node becomes the header name, and its value its value. To sign your multipart, supply a [sign] node, with
        ///     the private key reference as value, and password to release private key from your GnuPG storage as [password] beneath your [sign] node.
        ///     If you wish, you can override the algorithm used to sign your message with any of the given algorithms; 'MD5', 'Sha1',
        ///     'RipeMD160', 'DoubleSha', 'MD2', 'Tiger192', 'Haval5160', 'Sha256', 'Sha384', 'Sha512', 'Sha224' and 'MD4'. The default algorithm is 'Sha1'.
        /// 
        ///     If you wish to encrypt your multipart, then you can add up a public key collection beneath an [encrypt] node, declaring which certificates
        ///     you wish to use as values of children beneath [encrypt]. If you choose to both sign and encrypt your message, then your multipart will first
        ///     be signed, for then to be encrypted.
        /// 
        ///     Your multipart will be returned as the value of the main node after creation.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.mime.create-multipart")]
        private static void pf_mime_create_multipart (ApplicationContext context, ActiveEventArgs e)
        {
            // needed to be able to dispose streams created during creation
            List<Stream> streams = new List<Stream> ();

            // finding all parameters
            var nodes = e.Args.FindAll (ix => ix.Name != "headers" && ix.Name != "sign" && ix.Name != "encrypt");

            // figuring out Content-Type, defaulting to "multipart/mixed"
            ContentType contentType = new ContentType ("multipart", "mixed");
            if (e.Args ["headers"] != null && e.Args ["headers"] ["Content-Type"] != null) {
                contentType = ContentType.Parse (e.Args ["headers"] ["Content-Type"].GetExValue<string> (context));
                if (contentType.MediaType != "multipart")
                    throw new ArgumentException ("You cannot create a Multipart with a MIME header that's not of 'multipart/xxx' something.");
            }

            try
            {
                // creating Multipart
                Multipart multipart = CreateRootMultipart (contentType);
                if (e.Args ["headers"] != null) {
                    foreach (var idxHeader in e.Args ["headers"].FindAll (ix => ix.Name != "Content-Type")) {
                        multipart.Headers.Replace (idxHeader.Name, idxHeader.GetExValue<string> (context));
                    }
                }
                foreach (var idxArg in nodes) {
                    multipart.Add (CreateMimeEntity (context, idxArg, streams));
                }

                // checking if we should sign and/or encrypt message, and which signing key and/or encryption certificate we should use
                // before we return Multipart to caller
                var resultMultipart = SignAndEncryptEntity (context, e.Args, multipart);

                // returning Multipart as value of main node
                using (MemoryStream stream = new MemoryStream ()) {
                    resultMultipart.WriteTo (stream);
                    e.Args.Value = stream.ToArray ();
                }

                // adding headers created during creation of multipart
                e.Args.Add ("headers");
                foreach (var idxHeader in resultMultipart.Headers) {
                    e.Args.LastChild.Add (idxHeader.Field, idxHeader.Value);
                }
            }
            finally
            {
                foreach (var idxStream in streams) {
                    idxStream.Dispose ();
                }
            }
        }

        /// <summary>
        ///     Creates a MIME Multipart from the given arguments.
        /// 
        ///     Not intended to be invoked directly from pf.lambda. Constructs a MimeKit Multipart from the given arguments.
        ///     Arguments that must be present is [ContentType] which is the ContentType of the multipart, [streams] which
        ///     is a List&lt;Stream&gt; which will be populated with all streams creating during the process, [entities] which
        ///     is an IEnumerable&lt;Node&gt; which are arguments or individual Mime entities to be constructed, and [main-node]
        ///     necessary to deduct if Multipart should be signed, encrypted, and so on.
        /// 
        ///     Will return the constructed Multipart (which might be both signed and encrypted) to caller as value of main node.
        /// 
        ///     This Active event will use less resources for you if you use it from C#, then the version wwith the same name, without the underscore,
        ///     since it does not require you to store the created Multipart in memory, allowing you to directly serialize it to a stream during creation.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "_pf.mime.create-multipart")]
        private static void _pf_mime_create_multipart (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving parameters
            var contentType = e.Args ["ContentType"].Get<ContentType> (context);
            var streams = e.Args ["streams"].Get<List<Stream>> (context);
            var nodes = e.Args ["entities"].Get<IEnumerable<Node>> (context);
            var node = e.Args ["main-node"].Get<Node> (context);

            // creating Multipart
            Multipart multipart = CreateRootMultipart (contentType);
            if (e.Args ["headers"] != null) {
                foreach (var idxHeader in e.Args ["headers"].FindAll (ix => ix.Name != "Content-Type")) {
                    multipart.Headers.Replace (idxHeader.Name, idxHeader.GetExValue<string> (context));
                }
            }
            foreach (var idxArg in nodes) {
                multipart.Add (CreateMimeEntity (context, idxArg, streams));
            }

            // checking if we should sign and/or encrypt message, and which signing key and/or encryption certificate we should use
            // before we return Multipart to caller
            e.Args.Value = SignAndEncryptEntity (context, node, multipart);
        }

        /*
         * creates the "root" Multipart MimeEntity
         */
        private static Multipart CreateRootMultipart (ContentType type)
        {
            // making sure we pass in the MediaSubtype
            Multipart multipart = new Multipart (type.MediaSubtype);

            // adding all existing parameters from 'Content-Type'
            foreach (var idxHeader in type.Parameters) {
                multipart.ContentType.Parameters [idxHeader.Name] = idxHeader.Value;
            }

            // returning Multipart to caller
            return multipart;
        }

        /*
         * creates a single MimeEntity from the given node
         */
        private static MimeEntity CreateMimeEntity (ApplicationContext context, Node node, List<Stream> streams)
        {
            // figuring out content's disposition
            ContentDisposition cntDisp = GetDisposition (context, node);

            // figuring out Content-Type
            ContentType cntType = GetContentType (context, node);

            MimeEntity part;
            bool isFile = false;
            if (cntDisp != null && !string.IsNullOrEmpty (cntDisp.FileName) && node.Value == null) {

                // part's content is in a file
                part = CreateMimeEntityFromFile (context, cntDisp, streams);
                isFile = true;
            } else if (cntType != null && cntType.MediaType == "multipart" && node.Value == null && node ["children"] != null) {

                // part is a nested Multipart, with MimeEntity items in [children] node
                part = CreateNestedMultipart (context, node, cntType, streams);
            } else if (node.Value != null) {

                // part is in value of node, somehow
                part = CreateMimeEntityFromValue (context, node, streams);
            } else {
                throw new ArgumentException ("Don't know how to create a MimeEntity from the given arguments");
            }

            // decorating MimeEntity with headers, making sure we only use children node's with a value, to avoid nodes
            // such as [children] and formatting parameters
            foreach (var idxHeader in node.FindAll (ix => ix.Value != null && ix.Name != string.Empty)) {
                part.Headers.Replace (idxHeader.Name, idxHeader.GetExValue<string> (context));
            }
            
            // checking to see if we should strip path from filename argument
            if (isFile && node ["Content-Disposition"].GetExChildValue ("strip-path", context, true)) {
                part.ContentDisposition.Parameters ["filename"] = Path.GetFileName (part.ContentDisposition.Parameters ["filename"]);
            }
            return part;
        }

        /*
         * creates a MIME entity from file
         */
        private static MimeEntity CreateMimeEntityFromFile (ApplicationContext context, ContentDisposition cntDisp, List<Stream> streams)
        {
            Stream stream = File.OpenRead (GetBasePath (context) + cntDisp.FileName);
            streams.Add (stream); // adding stream to list of streams to dispose when we're done
            MimePart retVal = new MimePart ();
            retVal.ContentObject = new ContentObject (stream);
            return retVal;
        }

        /*
         * creates a nested Multipart MIME entity
         */
        private static MimeEntity CreateNestedMultipart (ApplicationContext context, Node node, ContentType cntType, List<Stream> streams)
        {
            Multipart multipart = new Multipart (cntType.MediaSubtype);
            foreach (var idxChild in node ["children"].Children) {
                MimeEntity entity = CreateMimeEntity (context, idxChild, streams);
                multipart.Add (entity);
            }
            return multipart;
        }

        /*
         * creates a MIME entity from the value of the node
         */
        private static MimeEntity CreateMimeEntityFromValue (ApplicationContext context, Node node, List<Stream> streams)
        {
            // parts content is in its value somehow
            var byteValue = node.GetExValue<byte[]> (context, null);
            Stream stream = new MemoryStream (byteValue);
            streams.Add (stream);
            MimePart retVal = new MimePart ();
            retVal.ContentObject = new ContentObject (stream);
            return retVal;
        }

        /*
         * signs and encrypts MimeEntity if caller requests it, otherwise returning entity given
         */
        private static MimeEntity SignAndEncryptEntity (ApplicationContext context, Node node, MimeEntity entity)
        {
            string signatureEmail = node.GetExChildValue<string> ("sign", context);
            bool encrypt = node ["encrypt"] != null;
            if (!string.IsNullOrEmpty (signatureEmail)) {

                // signing first
                Node signEncrNode = new Node (string.Empty, entity);
                signEncrNode.Add ("email", signatureEmail);
                if (node ["sign"] ["password"] != null)
                    signEncrNode.Add ("password", node ["sign"].GetExChildValue<string> ("password", context));
                if (node ["sign"] ["algo"] != null)
                    signEncrNode.Add ("algo", node ["sign"].GetExChildValue<string> ("algo", context));
                entity = context.Raise ("_pf.crypto.pgp.sign", signEncrNode).Get<MimeEntity> (context);
            }
            if (encrypt) {

                // then encrypting
                Node signEncrNode = new Node (string.Empty, entity);
                signEncrNode.Add ("emails");
                foreach (var idxEncrNode in node ["encrypt"].Children) {
                    signEncrNode.LastChild.Add (string.Empty, idxEncrNode.GetExValue<string> (context));
                }
                entity = context.Raise ("_pf.crypto.pgp.encrypt", signEncrNode).Get<MimeEntity> (context);
            }
            return entity;
        }

        /*
         * returns the ContentDisposition for the given node, if there is any
         */
        private static ContentDisposition GetDisposition (ApplicationContext context, Node node)
        {
            var cntNode = node ["Content-Disposition"];
            if (cntNode != null)
                return ContentDisposition.Parse (cntNode.GetExValue<string> (context));
            return null;
        }

        /*
         * returns the ContentDisposition for the given node, if there is any
         */
        private static ContentType GetContentType (ApplicationContext context, Node node)
        {
            var cntNode = node ["Content-Type"];
            if (cntNode != null)
                return ContentType.Parse (cntNode.GetExValue<string> (context));
            return null;
        }

        /*
         * returns base path for application.
         */
        private static string _basePath;
        private static string GetBasePath (ApplicationContext context)
        {
            if (_basePath == null) {
                Node node = new Node ();
                context.Raise ("pf.core.application-folder", node);
                _basePath = node.Get<string> (context);
            }
            return _basePath;
        }
    }
}
