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
using System.Linq;
using p5.exp;
using p5.core;
using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace p5.crypto.helpers
{
    /*
     * Common helper class for enumerations in a GnuPrivacyContext.
     */
    static class PGPKeyIterator
    {
        // Delegates to use as callback during iteration.
        internal delegate void MatchingPrivateKeysDelegate (PgpSecretKey ctx);
        internal delegate void MatchingPublicKeysDelegate (PgpPublicKey ctx);

        /*
         * Iterates all secret keys in all keyrings in GnuPrivacyContext, and invokes given delegate for every key matching filter condition.
         */
        internal static void Find (ApplicationContext context, Node args, MatchingPrivateKeysDelegate functor, bool write)
        {
            // House cleaning.
            using (new ArgsRemover (args, true)) {

                // Storing all filter given by caller in list, to avoid destroying enumeration iterator, due to modifying collection in caller's callback.
                var filters = XUtil.Iterate<string> (context, args).Where (ix => ix != null).ToList ();

                /* 
                 * Retrieving GnuPG context to let MimeKit import keys into GnuPG database.
                 * Making sure we retrieve it in mode specified by caller.
                 */
                using (var ctx = context.RaiseEvent (".p5.crypt.get-pgp-context", new Node ("", write)).Get<OpenPgpContext> (context)) {

                    // Iterating all secret keyrings.
                    foreach (PgpSecretKeyRing idxRing in ctx.SecretKeyRingBundle.GetKeyRings ()) {

                        // Iterating all keys in currently iterated secret keyring.
                        foreach (PgpSecretKey idxSecretKey in idxRing.GetSecretKeys ()) {

                            // Checking if caller provided filters, and if not, yielding "everything".
                            if (filters.Count == 0) {

                                // No filters provided, matching everything.
                                functor (idxSecretKey);

                            } else {

                                // Iterating all filters given by caller.
                                foreach (var idxFilter in filters) {

                                    // Checking if current filter is a match.
                                    if (IsMatch (idxSecretKey.PublicKey, idxFilter)) {

                                        // Invoking callback supplied by caller, and breaking current filter enumeration, to avoid adding key twice.
                                        functor (idxSecretKey);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /*
         * Iterates all public keys in all keyrings in GnuPrivacyContext, and invokes given delegate for every key matching filter condition.
         */
        internal static void Find (ApplicationContext context, Node args, MatchingPublicKeysDelegate functor, bool write)
        {
            // House cleaning.
            using (new ArgsRemover (args, true)) {

                // Storing all filter given by caller in list, to avoid destroying enumeration iterator, due to modifying collection in caller's callback.
                var filters = XUtil.Iterate<string> (context, args).Where (ix => ix != null).ToList ();

                // Creating new GnuPG context.
                using (var ctx = context.RaiseEvent (".p5.crypt.get-pgp-context", new Node ("", write)).Get<OpenPgpContext> (context)) {

                    // Iterating all secret keyrings.
                    foreach (PgpPublicKeyRing idxRing in ctx.PublicKeyRingBundle.GetKeyRings ()) {

                        // Iterating all keys in currently iterated secret keyring.
                        foreach (PgpPublicKey idxPublicKey in idxRing.GetPublicKeys ()) {

                            // Verifying that this is a normal plain public key.
                            // Notice, we only return keys with at least one User ID.
                            if (!idxPublicKey.GetUserIds ().GetEnumerator ().MoveNext ())
                                continue; // Probably just a signature for another key, or something.

                            // Checking if caller provided filters, and if not, yielding "everything".
                            if (filters.Count == 0) {

                                // No filters provided, matching everything.
                                functor (idxPublicKey);

                            } else {

                                // Iterating all filters given by caller.
                                foreach (var idxFilter in filters) {

                                    // Checking if current filter is a match.
                                    if (IsMatch (idxPublicKey, idxFilter)) {

                                        // Invoking callback supplied by caller, and breaking current filter enumeration, to avoid adding key twice.
                                        functor (idxPublicKey);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /*
         * Checks to see if public key matches filter and returns true if so.
         */
        internal static bool IsMatch (PgpPublicKey key, string filter, bool matchUserID = true)
        {
            // Checking fingerprint.
            var fingerprint = BitConverter.ToString (key.GetFingerprint ()).Replace ("-", "").ToLower ();
            if (fingerprint == filter.ToLower ().ToLower ())
                return true;

            // Checking keyID.
            var keyID = ((int)key.KeyId).ToString ("X").ToLower ();
            if (keyID == filter.ToLower ())
                return true;

            // Enumerating user IDs for key, but only if caller has specified we should search in UserIDs.
            if (matchUserID) {

                // Enumerating UerIDs looking for a match.
                foreach (var idxUserID in key.GetUserIds ()) {

                    // Checking if user ID is a string, and if so, checking for a match.
                    var userID = idxUserID as string;
                    if (userID != null) {

                        // Checking if currently iterated userID contains specified filter.
                        if (userID.ToLower ().Contains (filter.ToLower ()))
                            return true;
                    }
                }
            }

            // No match!
            return false;
        }
    }
}

