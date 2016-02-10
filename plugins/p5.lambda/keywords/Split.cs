/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.lambda.keywords
{
    /// <summary>
    ///     Class wrapping the [split] keyword in p5 lambda
    /// </summary>
    public static class Split
    {
        /// <summary>
        ///     The [split] keyword, allows you to split a string into multiple strings, either by index or string
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "split", Protection = EventProtection.LambdaClosed)]
        public static void lambda_split (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Figuring out source value of [split]
                string source = XUtil.Single<string> (context, e.Args, true);
                if (source == null)
                    return; // Nothing to split

                // Retrieving separator objects (which might be multiple integers, multiple strings or a single regular expression)
                var sepObjects = e.Args.Children
                    .Where (ix => ix.Name == "=")
                    .Select (ix => ix.GetExValue<object> (context))
                    .ToList ();

                // Checking if there were any separators
                if (sepObjects.Count > 0) {

                    // We have separator objects, now checking type of separator objects
                    if (sepObjects [0] is string) {

                        // String split operation, converting entire array to string array
                        var sepStrings = sepObjects.Select (ix => Utilities.Convert<string> (context, ix)).ToList ();
                        e.Args.AddRange (
                            source.Split (
                                sepStrings.ToArray (), 
                                System.StringSplitOptions.RemoveEmptyEntries).Select (ix => new Node (ix)));
                    } else if (sepObjects [0] is int) {

                        // Integer split operation, converting all values to integers and running substring upon all values
                        var sepIntegers = sepObjects.Select (ix => Utilities.Convert<int> (context, ix, -1)).ToList ();
                        sepIntegers.Sort ();
                        var start = 0;
                        foreach (var idxSplitInteger in sepIntegers) {
                            e.Args.Add (source.Substring (start, idxSplitInteger - start));
                            start = idxSplitInteger;
                        }
                        e.Args.Add (source.Substring (start));
                    } else if (sepObjects [0] is Regex) {

                        // Regex split operation, converting all values to regular expressions, and running Regex.Split on result
                        if (sepObjects.Count > 1)
                            throw new LambdaException ("When supplying a regex to [split], only one [=] operator is allowed", e.Args, context);
                        var sepRegex = sepObjects [0] as Regex;
                        e.Args.AddRange (sepRegex.Split (source).Select (ix => new Node (ix)));
                    } else {

                        // Oops ...!!
                        throw new LambdaException (
                            "Don't know how to split upon anything else but integers, strings and regular expressions", 
                            e.Args, 
                            context);
                    }
                } else {

                    // Special case, no separators, splitting entire string into characters
                    foreach (var idxCh in source) {
                        e.Args.Add (idxCh.ToString ());
                    }
                }
            }
        }
    }
}
