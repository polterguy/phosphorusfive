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
using System.Text;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using MimeKit.Cryptography;
using p5.exp;
using p5.core;
using p5.crypto.helpers;

namespace p5.crypto
{
    /// <summary>
    ///     Class wrapping importing PGP keys.
    /// </summary>
    public static class ImportPGPKey
    {
        /// <summary>
        ///     Imports the supplied public key(s) into public PGP keychain.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.pgp-keys.public.import")]
        static void p5_crypto_pgp_keys_public_import (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning.
            using (new ArgsRemover (e.Args, true)) {

                // Creating new GnuPG context.
                using (var ctx = context.RaiseEvent (".p5.crypto.pgp-keys.context.create", new Node ("", true)).Get<OpenPgpContext> (context)) {

                    // Looping through each public key (in ascii armored format) and importing into context.
                    foreach (var idxKey in XUtil.Iterate<string> (context, e.Args)) {

                        // Creating armored input stream to wrap key.
                        using (var memStream = new MemoryStream (Encoding.UTF8.GetBytes (idxKey.Replace ("\r\n", "\n")))) {
                            using (var armored = new ArmoredInputStream (memStream)) {
                                var key = new PgpPublicKeyRing (armored);
                                ctx.Import (key);
                                e.Args.Add (BitConverter.ToString (key.GetPublicKey ().GetFingerprint ()).Replace ("-", ""));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Imports the supplied private key(s) into private PGP keychain.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.pgp-keys.private.import")]
        static void p5_crypto_pgp_keys_private_import (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning.
            using (new ArgsRemover (e.Args, true)) {

                // Creating new GnuPG context.
                using (var ctx = context.RaiseEvent (".p5.crypto.pgp-keys.context.create", new Node ("", true)).Get<OpenPgpContext> (context)) {

                    // Looping through each public key (in ascii armored format) and importing into GnuPG context.
                    foreach (var idxKey in XUtil.Iterate<string> (context, e.Args)) {

                        // Creating armored input stream to wrap key.
                        using (var memStream = new MemoryStream (Encoding.UTF8.GetBytes (idxKey.Replace ("\r\n", "\n")))) {
                            using (var armored = new ArmoredInputStream (memStream)) {
                                var key = new PgpSecretKeyRing (armored);
                                ctx.Import (key);
                                e.Args.Add (BitConverter.ToString (key.GetPublicKey ().GetFingerprint ()).Replace ("-", ""));
                            }
                        }
                    }
                }
            }
        }
    }
}

