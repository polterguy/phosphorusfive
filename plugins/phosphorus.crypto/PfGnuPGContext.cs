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
    public class PfGnuPGContext : GnuPGContext
    {
        protected override string GetPasswordForKey (PgpSecretKey key)
        {
            return Password;
        }

        /*
         * used to hold password for decryption and signing
         */
        private string Password {
            get;
            set;
        }

        /// <summary>
        ///     Verifies the given MimeEntity is signed correctly.
        /// 
        ///     Will construct a MimeEntity out of the value of the node, and verify the entity is correctly signed. If successful, then it will
        ///     return [verified] with a value of 'true', and each signature's email address that was verified as children node beneath [verified].
        ///     If not successful, it will return 'false' as value of [verified]. Notice that all signatures in entity must be verified for this Active 
        ///     Event to return true, but if the two first signatures are verified, but the third fails, then each email that is verified, will be 
        ///     returned as children nodes of [verified], even though [verified] itself will have a value of 'false'.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.crypto.pgp.verify-signature")]
        private static void pf_crypto_pgp_verify_signature (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add ("verified", false);
            var entityBytes = e.Args.GetExValue<byte[]> (context);
            if (entityBytes == null)
                return;
            using (MemoryStream stream = new MemoryStream (entityBytes)) {
                MimeEntity entity = MimeEntity.Load (stream);
                var signedEntity = entity as MultipartSigned;
                if (signedEntity != null) {
                    foreach (var signature in signedEntity.Verify ()) {
                        if (!signature.Verify ()) {
                            e.Args ["verified"].Value = false;
                            return;
                        }
                        e.Args ["verified"].Add (string.Empty, signature.SignerCertificate.Email);
                    }
                    e.Args ["verified"].Value = true; // success, all signatures was verified ...
                }
            }
        }
        
        /// <summary>
        ///     Decrypts the given MimeEntity.
        /// 
        ///     Will construct a MimeEntity out of the value of the node, and decrypt the entity if possible, and return the 
        ///     decrypted entity as raw bytes in [result].
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.crypto.pgp.decrypt")]
        private static void pf_crypto_pgp_decrypt (ApplicationContext context, ActiveEventArgs e)
        {
            var entityBytes = e.Args.GetExValue<byte[]> (context);
            if (entityBytes == null)
                return;
            using (var ctx = new PfGnuPGContext ()) {
                if (e.Args ["password"] != null) {
                    ctx.Password = e.Args ["password"].GetExValue<string> (context);
                }
                using (MemoryStream stream = new MemoryStream (entityBytes)) {
                    MimeEntity entity = MimeEntity.Load (stream);
                    var encryptedEntity = entity as MultipartEncrypted;
                    if (encryptedEntity != null) {
                        MimeEntity resultEntity = encryptedEntity.Decrypt (ctx);
                        using (MemoryStream resultStream = new MemoryStream()) {
                            resultEntity.WriteTo (resultStream);
                            e.Args.Add ("result", resultStream.ToArray ());
                        }
                    }
                }
            }
        }
        
        /// <summary>
        ///     Only here to register our CryptographyContext for MimeKit.
        /// 
        ///     Registers the PfGnuPGContext class as a CryptographyContext for MimeKit.
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
        ///     Not supposed to be invoked from pf.lambda, but signs the given MimeEntity from the value of the main node, 
        ///     and returns the signed MimeEntity to caller. The email to use as lookup for which private key to use to sign the
        ///     message, is passed in as a [sign] parameter. The password necessary to release that private key from the GnuPG, must
        ///     be given as [password] beneath the [sign] node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "_pf.crypto.pgp.sign")]
        private static void pf_crypto_pgp_sign (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = SignMimeEntity (e.Args.GetExValue<MimeEntity> (context), e.Args, context);
        }

        /// <summary>
        ///     Encrypts a MimeEntity.
        /// 
        ///     Not supposed to be invoked from pf.lambda, but encrypts the given MimeEntity from the value of the main node, 
        ///     and returns the encrypted MimeEntity to caller. The email to use as lookup for which public certificate to use to encrypt the
        ///     message, is passed in as a [encrypt] parameter. This Active Event will use the GnuPG storage to lookup certificate(s) matching the
        ///     given [encrypt] parameter's children's values.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "_pf.crypto.pgp.encrypt")]
        private static void pf_crypto_pgp_encrypt (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = EncryptMimeEntity (e.Args.GetExValue<MimeEntity> (context), context, e.Args);
        }

        /// <summary>
        ///     Signs and encrypts a MimeEntity, in that order.
        /// 
        ///     Not supposed to be invoked from pf.lambda, but first it signs the given MimeEntity from the value of the main node, 
        ///     for then to encrypt the same MimeEntity. The email to use as lookup for which private key to use to sign the
        ///     message, is passed in as a [sign] parameter. The password necessary to release that private key from the GnuPG, must
        ///     be given as [password] beneath the [sign] node. The email(s) used to encrypt the MimeEntity, are given as children
        ///     nodes of [encrypt].
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "_pf.crypto.pgp.sign-and-encrypt")]
        private static void pf_crypto_pgp_sign_and_encrypt (ApplicationContext context, ActiveEventArgs e)
        {
            var entity = e.Args.Get<MimeEntity> (context);
            entity = SignMimeEntity (entity, e.Args, context);
            e.Args.Value = EncryptMimeEntity (entity, context, e.Args);
        }

        private static MimeEntity SignMimeEntity (MimeEntity entity, Node node, ApplicationContext context)
        {
            using (var ctx = new PfGnuPGContext ()) {
                if (node ["sign"] ["password"] != null) {
                    ctx.Password = node ["sign"] ["password"].GetExValue<string> (context);
                }
                MailboxAddress secureMail = new MailboxAddress ("", node.GetExChildValue<string> ("sign", context));
                return MultipartSigned.Create (
                    ctx, 
                    secureMail,
                    DigestAlgorithm.Sha1,
                    entity);
            }
        }
        
        private static MimeEntity EncryptMimeEntity (MimeEntity entity, ApplicationContext context, Node node)
        {
            using (var ctx = new PfGnuPGContext ()) {
                return MultipartEncrypted.Create (
                    ctx, 
                    GetEncryptionKeys (context, node ["encrypt"]), 
                    entity);
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
