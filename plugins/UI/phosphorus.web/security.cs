/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Globalization;
using System.Security.Cryptography;
using phosphorus.core;
using phosphorus.lambda;
using phosphorus.ajax.widgets;

namespace phosphorus.web
{
    /// <summary>
    /// helper to retrieve and change properties of widgets
    /// </summary>
    public static class security
    {
        /// <summary>
        /// creates a hash out of the value from [pf.hash-string] and returns the hash value as the value of
        /// the first child of [pf.hash-string] named [value]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.hash-string")]
        private static void pf_hash_string (ApplicationContext context, ActiveEventArgs e)
        {
            using (MD5 md5 = MD5.Create ()) {
                string hashValue = Convert.ToBase64String (md5.ComputeHash (Encoding.UTF8.GetBytes (e.Args.Get<string> ())));
                e.Args.Add (new Node ("value", hashValue));
            }
        }
    }
}
