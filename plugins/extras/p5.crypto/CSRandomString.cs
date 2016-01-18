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
    public static class CSRandomString
    {
        /// <summary>
        ///     Creates a Cryptographically Secure Random array of bytes, and returns as base64 encoded string
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "create-cs-random-string", Protection = EventProtection.LambdaClosed)]
        public static void create_cs_random_string (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Creating a new random number and returning to caller
                using (RNGCryptoServiceProvider csRandomGenerator = new RNGCryptoServiceProvider ()) {

                    // Creating buffer byte array to hold results, in specified resolution, defaulting to 24 bytes
                    byte[] buffer = new byte [e.Args.GetExChildValue ("resolution", context, 24)];

                    // Filling buffer with random bytes
                    csRandomGenerator.GetBytes (buffer);

                    // Converting buffer bytes to base64 encoded string and returning to caller
                    e.Args.Value = Convert.ToBase64String (buffer);
                }
            }
        }
    }
}
