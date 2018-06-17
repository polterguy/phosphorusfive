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
using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using p5.core;
using p5.crypto.helpers;

namespace p5.crypto
{
    /// <summary>
    ///     Class wrapping the meta events for PGP keys in Phosphorus Five.
    /// </summary>
    public static class GnuPGKeys
    {
        /// <summary>
        ///     Lists all public keys matching the given filter from the PGP context.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.pgp-keys.public.list")]
        static void p5_crypto_pgp_keys_public_list (ApplicationContext context, ActiveEventArgs e)
        {
            // Using common helper to iterate all public keyrings matching filter.
            PGPKeyIterator.Find (context, e.Args, delegate (OpenPgpContext ctx, PgpPublicKeyRing keyring) {
                
                // Retrieving fingerprint of currently iterated key, and returning to caller.
                var fingerprint = Fingerprint.FingerprintString (keyring.GetPublicKey ().GetFingerprint ());
                e.Args.Add (fingerprint);

            }, false);
        }

        /// <summary>
        ///     Lists all private keys matching the given filter from the PGP context.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.pgp-keys.private.list")]
        static void p5_crypto_pgp_keys_private_list (ApplicationContext context, ActiveEventArgs e)
        {
            // Using common helper to iterate all secret keyrings matching filter.
            PGPKeyIterator.Find (context, e.Args, delegate (OpenPgpContext ctx, PgpSecretKeyRing keyring) {

                // Retrieving fingerprint of currently iterated key, and returning to caller.
                var fingerprint = Fingerprint.FingerprintString (keyring.GetPublicKey ().GetFingerprint ());
                e.Args.Add (fingerprint);

            }, false);
        }

        /// <summary>
        ///     Returns the details (meta information) about all specified PGP key.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.pgp-keys.get-details")]
        static void p5_crypto_pgp_keys_get_details (ApplicationContext context, ActiveEventArgs e)
        {
            /*
             * Using common helper to iterate all public keys, assuming that if caller
             * has supplied a fingerprint or key-id to a private key, the public key
             * will also exist in PGP context.
             */
            PGPKeyIterator.Find (context, e.Args, delegate (OpenPgpContext ctx, PgpPublicKeyRing keyring) {

                // This key is matching specified filter criteria.
                var key = keyring.GetPublicKey ();
                var fingerprint = Fingerprint.FingerprintString (key.GetFingerprint ());
                var node = e.Args.Add (fingerprint).LastChild;
                node.Add ("id", ((int)key.KeyId).ToString ("X"));
                node.Add ("algorithm", key.Algorithm.ToString ());
                node.Add ("strength", key.BitStrength);
                node.Add ("creation-time", key.CreationTime);
                node.Add ("is-encryption-key", key.IsEncryptionKey);
                node.Add ("is-master-key", key.IsMasterKey);
                node.Add ("is-revoked", key.IsRevoked ());
                node.Add ("version", key.Version);
                DateTime expires = key.CreationTime.AddSeconds (key.GetValidSeconds ());
                node.Add ("expires", expires);

                // Returning all user IDs that are strings to caller.
                foreach (var idxUserId in key.GetUserIds ()) {
                    if (idxUserId is string)
                        node.FindOrInsert ("user-ids").Add ("", idxUserId);
                }

                // Adding key IDs of all keys that have signed this key.
                foreach (PgpSignature signature in key.GetSignatures ()) {
                    node.FindOrInsert ("signed-by").Add (((int)signature.KeyId).ToString ("X"), signature.CreationTime);
                }

            }, false);
        }

        /// <summary>
        ///     Returns the specified public PGP keys.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.pgp-keys.public.get")]
        static void p5_crypto_pgp_keys_public_get (ApplicationContext context, ActiveEventArgs e)
        {
            // Using common helper to iterate all secret keys.
            PGPKeyIterator.Find (context, e.Args, delegate (OpenPgpContext ctx, PgpPublicKeyRing keyring) {

                // Retrieving fingerprint of currently iterated key, and returning to caller.
                var key = keyring.GetPublicKey ();
                var fingerprint = Fingerprint.FingerprintString (key.GetFingerprint ());
                var node = e.Args.Add (fingerprint).LastChild;

                // Returning public key as armored ASCII.
                using (var memStream = new MemoryStream ()) {
                    using (var armored = new ArmoredOutputStream (memStream)) {
                        key.Encode (armored);
                        armored.Flush ();
                    }
                    memStream.Flush ();
                    memStream.Position = 0;
                    var sr = new StreamReader (memStream);
                    node.Value = sr.ReadToEnd ();
                }
            }, false);
        }

        /// <summary>
        ///     Deletes a public key from your public PGP keychain.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.pgp-keys.public.delete")]
        static void p5_crypto_pgp_keys_public_delete (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving server's main PGP key to make sure caller doesn't accidentally delete that key.
            var serverKey = context.RaiseEvent ("p5.auth.pgp.get-fingerprint").Get<string> (context);

            // Using common helper to iterate all secret keys.
            PGPKeyIterator.Find (context, e.Args, delegate (OpenPgpContext ctx, PgpPublicKeyRing keyRing) {

                /*
                 * Notice, since server would effectively become useless if we allowed the caller to
                 * accidentally delete the main server PGP key, we check that the currently iterated key
                 * is not the server's main key.
                 */
                if (Fingerprint.FingerprintString (keyRing.GetPublicKey ().GetFingerprint ()) != serverKey) {

                    // Deleting key.
                    ctx.Delete (keyRing);
                }

            }, true, false);
        }

        /// <summary>
        ///     Deletes a public key from your private PGP keychain.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.pgp-keys.private.delete")]
        static void p5_crypto_pgp_keys_private_delete (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving server's main PGP key to make sure caller doesn't accidentally delete that key.
            var serverKey = context.RaiseEvent ("p5.auth.pgp.get-fingerprint").Get<string> (context);

            // Using common helper to iterate all secret keys.
            PGPKeyIterator.Find (context, e.Args, delegate (OpenPgpContext ctx, PgpSecretKeyRing keyRing) {

                /*
                 * Notice, since server would effectively become useless if we allowed the caller to
                 * accidentally delete the main server PGP key, we check that the currently iterated key
                 * is not the server's main key.
                 */
                if (Fingerprint.FingerprintString (keyRing.GetPublicKey ().GetFingerprint ()) != serverKey) {

                    // Deleting key.
                    ctx.Delete (keyRing);
                }

            }, true, false);
        }
    }
}

