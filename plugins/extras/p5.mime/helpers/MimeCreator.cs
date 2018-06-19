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
using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using MimeKit;
using MimeKit.IO;
using MimeKit.Cryptography;

namespace p5.mime.helpers
{
    /// <summary>
    ///     Helper to create a MimeEntity.
    /// </summary>
    public class MimeCreator
    {
        readonly ApplicationContext _context;
        Node _entityNode;
        readonly List<Stream> _streams;

        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.mime.helpers.MimeCreator"/> class.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="entityNode">Entity node, declaring MimeEntity</param>
        /// <param name="streams">Streams created during creation process. It is caller's responsibility to close and dispose these streams afterwards</param>
        public MimeCreator (ApplicationContext context, Node entityNode, List<Stream> streams)
        {
            _context = context;
            _entityNode = entityNode;
            _streams = streams;
        }

        /// <summary>
        ///     Creates a MimeEntity according to declaration in EntityNode, and returns to caller.
        /// </summary>
        public MimeEntity Create ()
        {
            // Recursively creates a MimeEntity according to given EntityNode.
            return Create (_entityNode);
        }

        /*
         * Actual implementation of creation of MimeEntity, recursively runs through given node, and creates a MimeEntity accordingly.
         */
        MimeEntity Create (Node entityNode)
        {
            // Sanity check.
            if (string.IsNullOrEmpty (entityNode.Value as string))
                throw new LambdaException (
                    string.Format ("No media subtype provided for '{0}' to MIME builder", entityNode.Name),
                    entityNode,
                    _context);

            // Setting up a return value.
            MimeEntity retVal = null;

            // Figuring out which type to create.
            switch (entityNode.Name) {
                case "multipart":
                    retVal = CreateMultipart (entityNode);
                    break;
                case "text":
                case "image":
                case "application":
                case "audio":
                case "video":
                case "message":
                case "example":
                case "model":
                    retVal = CreateLeafPart (entityNode);
                    break;
                default:
                    throw new LambdaException (
                        string.Format ("Unknown media type '{0}' for MIME builder", entityNode.Name),
                        entityNode,
                        _context);
            }

            // Figuring out if entity should be encrypted and/or signed.
            bool shouldSign = entityNode ["sign"] != null;
            bool shouldEncrypt = entityNode ["encrypt"] != null;

            // Signing and/or encrypting entity, if we should.
            if (shouldSign && !shouldEncrypt) {

                // Only signing entity.
                retVal = SignEntity (entityNode, retVal);

            } else if (shouldEncrypt && !shouldSign) {

                // Only encrypting entity.
                retVal = EncryptEntity (entityNode, retVal);

            } else if (shouldEncrypt && shouldSign) {

                // Signing and encrypting entity.
                retVal = SignAndEncryptEntity (entityNode, retVal);
            }

            // Returning entity to caller.
            return retVal;
        }

        /*
         * Creates a Multipart MimeEntity, which might be encrypted, signed, or both.
         */
        Multipart CreateMultipart (Node multipartNode)
        {
            // Setting up a return value.
            Multipart multipart = new Multipart (multipartNode.GetExValue<string> (_context));

            // Adding headers.
            DecorateEntityHeaders (multipart, multipartNode);

            /*
             * Looping through all children nodes of multipartNode that are not properties of multipart, assuming they're childred MIME entities.
             * Notice, all MIME headers have Capital letters in them.
             */
            foreach (var idxChildNode in multipartNode.Children.Where (ix =>
                ix.Name != "sign" &&
                ix.Name != "encrypt" &&
                ix.Name.ToLower () == ix.Name)) {

                // Adding currently iterated part.
                multipart.Add (Create (idxChildNode));
            }

            // Returning multipart to caller.
            return multipart;
        }

        /*
         * Creates a leaf MimePart.
         */
        MimePart CreateLeafPart (Node mimePartNode)
        {
            // Setting up a return value.
            MimePart retVal = new MimePart (ContentType.Parse (mimePartNode.Name + "/" + mimePartNode.GetExValue <String> (_context)));

            // Adding headers.
            DecorateEntityHeaders (retVal, mimePartNode);

            // Checking which type of content is provided, supported types are [content] or [filename].
            if (mimePartNode ["content"] != null) {

                // Sanity checking node.
                if (mimePartNode ["filename"] != null)
                    throw new LambdaException ("[filename] and [content] are mutual exclusive when creating MIME envelopes.", mimePartNode, _context);

                // Simple inline content.
                CreateContentObjectFromContent (mimePartNode ["content"], retVal);

            } else if (mimePartNode ["filename"] != null) {

                // Creating content from filename.
                CreateContentObjectFromFilename (mimePartNode ["filename"], retVal);

            } else {

                // Oops, no content!
                throw new LambdaException (
                    "No content found for MIME part, use either [content] or [filename] to supply content",
                    mimePartNode,
                    _context);
            }

            // Returning MimePart to caller.
            return retVal;
        }

        /*
         * Only signs the given MimeEntity.
         */
        MultipartSigned SignEntity (Node entityNode, MimeEntity entity)
        {
            // Retrieving signature node to use for signing operation.
            var signatureNode = entityNode ["sign"];

            // Getting signature email as provided by caller.
            var signatureAddress = GetSignatureMailboxAddress (signatureNode);

            /*
             * Figuring out signature Digest Algorithm to use for signature, defaulting to SHA256.
             * SHA256 should be safe, since there are no known collision weaknesses in it.
             * Therefor we default to SHA256, unlesss caller explicitly tells us he wants to use another algorithm.
             */
            var algo = signatureNode.GetChildValue ("digest-algorithm", _context, DigestAlgorithm.Sha256);

            /*
             * Creating our PGP context, passing in password specified by caller.
             */
            using (var ctx = _context.RaiseEvent (
                ".p5.crypto.pgp-keys.context.create", 
                new Node ("", false, 
                          new Node [] {
                    new Node ("password", signatureAddress.Item1), 
                    new Node ("fingerprint", signatureNode ["fingerprint"]?.GetExChildValue<string> ("fingerprint", _context, null) ??
                              _context.RaiseEvent ("p5.auth.pgp.get-fingerprint").Get<string> (_context)) }))
                   .Get<OpenPgpContext> (_context)) {

                // Signing content of email and returning to caller.
                return MultipartSigned.Create (
                    ctx,
                    signatureAddress.Item2,
                    algo,
                    entity);
            }
        }

        /*
         * Only encrypts the given MimeEntity.
         */
        MultipartEncrypted EncryptEntity (Node entityNode, MimeEntity entity)
        {
            // Retrieving node that declares encryption settings for us.
            var encryptionNode = entityNode ["encrypt"];

            // Retrieving MailboxAddresses to encrypt message for.
            var receivers = GetReceiversMailboxAddress (encryptionNode);

            /*
             * Creating our PGP context.
             * Notice, the password is not necessary when doing encryption, since we're only using public keys.
             */
            using (var ctx = _context.RaiseEvent (
                ".p5.crypto.pgp-keys.context.create",
                new Node ("", false)).Get<OpenPgpContext> (_context)) {

                // Encrypting content of email and returning to caller.
                var retVal = MultipartEncrypted.Encrypt (
                    ctx,
                    receivers,
                    entity);

                // Returning encrypted Multipart.
                return retVal;
            }
        }

        /*
         * Signs and encrypts the given MimeEntity.
         */
        MultipartEncrypted SignAndEncryptEntity (Node entityNode, MimeEntity entity)
        {
            // Retrieving [sign] and [encrypt] nodes.
            var signatureNode = entityNode ["sign"];
            var encryptionNode = entityNode ["encrypt"];

            // Getting signature email as provided by caller.
            var signatureAddress = GetSignatureMailboxAddress (signatureNode);

            // Retrieving MailboxAddresses to encrypt message for.
            var receivers = GetReceiversMailboxAddress (encryptionNode);

            /*
             * Figuring out signature Digest Algorithm to use for signature, defaulting to SHA256.
             * SHA256 should be safe, since there are no known collision weaknesses in it.
             * Therefor we default to SHA256, unlesss caller explicitly tells us he wants to use another algorithm.
             */
            var algo = signatureNode.GetChildValue ("digest-algorithm", _context, DigestAlgorithm.Sha256);

            /*
             * Creating our Gnu Privacy Guard context, passing in password to retrieve private signing key.
             */
            using (var ctx = _context.RaiseEvent (
                ".p5.crypto.pgp-keys.context.create",
                new Node ("", false,
                          new Node [] {
                    new Node ("password", signatureAddress.Item1),
                    new Node ("fingerprint", signatureNode ["fingerprint"]?.GetExChildValue<string> ("fingerprint", _context, null) ??
                              _context.RaiseEvent ("p5.auth.pgp.get-fingerprint").Get<string> (_context)) }))
                    .Get<OpenPgpContext> (_context)) {

                // Signing and Encrypting content of email.
                var retVal = MultipartEncrypted.SignAndEncrypt (
                    ctx,
                    signatureAddress.Item2,
                    algo,
                    receivers,
                    entity);

                // Returning encrypted Multipart.
                return retVal;
            }
        }

        /*
         * Returns list of MailboxAddresses according to given node.
         */
        List<MailboxAddress> GetReceiversMailboxAddress (Node encryptionNode)
        {
            // The MailboxAddress list returned to caller.
            var retVal = new List<MailboxAddress> ();

            /*
             * Checking if no receivers were declared, at which point we use the server PGP
             * key by default.
             */
            if (encryptionNode.Count == 0) {

                // Assuming caller wants to use server's PGP fingerprint.
                var fingerprint = _context.RaiseEvent ("p5.auth.pgp.get-fingerprint").Get<string> (_context);
                retVal.Add (new SecureMailboxAddress ("", "foo@bar.com", fingerprint));

            } else {

                // One or more explicitly declared [fingerprint] or [email] arguments.
                foreach (var idx in encryptionNode.Children) {

                    // Checking if email address was given, or if fingerprint was given.
                    if (idx.Name == "email")
                        retVal.Add (new MailboxAddress ("", idx.GetExValue<string> (_context)));
                    else if (idx.Name == "fingerprint")
                        retVal.Add (new SecureMailboxAddress ("", "foo@bar.com", idx.GetExValue<string> (_context)));
                    else
                        throw new LambdaException (
                            string.Format ("Sorry, don't know how to encrypt for a [{0}] type of node, I only understand [email] or [fingerprint]", idx.Name),
                            idx,
                            _context);
                }
            }

            // Returning list of mailboxes to encrypt for.
            return retVal;
        }

        /*
         * Returns email for signing entity, and password to release private key from GnuPG.
         */
        Tuple<string, MailboxAddress> GetSignatureMailboxAddress (Node signatureNode)
        {
            /*
             * Checking if [sign] node has no children, at which point we assume caller wants
             * to use the server key.
             */
            if (signatureNode.Count == 0) {

                // Assuming caller wants to use server's PGP fingerprint.
                var fingerprint = _context.RaiseEvent ("p5.auth.pgp.get-fingerprint").Get<string> (_context);
                var password = _context.RaiseEvent (".p5.config.get", new Node (".p5.config.get", "gpg-server-keypair-password")) [0]?.Get<string> (_context) ?? null;
                
                // Returning password and MailboxAddress to sign entity on behalf of.
                return new Tuple<string, MailboxAddress> (password, new SecureMailboxAddress ("", "foo@bar.com", fingerprint));

            } else {

                // Figuring out which private key to use for signing entity.
                string email = "foo@bar.com", fingerprint = "", password = "";
                password = signatureNode.Children.First (ix => ix.Name == "email" || ix.Name == "fingerprint").GetExChildValue ("password", _context, "");
                email = signatureNode.GetExChildValue ("email", _context, "foo@bar.com");
                fingerprint = signatureNode.GetExChildValue ("fingerprint", _context, "");
                
                // Returning password and MailboxAddress to sign entity on behalf of.
                return new Tuple<string, MailboxAddress> (password, new SecureMailboxAddress ("", email, fingerprint));
            }
        }

        /*
         * Decorates headers for given MimeEntity.
         */
        void DecorateEntityHeaders (MimeEntity entity, Node entityNode)
        {
            /* 
             * Looping through each child node of MimeEntity node, making sure we only use those children that
             * have Capital letters in them, since MIME headers all have some sort of Capital letters in them.
             * 
             * Notice, [Content-Type] is especially handled.
             */
            foreach (var idxHeader in entityNode.Children.Where (ix => ix.Name != "Content-Type" && ix.Name.ToLower () != ix.Name)) {

                // Adding currently iterated MIME header to entity.
                entity.Headers.Replace (idxHeader.Name, idxHeader.GetExValue<string> (_context));
            }
        }

        /*
         * Creates a ContentObject for MimePart from [content] supplied.
         */
        void CreateContentObjectFromContent (Node contentNode, MimePart entity)
        {
            /*
             * Creating stream to hold content.
             * Notice, there is no need to add stream to list of streams, since it
             * doesn't require closing or being disposed, since its implementation
             * is a list of byte[].
             */
            var stream = new MemoryBlockStream ();

            // Applying content object, but first checking type of object, special handling of blob/byte[].
            if (contentNode.Value is byte []) {

                // This is byte[] array (blob).
                byte [] value = contentNode.Value as byte [];
                stream.Write (value, 0, value.Length);
                stream.Position = 0;

            } else {

                /*
                 * Anything BUT byte[].
                 * Here we rely on conversion Active Events, making "everything else" serialise as strings.
                 * But first retrieving content, which might be in Hyperlambda format, or an expression.
                 */
                var content = contentNode.GetExValue<string> (_context, null);
                if (content == null) {

                    // Before we throw exception we check if contentNode has children, at which point it's Hyperlambda content.
                    if (contentNode.Count == 0)
                        throw new LambdaException ("No [content] in your MIME envelope", _entityNode, _context);

                    // Converting lambda content specified to Hyperlambda and adding it as content.
                    _context.RaiseEvent ("lambda2hyper", contentNode);
                    content = contentNode.Get<string> (_context);
                }

                /*
                 * Creating StreamWriter to make it easier to write content to stream.
                 * Notice, we cannot close StreamWriter since that'll close the underlaying stream.
                 */
                var streamWriter = new StreamWriter (stream);

                // Writing content to streamWriter.
                streamWriter.Write (content);
                streamWriter.Flush ();
                stream.Position = 0;
            }

            // Retrieving ContentEncoding to use for reading stream.
            ContentEncoding encoding = ContentEncoding.Default;
            if (contentNode ["Content-Encoding"] != null)
                encoding = (ContentEncoding)Enum.Parse (typeof (ContentEncoding), contentNode ["Content-Encoding"].GetExValue<string> (_context));

            // Creating a ContentObject for MimePart from MemoryBlockStream.
            entity.Content = new MimeContent (stream, encoding);
        }

        /*
         * Creates a ContentObject for MimePart from some file name given.
         */
        void CreateContentObjectFromFilename (
            Node fileNode,
            MimePart entity)
        {
            // Figuring out filename.
            string fileName = fileNode.GetExValue<string> (_context);

            // Verifying user is authorised to read from file given.
            fileName = _context.RaiseEvent (".p5.io.unroll-path", new Node ("", fileName)).Get<string> (_context);
            _context.RaiseEvent (".p5.io.authorize.read-file", new Node ("", fileName).Add ("args", fileNode));

            // Retrieving ContentEncoding to use for reading stream.
            ContentEncoding encoding = ContentEncoding.Default;
            if (fileNode ["Content-Encoding"] != null)
                encoding = (ContentEncoding)Enum.Parse (typeof (ContentEncoding), fileNode ["Content-Encoding"].Get<string> (_context));

            /*
             * Creating a defualt "Content-Disposition" unless one was explicitly given by caller, at which point
             * the MIME header for ContentDisposition will already have been set.
             */
            if (entity.ContentDisposition == null) {

                // Defaulting Content-Disposition to; "attachment; filename=whatever.xyz"
                entity.ContentDisposition = new ContentDisposition ("attachment");
                entity.ContentDisposition.FileName = Path.GetFileName (fileName);
            }

            /*
             * Applying content object, notice that the stream created here, is owned by the caller, hence there is
             * no disposal done.
             * 
             * We do however need to add the stream to our list of streams, such that it is disposed when we are
             * done creating our MIME envelope.
             */
            Stream stream = File.OpenRead (Common.GetRootFolder (_context) + fileName);
            _streams.Add (stream);
            entity.Content = new MimeContent (stream, encoding);
        }
    }
}

