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
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.IO;
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
        [ActiveEvent (Name = "p5.crypto.import-public-pgp-key")]
        private static void p5_crypto_import_public_pgp_key (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Creating new GnuPG context
                using (var ctx = new GnuPrivacyContext ()) {

                    // Looping through each public key (in ascii armored format) and importing into GnuPG database
                    foreach (var idxKey in XUtil.Iterate<string> (context, e.Args, true)) {

                        // Creating armored input stream to wrap key
                        using (var memStream = new MemoryStream (System.Text.UTF8Encoding.UTF8.GetBytes (idxKey))) {
                            using (var armored = new ArmoredInputStream (memStream)) {
                                var key = new PgpPublicKeyRing (armored);
                                ctx.Import (key);
                            }
                        }
                    }
                }
            }
        }
    }
}

