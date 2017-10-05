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
using System.Security;
using System.Threading;
using System.Collections.Generic;
using MimeKit;
using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg.OpenPgp;
using p5.exp;

namespace p5.mime.helpers
{
    /// <summary>
    ///     Gnu Privacy Guard context for encrypting and decrypting Mime messages using Open PGP.
    /// </summary>
    public class GnuPrivacyContext : GnuPGContext
    {
        /// <summary>
        ///     Key password mapper, for complex operations, where you might have multiple keys used to decrypt same message.
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

        /*
         * Used to synchornize access to context.
         */
        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim ();
        private bool _write;

        public GnuPrivacyContext (bool write)
        {
            _write = write;
            if (write)
                _lock.EnterWriteLock ();
            else
                _lock.EnterReadLock ();
        }

        public GnuPrivacyContext ()
        {
            _lock.EnterReadLock ();
        }

        /// <summary>
        ///     Gets or sets the password to retrieve a private key from GnuPG.
        /// </summary>
        /// <value>The password necessary to retrieve key</value>
        public string Password {
            get;
            set;
        }

        /// <summary>
        ///     Gets the last used UserId from SecretKey this instance tried to retrieve password for.
        /// </summary>
        /// <value>The UserId of the PrivateKey this instance last requested password on behalf of</value>
        public string LastUsedUserId {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets multiple passwords, for complex operations, where you need several keys to decrypt message.
        /// </summary>
        /// <value>The passwords.</value>
        public List<KeyPasswordMapper> Passwords {
            get;
            set;
        }

        /// <summary>
        ///     Saves the secret key ring bundle.
        /// </summary>
        public void SaveSecretKeyRingBundle (PgpSecretKeyRingBundle bundle)
        {
            this.SecretKeyRingBundle = bundle;
            this.SaveSecretKeyRingBundle ();
        }

        /// <summary>
        ///     Saves the public key ring bundle.
        /// </summary>
        public void SavePublicKeyRingBundle (PgpPublicKeyRingBundle bundle)
        {
            this.PublicKeyRingBundle = bundle;
            this.SavePublicKeyRingBundle ();
        }

        /// <summary>
        ///     Gets the password for the specified key, used as sink by MimeKit to decrypt private key.
        /// </summary>
        /// <returns>The password for the requested key</returns>
        /// <param name="key">The key to retrieve the password for</param>
        protected override string GetPasswordForKey (PgpSecretKey key)
        {
            var enumerator = key.UserIds.GetEnumerator ();
            enumerator.MoveNext ();
            LastUsedUserId = enumerator.Current.ToString ();
            LastUsedUserId = LastUsedUserId.Substring (LastUsedUserId.IndexOfEx ("<") + 1);
            LastUsedUserId = LastUsedUserId.Substring (0, LastUsedUserId.IndexOfEx (">"));
            if (Passwords != null) {

                // Multiple passwords, need to figure out which to use to release private key.
                return FromEmailMappedPasswords (key);

            } else {

                // Simple password.
                return Password;
            }
        }

        /*
         * Returns the password from an email address or a fingerprint supplied.
         */
        private string FromEmailMappedPasswords (PgpSecretKey key)
        {
            // Looping through all MailboxAddresses we've got.
            foreach (var idxMailbox in Passwords) {

                // Checking if we have the password for this key in our list.
                if (idxMailbox.Mailbox is SecureMailboxAddress) {

                    // Using fingerprint.
                    if ((idxMailbox.Mailbox as SecureMailboxAddress).Fingerprint.ToLower () ==
                        BitConverter.ToString (key.PublicKey.GetFingerprint ()).Replace ("-", "").ToLower ())
                        return idxMailbox.Password;

                } else {

                    // Using UserIds.
                    foreach (string idxUserId in key.UserIds) {

                        if (idxUserId.IndexOfEx (idxMailbox.Mailbox.Address) != -1) {

                            // Returning associated password for key.
                            return idxMailbox.Password;
                        }
                    }
                }
            }

            // Throwing exception since we found no password for requested key, showing user the first UserId object from Secret Key.
            throw new SecurityException (string.Format ("No password supplied for GnuPG private key '{0}'", LastUsedUserId));
        }

        /// <summary>
        ///     Overridden to make sure we release our lock.
        /// </summary>
        /// <returns>The dispose.</returns>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected override void Dispose (bool disposing)
        {
            if (disposing) {
                if (_write)
                    _lock.ExitWriteLock ();
                else
                    _lock.ExitReadLock ();
            }
            base.Dispose (disposing);
        }
    }
}

