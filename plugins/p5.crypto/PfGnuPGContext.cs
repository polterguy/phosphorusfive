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
    ///     GnuPG Cryptography Context.
    /// 
    ///     Will use the GnuPG database on your system to lookup private and public PGP keys.
    /// </summary>
    public class PfGnuPGContext : GnuPGContext
    {
        protected override string GetPasswordForKey (PgpSecretKey key)
        {
            return Password;
        }

        /*
         * used to hold password for decryption and signing
         */
        public string Password {
            get;
            set;
        }
    }
}
