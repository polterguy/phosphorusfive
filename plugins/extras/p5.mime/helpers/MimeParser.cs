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
    ///     Helper to parse and process a MimeEntity
    /// </summary>
    public class MimeParser
    {
        private bool _attachmentFolderExist;
        private int _noNameAttachments;

        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.mime.helpers.MimeParser"/> class.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="args">Arguments</param>
        /// <param name="entity">MimeEntity to process</param>
        /// <param name="password">Password to retrieve private PGP key from GnuPG</param>
        public MimeParser (
            ApplicationContext context, 
            Node args, 
            MimeEntity entity, 
            string attachmentFolder)
        {
            // Retrieving passwords from args
            if (args ["decryption-keys"] == null) {

                // Caller did not supply decryption keys, adding up the machine server key anyway
                Passwords = new List<GnuPrivacyContext.KeyPasswordMapper>();
                var key = context.RaiseNative ("p5.security.get-marvin-pgp-key").Get<string> (Context);
                string email = "foo@bar.com", fingerprint = "";
                if (key.IndexOf ("@") == -1)
                    fingerprint = key;
                else
                    email = key;
                var password = context.RaiseNative ("p5.security.get-marvin-pgp-key-password").Get<string> (Context);
                var mailboxAdr = string.IsNullOrEmpty (fingerprint) ? new MailboxAddress ("", email) : new SecureMailboxAddress ("", email, fingerprint);
                Passwords.Add (new GnuPrivacyContext.KeyPasswordMapper (mailboxAdr, password));
            } else {

                // Caller supplied decryption keys, enumerating them, and storing to list of key, making sure we DETACH them
                // from args, such that they don't leave method in case of exception
                Passwords = new List<GnuPrivacyContext.KeyPasswordMapper>();
                var keys = args["decryption-keys"].UnTie ();
                foreach (var idxKey in keys.Children) {
                    if (idxKey.Name == "email")
                        Passwords.Add (
                            new GnuPrivacyContext.KeyPasswordMapper (
                                new MailboxAddress ("", idxKey.Get<string> (context)), 
                                idxKey.GetChildValue<string> ("password", context)));
                    else if (idxKey.Name == "fingerprint")
                        Passwords.Add (
                            new GnuPrivacyContext.KeyPasswordMapper (
                                new SecureMailboxAddress ("", "foo@bar.com", idxKey.Get<string> (context)), 
                                idxKey.GetChildValue<string> ("password", context)));
                    else
                        throw new LambdaException (
                            string.Format ("I don't know how to use a '{0}' to lookup a decryption key from GnuPG", idxKey.Name),
                            args,
                            context);
                }
            }

            // Retrieving other arguments
            Context = context;
            Args = args;
            RootEntity = entity;
            if (!string.IsNullOrEmpty (AttachmentFolder) && (attachmentFolder [0] != '/' || attachmentFolder [attachmentFolder.Length - 1] != '/'))
                throw new LambdaException ("Attachment folder was not a valid path", Args, context);
            AttachmentFolder = attachmentFolder;
        }

        /// <summary>
        ///     Returns the Application Context used for the processing operation
        /// </summary>
        /// <value>The Application Context</value>
        public ApplicationContext Context {
            get;
            private set;
        }

        /// <summary>
        ///     Returns the Node used to store the results of the processing operation
        /// </summary>
        /// <value>The result node</value>
        public Node Args {
            get;
            private set;
        }

        /// <summary>
        ///     Returns the root MimeEntity being processed
        /// </summary>
        /// <value>The root entity</value>
        public MimeEntity RootEntity {
            get;
            private set;
        }

        /*
         * Password used to retrieve private PGP key from GnuPG
         */
        private List<GnuPrivacyContext.KeyPasswordMapper> Passwords {
            get;
            set;
        }

        /*
         * Folder where to store attachments found in MIME entity
         */
        private string AttachmentFolder {
            get;
            set;
        }

        /// <summary>
        ///     Processes the RootEntity, and puts results into Args
        /// </summary>
        public void Process ()
        {
            ProcessEntity (RootEntity, Args);
        }

        /*
         * Processes one MimeEntity
         */
        private void ProcessEntity (MimeEntity entity, Node args)
        {
            if (entity is MimePart) {

                // Leaf entity
                ProcessMimePart (entity as MimePart, args);
            } else {

                // Some sort of Multipart, can also be encrypted or signed
                ProcessMultipart (entity as Multipart, args);
            }
        }

        /*
         * Processes a MimePart (leaf entity)
         */
        private void ProcessMimePart (MimePart part, Node args)
        {
            Node entityNode = args.Add (part.ContentType.MediaType, part.ContentType.MediaSubtype).LastChild;
            ProcessHeaders (part, entityNode);

            // Figuring out if this is an inline element or an attachment
            if (TreatAsAttachment (part)) {

                // This is an attachment, and caller supplied an attachment folder to serialise attachments to
                SaveMimePartToDisc (part, entityNode);
            } else {

                // This is not an attachment, or caller did not supply an attachment folder
                ProcessMimePartInline (part, entityNode);
            }
        }

        /*
         * Returns true if part should be treated like an attachment, otherwise false
         */
        private bool TreatAsAttachment (MimePart part)
        {
            if (string.IsNullOrEmpty (AttachmentFolder))
                return false; // We cannot store attachments, unless caller supplies an [attachment-folder] argument
            if (part.IsAttachment) {

                // Still we are not 100% certain, since we do not want to store ALL actual attachments as attachments
                if (part.ContentType.MediaType == "application" && part.ContentType.MediaSubtype == "pgp-encrypted")
                    return false; // No need to save these parts to disc
                return true;
            }
            return false;
        }

        /*
         * Stores attachment to [attachment-folder] supplied by caller, 
         * making sure folder is created if it does not exist
         */
        private void SaveMimePartToDisc (MimePart part, Node entityNode)
        {
            // Makes sure attachment folder exist, and if not, creates it
            EnsureAttachmentFolderExist ();

            // Creating an intelligent filename
            string rootFolder = Common.GetRootFolder (Context).TrimEnd ('/');
            string fileName = "";
            if (part.ContentDisposition == null || string.IsNullOrEmpty (part.ContentDisposition.FileName)) {
                fileName = "noname";
                if (_noNameAttachments != 0)
                    fileName += "-" + _noNameAttachments;
                _noNameAttachments += 1;
            } else {
                fileName = part.ContentDisposition.FileName;
            }

            // Verifying user is authorized to writing to destination file
            Context.RaiseNative ("p5.io.authorize.modify-file", new Node ("", AttachmentFolder + fileName).Add ("args", Args));

            // Saving attachment to disc
            using (FileStream stream = File.Create (rootFolder + AttachmentFolder + fileName)) {
                part.ContentObject.DecodeTo (stream);
            }

            // Making sure we return to caller the entire filename that was used to persist the file
            entityNode.Add ("filename", AttachmentFolder + fileName);
        }

        /*
         * Ensures that AttachmentFolder exist, and if it doesn't, will create it
         */
        private void EnsureAttachmentFolderExist ()
        {
            // Checking if we have already checked, and created attachment folder previously
            if (_attachmentFolderExist)
                return;
            
            // Verifying user is authorized to writing to destination folder
            Context.RaiseNative ("p5.io.authorize.modify-folder", new Node ("", AttachmentFolder).Add ("args", Args));

            // Verifies folder exist, and creates entire path if not
            string baseFolder = Common.GetRootFolder (Context).TrimEnd ('/') + "/";
            string[] folderSplits = AttachmentFolder.Split (new char [] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var idxFolder in folderSplits) {

                baseFolder += idxFolder + "/";
                if (!Directory.Exists (baseFolder))
                    Directory.CreateDirectory (baseFolder);
            }

            // Making sure we do not have to go through this again
            _attachmentFolderExist = true;
        }

        /*
         * Returns a MimePart as inline content to caller
         */
        private void ProcessMimePartInline (MimePart part, Node entityNode)
        {
            // Creating a stream to decode our entity to
            using (MemoryStream stream = new MemoryStream ()) {

                // Decoding content to memory
                part.ContentObject.DecodeTo (stream);

                // Resetting position and setting up a buffer object to hold content
                stream.Position = 0;
                object buffer = null;

                // Checking how to handle content, which can be either binary or text
                if (HandlePartAsText (part)) {

                    // Content is text of some kind, decoding to text through StringReader
                    StreamReader reader = new StreamReader (stream);
                    buffer = reader.ReadToEnd ();
                } else {

                    // Content is binary, simply returning byte[] value raw
                    buffer = stream.ToArray ();
                }

                // Putting content into return node for MimeEntity
                entityNode.Add ("content", buffer);
            }
        }

        /*
         * Returns true if MimePart should be handled as text
         */
        private bool HandlePartAsText (MimePart part)
        {
            switch (part.ContentType.MediaType + "/" + part.ContentType.MediaSubtype) {

                // Some "application" types are actually text, and should be handled as such
                // Making sure we handle the most common text application types as such, to save caller from a conversion roundtrip
                case "application/x-hyperlisp":
                case "application/javascript":
                case "application/x-javascript":
                case "application/ecmascript":
                case "application/json":
                case "application/pgp-signature":
                case "application/pgp-encrypted":
                    return true;
                default:
                    if (part.ContentType.MediaType == "text")
                        return true;
                    return false;
            }
        }

        /*
         * Processes a Multipart, which can be either signed, encrypted, or any other types of Multipart MimeEntity
         */
        private void ProcessMultipart (Multipart multipart, Node args)
        {
            Node entityNode = args.Add (multipart.ContentType.MediaType, multipart.ContentType.MediaSubtype).LastChild;
            ProcessHeaders (multipart, entityNode);

            // Handling preamble
            if (!string.IsNullOrEmpty ((multipart.Preamble ?? "").Trim ()))
                entityNode.Add ("preamble", multipart.Preamble.Trim ());

            if (multipart is MultipartEncrypted && Passwords != null && Passwords.Count > 0) {

                // Encrypted Multipart, might also be signed
                ProcessEncryptedMultipart (multipart as MultipartEncrypted, entityNode);
            } else if (multipart is MultipartSigned) {

                // Only signed Multipart, is NOT encrypted
                ProcessSignedMultipart (multipart as MultipartSigned, entityNode);
            } else {

                // Plain multipart
                foreach (var idxEntity in multipart) {
                    ProcessEntity (idxEntity, entityNode);
                }
            }

            // Handling preamble
            if (!string.IsNullOrEmpty ((multipart.Epilogue ?? "").Trim ()))
                entityNode.Add ("epilogue", multipart.Epilogue.Trim ());
        }

        /*
         * Processes an encrypted Multipart
         */
        private void ProcessEncryptedMultipart (MultipartEncrypted encryptedMultipart, Node entityNode)
        {
            try {
                // Creating cryptographic context
                using (var ctx = new GnuPrivacyContext ()) {

                    // Associating our KeyPasswordMapper collection with GnuPG CryptographyContext
                    ctx.Passwords = Passwords;

                    // Decrypting entity, making sure we retrieve signatures at the same time, if there are any
                    DigitalSignatureCollection signatures;
                    var decryptedMultipart = encryptedMultipart.Decrypt (ctx, out signatures);

                    // Making sure caller gets notified of which private key was used for decrypting encrypted multipart
                    entityNode.Add ("decryption-key", ctx.LastUsedUserId);

                    // Adding signatures
                    ProcessSignatures (entityNode, signatures);

                    // Parsing decrypted result
                    ProcessEntity (decryptedMultipart, entityNode);
                }

            } catch (Exception err) {

                // Couldn't decrypt Multipart, returning raw cipher content!
                entityNode.Add ("processing-message", err.Message);
                foreach (var idxEntity in encryptedMultipart) {
                    ProcessEntity (idxEntity, entityNode);
                }
            }
        }

        /*
         * Processes a signed Multipart
         */
        private void ProcessSignedMultipart (MultipartSigned signedMultipart, Node entityNode)
        {
            // Creating cryptographic context
            using (var ctx = new GnuPrivacyContext ()) {

                // Adding signatures
                ProcessSignatures (entityNode, signedMultipart.Verify (ctx));

                // Looping through all entities in Multipart, processing recursively
                foreach (var idxEntity in signedMultipart) {
                    ProcessEntity (idxEntity, entityNode);
                }
            }
        }

        /*
         * Processes the given signature collection
         */
        private void ProcessSignatures (Node entityNode, DigitalSignatureCollection signatures)
        {
            // Making sure there are any signatures
            if (signatures == null)
                return;

            // Looping through each signature
            foreach (var idxSignature in signatures) {

                // Making sure we return email of PGP key used to sign, and true as value of node if signature is valid
                var signatureNode = entityNode.FindOrCreate ("signature").Add (idxSignature.SignerCertificate.Email, idxSignature.Verify ()).LastChild;

                // Adding fingerprint of PGP key used to sign entity
                signatureNode.Add ("fingerprint", idxSignature.SignerCertificate.Fingerprint);
            }
        }

        /*
         * Processes the MIME headers of the given MimeEntity
         */
        private void ProcessHeaders (MimeEntity entity, Node args)
        {
            // Looping through all headers
            foreach (var idxHeader in entity.Headers) {

                // No need to handle Content-Type, since it's already handled as root entity name/value
                if (idxHeader.Id == HeaderId.ContentType)
                    continue;

                // Adding header as child node of main MimeEntity node
                args.Add (idxHeader.Field, idxHeader.Value);
            }
        }
    }
}

