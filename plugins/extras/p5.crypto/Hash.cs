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
    ///     Helper class to create MD5 hash of whatever input given
    /// </summary>
    public static class Hash
    {
        /// <summary>
        ///     Creates a base64 encoded MD5 hash string of input given
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "md5-hash", Protection = EventProtection.LambdaClosed)]
        private static void md5_hash (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Retrieving value to hash as a single string
                var whatToHash = XUtil.Single<string> (context, e.Args, true);

                // Creating MD5 hash, and returning as value of args
                using (var md5 = MD5.Create ()) {

                    // Returning MD5 hash as base64 encoded string
                    e.Args.Value = Convert.ToBase64String (md5.ComputeHash (Encoding.UTF8.GetBytes (whatToHash)));
                }
            }
        }

        /// <summary>
        ///     Creates a base64 encoded Sha1 hash string of input given
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "sha1-hash", Protection = EventProtection.LambdaClosed)]
        private static void sha1_hash (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Retrieving value to hash as a single string
                var whatToHash = XUtil.Single<string> (context, e.Args, true);

                // Creating MD5 hash, and returning as value of args
                using (var sha1 = SHA1.Create ()) {

                    // Returning MD5 hash as base64 encoded string
                    e.Args.Value = Convert.ToBase64String (sha1.ComputeHash (Encoding.UTF8.GetBytes (whatToHash)));
                }
            }
        }

        /// <summary>
        ///     Creates a base64 encoded Sha256 hash string of input given
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "sha256-hash", Protection = EventProtection.LambdaClosed)]
        private static void sha256_hash (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Retrieving value to hash as a single string
                var whatToHash = XUtil.Single<string> (context, e.Args, true);

                // Creating MD5 hash, and returning as value of args
                using (var sha256 = SHA256.Create ()) {

                    // Returning MD5 hash as base64 encoded string
                    e.Args.Value = Convert.ToBase64String (sha256.ComputeHash (Encoding.UTF8.GetBytes (whatToHash)));
                }
            }
        }
    }
}
