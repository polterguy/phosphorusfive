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

/// <summary>
///     Main namespace regarding all MIME features in Phosphorus Five
/// </summary>
namespace p5.mime.helpers
{
    /// <summary>
    ///     Helper to create a MimeEntity
    /// </summary>
    public static class CreateMime
    {
        /// <summary>
        ///     Recursively creates a MimeEntity, and returns to caller according to given args
        /// </summary>
        /// <returns>The MimeEntity</returns>
        /// <param name="context">Application Context</param>
        /// <param name="mimeNode">[body] node, containing MIME entity/entities</param>
        /// <param name="streams">List of streams supplied by caller such that caller can dispose after finishing with MimeEntity</param>
        public static MimeEntity CreateEntity (
            ApplicationContext context, 
            Node mimeNode,
            List<Stream> streams)
        {
            // Figuring out which type to create
            switch (mimeNode.Name) {
                case "multipart":
                    return CreateMultipart (context, mimeNode, streams);
                default:
                    return CreateLeafPart (context, mimeNode, streams);
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
            DecorateEntityHeaders (context, retVal, mimeNode);

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
            Multipart multipart = new Multipart ();

            // Adding headers
            DecorateEntityHeaders (context, multipart, mimeNode);

            // Looping through all children nodes that are content recursively, and adding up to Multipart as child MimeParts
            // Logic here is to use everything that is not [signature], [encryption], [preamble], [epilogue] and that has no
            // capital letters in MIME media type, since all MIME headers have capital letters in them, which means that everything
            // that contains a capital letter is treated like a MIME header, and appended into the headers collection of the Multipart
            foreach (var idxChildNode in mimeNode.Children.Where (
                ix => ix.Name != "signature" && 
                ix.Name != "encryption" && 
                ix.Name != "preamble" &&
                ix.Name != "epilogue" &&
                ix.Name.ToLower () == ix.Name)) {

                // Adding currently iterated part
                multipart.Add (CreateEntity (context, idxChildNode, streams));
            }

            // Processes multipart, adding preamble, epilogue, and encrypts and/or signs if requested
            // ProcessEntity will eitehr return the same MimeEntity given (which is "retVal") or it will
            // return a MultipartSigned or MultipartEncrypted, hence casting value to Multipart should
            // be perfectly safe in this context
            multipart = (Multipart)ProcessEntity (context, mimeNode, multipart);

            // Returning TextPart to caller
            return multipart;
        }

        /*
         * Encrypts and signs given MimePart, if we should
         */
        private static MimeEntity ProcessEntity (
            ApplicationContext context, 
            Node mimeNode, 
            MimeEntity mimePart)
        {
            // Return by default what came in.
            // Only if encryption and/or signatures are specified, we return something else
            MimeEntity retVal = mimePart;

            // Checking if we should encrypt MimePart
            if (mimeNode ["encryption"] != null) {

                // Caller requested entity to be encrypted, setting up a new value to return, and checking if we should also sign entity
                Multipart mEnc = null;
                if (mimeNode ["signature"] != null) {

                    // Signing and encrypting
                    mEnc = SignAndEncryptEntity (context, mimeNode ["encryption"], mimeNode ["signature"], retVal);
                } else {

                    // Only encrypting
                    mEnc = EncryptEntity (context, mimeNode ["encryption"], retVal);
                }

                // Retrieving preamble and epilogue, creating our own little "easter egg" if none is given
                mEnc.Preamble = mimeNode.GetChildValue ("preamble", context, "Cryptoxified by Phosphorus Five");
                mEnc.Epilogue = mimeNode.GetChildValue<string> ("epilogue", context);

                // Making sure retVal is update to encrypted, and possibly also signed version
                retVal = mEnc;
            } else if (mimeNode ["signature"] != null) {

                // Caller requested MimePart to be signed, without encryption
                var mSign = SignEntity (context, mimeNode ["signature"], retVal);

                // Retrieving preamble and epilogue, creating our own little "easter egg" if none is given
                mSign.Preamble = mimeNode.GetChildValue ("preamble", context, "Veryfixed by Phosphorus Five");
                mSign.Epilogue = mimeNode.GetChildValue<string> ("epilogue", context);

                // Making sure retVal is update to signed version
                retVal = mSign;
            } else if (retVal is Multipart) {

                // Retrieving preamble and epilogue
                var multi = retVal as Multipart;
                multi.Preamble = mimeNode.GetChildValue<string> ("preamble", context);
                multi.Epilogue = mimeNode.GetChildValue<string> ("epilogue", context);
            }

            // Returning processed MimeEntity to caller
            return retVal;
        }

        /*
         * Signs and encrypts the given MimeEntity
         */
        private static MultipartEncrypted SignAndEncryptEntity (
            ApplicationContext context,
            Node encNode,
            Node signNode,
            MimeEntity entity)
        {
            // Basic syntax checking
            if (signNode.Children.Count != 1 || (signNode.FirstChild.Name != "email" && signNode.FirstChild.Name != "fingerprint"))
                throw new LambdaException (
                    "No [email] or [fingerprint] supplied to [signature], supply one of these, and ONLY one of these",
                    signNode,
                    context);
            if (signNode.FirstChild["password"] == null)
                throw new LambdaException (
                    "No [password] supplied to [signature], supply password beneath [email] or [fingerprint] supplied",
                    signNode,
                    context);

            // Creating our Gnu Privacy Guard context
            using (var ctx = new GnuPrivacyContext ()) {

                // Setting password to retrieve signing certificate from GnuPG context
                ctx.Password = signNode [0].GetChildValue ("password", context, "");

                // Creating a MailboxAddress to sign Multipart on behalf of
                SecureMailboxAddress sigAdr = new SecureMailboxAddress (
                    "", 
                    signNode.GetChildValue ("email", context, "foo@bar.com"), // Even though Sec
                    signNode.GetChildValue ("fingerprint", context, ""));

                // Retrieving MailboxAddresses to encrypt message for
                var rec = new List<MailboxAddress> ();
                foreach (var idxRec in encNode.Children) {

                    // Checking if email address was given, or if fingerprint was given
                    if (idxRec.Name == "email")
                        rec.Add (new MailboxAddress ("", idxRec.Get<string> (context)));
                    else if (idxRec.Name == "fingerprint")
                        rec.Add (new SecureMailboxAddress ("", "foo@bar.com", idxRec.Get<string> (context)));
                    else
                        throw new LambdaException (
                            string.Format ("Sorry, don't know how to encrypt for a [{0}] type of node, I only understand [email] or [fingerprint]", idxRec.Name),
                            idxRec,
                            context);
                }

                // Figuring out signature Digest Algorithm to use for signature
                DigestAlgorithm algo = DigestAlgorithm.Sha1;
                if (signNode.FirstChild ["digest-algorithm"] != null)
                    algo = (DigestAlgorithm)Enum.Parse (typeof (DigestAlgorithm), signNode.FirstChild ["digest-algorithm"].Get<string> (context));

                // Signing and Encrypting content of email
                return MultipartEncrypted.SignAndEncrypt (
                    ctx, 
                    sigAdr, 
                    algo, 
                    rec, 
                    entity);
            }
        }

        /*
         * Only encrypts the given MimeEntity
         */
        private static MultipartEncrypted EncryptEntity (
            ApplicationContext context,
            Node encNode,
            MimeEntity entity)
        {
            // Creating our Gnu Privacy Guard context
            using (var ctx = new GnuPrivacyContext ()) {

                // Retrieving MailboxAddresses to encrypt message for
                var rec = new List<MailboxAddress> ();
                foreach (var idxRec in encNode.Children) {

                    // Checking if email address was given, or if fingerprint was given
                    if (idxRec.Name == "email")
                        rec.Add (new MailboxAddress ("", idxRec.Get<string> (context)));
                    else if (idxRec.Name == "fingerprint")
                        rec.Add (new SecureMailboxAddress ("", "foo@bar.com", idxRec.Get<string> (context)));
                    else
                        throw new LambdaException (
                            string.Format ("Sorry, don't know how to encrypt for a [{0}] type of node", idxRec.Name),
                            idxRec,
                            context);
                }

                // Encrypting content of email and returning to caller
                return MultipartEncrypted.Encrypt (
                    ctx, 
                    rec, 
                    entity);
            }
        }

        /*
         * Only signs the given MimeEntity
         */
        private static MultipartSigned SignEntity (
            ApplicationContext context,
            Node signNode,
            MimeEntity entity)
        {
            // Basic syntax checking
            if (signNode.Children.Count != 1 || (signNode.FirstChild.Name != "email" && signNode.FirstChild.Name != "fingerprint"))
                throw new LambdaException (
                    "No [email] or [fingerprint] supplied to [signature], supply one of these, and ONLY one of these",
                    signNode,
                    context);
            if (signNode.FirstChild["password"] == null)
                throw new LambdaException (
                    "No [password] supplied to [signature], supply password beneath [email] or [fingerprint] supplied",
                    signNode,
                    context);

            // Creating our Gnu Privacy Guard context
            using (var ctx = new GnuPrivacyContext ()) {

                // Setting password to retrieve signing certificate from GnuPG context
                ctx.Password = signNode[0].GetChildValue ("password", context, "");

                // Creating a MailboxAddress to sign Multipart on behalf of
                SecureMailboxAddress signAdr = new SecureMailboxAddress (
                    "", 
                    signNode.GetChildValue ("email", context, "foo@bar.com"),
                    signNode.GetChildValue ("fingerprint", context, ""));

                // Figuring out signature Digest Algorithm to use for signature, defaulting to Sha1
                DigestAlgorithm algo = DigestAlgorithm.Sha1;
                if (signNode.FirstChild ["digest-algorithm"] != null)
                    algo = (DigestAlgorithm)Enum.Parse (typeof (DigestAlgorithm), signNode.FirstChild ["digest-algorithm"].Get<string> (context));

                // Signing content of email and returning to caller
                return MultipartSigned.Create (
                    ctx, 
                    signAdr, 
                    algo, 
                    entity);
            }
        }

        /*
         * Decorates headers for given MimeEntity
         */
        private static void DecorateEntityHeaders (
            ApplicationContext context, 
            MimeEntity entity, 
            Node entityNode)
        {
            // Getting the Content-Type correct according to arguments supplied
            ContentType type = ContentType.Parse (entityNode.Name + "/" + entityNode.Value);
            entity.ContentType.MediaSubtype = type.MediaSubtype;

            // Adding all supplied parameters, if any, for entity's Content-Type
            foreach (var idxArg in type.Parameters) {
                entity.ContentType.Parameters.Add (idxArg);
            }

            // Looping through all child nodes of MimeEntity node, making sure ONLY use those children that
            // have Capital letters in them
            foreach (var idxHeader in entityNode.Children.Where (ix => ix.Name.ToLower () != ix.Name && ix.Name != "Content-Type")) {

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
            // Creating stream to hold content
            var stream = new MemoryStream ();

            // This is probably not much point, but for consistency reasons, we still choose to do it
            streams.Add (stream);

            // Applying content object, but first checking type of object, special handling of blob/byte[]
            if (contentNode.Value is byte[]) {

                // This is byte[] array (blob)
                byte[] value = contentNode.Value as byte [];
                stream.Write (value, 0, value.Length);
            } else {

                // Anything BUT byte[]
                // Here we rely on conversion Active Events, making "everything else" serialise as strings
                StreamWriter streamWriter = new StreamWriter (stream);

                // Writing content to streamWrite
                streamWriter.Write (contentNode.Get<string> (context));
                streamWriter.Flush ();
                streamWriter.BaseStream.Position = 0;
            }

            // Retrieving ContentEncoding to use for reading stream
            ContentEncoding encoding = ContentEncoding.Default;
            if (contentNode ["Content-Encoding"] != null)
                encoding = (ContentEncoding)Enum.Parse (typeof(ContentEncoding), contentNode ["Content-Encoding"].Get<string> (context));

            // Creating a ContentObject for MimePart from MemoryStream in StreamWriter
            entity.ContentObject = new ContentObject (stream, encoding);
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

