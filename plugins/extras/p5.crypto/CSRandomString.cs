/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Prng;
using p5.exp;
using p5.core;

namespace phosphorus.crypto
{
    /// <summary>
    ///     Helper class to create random bytes.
    /// </summary>
    public static class CSRandomString
    {
        // To make sure we only seed the RNG once, we store it cached in class, and reuse it upon consecutive invocations.
        static SecureRandom _secureRandom = null;

        /// <summary>
        ///     Creates a Cryptographically Secure Random array of bytes, and returns result to caller.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.create-random-bytes")]
        public static void p5_crypto_create_random_bytes (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Creating new secure random from BouncyCastle to use for our random bytes.
                var sr = CreateNewSecureRandom (context, e.Args);
                var buffer = new byte [e.Args.GetExChildValue ("resolution", context, 24)];
                sr.NextBytes (buffer, 0, buffer.Length);

                // Checking if caller wants "raw bytes".
                if (e.Args.GetExChildValue ("raw", context, false)) {

                    // Returning raw bytes.
                    e.Args.Value = buffer;

                } else if (e.Args.GetExChildValue ("hex", context, false)) {

                    // Returning value as hexadecimal string.
                    e.Args.Value = BitConverter.ToString (buffer).Replace ("-", string.Empty);


                } else {

                    // Converting buffer bytes to base64 encoded string and returning to caller.
                    e.Args.Value = Convert.ToBase64String (buffer);
                }
            }
        }

        /// <summary>
        ///     Creates a Cryptographically Secure Random array of bytes, and returns result to caller.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.create-random-integer")]
        public static void p5_crypto_create_random_integer (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Creating new secure random from BouncyCastle to use for our random bytes.
                var sr = CreateNewSecureRandom (context, e.Args);
                e.Args.Value = sr.Next ();
            }
        }

        /// <summary>
        ///     Creates a Cryptographically Secure Random array of bytes, and returns result to caller.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.create-random-double")]
        public static void p5_crypto_create_random_double (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Creating new secure random from BouncyCastle to use for our random bytes.
                var sr = CreateNewSecureRandom (context, e.Args);
                e.Args.Value = sr.NextDouble ();
            }
        }

        /// <summary>
        ///     Creates a Cryptographically Secure Random array of bytes, and returns result to caller.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.create-random-long")]
        public static void p5_crypto_create_random_long (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Creating new secure random from BouncyCastle to use for our random bytes.
                var sr = CreateNewSecureRandom (context, e.Args);
                e.Args.Value = sr.NextLong ();
            }
        }

        /*
         * Creates and seeds a new SecureRandom to be used for keypair creation
         */
        static SecureRandom CreateNewSecureRandom (ApplicationContext context, Node args)
        {
            // Checking if we have previously instantiated SecureRandom, and seeded it before.
            if (_secureRandom != null)
                return _secureRandom;

            // Used to to hold seed for random number generator.
            List<byte> seed = new List<byte> ();

            // First we retrieve the seed provided by caller through the [seed] argument, defaulting to "foobar" if no user seed is provided.
            seed.AddRange (Encoding.UTF8.GetBytes (args.GetExChildValue<string> ("seed", context, Guid.NewGuid ().ToString ()) ?? Guid.NewGuid ().ToString ()));

            // Then retrieving "seed generator" from BouncyCastle.
            seed.AddRange (new ThreadedSeedGenerator ().GenerateSeed (128, false));

            // Then we retrieve the server password salt.
            seed.AddRange (Encoding.UTF8.GetBytes (context.RaiseEvent (".p5.auth.get-server-salt").Get<string> (context, "in-case-server-hasn't-been-salted!!")));

            // Then we retrieve the ticks of server.
            seed.AddRange (Encoding.UTF8.GetBytes (DateTime.Now.Ticks.ToString ()));

            // At this point, we are fairly certain that we have a pretty random and cryptographically securely seeded RNG.
            _secureRandom = new SecureRandom ();
            _secureRandom.SetSeed (seed.ToArray ());

            return _secureRandom;
        }
    }
}
