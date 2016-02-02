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
        [ActiveEvent (Name = "p5.crypto.import-public-pgp-key", Protection = EventProtection.LambdaClosed)]
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

