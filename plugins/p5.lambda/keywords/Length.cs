/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.lambda.keywords
{
    /// <summary>
    ///     Class wrapping the [length] keyword in p5 lambda.
    /// </summary>
    public static class Length
    {
        /// <summary>
        ///     The [length] keyword, retrieves the length of the specified string
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "length", Protection = EventProtection.LambdaClosed)]
        public static void lambda_length (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Figuring out source value for [length]
                string source = XUtil.Single<string> (context, e.Args, true);
                if (source == null)
                    return; // Nothing to check

                e.Args.Value = source.Length;
            }
        }
    }
}
