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
        private static string _password = "";
        private static object _locker = new object ();

        protected override string GetPasswordForKey (PgpSecretKey key)
        {
            lock (_locker) {
                string retVal = _password;
                _password = string.Empty;
                return retVal;
            }
        }

        [ActiveEvent (Name = "pf.core.application-start")]
        private static void pf_core_application_start (ApplicationContext context, ActiveEventArgs e)
        {
            CryptographyContext.Register (typeof (PfGnuPGContext));
        }
        
        [ActiveEvent (Name = "_pf.crypto.pgp.sign")]
        private static void pf_crypto_pgp_sign (ApplicationContext context, ActiveEventArgs e)
        {
            var entity = e.Args.Get<MimeEntity> (context);
            using (var ctx = new PfGnuPGContext ()) {
                MailboxAddress secureMail = new MailboxAddress ("", e.Args.GetExChildValue<string> ("sign", context));
                lock (_locker) {
                    if (e.Args ["sign"] ["password"] != null) {
                        _password = e.Args ["sign"] ["password"].GetExValue<string> (context);
                    }
                    e.Args.Value = MultipartSigned.Create (
                        ctx, 
                        secureMail,
                        DigestAlgorithm.Sha1,
                        entity);
                }
            }
        }
        
        [ActiveEvent (Name = "_pf.crypto.pgp.encrypt")]
        private static void pf_crypto_pgp_encrypt (ApplicationContext context, ActiveEventArgs e)
        {
            var entity = e.Args.Get<MimeEntity> (context);
            using (var ctx = new PfGnuPGContext ()) {
                e.Args.Value = MultipartEncrypted.Create (
                    ctx, 
                    GetEncryptionKeys (context, e.Args ["encrypt"]), 
                    entity);
            }
        }
        
        [ActiveEvent (Name = "_pf.crypto.pgp.sign-and-encrypt")]
        private static void pf_crypto_pgp_sign_and_encrypt (ApplicationContext context, ActiveEventArgs e)
        {
            var entity = e.Args.Get<MimeEntity> (context);
            using (var ctx = new PfGnuPGContext ()) {
                MailboxAddress signerMail = new MailboxAddress ("", e.Args.GetExChildValue<string> ("sign", context));
                using (MemoryStream stream = new MemoryStream ()) {
                    entity.WriteTo (stream);
                    lock (_locker) {
                        if (e.Args ["sign"] ["password"] != null) {
                            _password = e.Args ["sign"] ["password"].GetExValue<string> (context);
                        }
                        e.Args.Value = ctx.SignAndEncrypt (signerMail, DigestAlgorithm.Sha1, GetEncryptionKeys (context, e.Args ["encrypt"]), stream);
                    }
                }
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
