/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;
using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace phosphorus.crypto
{
    /// <summary>
    ///     Class encapsulating PGP Active Events.
    /// 
    ///     This class encapsulates all Pretty Good Privacy (PGP) Active Events, such as encryption, 
    ///     decryption and signing MIME messages.
    /// </summary>
    public class Pgp
    {
        /// <summary>
        ///     Verifies the given MimeEntity(s) is signed correctly.
        /// 
        ///     Will construct a MimeEntity out of the value(s) of the node, and verify the entity is correctly signed. If successful, then it will
        ///     return [success] with a value of 'true', and each signature's email address that was verified as children node beneath [success].
        ///     If not successful, it will return 'false' as value of [success]. Notice that all signatures in entity must be verified for this Active 
        ///     Event to return true, but if the two first signatures are verified, but the third fails, then each email that is verified, will be 
        ///     returned as children nodes of [success], even though [success] itself will have a value of 'false'.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.crypto.pgp.verify-signature")]
        private static void pf_crypto_pgp_verify_signature (ApplicationContext context, ActiveEventArgs e)
        {
            // defaulting to "unsuccessful" return value ...
            e.Args.Add ("success", false);

            // looping through result of expression or constant given
            bool sawSignature = false;
            foreach (var entityBytes in XUtil.Iterate<byte[]> (e.Args.Value, e.Args, context)) { // to not confuse it with the [success] node, among other things
                sawSignature = true;
                if (entityBytes == null)
                    return; // we cannot verify something pointing into oblivion ...

                // loading the current multipart into a MimeEntity, and verifying it was a MultipartSigned instance
                using (MemoryStream stream = new MemoryStream (entityBytes)) {
                    MimeEntity entity = MimeEntity.Load (stream);
                    var signedEntity = entity as MultipartSigned;
                    if (signedEntity == null)
                        return; // Ooops, message was not signed, cannot verify it (obviously) ...

                    // looping through each signature in entity...
                    foreach (var signature in signedEntity.Verify ()) {
                        if (!signature.Verify ()) {
                            return; // failure ...!!
                        }

                        // success for current signature. Making sure caller gets to know which signature(s) was a success
                        e.Args ["success"].Add (string.Empty, signature.SignerCertificate.Email);
                        e.Args ["success"].LastChild.Add (string.Empty, signature.SignerCertificate.Fingerprint);
                    }
                }
            }
            if (sawSignature)
                e.Args ["success"].Value = true; // success, all signatures was verified ...
        }
        
        /// <summary>
        ///     Imports the specified public PGP keys.
        /// 
        ///     Will loop through each public key given, create an 'application/pgp-signature' MimeEntity from it, and
        ///     import the public PGP key into the GnuPG database.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.crypto.pgp.import-public-key")]
        private static void pf_crypto_pgp_import_public_key (ApplicationContext context, ActiveEventArgs e)
        {
            // looping through all public keys pointed to by expression or constant
            using (var ctx = new PfGnuPGContext ()) {
                foreach (var idxByteValue in XUtil.Iterate<byte[]> (e.Args, context)) {

                    // signature is (probably) handed to us without any MIME headers, hence we've got to 
                    // prepend the MIME headers, to make CryptographyContext.Import recognize it as a PGP signature
                    using (var streamInput = new MemoryStream (idxByteValue)) {

                        // constructing our signature from raw data
                        MimePart signature = new MimePart ("application", "pgp-signature");
                        signature.ContentObject = new ContentObject (streamInput);

                        // extracting signature with Content-Type header prepended now, and importing into CryptographyContext
                        using (var streamOutput = new MemoryStream ()) {
                            signature.WriteTo (streamOutput);
                            ctx.Import (streamOutput);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        ///     Decrypts the given MimeEntity.
        /// 
        ///     Will construct a MimeEntity out of the value(s) of the node, and decrypt the entities if possible, and return the 
        ///     decrypted entity as raw bytes in [result], using the supplied [password] to retrieve the private PGP key necessary to
        ///     decrypt the message from the GnuPG database.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.crypto.pgp.decrypt")]
        private static void pf_crypto_pgp_decrypt (ApplicationContext context, ActiveEventArgs e)
        {
            using (var ctx = new PfGnuPGContext ()) {
                if (e.Args ["password"] != null) {
                    ctx.Password = e.Args ["password"].GetExValue<string> (context);
                }

                // looping through each entity caller wants to have decrypted
                foreach (var idxEntityBytes in XUtil.Iterate<byte[]> (e.Args.Value, e.Args, context)) {

                    // creating a MimeEntity out of the current item, and checking to see if it's a MultipartEncrypted entity
                    using (MemoryStream stream = new MemoryStream (idxEntityBytes)) {
                        MimeEntity entity = MimeEntity.Load (stream);
                        var encryptedEntity = entity as MultipartEncrypted;
                        if (encryptedEntity != null) {

                            // item was an encrypted entity, decrypting entity, and returning raw bytes back to caller
                            MimeEntity resultEntity = encryptedEntity.Decrypt (ctx);
                            using (MemoryStream resultStream = new MemoryStream()) {
                                resultEntity.WriteTo (resultStream);
                                e.Args.Add ("result", resultStream.ToArray ());
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        ///     Necessary to register our CryptographyContext for MimeKit.
        /// 
        ///     Registers the PfGnuPGContext class as a CryptographyContext for MimeKit during startup of Application Pool.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.core.application-start")]
        private static void pf_core_application_start (ApplicationContext context, ActiveEventArgs e)
        {
            CryptographyContext.Register (typeof (PfGnuPGContext));
        }

        /// <summary>
        ///     Signs a MimeEntity.
        /// 
        ///     Not supposed to be invoked from pf.lambda. Signs the given MimeEntity from the value of the main node, 
        ///     and returns the signed MimeEntity to caller. The email to use as lookup for which private key to use to sign the
        ///     message, is passed in as [email]. The password necessary to release that private key from the 
        ///     GnuPG database, must be given as [password].
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "_pf.crypto.pgp.sign")]
        private static void _pf_crypto_pgp_sign (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = SignMimeEntity (context, e.Args);
        }
        
        /// <summary>
        ///     Verifies the signature of a MimeEntity.
        /// 
        ///     Not supposed to be invoked from pf.lambda. Checks the signature of the given MultipartSigned, and returns
        ///     the [email] and [fingerprint] of the signature.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "_pf.crypto.pgp.verify-signature")]
        private static void _pf_crypto_pgp_verify_signature (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = VerifySignature (context, e.Args);
        }

        /// <summary>
        ///     Encrypts a MimeEntity.
        /// 
        ///     Not supposed to be invoked from pf.lambda. Encrypts the given MimeEntity from the value of the main node, 
        ///     and returns the encrypted MimeEntity to caller. The email(s) to use as lookup for which public certificate(s) to use 
        ///     to encrypt the message, is passed in as children of [emails]. This Active Event will use the GnuPG storage to lookup 
        ///     certificate(s) matching the given [emails] parameter's children's values.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "_pf.crypto.pgp.encrypt")]
        private static void _pf_crypto_pgp_encrypt (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = EncryptMimeEntity (context, e.Args);
        }
        
        /// <summary>
        ///     Decrypts a Multipart.
        /// 
        ///     Not supposed to be invoked from pf.lambda. Decrypts the given Multipart from the value of the main node, 
        ///     and returns the decrypted MimeEntity to caller. The email to use as lookup for which private key to use 
        ///     to decrypt the message, is passed in as [email]. This Active Event will use the GnuPG storage to lookup 
        ///     a private key matching the given [email] parameter. The password used to retrieve the private key from 
        ///     your GnuPG storage, must be given as [password].
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "_pf.crypto.pgp.decrypt")]
        private static void _pf_crypto_pgp_decrypt (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = DecryptMimeEntity (context, e.Args);
        }

        private static MimeEntity SignMimeEntity (ApplicationContext context, Node node)
        {
            var password = node.GetExChildValue<string> ("password", context);
            if (string.IsNullOrEmpty (password))
                throw new ArgumentException ("No [password] supplied for signing operation.");
            MimeEntity entity = node.GetExValue<MimeEntity> (context);
            using (var ctx = new PfGnuPGContext ()) {
                ctx.Password = password;
                MailboxAddress secureMail = new MailboxAddress ("", node.GetExChildValue<string> ("email", context));
                return MultipartSigned.Create (
                    ctx, 
                    secureMail,
                    (DigestAlgorithm)Enum.Parse (typeof (DigestAlgorithm), node.GetExChildValue<string> ("algo", context, "Sha1")),
                    entity);
            }
        }

        private static MimeEntity EncryptMimeEntity (ApplicationContext context, Node node)
        {
            MimeEntity entity = node.GetExValue<MimeEntity> (context);
            using (var ctx = new PfGnuPGContext ()) {
                return MultipartEncrypted.Create (
                    ctx, 
                    GetEncryptionKeys (context, node ["emails"]), 
                    entity);
            }
        }

        private static Node VerifySignature (ApplicationContext context, Node node)
        {
            // defaulting retVal to 'false'
            Node retVal = new Node ("success", false);

            // looping through each signature in entity...
            MultipartSigned signedEntity = node.Get<MultipartSigned> (context);
            bool sawSignature = false;
            foreach (var signature in signedEntity.Verify ()) {
                sawSignature = true;
                if (!signature.Verify ()) {
                    return retVal; // failure ...!!
                }

                // success for current signature. Making sure caller gets to know which signature(s) was a success
                retVal.Add (string.Empty, signature.SignerCertificate.Email);
                retVal.LastChild.Add (string.Empty, signature.SignerCertificate.Fingerprint);
            }
            if (sawSignature)
                retVal.Value = true;
            return retVal;
        }
        
        private static MimeEntity DecryptMimeEntity (ApplicationContext context, Node node)
        {
            MultipartEncrypted entity = node.GetExValue<MultipartEncrypted> (context);
            using (var ctx = new PfGnuPGContext ()) {
                ctx.Password = node.GetExChildValue<string> ("password", context);
                return entity.Decrypt (ctx);
            }
        }

        private static IEnumerable<MailboxAddress> GetEncryptionKeys (ApplicationContext context, Node node)
        {
            foreach (Node idxEncrNode in node.Children) {
                yield return new MailboxAddress (string.Empty, idxEncrNode.GetExValue<string> (context));
            }
        }
    }
}
