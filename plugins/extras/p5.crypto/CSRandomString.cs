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
using p5.exp;
using p5.core;

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
        [ActiveEvent (Name = "p5.crypto.create-random")]
        public static void p5_crypto_create_random (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new ArgsRemover (e.Args)) {

                // Creating a new random number and returning to caller
                using (RNGCryptoServiceProvider csRandomGenerator = new RNGCryptoServiceProvider ()) {

                    // Creating buffer byte array to hold results, in specified resolution, defaulting to 24 bytes
                    byte[] buffer = new byte [e.Args.GetExChildValue ("resolution", context, 24)];

                    // Filling buffer with random bytes
                    csRandomGenerator.GetBytes (buffer);

                    // Checking if caller wants "raw bytes"
                    if (e.Args.GetExChildValue ("raw", context, false)) {

                        // Returning raw bytes
                        e.Args.Value = buffer;
                    } else {

                        // Converting buffer bytes to base64 encoded string and returning to caller
                        e.Args.Value = Convert.ToBase64String (buffer);
                    }
                }
            }
        }
    }
}
