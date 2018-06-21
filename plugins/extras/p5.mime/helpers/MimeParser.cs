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
using p5.exp;
using p5.core;
using MimeKit;
using MimeKit.IO;
using MimeKit.Cryptography;

namespace p5.mime.helpers
{
    /// <summary>
    ///     Helper to parse and process a MimeEntity.
    /// </summary>
    public class MimeParser
    {
        ApplicationContext _context;
        Node _args;
        MimeEntity _rootEntity;
        int _noNameAttachments;
        string _attachmentFolder;
        bool _addPrefixToAttachmentPath;
        string _password;
        string _fingerprint;

        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.mime.helpers.MimeParser"/> class.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="args">Arguments</param>
        /// <param name="entity">MimeEntity to process</param>
        /// <param name="attachmentFolder">path to attachment folder, where to save any attachments.</param>
        public MimeParser (ApplicationContext context, Node args, MimeEntity entity, string attachmentFolder, bool addPrefixToAttachmentPath = true)
        {
            // Decorating instance with arguments.
            _context = context;
            _args = args;
            _rootEntity = entity;

            // Checking if caller supplied an attachment folder, at which point we unroll it.
            if (!string.IsNullOrEmpty (attachmentFolder))
                _attachmentFolder = context.RaiseEvent (".p5.io.unroll-path", new Node ("", attachmentFolder)).Get<string> (context, null);

            // Sotring whether or not we should add prefix to attachments to randomise file names.
            _addPrefixToAttachmentPath = addPrefixToAttachmentPath;

            // Checking if caller wants to decrypt any encrypted content.
            if (args ["decrypt"] != null) {

                // Checking to see if an explicit [password] was supplied.
                if (args ["decrypt"] ["password"] != null) {

                    // [password] argument was supplied.
                    _password = args ["decrypt"].GetExChildValue<string> ("password", context, null);
                    _fingerprint = args ["decrypt"].GetExChildValue<string> ("fingerprint", context, null);

                } else {

                    // No password was supplied, assuming caller wants to use password from web.config.
                    _password = _context.RaiseEvent (".p5.config.get", new Node (".p5.config.get", "gpg-server-keypair-password")) [0]?.Get<string> (_context) ?? null;
                    _fingerprint = _context.RaiseEvent ("p5.auth.pgp.get-fingerprint").Get<string> (context);
                }
            } else {

                /*
                 * We still store the server's PGP password, and use it, in case envelope was encrypted for current server.
                 * This allows for simpler syntax when used.
                 */
                _password = _context.RaiseEvent (".p5.config.get", new Node (".p5.config.get", "gpg-server-keypair-password")) [0]?.Get<string> (_context) ?? null;
                _fingerprint = _context.RaiseEvent ("p5.auth.pgp.get-fingerprint").Get<string> (context);
            }
        }

        /// <summary>
        ///     Processes the RootEntity, and puts results into Args.
        /// </summary>
        public void Process ()
        {
            ProcessEntity (_rootEntity, _args);
        }

        /*
         * Processing one single MimeEntity, which might involve recursively processing multiparts.
         */
        void ProcessEntity (MimeEntity entity, Node args)
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
        void ProcessLeafPart (MimePart part, Node args)
        {
            // Verifying part actually has content, before trying to de-serialize it.
            if (part.Content == null)
                return;

            /*
             * Notice, is MIME entity is of type "application/pgp-signature", we simply discard
             * it in its entirety, since signature has already been handled at this point, and
             * (hopefully) verified, and there are no reasons to keep the actual signature node
             * around anymore. If we kept the part around, it would simply add to the confusion, since
             * it's actually processed as an attachment.
             */
            if (part.ContentType != null && part.ContentType.MediaType == "application" && part.ContentType.MediaSubtype == "pgp-signature")
                return;

            // Creating entity's root node.
            Node entityNode = args.Add (part.ContentType.MediaType, part.ContentType.MediaSubtype).LastChild;

            // Processing MIME headers.
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
        bool TreatAsAttachment (MimePart part)
        {
            // Checking if caller supplied an attachment folder.
            if (string.IsNullOrEmpty (_attachmentFolder))
                return false; // We cannot store attachments, unless caller supplies an [attachment-folder] argument.

            // Checking if MIME entity actually is an attachment.
            if (part.IsAttachment) {

                // This is an actual attachment, and should be treated as such.
                return true;

            } else if (!string.IsNullOrEmpty (part.FileName) && part.ContentDisposition != null && part.ContentDisposition.Disposition == "form-data") {

                /*
                 * Form data file attachment should also be treated as an attachment, even though
                 * it has no Content-Disposition filename.
                 */
                return true;
            }
            return false;
        }

        /*
         * Stores attachment to [attachment-folder] supplied by caller.
         */
        void SaveMimePartToDisc (MimePart part, Node entityNode)
        {
            // Creating an intelligent filename.
            string rootFolder = Common.GetRootFolder (_context);
            string fileName = "";
            if (part.ContentDisposition == null || string.IsNullOrEmpty (part.ContentDisposition.FileName)) {

                /*
                 * No explicit filename given, defaulting to "noname".
                 * Notice, since there might be multiple "noname" attachments, we
                 * make sure we give each a unique filename, by adding an integer to its
                 * filename.
                 */
                fileName = "noname";
                if (_noNameAttachments != 0)
                    fileName += "-" + _noNameAttachments;
                _noNameAttachments += 1;

            } else {

                // Explicit filename given.
                fileName = part.ContentDisposition.FileName;
            }

            /*
             * Checking if we should add a prefix to attachment path.
             * This is done to make sure file doesn't overwrite each other, and that
             * each attachment has a unique filename.
             */
            var physical_full_filename = "";
            if (_addPrefixToAttachmentPath) {

                // Making sure we create a unique file "prefix", such that files with similar names don't overwrite each other.
                var unique = Guid.NewGuid ().ToString ().Replace ("-", "") + "-";
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
                part.Content.DecodeTo (stream);
            }
        }

        /*
         * Returns a MimePart as inline content to caller.
         */
        void ProcessMimePartInline (MimePart part, Node entityNode)
        {
            // Checking if MIME entity is a simple "text" part.
            var txtPart = part as TextPart;
            if (txtPart != null) {

                // Part is text part.
                entityNode.Add ("content", txtPart.Text);

            } else if (part.ContentType.MediaType == "application" && part.ContentType.MediaSubtype == "x-hyperlambda") {
                
                /*
                 * Current part is inline Hyperlambda content.
                 * 
                 * Creating a stream to decode our entity to.
                 */
                using (var stream = new MemoryBlockStream ()) {

                    // Decoding content to memory.
                    part.Content.DecodeTo (stream);

                    // Resetting position and setting up a buffer object to hold content.
                    stream.Position = 0;

                    // Getting content as text.
                    using (var reader = new StreamReader (stream)) {

                        // Reading content is string.
                        var content = reader.ReadToEnd ();
                        
                        // Putting content into return node for MimeEntity.
                        entityNode.Add ("content").LastChild.AddRange (_context.RaiseEvent ("hyper2lambda", new Node ("", content)).Children);
                    }
                }

            } else {

                /*
                 * Part is not text part, and is not Hyperlambda content, hence we
                 * make sure we add it to returned [content] node as binary data.
                 * 
                 * First creating a stream to decode our entity to.
                 */
                using (var stream = new MemoryBlockStream ()) {

                    // Decoding content to memory.
                    part.Content.DecodeTo (stream);

                    // Resetting position and setting up a buffer object to hold content.
                    stream.Position = 0;

                    // Putting content into return node for MimeEntity.
                    entityNode.Add ("content", stream.ToArray ());
                }
            }
        }

        /*
         * Processes a Multipart, which can be either signed, encrypted, or any other types of Multipart MimeEntity.
         */
        void ProcessMultipart (Multipart multipart, Node args)
        {
            // Creating our MIME entity root node.
            Node entityNode = args.Add (multipart.ContentType.MediaType, multipart.ContentType.MediaSubtype).LastChild;

            // Processing MIME headers.
            ProcessHeaders (multipart, entityNode);

            /*
             * Checking type of multipart, making sure we correctly handle signed
             * and encrypted parts.
             */
            if (multipart is MultipartEncrypted) {

                // Encrypted Multipart, might also be signed.
                ProcessEncryptedMultipart (multipart as MultipartEncrypted, entityNode);

            } else if (multipart is MultipartSigned) {

                // Only signed Multipart and not encrypted.
                ProcessSignedMultipart (multipart as MultipartSigned, entityNode);

            } else {

                // Plain multipart.
                foreach (var idxEntity in multipart) {
                    ProcessEntity (idxEntity, entityNode);
                }
            }
        }

        /*
         * Processes an encrypted multipart.
         */
        void ProcessEncryptedMultipart (MultipartEncrypted encryptedMultipart, Node entityNode)
        {
            // Creating cryptography context.
            using (var ctx = _context.RaiseEvent (
                ".p5.crypto.pgp-keys.context.create",
                new Node ("", false, new Node [] {
                    new Node ("password", _password),
                    new Node ("fingerprint", _fingerprint)}))
                   .Get<OpenPgpContext> (_context)) {

                // Decrypting entity, making sure we retrieve signatures at the same time, if there are any.
                DigitalSignatureCollection signatures;
                var decryptedMultipart = encryptedMultipart.Decrypt (ctx, out signatures);

                // Adding signatures.
                ProcessSignatures (entityNode, signatures);

                // Parsing decrypted result.
                ProcessEntity (decryptedMultipart, entityNode);
            }
        }

        /*
         * Processes a signed multipart.
         */
        void ProcessSignedMultipart (MultipartSigned signedMultipart, Node entityNode)
        {
            // Creating cryptographic context.
            using (var ctx = _context.RaiseEvent (
                ".p5.crypto.pgp-keys.context.create",
                new Node ("", false)).Get<OpenPgpContext> (_context)) {

                // Adding signatures.
                ProcessSignatures (entityNode, signedMultipart.Verify (ctx));

                // Looping through all entities in Multipart, processing recursively.
                foreach (var idxEntity in signedMultipart) {

                    // Processing currently iterated MIME part.
                    ProcessEntity (idxEntity, entityNode);
                }
            }
        }

        /*
         * Processes the given signature collection.
         */
        void ProcessSignatures (Node entityNode, DigitalSignatureCollection signatures)
        {
            // Making sure there are any signatures.
            if (signatures == null)
                return; // Nothing to do here ...

            // Looping through each signature.
            foreach (var idxSignature in signatures) {

                // Making sure we return email of PGP key used to sign, and true as value of node if signature is valid.
                var signatureNode = entityNode.FindOrInsert ("signature").Add (idxSignature.SignerCertificate.Email, idxSignature.Verify ()).LastChild;

                // Adding fingerprint of PGP key used to sign entity.
                signatureNode.Add ("fingerprint", idxSignature.SignerCertificate.Fingerprint.ToLower ());
            }
        }

        /*
         * Processes the MIME headers of the given MimeEntity.
         */
        void ProcessHeaders (MimeEntity entity, Node args)
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

