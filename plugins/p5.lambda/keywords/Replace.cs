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
    ///     Class wrapping the [replace] keyword in p5 lambda.
    /// </summary>
    public static class Replace
    {
        /// <summary>
        ///     The [replace] keyword, allows you to replace occurrencies of strings with other strings
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "replace", Protection = EventProtection.LambdaClosed)]
        public static void lambda_replace (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Figuring out source value for [length]
                string source = XUtil.Single<string> (context, e.Args, true);
                if (source == null)
                    return; // Nothing to check

                // Retrieving what to replace
                var what = e.Args.GetExChildValue ("what", context, "");
                var with = e.Args.GetExChildValue ("with", context, "");
                if (string.IsNullOrEmpty (what))
                    throw new LambdaException ("No [what] argument supplied to [replace]", e.Args, context);

                e.Args.Value = source.Replace (what, with);
            }
        }
    }
}
