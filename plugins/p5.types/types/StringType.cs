/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.types.types
{
    /// <summary>
    ///     Class contains helper methods for string type
    /// </summary>
    public static class StringType
    {
        /// <summary>
        ///     Creates a bool from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.string.decode-base64", Protection = EventProtection.LambdaClosed)]
        private static void p5_string_decode_base64 (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Looping through each argument given
                foreach (var idxNode in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Decoding given value from base64 and returning to caller
                    e.Args.Add ("", Convert.FromBase64String (XUtil.Single<string> (context, e.Args, true)));
                }
            }
        }
    }
}
