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
     * Common helper class for enumerating your PGP keys in your PGP context.
     */
    static class PGPKeyIterator
    {
        // Delegates to use as callback during iteration.
        internal delegate void MatchingSecretKeyRingDelegate (OpenPgpContext ctx, PgpSecretKeyRing keyRing);
        internal delegate void MatchingPublicKeyRingDelegate (OpenPgpContext ctx, PgpPublicKeyRing keyRing);

        /*
         * Iterates all secret keys in all keyrings in PGP context, and invokes given delegate for every key matching filter condition.
         */
        internal static void Find (ApplicationContext context, Node args, MatchingSecretKeyRingDelegate functor, bool write)
        {
            // House cleaning.
            using (new ArgsRemover (args, true)) {

                // Storing all filter given by caller in list, to avoid destroying enumeration iterator, due to modifying collection in caller's callback.
                var filters = XUtil.Iterate<string> (context, args).Where (ix => ix != null).Select (ix => ix.ToLower ()).ToList ();

                /* 
                 * Retrieving GnuPG context to let MimeKit import keys into GnuPG database.
                 * Making sure we retrieve it in mode specified by caller.
                 */
                using (var ctx = context.RaiseEvent (".p5.crypto.pgp-keys.context.create", new Node ("", write)).Get<OpenPgpContext> (context)) {

                    // Iterating all secret keyrings.
                    foreach (PgpSecretKeyRing idxRing in ctx.SecretKeyRingBundle.GetKeyRings ()) {

                        // Checking if caller provided filters, and if not, yielding "everything".
                        if (filters.Count == 0) {

                            // No filters provided, matching everything.
                            functor (ctx, idxRing);

                        } else {

                            // Checking if key exists in filter.
                            if (filters.Any (ix => IsMatch (idxRing.GetPublicKey (), ix)))
                                functor (ctx, idxRing);
                        }
                    }
                }
            }
        }

        /*
         * Iterates all public keys in all keyrings in GnuPrivacyContext, and invokes given delegate for every key matching filter condition.
         */
        internal static void Find (ApplicationContext context, Node args, MatchingPublicKeyRingDelegate functor, bool write)
        {
            // House cleaning.
            using (new ArgsRemover (args, true)) {

                // Storing all filter given by caller in list, to avoid destroying enumeration iterator, due to modifying collection in caller's callback.
                var filters = XUtil.Iterate<string> (context, args).Where (ix => ix != null).Select (ix => ix.ToLower ()).ToList ();

                // Creating new GnuPG context.
                using (var ctx = context.RaiseEvent (".p5.crypto.pgp-keys.context.create", new Node ("", write)).Get<OpenPgpContext> (context)) {

                    // Iterating all secret keyrings.
                    foreach (PgpPublicKeyRing idxRing in ctx.PublicKeyRingBundle.GetKeyRings ()) {

                        // Checking if caller provided filters, and if not, yielding "everything".
                        if (filters.Count == 0) {

                            // No filters provided, matching everything.
                            functor (ctx, idxRing);

                        } else {

                            // Checking if key exists in filter.
                            if (filters.Any (ix => IsMatch (idxRing.GetPublicKey (), ix)))
                                functor (ctx, idxRing);
                        }
                    }
                }
            }
        }

        /*
         * Checks to see if public key matches filter and returns true if so.
         */
        internal static bool IsMatch (PgpPublicKey key, string filter)
        {
            // Checking fingerprint.
            var fingerprint = BitConverter.ToString (key.GetFingerprint ()).Replace ("-", "").ToLower ();
            if (fingerprint == filter)
                return true;

            // Checking key ID.
            var keyID = ((int)key.KeyId).ToString ("X").ToLower ();
            if (keyID == filter)
                return true;

            // Enumerating user IDs looking for a "contains" match.
            foreach (var idxUserID in key.GetUserIds ()) {

                // Checking if user ID is a string, and if so, checking for a match.
                var userID = idxUserID as string;
                if (userID != null) {

                    // Checking if currently iterated userID contains specified filter.
                    if (userID.ToLower ().Contains (filter))
                        return true;
                }
            }

            // No match.
            return false;
        }
    }
}

