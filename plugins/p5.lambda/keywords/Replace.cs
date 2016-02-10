/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        ///     The [replace] keyword, allows you to replace occurrencies of a string or regular expression with another strings
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "replace", Protection = EventProtection.LambdaClosed)]
        public static void lambda_replace (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Figuring out source value
                string source = XUtil.Single<string> (context, e.Args, true);
                if (source == null)
                    return; // Nothing to check

                // Retrieving what to replace
                var what = e.Args.GetExChildValue<object> ("what", context, null);
                var with = e.Args.GetExChildValue ("with", context, "");
                if (what == null || (what is string && string.IsNullOrEmpty (what as string)))
                    throw new LambdaException ("No [what] argument supplied to [replace]", e.Args, context);

                // Checking type of what
                if (what is Regex) {

                    // Regular expression search
                    var regex = what as Regex;
                    e.Args.Value = regex.Replace (source, with);
                } else {

                    // Simple string search
                    e.Args.Value = source.Replace (Utilities.Convert<string> (context, what), with);
                }
            }
        }
    }
}
