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
using p5.exp;
using p5.core;
using p5.mime.helpers;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace p5.mime
{
    /// <summary>
    ///     Class wrapping the MIME creation features of Phosphorus Five
    /// </summary>
    public static class ImportPGPKey
    {
        /// <summary>
        ///     Imports the supplied key(s) into GnuPG database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.preview-public-pgp-key")]
        private static void p5_crypto_preview_public_pgp_key (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new ArgsRemover (e.Args, true)) {

                // Looping through each public key (in ascii armored format) and importing into GnuPG database
                foreach (var idxKey in XUtil.Iterate<string> (context, e.Args)) {

                    // Creating armored input stream to wrap key
                    using (var memStream = new MemoryStream (Encoding.UTF8.GetBytes (idxKey.Replace ("\r\n", "\n")))) {
                        using (var armored = new ArmoredInputStream (memStream)) {
                            var keys = new PgpPublicKeyRing (armored);

                            // Now returning key details to caller.
                            foreach (PgpPublicKey key in keys.GetPublicKeys ()) {
                                var node = e.Args.Add (BitConverter.ToString (key.GetFingerprint ()).Replace ("-", "")).LastChild;
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
							}  
                        }
                    }
                }
            }
        }

		/// <summary>
		///     Imports the supplied key(s) into GnuPG database
		/// </summary>
		/// <param name="context">Application Context</param>
		/// <param name="e">Active Event arguments</param>
		[ActiveEvent(Name = "p5.crypto.import-public-pgp-key")]
		private static void p5_crypto_import_public_pgp_key(ApplicationContext context, ActiveEventArgs e)
		{
			// House cleaning
			using (new ArgsRemover (e.Args, true)) {

				// Creating new GnuPG context
				using (var ctx = new GnuPrivacyContext ()) {

					// Looping through each public key (in ascii armored format) and importing into GnuPG database
					foreach (var idxKey in XUtil.Iterate<string> (context, e.Args)) {

						// Creating armored input stream to wrap key
						using (var memStream = new MemoryStream (Encoding.UTF8.GetBytes (idxKey.Replace ("\r\n", "\n")))) {
							using (var armored = new ArmoredInputStream (memStream)) {
								var key = new PgpPublicKeyRing (armored);
								ctx.Import (key);
								e.Args.Add (BitConverter.ToString (key.GetPublicKey().GetFingerprint()).Replace("-", ""));
							}
						}
					}
				}
			}
		}

		/// <summary>
		///     Imports the supplied key(s) into GnuPG database
		/// </summary>
		/// <param name="context">Application Context</param>
		/// <param name="e">Active Event arguments</param>
		[ActiveEvent (Name = "p5.crypto.import-private-pgp-key")]
        private static void p5_crypto_import_private_pgp_key (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new ArgsRemover (e.Args, true)) {

                // Creating new GnuPG context
                using (var ctx = new GnuPrivacyContext ()) {

                    // Looping through each public key (in ascii armored format) and importing into GnuPG database
                    foreach (var idxKey in XUtil.Iterate<string> (context, e.Args)) {

                        // Creating armored input stream to wrap key
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

