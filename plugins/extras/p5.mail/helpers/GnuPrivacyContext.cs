/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg.OpenPgp;
using p5.exp;
using p5.core;

namespace p5.mail.helpers
{
    /// <summary>
    ///     Gnu Privacy Guard context for encrypting and decrypting Mime messages using PGP
    /// </summary>
    public class GnuPrivacyContext : GnuPGContext
    {
        public string Password {
            get;
            set;
        }

        protected override string GetPasswordForKey (PgpSecretKey key)
        {
            return Password;
        }
    }
}

