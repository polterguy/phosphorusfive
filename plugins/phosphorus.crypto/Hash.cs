/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Security.Cryptography;
using System.Text;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.crypto
{
    /// <summary>
    ///     Helper to create hashed values from nodes and values
    /// </summary>
    public static class Hash
    {
        /// <summary>
        ///     Creates an MD5 hash-string from the value(s) given
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.crypto.hash-string")]
        private static void pf_crypto_hash_string (ApplicationContext context, ActiveEventArgs e)
        {
            var whatToHash = XUtil.Single<string> (e.Args, context);
            if (whatToHash == null)
                return; // nothing to hash here ...

            using (var md5 = MD5.Create ()) {
                var hashValue = Convert.ToBase64String (md5.ComputeHash (Encoding.UTF8.GetBytes (whatToHash)));
                e.Args.Add (new Node ("value", hashValue));
            }
        }
    }
}