/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
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
using p5.exp;
using p5.core;
using p5.mime.helpers;
using p5.exp.exceptions;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace p5.mime
{
    /// <summary>
    ///     Class wrapping the MIME creation features of Phosphorus Five
    /// </summary>
    public static class GnuPGKeys
    {
        /// <summary>
        ///     Lists all private keys matching the given filter from the GnuPG database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.list-private-keys")]
        private static void p5_crypto_list_private_keys (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Checking if user provided a filter
                string filter = e.Args.GetExValue<string> (context, null);

                // Creating new GnuPG context
                using (var ctx = new GnuPrivacyContext ()) {

                    // Looping through each secret key in GnuPG database
                    foreach (PgpSecretKeyRing idxRing in ctx.SecretKeyRingBundle.GetKeyRings ()) {

                        // Looping through each secret key in keyring
                        foreach (PgpSecretKey idxSecretKey in idxRing.GetSecretKeys ()) {

                            // Looping through each UserID in key
                            foreach (var idxIdentity in idxSecretKey.UserIds) {

                                // Converting to string, before checking for a match, but only if object actually is a string
                                if (idxIdentity is string) {
                                    var identity = idxIdentity.ToString ();

                                    // Checking if filter is not null, and if so, making sure identity of currently iterated key matches filter
                                    if (string.IsNullOrEmpty (filter) || identity.Contains (filter)) {

                                        // Returning identity and key ID to caller
                                        e.Args.Add (identity, idxSecretKey.KeyId.ToString ("X"));

                                        // We'll risk adding the same key twice unless we break here!
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all public keys matching the given filter from the GnuPG database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.list-public-keys")]
        private static void p5_crypto_list_public_keys (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Checking if user provided a filter
                string filter = e.Args.GetExValue<string> (context, null);

                // Creating new GnuPG context
                using (var ctx = new GnuPrivacyContext ()) {

                    // Looping through each public key in GnuPG database
                    foreach (PgpPublicKeyRing idxRing in ctx.PublicKeyRingBundle.GetKeyRings ()) {

                        // Looping through each key in keyring
                        foreach (PgpPublicKey idxPublicKey in idxRing.GetPublicKeys ()) {

                            // Finding identity of key
                            foreach (var idxUserID in idxPublicKey.GetUserIds()) {

                                // Converting to a string, before checking for a match, but only if object is a string
                                if (idxUserID is string) {
                                    var userID = idxUserID.ToString ();
                                
                                    // Checking if filter is not null, and if so, making sure identity of currently iterated key matches filter
                                    if (string.IsNullOrEmpty (filter) || userID.Contains (filter)) {

                                        // Returning identity and key ID to caller
                                        e.Args.Add (userID, idxPublicKey.KeyId.ToString ("X"));

                                        // We'll risk adding the same key twice unless we break here!
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all public keys matching the given filter from the GnuPG database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.get-key-details")]
        private static void p5_crypto_get_key_details (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Getting key ID to look for
                string keyID = e.Args.GetExValue<string> (context, null);
                if (string.IsNullOrEmpty (keyID))
                    throw new LambdaException ("No ID given to use for looking up key", e.Args, context);

                // Creating new GnuPG context
                using (var ctx = new GnuPrivacyContext ()) {

                    // Looping through each public key in GnuPG database
                    foreach (PgpPublicKeyRing idxRing in ctx.PublicKeyRingBundle.GetKeyRings ()) {

                        // Looping through each key in keyring
                        foreach (PgpPublicKey idxPublicKey in idxRing.GetPublicKeys ()) {

                            // Checking if this is the requested key
                            if (idxPublicKey.KeyId.ToString ("X") == keyID) {

                                // This is the key we're looking for
                                e.Args.Add ("algorithm", idxPublicKey.Algorithm.ToString ());
                                e.Args.Add ("strength", idxPublicKey.BitStrength);
                                e.Args.Add ("creation-time", idxPublicKey.CreationTime);
                                e.Args.Add ("is-encryption-key", idxPublicKey.IsEncryptionKey);
                                e.Args.Add ("is-master-key", idxPublicKey.IsMasterKey);
                                e.Args.Add ("is-revoked", idxPublicKey.IsRevoked ());
                                e.Args.Add ("version", idxPublicKey.Version);
                                DateTime expires = idxPublicKey.CreationTime.AddSeconds (idxPublicKey.GetValidSeconds ());
                                e.Args.Add ("expires", expires);
                                e.Args.Add ("fingerprint", BitConverter.ToString (idxPublicKey.GetFingerprint ()).Replace ("-", ""));
                                foreach (var idxUserId in idxPublicKey.GetUserIds()) {
                                    e.Args.FindOrCreate ("user-ids").Add ("", idxUserId);
                                }
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all public keys matching the given filter from the GnuPG database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.get-public-key")]
        private static void p5_crypto_get_public_key (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting key ID to look for
                string keyID = e.Args.GetExValue<string> (context, null);
                if (string.IsNullOrEmpty (keyID))
                    throw new LambdaException ("No ID given to use for looking up key", e.Args, context);

                // Creating new GnuPG context
                using (var ctx = new GnuPrivacyContext ()) {

                    // Looping through each public key in GnuPG database
                    foreach (PgpPublicKeyRing idxRing in ctx.PublicKeyRingBundle.GetKeyRings ()) {

                        // Looping through each key in keyring
                        foreach (PgpPublicKey idxPublicKey in idxRing.GetPublicKeys ()) {

                            // Checking if this is the requested key
                            if (idxPublicKey.KeyId.ToString ("X") == keyID) {

                                // This is the key we're looking for
                                using (var memStream = new MemoryStream ()) {
                                    using (var armored = new ArmoredOutputStream (memStream)) {
                                        idxPublicKey.Encode (armored);
                                        armored.Flush ();
                                    }
                                    memStream.Flush ();
                                    memStream.Position = 0;
                                    var sr = new StreamReader (memStream);
                                    e.Args.Value = sr.ReadToEnd ();
                                }
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Removes a private key from GnuPG database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.delete-private-key")]
        private static void p5_crypto_delete_private_key (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Creating new GnuPG context
                using (var ctx = new GnuPrivacyContext ()) {

                    // Signaler boolean
                    bool somethingWasRemoved = false;
                    var bundle = ctx.SecretKeyRingBundle;

                    // Looping through each ID given by caller
                    foreach (var idxId in XUtil.Iterate<string> (context, e.Args, true)) {

                        // Looping through each public key ring in GnuPG database until we find given ID
                        foreach (PgpSecretKeyRing idxSecretKeyRing in bundle.GetKeyRings ()) {

                            // Looping through each key in keyring
                            foreach (PgpSecretKey idxSecretKey in idxSecretKeyRing.GetSecretKeys ()) {
                                if (idxId == idxSecretKey.KeyId.ToString ("X")) {

                                    // Removing entire keyring, and signaling to save keyring bundle
                                    somethingWasRemoved = true;
                                    bundle = PgpSecretKeyRingBundle.RemoveSecretKeyRing (bundle, idxSecretKeyRing);

                                    // Breaking inner most foreach
                                    break;
                                }
                            }
                        }
                    }

                    // Checking to see if something was removed, and if so, saving GnuPG context
                    if (somethingWasRemoved)
                        ctx.SaveSecretKeyRingBundle (bundle);
                }
            }
        }

        /// <summary>
        ///     Removes a public key from GnuPG database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.delete-public-key")]
        private static void p5_crypto_delete_public_key (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Creating new GnuPG context
                using (var ctx = new GnuPrivacyContext ()) {

                    // Signaler boolean
                    bool somethingWasRemoved = false;
                    var bundle = ctx.PublicKeyRingBundle;

                    // Looping through each ID given by caller
                    foreach (var idxId in XUtil.Iterate<string> (context, e.Args, true)) {

                        // Looping through each public key ring in GnuPG database until we find given ID
                        foreach (PgpPublicKeyRing idxPublicKeyRing in bundle.GetKeyRings ()) {

                            // Looping through each key in keyring
                            foreach (PgpPublicKey idxPublicKey in idxPublicKeyRing.GetPublicKeys ()) {
                                if (idxId == idxPublicKey.KeyId.ToString ("X")) {

                                    // Removing entire keyring, and signaling to save keyring bundle
                                    somethingWasRemoved = true;
                                    bundle = PgpPublicKeyRingBundle.RemovePublicKeyRing (bundle, idxPublicKeyRing);

                                    // Breaking inner most foreach
                                    break;
                                }
                            }
                        }
                    }

                    // Checking to see if something was removed, and if so, saving GnuPG context
                    if (somethingWasRemoved)
                        ctx.SavePublicKeyRingBundle (bundle);
                }
            }
        }
    }
}

