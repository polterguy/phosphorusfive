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

using System.Linq;
using p5.core;
using p5.exp.exceptions;

namespace p5.auth.helpers
{
    /// <summary>
    ///     Class wrapping PGP key features of Phosphorus Five.
    /// </summary>
    static class PGPKey
    {
        public const string GnuPgpFingerprintNodeName = "gnupg-keypair";

        /*
         * Returns PGP key's fingerprint.
         */
        public static string GetFingerprint (ApplicationContext context)
        {
            // Retrieving "auth" file in node format, for then to return fingerprint back to caller.
            return AuthFile.GetAuthFile (context).GetChildValue<string> (GnuPgpFingerprintNodeName, context);
        }
        
        /*
         * Sets the GnuPG keypair for server.
         */
        public static void SetFingerprint (ApplicationContext context, Node args, string fingerprint)
        {
            AuthFile.ModifyAuthFile (context, delegate (Node node) {
                if (node [GnuPgpFingerprintNodeName] != null)
                    throw new LambdaSecurityException ("Tried to change GnuPG keypair after initial creation", args, context);
                node.Add (GnuPgpFingerprintNodeName, fingerprint);
            });
        }
        
        /*
         * Changes the GnuPG keypair for server.
         */
        public static void ChangeServerPGPKey (ApplicationContext context, string fingerprint, Node args)
        {
            // Verifying key exists, both its public key, and its private key.
            var result = context.RaiseEvent ("p5.crypto.pgp-keys.public.list", new Node ("p5.crypto.pgp-keys.public.list", fingerprint));
            if (result.FirstChild.Name != fingerprint)
                throw new LambdaSecurityException ("The specified new public key does not exist", args, context);
            result = context.RaiseEvent ("p5.crypto.pgp-keys.private.list", new Node ("p5.crypto.pgp-keys.private.list", fingerprint));
            if (result.FirstChild.Name != fingerprint)
                throw new LambdaSecurityException ("The specified new public key does not exist", args, context);

            // Changing key.
            AuthFile.ModifyAuthFile (context, delegate (Node node) {
                node [GnuPgpFingerprintNodeName].Value = fingerprint;
            });
        }
    }
}