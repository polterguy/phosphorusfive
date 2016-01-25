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
    ///     Helper class to create Sha256 hash of whatever input given
    /// </summary>
    public static class Hash
    {
        /// <summary>
        ///     Creates a Sha256 hash of input given
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "sha256-hash", Protection = EventProtection.LambdaClosed)]
        public static void sha256_hash (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Retrieving value to hash as a single string
                var whatToHash = XUtil.Single<byte[]> (context, e.Args, true);

                // Creating Sha256 hash, and returning as value of args
                using (var sha256 = SHA256.Create ()) {

                    // Checking if caller wants "raw bytes"
                    if (e.Args.GetExChildValue ("raw", context, false)) {

                        // Returning Sha256 hash as raw bytes
                        e.Args.Value = sha256.ComputeHash (whatToHash);
                    } else {

                        // Returning Sha256 hash as base64 encoded string
                        e.Args.Value = Convert.ToBase64String (sha256.ComputeHash (whatToHash));
                    }
                }
            }
        }

        /// <summary>
        ///     Creates a Sha256 hash of input given
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "sha512-hash", Protection = EventProtection.LambdaClosed)]
        public static void sha512_hash (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Retrieving value to hash as a single string
                var whatToHash = XUtil.Single<byte[]> (context, e.Args, true);

                // Creating Sha256 hash, and returning as value of args
                using (var sha512 = SHA512.Create ()) {

                    // Checking if caller wants "raw bytes"
                    if (e.Args.GetExChildValue ("raw", context, false)) {

                        // Returning Sha512 hash as raw bytes
                        e.Args.Value = sha512.ComputeHash (whatToHash);
                    } else {

                        // Returning Sha256 hash as base64 encoded string
                        e.Args.Value = Convert.ToBase64String (sha512.ComputeHash (whatToHash));
                    }
                }
            }
        }
    }
}
