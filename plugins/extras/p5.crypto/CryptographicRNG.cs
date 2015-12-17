/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Text;
using System.Security.Cryptography;
using p5.core;
using p5.exp;

namespace phosphorus.crypto
{
    /// <summary>
    ///     Helper class to create random pieces of text
    /// </summary>
    public static class CryptographicRNG
    {
        /// <summary>
        ///     Creates a Cryptographic Random number, and returns as base64 encoded string
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "create-random", Protection = EventProtection.LambdaClosed)]
        private static void create_random (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Creating a new random number and returning to caller
                using (RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider ()) {
                    byte[] rawBytes = new byte [e.Args.GetExChildValue ("resolution", context, 24)];
                    csprng.GetBytes (rawBytes);
                    e.Args.Value = Convert.ToBase64String (rawBytes);
                }
            }
        }
    }
}
