/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using p5.core;
using p5.exp;

namespace phosphorus.crypto
{
    /// <summary>
    ///     Helper class to enrypt information
    /// </summary>
    public static class Encrypt
    {
        /// <summary>
        ///     Creates an encrypted cipher text and returns to caller
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.encrypt", Protection = EventProtection.LambdaClosed)]
        private static void p5_crypto_encrypt (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Retrieving all entities to encrypt
                List<string> entitiesToEncrypt = new List<string> (XUtil.Iterate<string> (context, e.Args, true));

                // Getting PGP public key to use for encryption
                var pgpPublicKey = e.Args.GetChildValue<string> ("email", context, null);

                // Figuring out which MIME type to use for encryption content
                if (entitiesToEncrypt.Count == 1) {

                    // Caller only wants to encrypt a single entity, hence using "text/plain" as root MimeEntity
                    var mimeEntity = new Node ("")
                        .Add ("text", "plain").LastChild
                            .Add ("encryption").LastChild
                                .Add ("email", pgpPublicKey).Parent
                            .Add ("content", entitiesToEncrypt [0]).Root;

                    // Using [p5.mime.create] as actual encryption implementation
                    e.Args.Value = context.RaiseNative ("p5.mime.create", mimeEntity).Get<string> (context);
                } else {

                    // Caller wants to encrypt multiple entities, hence using "multipart/mixed" as root MimeEntity
                    var mimeEntity = new Node ("")
                        .Add ("multipart", "mixed").LastChild
                            .Add ("encryption").LastChild
                                .Add ("email", pgpPublicKey).Parent;
                    foreach (var idxPlainText in entitiesToEncrypt) {

                        // Adding currently iterated entity as "text/plain" MimeEntity
                        mimeEntity.Add ("text", "plain")
                            .Add ("content", idxPlainText);
                    }

                    // Using [p5.mime.create] as actual encryption implementation
                    e.Args.Value = context.RaiseNative ("p5.mime.create", mimeEntity.Root);
                }
            }
        }
    }
}
