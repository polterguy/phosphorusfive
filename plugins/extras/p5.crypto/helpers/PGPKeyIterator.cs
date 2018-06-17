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
using System.Collections.Generic;
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

        // Helper delegate to extract commonalities between methods.
        private delegate void CommonContextDelegate (OpenPgpContext ctx, List<string> filters);

        /*
         * Iterates all secret keys in all keyrings in PGP context,
         * and invokes given delegate for every key matching filter condition.
         */
        internal static void Find (ApplicationContext context, Node args, MatchingSecretKeyRingDelegate functor, bool write, bool matchUserIds = true)
        {
            // Invoking common helper.
            Find (context, args, delegate (OpenPgpContext ctx, List<string> filters) {

                // Iterating all secret keyrings.
                foreach (PgpSecretKeyRing idxRing in ctx.SecretKeyRingBundle.GetKeyRings ()) {

                    // Checking if key exists in filter.
                    if (IsMatch (idxRing.GetPublicKey (), filters, matchUserIds))
                        functor (ctx, idxRing);
                }
            }, write);
        }

        /*
         * Iterates all public keys in all keyrings in GnuPrivacyContext,
         * and invokes given delegate for every key matching filter condition.
         */
        internal static void Find (ApplicationContext context, Node args, MatchingPublicKeyRingDelegate functor, bool write, bool matchUserIds = true)
        {
            // Invoking common helper.
            Find (context, args, delegate (OpenPgpContext ctx, List<string> filters) {

                // Iterating all public keyrings.
                foreach (PgpPublicKeyRing idxRing in ctx.PublicKeyRingBundle.GetKeyRings ()) {

                    // Checking if key exists in filter.
                    if (IsMatch (idxRing.GetPublicKey (), filters, matchUserIds))
                        functor (ctx, idxRing);
                }
            }, write);
        }

        /*
         * Helper method for above.
         */
        private static void Find (ApplicationContext context, Node args, CommonContextDelegate functor, bool write)
        {
            // House cleaning.
            using (new ArgsRemover (args, true)) {

                /*
                 * Storing all filters given by caller in list, to avoid destroying enumeration iterator,
                 * due to modifying collection in caller's callback.
                 */
                var filters = XUtil.Iterate<string> (context, args).Where (ix => ix != null).Select (ix => ix.ToLower ()).ToList ();

                // Creating PGP context.
                using (var ctx = context.RaiseEvent (".p5.crypto.pgp-keys.context.create", new Node ("", write)).Get<OpenPgpContext> (context)) {

                    // Invoking worker functor.
                    functor (ctx, filters);
                }
            }
        }

        /*
         * Checks to see if public key matches filter and returns true if so.
         */
        internal static bool IsMatch (PgpPublicKey key, List<string> filters, bool matchUserIds)
        {
            // Checking if there are no filters, at which point we return everything.
            if (filters.Count == 0)
                return true;

            // Looping through each filter.
            foreach (var filter in filters) {

                // Checking fingerprint.
                var fingerprint = Fingerprint.FingerprintString (key.GetFingerprint ());
                if (fingerprint == filter)
                    return true;

                // Checking key ID.
                var keyID = ((int)key.KeyId).ToString ("X").ToLower ();
                if (keyID == filter)
                    return true;

                // Checking if caller wants to match user IDs.
                if (matchUserIds) {

                    // Iterating all user IDs.
                    foreach (var idxUserID in key.GetUserIds ()) {

                        // Checking if user ID is a string, and if so, checking for a match.
                        var userID = idxUserID as string;
                        if (userID != null) {

                            // Checking if currently iterated userID contains specified filter.
                            if (userID.ToLower ().Contains (filter))
                                return true;
                        }
                    }
                }
            }

            // No match.
            return false;
        }
    }
}

