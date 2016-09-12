/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Security;
using System.Collections.Generic;
using MimeKit;
using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace p5.mime.helpers
{
    /// <summary>
    ///     Gnu Privacy Guard context for encrypting and decrypting Mime messages using PGP
    /// </summary>
    public class GnuPrivacyContext : GnuPGContext
    {
        /// <summary>
        ///     Key password mapper, for complex operations, where you might have multiple keys used to decrypt same message
        /// </summary>
        public class KeyPasswordMapper
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="p5.mime.helpers.GnuPrivacyContext+KeyPasswordMapper"/> class.
            /// </summary>
            /// <param name="address">MailboxAddress, can also be SecureMailboxAddress to use fingerprint for lookups</param>
            /// <param name="password">Password to release key from GnuPG</param>
            public KeyPasswordMapper (MailboxAddress address, string password)
            {
                Mailbox = address;
                Password = password;
            }

            internal MailboxAddress Mailbox {
                get;
                set;
            }

            internal string Password {
                get;
                set;
            }
        }

        /// <summary>
        ///     Gets or sets the password to retrieve a private key from GnuPG
        /// </summary>
        /// <value>The password necessary to retrieve key</value>
        public string Password {
            get;
            set;
        }

        /// <summary>
        ///     Gets the last used UserId from SecretKey this instance tried to retrieve password for
        /// </summary>
        /// <value>The UserId of the PrivateKey this instance last requested password on behalf of</value>
        public string LastUsedUserId {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets multiple passwords, for complex operations, where you need several keys to decrypt message
        /// </summary>
        /// <value>The passwords.</value>
        public List<KeyPasswordMapper> Passwords {
            get;
            set;
        }

        /// <summary>
        ///     Saves the secret key ring bundle
        /// </summary>
        public void SaveSecretKeyRingBundle (PgpSecretKeyRingBundle bundle)
        {
            this.SecretKeyRingBundle = bundle;
            this.SaveSecretKeyRingBundle ();
        }

        /// <summary>
        ///     Saves the public key ring bundle
        /// </summary>
        public void SavePublicKeyRingBundle (PgpPublicKeyRingBundle bundle)
        {
            this.PublicKeyRingBundle = bundle;
            this.SavePublicKeyRingBundle ();
        }

        /// <summary>
        ///     Gets the password for the specified key, used as sink by MimeKit to decrypt private key
        /// </summary>
        /// <returns>The password for the requested key</returns>
        /// <param name="key">The key to retrieve the password for</param>
        protected override string GetPasswordForKey (PgpSecretKey key)
        {
            var enumerator = key.UserIds.GetEnumerator ();
            enumerator.MoveNext ();
            LastUsedUserId = enumerator.Current.ToString ();
            LastUsedUserId = LastUsedUserId.Substring (LastUsedUserId.IndexOf ("<") + 1);
            LastUsedUserId = LastUsedUserId.Substring (0, LastUsedUserId.IndexOf (">"));
            if (Passwords != null) {

                // Multiple passwords, need to figure out which to use to release private key
                return FromEmailMappedPasswords (key);
            } else {

                // Simple password
                return Password;
            }
        }

        /*
         * Returns the password from an email address or a fingerprint supplied
         */
        private string FromEmailMappedPasswords (PgpSecretKey key)
        {
            // Looping through all MailboxAddresses we've got
            foreach (var idxMailbox in Passwords) {

                // Checking if we have the password for this key in our list
                if (idxMailbox.Mailbox is SecureMailboxAddress) {

                    // Using fingerprint
                    if ((idxMailbox.Mailbox as SecureMailboxAddress).Fingerprint == BitConverter.ToString (key.PublicKey.GetFingerprint ()).Replace ("-", ""))
                        return idxMailbox.Password;
                } else {

                    // Using UserIds
                    foreach (string idxUserId in key.UserIds) {
                        if (idxUserId.IndexOf (idxMailbox.Mailbox.Address) != -1) {

                            // Returning associated password for key
                            return idxMailbox.Password;
                        }
                    }
                }
            }

            // Throwing exception since we found no password for requested key, showing user the first UserId object from Secret Key
            throw new SecurityException (string.Format("No password supplied for GnuPG private key '{0}'", LastUsedUserId));
        }
    }
}

