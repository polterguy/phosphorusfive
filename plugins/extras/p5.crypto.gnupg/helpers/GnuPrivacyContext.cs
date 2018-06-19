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
using System.Threading;
using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace p5.crypto.gnupg.helpers
{
    /// <summary>
    ///     Gnu Privacy Guard PGP context for accessing PGP keys from GnuPG.
    /// </summary>
    public class GnuPrivacyContext : GnuPGContext
    {
        static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim ();
        readonly bool _write;
        bool _disposed = false;

        /*
         * Necessary to be able to register as a cryptography context for MimeKit.
         */
        public GnuPrivacyContext ()
        { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:p5.crypto.helpers.GnuPrivacyContext"/> class.
        /// </summary>
        /// <param name="write">If set to <c>true</c> write.</param>
        public GnuPrivacyContext (bool write, string fingerprint, string password)
        {
            if ((password != null && fingerprint == null) || (password == null && fingerprint != null))
                throw new ArgumentException ("If you supply a [password] or [fingerprint] to [.p5.crypto.pgp-keys.context.create] you must supply both");
            _write = write;
            if (write)
                _lock.EnterWriteLock ();
            else
                _lock.EnterReadLock ();
            Password = password;
            Fingerprint = fingerprint;
        }

        /// <summary>
        ///     Gets or sets the password to retrieve a private key from GnuPG.
        /// </summary>
        /// <value>The password necessary to retrieve key</value>
        private string Password {
            get;
            set;
        }
        
        /// <summary>
        ///     Gets or sets the fingerprint to retrieve a private key from GnuPG.
        /// </summary>
        /// <value>The fingerprint necessary to retrieve key</value>
        private string Fingerprint {
            get;
            set;
        }

        /// <summary>
        ///     Gets the password for the specified key, used as sink by MimeKit to decrypt private key.
        /// </summary>
        /// <returns>The password for the requested key</returns>
        /// <param name="key">The key to retrieve the password for</param>
        protected override string GetPasswordForKey (PgpSecretKey key)
        {
            if (BitConverter.ToString (key.PublicKey.GetFingerprint ()).Replace ("-", "").ToLower () != Fingerprint)
                throw new ArgumentException ("Wrong PGP key");

            // Returning password.
            return Password;
        }

        /// <summary>
        ///     Making sure we release our lock.
        /// </summary>
        /// <returns>The dispose.</returns>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected override void Dispose (bool disposing)
        {
            if (disposing && !_disposed) {
                _disposed = true;
                if (_write)
                    _lock.ExitWriteLock ();
                else
                    _lock.ExitReadLock ();
            }
            base.Dispose (disposing);
        }
    }
}

