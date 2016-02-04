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
    ///     Class wrapping the [regex-replace] keyword in p5 lambda.
    /// </summary>
    public static class LambdaRegexReplace
    {
        /// <summary>
        ///     The [regex-replace] keyword, allows you to replace occurrencies of regular expressions in a string
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "regex-replace", Protection = EventProtection.LambdaClosed)]
        public static void lambda_regex_replace (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Figuring out source value for [length]
                string source = XUtil.Single<string> (context, e.Args, true);
                if (source == null)
                    return; // Nothing to check

                // Figuring out regex pattern to use
                var pattern = e.Args.GetExChildValue ("pattern", context, "");
                var with = e.Args.GetExChildValue ("with", context, "");
                if (string.IsNullOrEmpty (pattern))
                    throw new LambdaException ("No [pattern] supplied to [regex-replace]", e.Args, context);

                // Figuring out options to use
                RegexOptions options = RegexOptions.None;
                foreach (var idxOption in e.Args.Children.Where (ix => ix.Name != "pattern" && ix.Name != "with").Select (ix => ix.Name)) {
                    options |= (RegexOptions)Enum.Parse (typeof(RegexOptions), idxOption);
                }

                // Running regular expression
                var regex = new Regex (pattern, options);
                e.Args.Value = regex.Replace (source, with);
            }
        }
    }
}
