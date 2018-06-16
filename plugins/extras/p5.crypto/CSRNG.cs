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
using p5.exp.exceptions;
using p5.core;

namespace phosphorus.crypto
{
    /// <summary>
    ///     Helper class to create cryptographically secure random numbers.
    /// </summary>
    public static class CSRNG
    {
        /*
         * To make sure we only seed the RNG once, which could create false secureity,
         * we store it cached in class, and reuse it upon consecutive invocations.
         * 
         * Notice, according to the source code of BC, SecureRandom *should* be thread safe.
         */
        static SecureRandom _secureRandom = null;
        static object _lock = new object ();

        /// <summary>
        ///     Creates a Cryptographically Secure Random array of bytes, and returns result to caller.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.rng.create-bytes")]
        public static void p5_crypto_rng_create_bytes (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Retrieving a bunch of random bytes according to caller's specifications.
                var buffer = new byte [e.Args.GetExChildValue ("resolution", context, 24)];
                GetSecureRandom (context, e.Args).NextBytes (buffer, 0, buffer.Length);

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
        ///     Creates a Cryptographically Secure Random integer, and returns result to caller.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.rng.create-integer")]
        public static void p5_crypto_rng_create_integer (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Creating new secure random from BouncyCastle to use for our random bytes.
                e.Args.Value = GetSecureRandom (context, e.Args).Next (
                    e.Args.GetExChildValue ("min", context, int.MinValue),
                    e.Args.GetExChildValue ("max", context, int.MaxValue));
            }
        }

        /// <summary>
        ///     Creates a Cryptographically Secure Random double, and returns result to caller.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.rng.create-double")]
        public static void p5_crypto_rng_create_double (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Creating new secure random from BouncyCastle to use for our random bytes.
                e.Args.Value = GetSecureRandom (context, e.Args).NextDouble ();
            }
        }

        /// <summary>
        ///     Creates a Cryptographically Secure Random long, and returns result to caller.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.rng.create-long")]
        public static void p5_crypto_rng_create_long (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Creating new secure random from BouncyCastle to use for our random bytes.
                e.Args.Value = GetSecureRandom (context, e.Args).NextLong ();
            }
        }

        /// <summary>
        ///     Seeds the Cryptographically Secure RNG generator.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.rng.seed")]
        public static void p5_crypto_rng_seed (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args, true)) {

                // Making sure caller supplied a [seed] argument.
                if (string.IsNullOrEmpty (e.Args.GetExChildValue<string> ("seed", context, null)))
                    throw new LambdaException ("No [seed] argument supplied to [p5.crypto.rng.seed]", e.Args, context);

                /* 
                 * Since this method will correctly instantiate our SecureRandom, and accommodate for any [seed] arguments,
                 * we use it as a convenience implementation directly.
                 */
                GetSecureRandom (context, e.Args);
            }
        }

        /// <summary>
        ///     Returns the SecureRandom instance to caller.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.crypto.rng.secure-random.get")]
        public static void _p5_crypto_rng_secure_random_get (ApplicationContext context, ActiveEventArgs e)
        {
            // Returning the SecureRandom instance to caller.
            e.Args.Value = GetSecureRandom (context, e.Args);
        }

        /*
         * Creates and seeds a new SecureRandom to be used for keypair creation
         */
        static SecureRandom GetSecureRandom (ApplicationContext context, Node args)
        {
            // Thread synchronization.
            lock (_lock) {

                // Checking if we have previously instantiated SecureRandom, and seeded it before.
                if (_secureRandom != null) {

                    // Checking if caller supplied a [seed] argument.
                    if (args ["seed"] != null) {

                        /*
                         * Seeding instance, making sure we hash [seed] to further eliminate predictability in cases
                         * where parts of the seed is known by an adversary for some reasons.
                         * 
                         * Hence, by hashing it, we completely "randomize" the entire sequence of bytes, even if only one
                         * single byte from the seed changes.
                         * 
                         * This is probably overkill, and highly likely also accommodated for internally in BouncyCastle,
                         * but it's better to stay safe than sorry, and since this has little if any overhead for us,
                         * we do it to stay sure.
                         */
                        using (var sha512 = SHA512.Create ()) {

                            // Creating our RNG and seeding it with the seed's value hashed.
                            _secureRandom.SetSeed (
                                sha512.ComputeHash (args.GetExChildValue<byte []> ("seed", context, Guid.NewGuid ().ToByteArray ())));
                        }
                    }
                    return _secureRandom;
                }

                // Used to to hold seed for random number generator.
                var seed = new List<byte> ();

                // Retrieving the seed provided by caller through the [seed] argument, if any.
                // Notice, the seed as a whole will be hashed before used, so there's no need to hash the [seed] argument here.
                if (args ["seed"] != null)
                    seed.AddRange (args.GetExChildValue<byte []> ("seed", context, Guid.NewGuid ().ToByteArray ()));

                // Then retrieving an additional seed from BouncyCastle's internal seed generator.
                seed.AddRange (new ThreadedSeedGenerator ().GenerateSeed (128, false));

                // Then we retrieve the server salt and adds that to our seed.
                seed.AddRange (
                    context.RaiseEvent (
                        ".p5.auth.get-server-salt").Get<byte []> (
                            context,
                            Encoding.UTF8.GetBytes ("in-case-server-hasn't-been-salted!!")));

                // Then we retrieve the ticks of server and adds that to our seed.
                seed.AddRange (Encoding.UTF8.GetBytes (DateTime.Now.Ticks.ToString ()));

                // Creating our instance.
                _secureRandom = new SecureRandom ();

                /*
                 * Then to guard against cases where parts of the seed has somehow been compromised, we
                 * hash the entire seed, to make consecutive RNG values more unpredictable, and knowledge to
                 * parts of the seed less dangerous.
                 *
                 * This is probably overkill, and probably accommodated for internally in BouncyCastle,
                 * but has little if any cost to us, hence we do it to stay safe.
                 */
                using (var sha512 = SHA512.Create ()) {

                    // Creating our RNG and seeding it with the seed's value hashed.
                    _secureRandom.SetSeed (sha512.ComputeHash (seed.ToArray ()));
                }

                // Returning RNG to caller.
                return _secureRandom;
            }
        }
    }
}
