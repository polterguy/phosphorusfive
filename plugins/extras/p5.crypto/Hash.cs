/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Security.Cryptography;
using System.Text;
using p5.core;
using p5.exp;

/// <summary>
///     Main namespace for everything related to cryptographic services.
/// 
///     Contains all helper classes that somehow relates to cryptography.
/// </summary>
namespace phosphorus.crypto
{
    /// <summary>
    ///     Helper class to create hashed value.
    /// 
    ///     A hashed string is a one-way street, creating a unique value from the given value, making
    ///     it impossible to figure out what the original value was by using the hashed value, still having
    ///     the possibility of re-creating the same hash if you know what was used to create the hash the first time.
    /// 
    ///     This happens to be a nice way to store passwords, and other types of sensitive information, such that even
    ///     if another person acquires access to the hashed value, he can still not quess the original password used to
    ///     generate the hash.
    /// </summary>
    public static class Hash
    {
        /// <summary>
        ///     Creates an MD5 hash-string.
        /// 
        ///     Creates an MD5 hash from the given value.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "p5.crypto.hash-string")]
        private static void p5_crypto_hash_string (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover args = new Utilities.ArgsRemover (e.Args)) {

                var whatToHash = XUtil.Single<string> (e.Args, context);
                if (whatToHash == null)
                    return; // nothing to hash here ...

                using (var md5 = MD5.Create ()) {
                    var hashValue = Convert.ToBase64String (md5.ComputeHash (Encoding.UTF8.GetBytes (whatToHash)));
                    e.Args.Value = hashValue;
                }
            }
        }
    }
}
