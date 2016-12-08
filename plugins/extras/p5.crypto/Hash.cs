/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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
        [ActiveEvent (Name = "sha256-hash")]
        public static void sha256_hash (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Retrieving value to hash as a single string
                var whatToHash = XUtil.Single<byte[]> (context, e.Args);

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
        [ActiveEvent (Name = "sha512-hash")]
        public static void sha512_hash (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Retrieving value to hash as a single string
                var whatToHash = XUtil.Single<byte[]> (context, e.Args);

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
