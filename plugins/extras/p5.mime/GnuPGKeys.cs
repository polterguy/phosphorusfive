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
        ///     Lists all private keys matching the given filter from the GnuPG database.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.list-private-keys")]
        private static void p5_crypto_list_private_keys (ApplicationContext context, ActiveEventArgs e)
        {
            // Using common helper to iterate all secret keys.
            ObjectIterator.MatchingPrivateKeys (context, e.Args, delegate (PgpSecretKey key) {

                // Retrieving fingerprint of currently iterated key, and returning to caller.
                var fingerprint = BitConverter.ToString(key.PublicKey.GetFingerprint()).Replace("-", "").ToLower();
                e.Args.Add(fingerprint);
            }, false);
        }

        /// <summary>
        ///     Lists all public keys matching the given filter from the GnuPG database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.list-public-keys")]
        private static void p5_crypto_list_public_keys (ApplicationContext context, ActiveEventArgs e)
        {
            // Using common helper to iterate all secret keys.
            ObjectIterator.MatchingPublicKeys (context, e.Args, delegate (PgpPublicKey key) {

                // Retrieving fingerprint of currently iterated key, and returning to caller.
                var fingerprint = BitConverter.ToString(key.GetFingerprint()).Replace("-", "").ToLower();
                e.Args.Add(fingerprint);
            }, false);
        }

        /// <summary>
        ///     Lists all public keys matching the given filter from the GnuPG database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.get-key-details")]
        private static void p5_crypto_get_key_details (ApplicationContext context, ActiveEventArgs e)
        {
            // Using common helper to iterate all secret keys.
            ObjectIterator.MatchingPublicKeys (context, e.Args, delegate (PgpPublicKey key) {

                // This key is matching specified filter criteria.
                var fingerprint = BitConverter.ToString(key.GetFingerprint()).Replace("-", "").ToLower();
                var node = e.Args.Add(fingerprint).LastChild;
                node.Add("id", ((int)key.KeyId).ToString("X"));
                node.Add("algorithm", key.Algorithm.ToString());
                node.Add("strength", key.BitStrength);
                node.Add("creation-time", key.CreationTime);
                node.Add("is-encryption-key", key.IsEncryptionKey);
                node.Add("is-master-key", key.IsMasterKey);
                node.Add("is-revoked", key.IsRevoked());
                node.Add("version", key.Version);
                DateTime expires = key.CreationTime.AddSeconds(key.GetValidSeconds());
                node.Add("expires", expires);
                foreach (var idxUserId in key.GetUserIds()) {
                    if (idxUserId is string)
                        node.FindOrInsert("user-ids").Add("", idxUserId);
                }
                foreach (PgpSignature signature in key.GetSignatures()) {
                    node.FindOrInsert("signed-by").Add(((int)signature.KeyId).ToString("X"), signature.CreationTime);
                }
            }, false);
        }

        /// <summary>
        ///     Lists all public keys matching the given filter from the GnuPG database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.get-public-key")]
        private static void p5_crypto_get_public_key (ApplicationContext context, ActiveEventArgs e)
        {
            // Using common helper to iterate all secret keys.
            ObjectIterator.MatchingPublicKeys (context, e.Args, delegate (PgpPublicKey key) {

                // Retrieving fingerprint of currently iterated key, and returning to caller.
                var fingerprint = BitConverter.ToString (key.GetFingerprint ()).Replace ("-", "").ToLower ();
                var node = e.Args.Add (fingerprint).LastChild;

                // This is the key we're looking for
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
        ///     Lists all private keys matching the given filter from the GnuPG database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.get-private-key")]
        private static void p5_crypto_get_private_key (ApplicationContext context, ActiveEventArgs e)
        {
            // Using common helper to iterate all secret keys.
            ObjectIterator.MatchingPrivateKeys (context, e.Args, delegate (PgpSecretKey key) {

                // Retrieving fingerprint of currently iterated key, and returning to caller.
                var fingerprint = BitConverter.ToString (key.PublicKey.GetFingerprint ()).Replace ("-", "").ToLower ();
                var node = e.Args.Add (fingerprint).LastChild;

                // This is the key we're looking for
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
		///     Signs the given public key(s).
		/// </summary>
		/// <param name="context">Application Context</param>
		/// <param name="e">Active Event arguments</param>
		[ActiveEvent(Name = "p5.crypto.sign-public-key")]
		private static void p5_crypto_sign_public_key(ApplicationContext context, ActiveEventArgs e)
		{
			// Figuring out which private key to use for signing, and doing some basic sanity check.
			var fingerprint = e.Args.GetExChildValue ("private-key", context, "").ToLower ();
			if (fingerprint == "")
		    	throw new LambdaException("No [private-key] argument supplied to [p5.crypto.sign-public-key]", e.Args, context);

			// Finding password to use to extract private key from GnuPG context, and doing some basic sanity check.
			var password = e.Args.GetExChildValue ("password", context, "");
			if (password == "")
				throw new LambdaException("No [password] argument supplied to [p5.crypto.sign-public-key] to extract your private key", e.Args, context);

			// Retrieving our private key to use for signing public key from GnuPG database.
			// Finding password to use to extract private key from GnuPG context, and doing some basic sanity check.
            var certain = e.Args.GetExChildValue ("certain", context, false);

			PgpSecretKey signingKey = null;
			using (var ctx = new GnuPrivacyContext(false)) {

				// Iterating all secret keyrings.
				foreach (PgpSecretKeyRing idxRing in ctx.SecretKeyRingBundle.GetKeyRings()) {

					// Iterating all keys in currently iterated secret keyring.
					foreach (PgpSecretKey idxSecretKey in idxRing.GetSecretKeys ()) {

						// Checking if caller provided filters, and if not, yielding "everything".
						if (BitConverter.ToString (idxSecretKey.PublicKey.GetFingerprint ()).Replace ("-", "").ToLower() == fingerprint) {

							// No filters provided, matching everything.
							signingKey = idxSecretKey;
							break;
						}
					}
					if (signingKey != null)
		    			break;
				}
			}

			// Using common helper to iterate all public keys caller wants to sign.
			PgpPublicKeyRing sRing = null;
			ObjectIterator.MatchingPublicKeys (context, e.Args, delegate (PgpPublicKey idxKey) {

				// Retrieving fingerprint of currently iterated key, and returning to caller.
				var node = e.Args.Add (BitConverter.ToString (idxKey.GetFingerprint ()).Replace ("-", "").ToLower ()).LastChild;

				// Doing the actual signing of currently iterated public key.
                sRing = new PgpPublicKeyRing (new MemoryStream (SignPublicKey (signingKey, password, idxKey, certain), false));

			}, false);

			// Creating new GnuPG context and importing signed key into context.
			using (var ctx = new GnuPrivacyContext (true)) {

				// Importing signed key.
				ctx.Import (sRing);
			}
		}

		/*
		 * Helper for above.
		 */
		private static byte[] SignPublicKey(
		    PgpSecretKey secretKey,
		    string password,
		    PgpPublicKey keyToBeSigned,
            bool isCertain)
		{
			// Extracting private key, and getting ready to create a signature.
			PgpPrivateKey pgpPrivKey = secretKey.ExtractPrivateKey (password.ToCharArray());
			PgpSignatureGenerator sGen = new PgpSignatureGenerator (secretKey.PublicKey.Algorithm, HashAlgorithmTag.Sha1);
            sGen.InitSign (isCertain ? PgpSignature.PositiveCertification : PgpSignature.CasualCertification, pgpPrivKey);

			// Creating a stream to wrap the results of operation.
			Stream os = new MemoryStream();
			BcpgOutputStream bOut = new BcpgOutputStream (os);
			sGen.GenerateOnePassVersion (false).Encode (bOut);

			// Creating a generator.
			PgpSignatureSubpacketGenerator spGen = new PgpSignatureSubpacketGenerator();
			PgpSignatureSubpacketVector packetVector = spGen.Generate();
			sGen.SetHashedSubpackets (packetVector);
			bOut.Flush();

			// Returning the signed public key.
			return PgpPublicKey.AddCertification (keyToBeSigned, sGen.Generate()).GetEncoded();
		}

		/// <summary>
		///     Removes a private key from GnuPG database
		/// </summary>
		/// <param name="context">Application Context</param>
		/// <param name="e">Active Event arguments</param>
		[ActiveEvent (Name = "p5.crypto.delete-private-key")]
        private static void p5_crypto_delete_private_key (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning.
            using (new ArgsRemover (e.Args, true)) {

                // Creating new GnuPG context.
                using (var ctx = new GnuPrivacyContext (true)) {

                    // Signaler boolean.
                    bool somethingWasRemoved = false;
                    var bundle = ctx.SecretKeyRingBundle;

                    // Looping through each ID given by caller.
                    foreach (var idxId in XUtil.Iterate<string> (context, e.Args)) {

                        // Looping through each public key ring in GnuPG database until we find given ID.
                        foreach (PgpSecretKeyRing idxSecretKeyRing in bundle.GetKeyRings ()) {

                            // Looping through each key in keyring.
                            foreach (PgpSecretKey idxSecretKey in idxSecretKeyRing.GetSecretKeys ()) {

                                // Checking for a match, making sure we do not match UserIDs.
                                if (ObjectIterator.IsMatch (idxSecretKey.PublicKey, idxId, false)) {

                                    // Removing entire keyring, and signaling to save keyring bundle.
                                    somethingWasRemoved = true;
                                    bundle = PgpSecretKeyRingBundle.RemoveSecretKeyRing (bundle, idxSecretKeyRing);

                                    // Breaking inner most foreach.
                                    break;
                                }
                            }

                            // Checking if currently iterated filter was found in currently iterated secret keyring.
                            if (somethingWasRemoved)
                                break;
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
            using (new ArgsRemover (e.Args, true)) {

                // Creating new GnuPG context
                using (var ctx = new GnuPrivacyContext (true)) {

                    // Signaler boolean
                    bool somethingWasRemoved = false;
                    var bundle = ctx.PublicKeyRingBundle;

                    // Looping through each ID given by caller
                    foreach (var idxId in XUtil.Iterate<string> (context, e.Args)) {

                        // Looping through each public key ring in GnuPG database until we find given ID
                        foreach (PgpPublicKeyRing idxPublicKeyRing in bundle.GetKeyRings ()) {

                            // Looping through each key in keyring
                            foreach (PgpPublicKey idxPublicKey in idxPublicKeyRing.GetPublicKeys ()) {

                                // Checking for a match, making sure we do not match UserIDs.
                                if (ObjectIterator.IsMatch (idxPublicKey, idxId, false)) {

                                    // Removing entire keyring, and signaling to save keyring bundle
                                    somethingWasRemoved = true;
                                    bundle = PgpPublicKeyRingBundle.RemovePublicKeyRing (bundle, idxPublicKeyRing);

                                    // Breaking inner most foreach
                                    break;
                                }
                            }

                            // Checking if currently iterated filter was found in currently iterated secret keyring.
                            if (somethingWasRemoved)
                                break;
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

