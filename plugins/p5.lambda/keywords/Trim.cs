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
    ///     Class wrapping the [trim] keyword in p5 lambda.
    /// </summary>
    public static class Trim
    {
        /// <summary>
        ///     The [trim] keyword, allows you to trim occurrencies of characters in a string
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "trim", Protection = EventProtection.LambdaClosed)]
        public static void lambda_trim (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting trim characters, defaulting to whitespace characters
                var characters = e.Args.GetExChildValue ("chars", context, " \r\n\t");

                // Returning length of constant or expression, converted to string if necessary
                e.Args.Value = XUtil.Single<string> (context, e.Args, true, "").Trim (characters.ToList().ToArray());
            }
        }
    }
}
