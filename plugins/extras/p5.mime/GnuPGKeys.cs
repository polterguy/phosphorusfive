/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.mime.helpers;
using helpers = p5.mime.helpers;
using p5.exp.exceptions;
using MimeKit;
using MimeKit.Cryptography;
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
        [ActiveEvent (Name = "p5.crypto.list-private-keys", Protection = EventProtection.LambdaClosed)]
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
                                        e.Args.Add (identity, idxSecretKey.KeyId);

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
        [ActiveEvent (Name = "p5.crypto.list-public-keys", Protection = EventProtection.LambdaClosed)]
        private static void p5_crypto_list_public_keys (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Checking if user provided a filter
                string filter = e.Args.GetExValue<string> (context, null);

                // Creating new GnuPG context
                using (var ctx = new GnuPrivacyContext ()) {

                    // Looping through each secret key in GnuPG database
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
                                        e.Args.Add (userID, idxPublicKey.KeyId);

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
        ///     Removes a private key from GnuPG database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.remove-private-key", Protection = EventProtection.LambdaClosed)]
        private static void p5_crypto_remove_private_key (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Creating new GnuPG context
                using (var ctx = new GnuPrivacyContext ()) {

                    // Signaler boolean
                    bool somethingWasRemoved = false;
                    var bundle = ctx.SecretKeyRingBundle;

                    // Looping through each ID given by caller
                    foreach (var idxId in XUtil.Iterate<long> (context, e.Args, true)) {

                        // Looping through each public key ring in GnuPG database until we find given ID
                        foreach (PgpSecretKeyRing idxSecretKeyRing in bundle.GetKeyRings ()) {

                            // Looping through each key in keyring
                            foreach (PgpSecretKey idxSecretKey in idxSecretKeyRing.GetSecretKeys ()) {
                                if (idxId == idxSecretKey.KeyId) {

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
        [ActiveEvent (Name = "p5.crypto.remove-public-key", Protection = EventProtection.LambdaClosed)]
        private static void p5_crypto_remove_public_key (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Creating new GnuPG context
                using (var ctx = new GnuPrivacyContext ()) {

                    // Signaler boolean
                    bool somethingWasRemoved = false;
                    var bundle = ctx.PublicKeyRingBundle;

                    // Looping through each ID given by caller
                    foreach (var idxId in XUtil.Iterate<long> (context, e.Args, true)) {

                        // Looping through each public key ring in GnuPG database until we find given ID
                        foreach (PgpPublicKeyRing idxPublicKeyRing in bundle.GetKeyRings ()) {

                            // Looping through each key in keyring
                            foreach (PgpPublicKey idxPublicKey in idxPublicKeyRing.GetPublicKeys ()) {
                                if (idxId == idxPublicKey.KeyId) {

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

