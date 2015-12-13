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

/// <summary>
///     Main namespace for everything related to cryptographic services in Phosphorus Five
/// </summary>
namespace phosphorus.crypto
{
    /// <summary>
    ///     Helper class to derypt information
    /// </summary>
    public static class Decrypt
    {
        /// <summary>
        ///     Creates an encrypted cipher text and returns to caller
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.decrypt", Protection = EventProtection.LambdaClosed)]
        private static void p5_crypto_decrypt (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Retrieving cipher text
                var cipherText = XUtil.Single<string> (context, e.Args, true);

                // Retrieving PGP private key to use for decryption and password to retrieve private key from Gnu Privacy Context
                var pgpPrivateKey = e.Args.GetChildValue<string> ("email", context, null);
                var pgpPassword = e.Args ["email"].GetChildValue<string> ("password", context, null);

                // Using [p5.mime.parse] as actual decryption implementation
                Node decryptNode = new Node ("", cipherText)
                    .Add ("email", pgpPrivateKey).LastChild
                        .Add ("password", pgpPassword);
                var resultNode = context.RaiseNative ("p5.mime.parse", decryptNode.Root);
                if (resultNode.Children.Count == 1) {

                    // Only one result, returning it as value of main args
                    e.Args.Value = resultNode [0]["content"].Value;
                } else {

                    // Multiple results, returning them all as values of children node of main args
                    foreach (var idxResult in resultNode.Children) {

                        // Creating node for currently iterated result
                        e.Args.Add ("result", idxResult[0]["content"].Value);
                    }
                }
            }
        }
    }
}
