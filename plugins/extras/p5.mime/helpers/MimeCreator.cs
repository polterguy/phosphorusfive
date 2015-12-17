/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MimeKit;
using MimeKit.Cryptography;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.mime.helpers
{
    /// <summary>
    ///     Helper to create a MimeEntity
    /// </summary>
    public class MimeCreator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.mime.helpers.MimeCreator"/> class.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="entityNode">Entity node, declaring which MimeEntity caller requests</param>
        /// <param name="streams">Streams created during creation process. It is caller's responsibility to close and dispose these streams afterwards</param>
        public MimeCreator (
            ApplicationContext context,
            Node entityNode,
            List<Stream> streams)
        {
            Context = context;
            EntityNode = entityNode;
            Streams = streams;
        }

        /// <summary>
        ///     Returns the Application Context used for the creating MimeEntity
        /// </summary>
        /// <value>The Application Context</value>
        public ApplicationContext Context {
            get;
            private set;
        }

        /// <summary>
        ///     Returns the Node used to create MimeEntity from
        /// </summary>
        /// <value>The result node</value>
        public Node EntityNode {
            get;
            private set;
        }

        /// <summary>
        ///    Streams created during creation process of instance. It is caller's responsibility to close and dispose these streams
        /// </summary>
        /// <value>The list of streams created during creation process</value>
        public List<Stream> Streams {
            get;
            private set;
        }

        /// <summary>
        ///     Creates a MimeEntity according to declaration in EntityNode, and returns to caller
        /// </summary>
        public MimeEntity Create()
        {
            // Recursively creates a MimeEntity according to given EntityNode
            return Create (EntityNode);
        }

        /*
         * Actual implementation of creation of MimeEntity, recursively runs through given node, and creates a MimeEntity accordingly
         */
        private MimeEntity Create (Node entityNode)
        {
            // Setting up a return value
            MimeEntity retVal = null;

            // Figuring out which type to create
            switch (entityNode.Name) {
                case "multipart":
                retVal = CreateMultipart (entityNode);
                    break;
                default:
                    retVal = CreateLeafPart (entityNode);
                    break;
            }

            // Figuring out if entity should be encrypted and/or signed
            bool shouldSign = entityNode ["signature"] != null;
            bool shouldEncrypt = entityNode ["encryption"] != null;

            // Signing and/or encrypting entity, if we should
            if (shouldSign && !shouldEncrypt)
                retVal = SignEntity (entityNode, retVal);
            else if (shouldEncrypt && !shouldSign)
                retVal = EncryptEntity (entityNode, retVal);
            else if (shouldEncrypt && shouldSign)
                retVal = SignAndEncryptEntity (entityNode, retVal);

            // Returning entity to caller
            return retVal;
        }

        /*
         * Creates a Multipart MimeEntity, which might be encrypted, signed, or both
         */
        private Multipart CreateMultipart(Node multipartNode)
        {
            // Setting up a return value
            Multipart multipart = new Multipart (multipartNode.Get<string> (Context));

            // Adding headers
            DecorateEntityHeaders (multipart, multipartNode);

            // Setting preamble (additional information), but only if multipart is NOT going to be encrypted later
            multipart.Preamble = multipartNode.GetChildValue<string> ("preamble", Context, null);

            // Looping through all children nodes of multipartNode that are not properties of multipart, assuming they're child entities
            // All headers have Capital letters in them, and preamble and epilogue are settings for the multipart itself
            foreach (var idxChildNode in multipartNode.Children.Where (ix =>
                ix.Name != "preamble" &&
                ix.Name != "epilogue" &&
                ix.Name != "signature" &&
                ix.Name != "encryption" &&
                ix.Name.ToLower () == ix.Name)) {

                // Adding currently iterated part
                multipart.Add (Create (idxChildNode));
            }

            // Setting epilogue (additional information), but only if multipart is NOT going to be encrypted later
            multipart.Epilogue = multipartNode.GetChildValue<string> ("epilogue", Context, null);

            // Returning multipart to caller
            return multipart;
        }

        /*
         * Creates a leaf MimePart
         */
        private MimePart CreateLeafPart(Node mimePartNode)
        {
            // Setting up a return value
            MimePart retVal = new MimePart (ContentType.Parse (mimePartNode.Name + "/" + mimePartNode.Value));

            // Adding headers
            DecorateEntityHeaders (retVal, mimePartNode);

            // Checking which type of content is provided, supported types are [content] or [filename]
            if (mimePartNode ["content"] != null) {

                // Simple inline content
                CreateContentObjectFromContent (mimePartNode ["content"], retVal);
            } else if (mimePartNode ["filename"] != null) {

                // Creating content from filename
                CreateContentObjectFromFilename (mimePartNode ["filename"], retVal);
            } else {

                // Oops, no content!
                throw new LambdaException (
                    "No content found for MIME part, use either [content] or [filename] to supply content",
                    mimePartNode,
                    Context);
            }

            // Returning MimePart to caller
            return retVal;
        }

        /*
         * Only signs the given MimeEntity
         */
        private MultipartSigned SignEntity (
            Node entityNode,
            MimeEntity entity)
        {
            // Retrieving signature node to use for signing operation
            var signatureNode = entityNode ["signature"];

            // Basic syntax checking
            if (signatureNode == null || signatureNode.Children.Count (ix => ix.Name == "email" || ix.Name == "fingerprint") == 0)
                throw new LambdaException (
                    "No [signature] or [email]/[fingerprint]/[password] node given to sign entity with", 
                    entityNode, 
                    Context);

            // Creating our Gnu Privacy Guard context
            using (var ctx = new GnuPrivacyContext ()) {

                // Setting password to retrieve signing certificate from GnuPG context
                ctx.Password = signatureNode.Children.First (ix => ix.Name == "email" || ix.Name == "fingerprint").GetChildValue ("password", Context, "");

                // Creating a MailboxAddress to sign Multipart on behalf of
                SecureMailboxAddress signAdr = new SecureMailboxAddress (
                    "", 
                    signatureNode.GetChildValue ("email", Context, "foo@bar.com"),
                    signatureNode.GetChildValue ("fingerprint", Context, ""));

                // Figuring out signature Digest Algorithm to use for signature, defaulting to Sha1
                DigestAlgorithm algo = DigestAlgorithm.Sha1;
                if (signatureNode ["digest-algorithm"] != null)
                    algo = (DigestAlgorithm)Enum.Parse (typeof (DigestAlgorithm), signatureNode ["digest-algorithm"].Get<string> (Context));

                // Signing content of email and returning to caller
                return MultipartSigned.Create (
                    ctx, 
                    signAdr, 
                    algo, 
                    entity);
            }
        }

        /*
         * Only encrypts the given MimeEntity
         */
        private MultipartEncrypted EncryptEntity (
            Node entityNode,
            MimeEntity entity)
        {
            // Retrieving node that declares encryption settings for us
            var encryptionNode = entityNode ["encryption"];

            // Creating our Gnu Privacy Guard context
            using (var ctx = new GnuPrivacyContext ()) {

                // Retrieving MailboxAddresses to encrypt message for
                var receivers = new List<MailboxAddress> ();
                foreach (var idxReceiverNode in encryptionNode.Children) {

                    // Checking if email address was given, or if fingerprint was given
                    if (idxReceiverNode.Name == "email")
                        receivers.Add (new MailboxAddress ("", idxReceiverNode.Get<string> (Context)));
                    else if (idxReceiverNode.Name == "fingerprint")
                        receivers.Add (new SecureMailboxAddress ("", "foo@bar.com", idxReceiverNode.Get<string> (Context)));
                    else
                        throw new LambdaException (
                            string.Format ("Sorry, don't know how to encrypt for a [{0}] type of node", idxReceiverNode.Name),
                            idxReceiverNode,
                            Context);
                }

                // Encrypting content of email and returning to caller
                var retVal = MultipartEncrypted.Encrypt (
                    ctx, 
                    receivers, 
                    entity);

                // Setting preamble and epilogue AFTER encryption, to give opportunity to give hints to receiver
                retVal.Preamble = entityNode.GetChildValue<string> ("preamble", Context, null);
                retVal.Epilogue = entityNode.GetChildValue<string> ("epilogue", Context, null);

                // Returning encrypted Multipart
                return retVal;
            }
        }

        /*
         * Signs and encrypts the given MimeEntity
         */
        private MultipartEncrypted SignAndEncryptEntity (
            Node entityNode,
            MimeEntity entity)
        {
            // Retrieving [signature] and [encryption] nodes
            var signNode = entityNode ["signature"];
            var encNode = entityNode ["encryption"];

            // Basic syntax checking
            if (signNode["email"] == null && signNode["fingerprint"] == null)
                throw new LambdaException (
                    "No [email] or [fingerprint] supplied to [signature], supply one of these to use for retrieving private key from GnuPG",
                    signNode,
                    Context);
            if (signNode.Children.First (ix => ix.Name == "email" || ix.Name == "fingerprint")["password"] == null)
                throw new LambdaException (
                    "No [password] supplied to [signature], supply password beneath [email] or [fingerprint] node to retrieve private key from GnuPG",
                    signNode,
                    Context);

            // Creating our Gnu Privacy Guard context
            using (var ctx = new GnuPrivacyContext ()) {

                // Setting password to retrieve signing certificate from GnuPG context
                ctx.Password = signNode.Children.First (ix => ix.Name == "email" || ix.Name == "fingerprint").GetChildValue ("password", Context, "");

                // Creating a MailboxAddress to sign Multipart on behalf of
                SecureMailboxAddress sigAdr = new SecureMailboxAddress (
                    "", 
                    signNode.GetChildValue ("email", Context, "foo@bar.com"),
                    signNode.GetChildValue ("fingerprint", Context, ""));

                // Retrieving MailboxAddresses to encrypt message for
                var rec = new List<MailboxAddress> ();
                foreach (var idxRec in encNode.Children) {

                    // Checking if email address was given, or if fingerprint was given
                    if (idxRec.Name == "email")
                        rec.Add (new MailboxAddress ("", idxRec.Get<string> (Context)));
                    else if (idxRec.Name == "fingerprint")
                        rec.Add (new SecureMailboxAddress ("", "foo@bar.com", idxRec.Get<string> (Context)));
                    else
                        throw new LambdaException (
                            string.Format ("Sorry, don't know how to encrypt for a [{0}] type of node, I only understand [email] or [fingerprint]", idxRec.Name),
                            idxRec,
                            Context);
                }

                // Figuring out signature Digest Algorithm to use for signature
                DigestAlgorithm algo = DigestAlgorithm.Sha1;
                if (signNode ["digest-algorithm"] != null)
                    algo = (DigestAlgorithm)Enum.Parse (typeof (DigestAlgorithm), signNode ["digest-algorithm"].Get<string> (Context));

                // Signing and Encrypting content of email
                var retVal = MultipartEncrypted.SignAndEncrypt (
                    ctx, 
                    sigAdr, 
                    algo, 
                    rec, 
                    entity);

                // Setting preamble and epilogue AFTER encryption, to give opportunity to give receiver hints
                retVal.Preamble = entityNode.GetChildValue<string> ("preamble", Context, null);
                retVal.Epilogue = entityNode.GetChildValue<string> ("epilogue", Context, null);

                // Returning encrypted Multipart
                return retVal;
            }
        }

        /*
         * Decorates headers for given MimeEntity
         */
        private void DecorateEntityHeaders (
            MimeEntity entity, 
            Node entityNode)
        {
            // Looping through all child nodes of MimeEntity node, making sure ONLY use those children that
            // have Capital letters in them, since MIME headers all have some sort of Capital letters in them
            foreach (var idxHeader in entityNode.Children.Where (ix => ix.Name.ToLower () != ix.Name)) {

                // Adding currently iterated MIME header to entity
                entity.Headers.Add (idxHeader.Name, idxHeader.Get<string> (Context));
            }
        }

        /*
         * Creates a ContentObject for MimePart from [content] supplied
         */
        private void CreateContentObjectFromContent (
            Node contentNode,
            MimePart entity)
        {
            // Creating stream to hold content
            var stream = new MemoryStream ();

            // This is probably not much point, but for consistency reasons, we still choose to do it
            Streams.Add (stream);

            // Applying content object, but first checking type of object, special handling of blob/byte[]
            if (contentNode.Value is byte[]) {

                // This is byte[] array (blob)
                byte[] value = contentNode.Value as byte [];
                stream.Write (value, 0, value.Length);
                stream.Position = 0;
            } else {

                // Anything BUT byte[]
                // Here we rely on conversion Active Events, making "everything else" serialise as strings
                StreamWriter streamWriter = new StreamWriter (stream);

                // Writing content to streamWrite
                streamWriter.Write (contentNode.Get<string> (Context));
                streamWriter.Flush ();
                stream.Position = 0;
            }

            // Retrieving ContentEncoding to use for reading stream
            ContentEncoding encoding = ContentEncoding.Default;
            if (contentNode ["Content-Encoding"] != null)
                encoding = (ContentEncoding)Enum.Parse (typeof(ContentEncoding), contentNode ["Content-Encoding"].Get<string> (Context));

            // Creating a ContentObject for MimePart from MemoryStream
            entity.ContentObject = new ContentObject (stream, encoding);
        }

        /*
         * Creates a ContentObject for MimePart from some file name given
         */
        private void CreateContentObjectFromFilename (
            Node fileNode,
            MimePart entity)
        {
            // File content object, creating a stream to supply to Content Object
            string fileName = fileNode.Get<string> (Context);

            // Verifying user is authorised to read from file given
            Context.RaiseNative ("p5.io.authorize.read-file", new Node ("", fileName).Add ("args", fileNode));

            // Retrieving ContentEncoding to use for reading stream
            ContentEncoding encoding = ContentEncoding.Default;
            if (fileNode ["Content-Encoding"] != null)
                encoding = (ContentEncoding)Enum.Parse (typeof(ContentEncoding), fileNode ["Content-Encoding"].Get<string> (Context));

            // Defaulting Filename of Content-Disposition, unless explicitly given
            if (entity.ContentDisposition == null) {

                // Defaulting Content-Disposition to; "attachment; filename=whatever.xyz"
                entity.ContentDisposition = new ContentDisposition("attachment");
                entity.ContentDisposition.FileName = Path.GetFileName (fileName);
            }

            // Applying content object, notice that the stream created here, is owned by the caller, hence there is
            // no disposal done
            Stream stream = File.OpenRead (Common.GetBaseFolder (Context) + fileName);
            Streams.Add (stream);
            entity.ContentObject = new ContentObject (stream, encoding);
        }
    }
}

