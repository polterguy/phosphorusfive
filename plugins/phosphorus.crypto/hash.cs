
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Text;
using System.Security.Cryptography;
using phosphorus.core;
using phosphorus.lambda;

namespace phosphorus.crypto
{
    /// <summary>
    /// helper wrapping some cryptographic Active Events
    /// </summary>
    public static class hash
    {
        /// <summary>
        /// creates a hash out of the value from [pf.crypto.hash-string] and returns the hash value as the value of
        /// the first child of [pf.crypto.hash-string] named [value]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.crypto.hash-string")]
        private static void pf_crypto_hash_string (ApplicationContext context, ActiveEventArgs e)
        {
            string whatToHash = Expression.Single (e.Args, true);
            using (MD5 md5 = MD5.Create ()) {
                string hashValue = Convert.ToBase64String (md5.ComputeHash (Encoding.UTF8.GetBytes (whatToHash)));
                e.Args.Add (new Node ("value", hashValue));
            }
        }
    }
}
