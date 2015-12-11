/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MimeKit;
using MailKit;
using MimeKit.Cryptography;
using p5.exp;
using p5.core;
using p5.mail.helpers;
using p5.exp.exceptions;

namespace p5.mail.mime
{
    /// <summary>
    ///     Helper to create a MimeEntity
    /// </summary>
    public static class CreateMime
    {
        /*
         * Recursively creates a MimeEntity and returns to caller
         */
        public static MimeEntity CreateEntity (
            ApplicationContext context, 
            Node mimeNode,
            List<Stream> streams)
        {
            // Figuring out which type to create
            switch (mimeNode.Name) {
                case "audio":
                case "image":
                case "message":
                case "application":
                case "text":
                case "video":
                    return CreateLeafPart (context, mimeNode, streams);
                case "multipart":
                    return CreateMultipart (context, mimeNode, streams);
                default:
                    throw new LambdaException (
                        string.Format ("Sorry, I do not know how to create a '{0}' type of MIME entity", mimeNode.Name),
                        mimeNode,
                        context);
            }
        }

        /*
         * Creates a text part and returns to caller
         */
        private static MimeEntity CreateLeafPart (
            ApplicationContext context, 
            Node mimeNode,
            List<Stream> streams)
        {
            // Setting up a return value
            MimePart retVal = new MimePart (ContentType.Parse (mimeNode.Name + "/" + mimeNode.Value));

            // Adding headers
            DecorateEntityHeaders (context, retVal, mimeNode, "content", "filename");

            // Checking which type of content is provided, [content] or [filename]
            if (mimeNode ["content"] != null) {

                // Simple inline content
                CreateContentObjectFromContent (context, mimeNode ["content"], streams, retVal);
            } else if (mimeNode ["filename"] != null) {

                // Creating content from filename
                CreateContentObjectFromFilename (context, mimeNode ["filename"], streams, retVal);
            } else {

                // Oops, no content!
                throw new LambdaException (
                    "No content found for MIME part, use either [content] or [filename] to supply content",
                    mimeNode,
                    context);
            }

            // Processing MimePart, possibly encrypting and signing, and returning the results to caller
            return ProcessEntity (context, mimeNode, retVal);
        }
           
        /*
         * Creates a multipart and returns to caller
         */
        private static Multipart CreateMultipart (
            ApplicationContext context, 
            Node mimeNode,
            List<Stream> streams)
        {
            // Setting up a return value
            Multipart retVal = new Multipart ();

            // Adding headers
            DecorateEntityHeaders (
                context, 
                retVal, 
                mimeNode, 
                "text", "multipart", "application", "video", "audio", "image", "message", "signature", "encryption", "preamble", "epilogue");

            // Looping through all children nodes that are content recursively, and adding up to Multipart
            foreach (var idxChildNode in mimeNode.Children.Where (
                ix => 
                ix.Name == "text"  || 
                ix.Name == "multipart" || 
                ix.Name == "application" ||
                ix.Name == "video" || 
                ix.Name == "audio" || 
                ix.Name == "image" || 
                ix.Name == "message")) {

                // Adding currently iterated part
                retVal.Add (CreateEntity (context, idxChildNode, streams));
            }

            // Processes multipart, adding preamble, epilogue, and encrypts and/or signs if requested
            retVal = (Multipart)ProcessEntity (context, mimeNode, retVal);

            // Returning TextPart to caller
            return retVal;
        }

        /*
         * Encrypts and signs given MimePart, if we should
         */
        static MimeEntity ProcessEntity (
            ApplicationContext context, 
            Node mimeNode, 
            MimeEntity mimePart)
        {
            // Return value, by default we return what came in, 
            // only if encryption and/or signatures are specified, we return something else
            MimeEntity retVal = mimePart;

            // Checking if we should encrypt MimePart
            if (mimeNode ["encryption"] != null) {

                // Caller requested entity to be encrypted, setting up a new value to return, and checking if we should also sign entity
                Multipart multipartEncrypted = null;
                if (mimeNode ["signature"] != null) {

                    // Signing and encrypting
                    multipartEncrypted = SignAndEncryptEntity (context, mimeNode ["encryption"], mimeNode ["signature"], retVal);
                } else {

                    // Only encrypting
                    multipartEncrypted = EncryptEntity (context, mimeNode ["encryption"], retVal);
                }

                // Retrieving preamble and epilogue, creating our own little "easter egg" if none is given
                multipartEncrypted.Preamble = mimeNode.GetChildValue ("preamble", context, "Cryptoxified by Phosphorus Five");
                multipartEncrypted.Epilogue = mimeNode.GetChildValue<string> ("epilogue", context);

                // Making sure retVal is update to encrypted, and possibly also signed version
                retVal = multipartEncrypted;
            } else if (mimeNode ["signature"] != null) {

                // Caller requested MimePart to be signed, without encryption
                var multipartSigned = SignEntity (context, mimeNode ["signature"], retVal);

                // Retrieving preamble and epilogue, creating our own little "easter egg" if none is given
                multipartSigned.Preamble = mimeNode.GetChildValue ("preamble", context, "Veryfixed by Phosphorus Five");
                multipartSigned.Epilogue = mimeNode.GetChildValue<string> ("epilogue", context);

                // Making sure retVal is update to signed version
                retVal = multipartSigned;
            } else if (retVal is Multipart) {

                // Retrieving preamble and epilogue
                var multipart = retVal as Multipart;
                multipart.Preamble = mimeNode.GetChildValue<string> ("preamble", context);
                multipart.Epilogue = mimeNode.GetChildValue<string> ("epilogue", context);
            }

            // Returning processed MimeEntity to caller
            return retVal;
        }

        /*
         * Signs and encrypts the given MimeEntity
         */
        private static MultipartEncrypted SignAndEncryptEntity (
            ApplicationContext context,
            Node encryptionNode,
            Node signatureNode,
            MimeEntity mimeEntity)
        {
            // Creating our Gnu Privacy Guard context
            using (var ctx = new GnuPrivacyContext ()) {

                // Setting password to retrieve signing certificate from GnuPG context
                ctx.Password = signatureNode.GetChildValue ("password", context, "");

                // Creating a MailboxAddress to sign Multipart on behalf of
                SecureMailboxAddress signerAdr = new SecureMailboxAddress (
                    "", 
                    signatureNode.GetChildValue ("email", context, "foo@bar.com"),
                    signatureNode.GetChildValue ("fingerprint", context, ""));

                // Retrieving MailboxAddresses to encrypt message for
                var recipients = new List<MailboxAddress> ();
                foreach (var idxRecipientNode in encryptionNode.Children) {

                    // Checking if email address was given, or if fingerprint was given
                    if (idxRecipientNode.Name == "email")
                        recipients.Add (new MailboxAddress ("", idxRecipientNode.Get<string> (context)));
                    else if (idxRecipientNode.Name == "fingerprint")
                        recipients.Add (new SecureMailboxAddress ("", "foo@bar.com", idxRecipientNode.Get<string> (context)));
                    else
                        throw new LambdaException (
                            string.Format ("Sorry, don't know how to encrypt for a [{0}] type of node", idxRecipientNode.Name),
                            idxRecipientNode,
                            context);
                }

                // Figuring out signature Digest Algorithm to use for signature
                DigestAlgorithm algo = DigestAlgorithm.Sha1;
                if (signatureNode ["digest-algorithm"] != null)
                    algo = (DigestAlgorithm)Enum.Parse (typeof (DigestAlgorithm), signatureNode ["digest-algorithm"].Get<string> (context));

                // Signing and Encrypting content of email
                var multipartSignedAndEncrypted = MultipartEncrypted.SignAndEncrypt (
                    ctx, 
                    signerAdr, 
                    algo, 
                    recipients, 
                    mimeEntity);

                // Returning encrypted Multipart to caller
                return multipartSignedAndEncrypted;
            }
        }

        /*
         * Only encrypts the given MimeEntity
         */
        private static MultipartEncrypted EncryptEntity (
            ApplicationContext context,
            Node encryptionNode,
            MimeEntity mimeEntity)
        {
            // Creating our Gnu Privacy Guard context
            using (var ctx = new GnuPrivacyContext ()) {

                // Retrieving MailboxAddresses to encrypt message for
                var recipients = new List<MailboxAddress> ();
                foreach (var idxRecipientNode in encryptionNode.Children) {

                    // Checking if email address was given, or if fingerprint was given
                    if (idxRecipientNode.Name == "email")
                        recipients.Add (new MailboxAddress ("", idxRecipientNode.Get<string> (context)));
                    else if (idxRecipientNode.Name == "fingerprint")
                        recipients.Add (new SecureMailboxAddress ("", "foo@bar.com", idxRecipientNode.Get<string> (context)));
                    else
                        throw new LambdaException (
                            string.Format ("Sorry, don't know how to encrypt for a [{0}] type of node", idxRecipientNode.Name),
                            idxRecipientNode,
                            context);
                }

                // Encrypting content of email
                var multipartEncrypted = MultipartEncrypted.Encrypt (
                    ctx, 
                    recipients, 
                    mimeEntity);

                // Returning encrypted Multipart to caller
                return multipartEncrypted;
            }
        }

        /*
         * Only signs the given MimeEntity
         */
        private static MultipartSigned SignEntity (
            ApplicationContext context,
            Node signatureNode,
            MimeEntity mimeEntity)
        {
            // Creating our Gnu Privacy Guard context
            using (var ctx = new GnuPrivacyContext ()) {

                // Setting password to retrieve signing certificate from GnuPG context
                ctx.Password = signatureNode.GetChildValue ("password", context, "");

                // Creating a MailboxAddress to sign Multipart on behalf of
                SecureMailboxAddress signerAdr = new SecureMailboxAddress (
                    "", 
                    signatureNode.GetChildValue ("email", context, "foo@bar.com"),
                    signatureNode.GetChildValue ("fingerprint", context, ""));

                // Figuring out signature Digest Algorithm to use for signature, defaulting to Sha1
                DigestAlgorithm algo = DigestAlgorithm.Sha1;
                if (signatureNode ["digest-algorithm"] != null)
                    algo = (DigestAlgorithm)Enum.Parse (typeof (DigestAlgorithm), signatureNode ["digest-algorithm"].Get<string> (context));

                // Signing content of email
                var multipartSigned = MultipartSigned.Create (
                    ctx, 
                    signerAdr, 
                    algo, 
                    mimeEntity);

                // Returning signed Multipart to caller
                return multipartSigned;
            }
        }

        /*
         * Decorates headers for given MimeEntity
         */
        private static void DecorateEntityHeaders (
            ApplicationContext context, 
            MimeEntity entity, 
            Node mimeNode,
            params string[] exclude)
        {
            // Getting the Content-Type correct according to arguments supplied
            ContentType type = ContentType.Parse (mimeNode.Name + "/" + mimeNode.Value);
            entity.ContentType.MediaSubtype = type.MediaSubtype;

            // Adding all supplied parameters, if any, for entity's Content-Type
            foreach (var idxArg in type.Parameters) {
                entity.ContentType.Parameters.Add (idxArg);
            }

            // Looping through all child nodes of MimeEntity node, excluding the nodes with the given "exclude" names
            // and adding up as Headers of entity
            List<string> excludeList = new List<string> (exclude);
            foreach (var idxHeader in mimeNode.Children.Where (ix => excludeList.IndexOf (ix.Name) == -1 && ix.Name != "")) {

                // Adding currently iterated MIME header to entity
                entity.Headers.Add (idxHeader.Name, idxHeader.Get<string> (context));
            }
        }

        /*
         * Creates a ContentObject for MimePart from some file name given
         */
        private static void CreateContentObjectFromContent (
            ApplicationContext context,
            Node contentNode,
            List<Stream> streams,
            MimePart entity)
        {
            // Applying content object
            StreamWriter streamWriter = new StreamWriter(new MemoryStream ());

            // This is probably not much point, but for consistency, we do it anyway!
            streams.Add (streamWriter.BaseStream);

            // Writing content to streamWrite
            streamWriter.Write (contentNode.Value);
            streamWriter.Flush ();
            streamWriter.BaseStream.Position = 0;

            // Retrieving ContentEncoding to use for reading stream
            ContentEncoding encoding = ContentEncoding.Default;
            if (contentNode ["Content-Encoding"] != null)
                encoding = (ContentEncoding)Enum.Parse (typeof(ContentEncoding), contentNode ["Content-Encoding"].Get<string> (context));

            // Creating a ContentObject for MimePart from MemoryStream in StreamWriter
            entity.ContentObject = new ContentObject (streamWriter.BaseStream, encoding);
        }

        /*
         * Creates a ContentObject for MimePart from some file name given
         */
        private static void CreateContentObjectFromFilename (
            ApplicationContext context,
            Node fileNode,
            List<Stream> streams,
            MimePart entity)
        {
            // File content object, creating a stream to supply to Content Object
            string fileName = fileNode.Get<string> (context);

            // Verifying user is authorised to read from file given
            context.RaiseNative ("p5.io.authorize.read-file", new Node ("p5.io.authorize.read-file", fileName).Add ("args", fileNode));

            // Retrieving ContentEncoding to use for reading stream
            ContentEncoding encoding = ContentEncoding.Default;
            if (fileNode ["Content-Encoding"] != null)
                encoding = (ContentEncoding)Enum.Parse (typeof(ContentEncoding), fileNode ["Content-Encoding"].Get<string> (context));

            // Defaulting Filename of Content-Disposition, unless explicitly given
            if (entity.ContentDisposition == null) {

                // Defaulting Content-Disposition to; "attachment; filename=whatever.xyz"
                entity.ContentDisposition = new ContentDisposition("attachment");
                entity.ContentDisposition.FileName = Path.GetFileName (fileName);
            }

            // Applying content object, notice that the stream created here, is owned by the caller, hence there is
            // no disposal done
            Stream stream = File.OpenRead (Common.GetBaseFolder (context) + fileName);
            streams.Add (stream);
            entity.ContentObject = new ContentObject (stream, encoding);
        }
    }
}

