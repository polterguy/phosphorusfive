/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.IO;
using System.Collections.Generic;
using p5.core;
using p5.exp.exceptions;
using MimeKit;
using MimeKit.Cryptography;

namespace p5.mime.helpers
{
    /// <summary>
    ///     Helper to parse and process a MimeEntity.
    /// </summary>
    public class MimeParser
    {
        private ApplicationContext _context;
        private Node _args;
        private MimeEntity _rootEntity;
        private int _noNameAttachments;
        private string _attachmentFolder;
        private bool _addPrefixToAttachmentPath;
        private List<GnuPrivacyContext.KeyPasswordMapper> _passwords;

        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.mime.helpers.MimeParser"/> class.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="args">Arguments</param>
        /// <param name="entity">MimeEntity to process</param>
        /// <param name="attachmentFolder">path to attachment folder, where to save any attachments.</param>
        public MimeParser (
            ApplicationContext context,
            Node args,
            MimeEntity entity,
            string attachmentFolder,
            bool addPrefixToAttachmentPath = true)
        {
            // Retrieving passwords from args.
            if (args ["decrypt"] != null) {

                // Caller supplied explicit decryption keys, making sure we add them up as keys to use for decrypting MIME entities.
                AddExplicitDecryptionKeys (context, args);
            }

            // Retrieving other arguments.
            _context = context;
            _args = args;
            _rootEntity = entity;
            if (!string.IsNullOrEmpty (attachmentFolder))
                _attachmentFolder = context.RaiseEvent (".p5.io.unroll-path", new Node ("", attachmentFolder)).Get<string> (context, null);
            _addPrefixToAttachmentPath = addPrefixToAttachmentPath;
        }

        /// <summary>
        ///     Processes the RootEntity, and puts results into Args.
        /// </summary>
        public void Process ()
        {
            ProcessEntity (_rootEntity, _args);
        }

        /*
         * Adds up explicitly given decryption keys and passwords to retrieve key from GnuPG.
         */
        private void AddExplicitDecryptionKeys (ApplicationContext context, Node args)
        {
            // Caller supplied decryption keys, enumerating them, and storing to list of key, making sure we DETACH them
            // from args, such that they don't leave method in case of exception - (they probably contain passwords in plain form).
            _passwords = new List<GnuPrivacyContext.KeyPasswordMapper> ();
            var keys = args ["decrypt"].UnTie ();

            // Looping through each decryption key specified by caller.
            foreach (var idxKey in keys.Children) {
                if (idxKey.Name == "email") {

                    // Email lookup.
                    _passwords.Add (new GnuPrivacyContext.KeyPasswordMapper (new MailboxAddress ("", idxKey.Get<string> (context)), idxKey.GetChildValue<string> ("password", context)));

                } else if (idxKey.Name == "fingerprint") {

                    // Fingerprint lookup.
                    _passwords.Add (new GnuPrivacyContext.KeyPasswordMapper (new SecureMailboxAddress ("", "foo@bar.com", idxKey.Get<string> (context)), idxKey.GetChildValue<string> ("password", context)));

                } else {

                    // Oops ...
                    throw new LambdaException (string.Format ("I don't know how to use a '{0}' to lookup a decryption key from GnuPG", idxKey.Name), args, context);
                }
            }
        }

        /*
         * Processes one MimeEntity.
         */
        private void ProcessEntity (MimeEntity entity, Node args)
        {
            if (entity is MimePart) {

                // Leaf entity.
                ProcessLeafPart (entity as MimePart, args);

            } else if (entity is Multipart) {

                // Some sort of Multipart, can also be encrypted or signed.
                ProcessMultipart (entity as Multipart, args);

            } else if (entity is MessagePart) {

                // TODO: Implement...!!
            }
        }

        /*
         * Processes a MimePart (leaf entity).
         */
        private void ProcessLeafPart (MimePart part, Node args)
        {
            // Verifying part actually has content, before trying to de-serialize it.
            if (part.ContentObject == null)
                return;

            Node entityNode = args.Add (part.ContentType.MediaType, part.ContentType.MediaSubtype).LastChild;
            ProcessHeaders (part, entityNode);

            // Figuring out if this is an inline element or an attachment.
            if (TreatAsAttachment (part)) {

                // This is an attachment, and caller supplied an attachment folder to serialise attachments to.
                SaveMimePartToDisc (part, entityNode);

            } else {

                // This is not an attachment, or caller did not supply an attachment folder.
                ProcessMimePartInline (part, entityNode);
            }
        }

        /*
         * Returns true if part should be treated like an attachment, otherwise false.
         */
        private bool TreatAsAttachment (MimePart part)
        {
            if (string.IsNullOrEmpty (_attachmentFolder))
                return false; // We cannot store attachments, unless caller supplies an [attachment-folder] argument.

            if (part.IsAttachment) {

                // Still we are not 100% certain, since we do not want to store ALL actual attachments as attachments.
                if (part.ContentType.MediaType == "application" && part.ContentType.MediaSubtype == "pgp-encrypted")
                    return false; // No need to save these parts to disc.

                return true;
            } else if (!string.IsNullOrEmpty (part.FileName) && part.ContentDisposition != null && part.ContentDisposition.Disposition == "form-data") {

                // Form data file attachment.
                return true;
            }
            return false;
        }

        /*
         * Stores attachment to [attachment-folder] supplied by caller.
         */
        private void SaveMimePartToDisc (MimePart part, Node entityNode)
        {
            // Creating an intelligent filename.
            string rootFolder = Common.GetRootFolder (_context).TrimEnd ('/');
            string fileName = "";
            if (part.ContentDisposition == null || string.IsNullOrEmpty (part.ContentDisposition.FileName)) {

                // No explicit filename given, defaulting to "noname".
                fileName = "noname";
                if (_noNameAttachments != 0)
                    fileName += "-" + _noNameAttachments;
                _noNameAttachments += 1;

            } else {

                // Explicit filename given.
                fileName = part.ContentDisposition.FileName;
            }

            // Checking if we should add a prefix to attachment path.
            var physical_full_filename = "";
            if (_addPrefixToAttachmentPath) {

                // Making sure we create a unique file "prefix", such that files with similar names, doesn't overwrite each other.
                var unique = Guid.NewGuid().ToString().Replace("-", "") + "-";
                physical_full_filename = _attachmentFolder + unique + fileName;

                // Making sure we return to caller the entire filename that was used to persist the file.
                // In addition, we make sure we also return the prefix, such that caller can actually find file on disc.
                entityNode.Add ("filename", fileName).LastChild.Add ("prefix", unique).Add ("folder", _attachmentFolder);

            } else {

                // No prefix should be used.
                physical_full_filename = _attachmentFolder + fileName;

                // Making sure we return to caller the entire filename that was used to persist the file.
                // In addition, we make sure we also return the prefix, such that caller can actually find file on disc.
                entityNode.Add ("filename", fileName).LastChild.Add ("folder", _attachmentFolder);
            }

            // Verifying user is authorized to writing to destination file.
            _context.RaiseEvent (".p5.io.authorize.modify-file", new Node ("", physical_full_filename).Add ("args", _args));

            // Saving attachment to disc.
            using (FileStream stream = File.Create (rootFolder + physical_full_filename)) {
                part.ContentObject.DecodeTo (stream);
            }
        }

        /*
         * Returns a MimePart as inline content to caller.
         */
        private void ProcessMimePartInline (MimePart part, Node entityNode)
        {
            var txtPart = part as TextPart;
            if (txtPart != null) {

                // Part is text part.
                entityNode.Add("content", txtPart.Text);

            } else {

				// Creating a stream to decode our entity to.
				using (MemoryStream stream = new MemoryStream()) {

					// Decoding content to memory.
					part.ContentObject.DecodeTo(stream);

					// Resetting position and setting up a buffer object to hold content.
					stream.Position = 0;

					// Putting content into return node for MimeEntity.
					entityNode.Add("content", stream.ToArray ());
				}
			}
        }

        /*
         * Returns true if MimePart should be handled as text.
         */
        private bool HandlePartAsText (MimePart part)
        {
            if (part is TextPart)
                return true;
            return false;
        }

        /*
         * Processes a Multipart, which can be either signed, encrypted, or any other types of Multipart MimeEntity.
         */
        private void ProcessMultipart (Multipart multipart, Node args)
        {
            Node entityNode = args.Add (multipart.ContentType.MediaType, multipart.ContentType.MediaSubtype).LastChild;
            ProcessHeaders (multipart, entityNode);

            // Handling preamble.
            if (!string.IsNullOrEmpty ((multipart.Preamble ?? "").Trim ()))
                entityNode.Add ("preamble", multipart.Preamble.Trim ());

            if (multipart is MultipartEncrypted) {

                // Encrypted Multipart, might also be signed.
                ProcessEncryptedMultipart (multipart as MultipartEncrypted, entityNode);

            } else if (multipart is MultipartSigned) {

                // Only signed Multipart, is NOT encrypted.
                ProcessSignedMultipart (multipart as MultipartSigned, entityNode);

            } else {

                // Plain multipart.
                foreach (var idxEntity in multipart) {
                    ProcessEntity (idxEntity, entityNode);
                }
            }

            // Handling preamble.
            if (!string.IsNullOrEmpty ((multipart.Epilogue ?? "").Trim ()))
                entityNode.Add ("epilogue", multipart.Epilogue.Trim ());
        }

        /*
         * Processes an encrypted Multipart.
         */
        private void ProcessEncryptedMultipart (MultipartEncrypted encryptedMultipart, Node entityNode)
        {
            try {
                // Creating cryptographic context.
                using (var ctx = new GnuPrivacyContext (false)) {

                    // Associating our KeyPasswordMapper collection with GnuPG CryptographyContext.
                    ctx.Passwords = _passwords;

                    // Decrypting entity, making sure we retrieve signatures at the same time, if there are any.
                    DigitalSignatureCollection signatures;
                    var decryptedMultipart = encryptedMultipart.Decrypt (ctx, out signatures);

                    // Making sure caller gets notified of which private key was used for decrypting encrypted multipart.
                    entityNode.Add ("decryption-key", ctx.LastUsedUserId);

                    // Adding signatures.
                    ProcessSignatures (entityNode, signatures);

                    // Parsing decrypted result.
                    ProcessEntity (decryptedMultipart, entityNode);
                }
            } catch (Exception err) {

                // Couldn't decrypt Multipart, returning raw encrypted content.
                entityNode.Add ("processing-message", err.Message);
                foreach (var idxEntity in encryptedMultipart) {
                    ProcessEntity (idxEntity, entityNode);
                }
            }
        }

        /*
         * Processes a signed Multipart.
         */
        private void ProcessSignedMultipart (MultipartSigned signedMultipart, Node entityNode)
        {
            // Creating cryptographic context.
            using (var ctx = new GnuPrivacyContext (false)) {

                // Adding signatures.
                ProcessSignatures (entityNode, signedMultipart.Verify (ctx));

                // Looping through all entities in Multipart, processing recursively.
                foreach (var idxEntity in signedMultipart) {
                    ProcessEntity (idxEntity, entityNode);
                }
            }
        }

        /*
         * Processes the given signature collection.
         */
        private void ProcessSignatures (Node entityNode, DigitalSignatureCollection signatures)
        {
            // Making sure there are any signatures.
            if (signatures == null)
                return;

            // Looping through each signature.
            foreach (var idxSignature in signatures) {

                // We cannot verify signatures unless we have the public PGP key in GnuPG database, hence the try thingie ...
                try {

                    // Making sure we return email of PGP key used to sign, and true as value of node if signature is valid.
                    var signatureNode = entityNode.FindOrInsert ("signature").Add (idxSignature.SignerCertificate.Email, idxSignature.Verify ()).LastChild;

                    // Adding fingerprint of PGP key used to sign entity.
                    signatureNode.Add ("fingerprint", idxSignature.SignerCertificate.Fingerprint);

                } catch {

                    // Inserting unknown fingerprint and signature email.
                    var signatureNode = entityNode.FindOrInsert ("signature").Add ("unknown", false).LastChild;
                    signatureNode.Add ("fingerprint", "unknown");
                }
            }
        }

        /*
         * Processes the MIME headers of the given MimeEntity.
         */
        private void ProcessHeaders (MimeEntity entity, Node args)
        {
            // Looping through all headers.
            foreach (var idxHeader in entity.Headers) {

                // No need to handle Content-Type, since it's already handled as root entity name/value.
                if (idxHeader.Id == HeaderId.ContentType)
                    continue;

                // Adding header as child node of main MimeEntity node.
                args.Add (idxHeader.Field, idxHeader.Value);
            }
        }
    }
}

