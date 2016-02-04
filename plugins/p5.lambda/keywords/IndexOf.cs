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
    ///     Class wrapping the [index-of] keyword in p5 lambda.
    /// </summary>
    public static class IndexOf
    {
        /// <summary>
        ///     The [index-of] keyword, retrieves the index of the specified string
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "index-of", Protection = EventProtection.LambdaClosed)]
        public static void lambda_index_of (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Figuring out source value for [index-of]
                string source = XUtil.Single<string> (context, e.Args, true);
                if (source == null)
                    return; // Nothing to check

                // Retrieving what to look for in source
                var sepStrings = e.Args.Children
                    .Where (ix => ix.Name == "=")
                    .Select (ix => ix.GetExValue<string> (context))
                    .ToList ();
                sepStrings.RemoveAll (ix => string.IsNullOrEmpty (ix));

                // Looping through each index-of string, adding into results
                var results = new List<int> ();
                foreach (var idxIndex in sepStrings) {
                    var curIndex = 0;

                    // Looping until we don't find anymore occurrencies
                    while (true) {
                        curIndex = source.IndexOf (idxIndex, curIndex);
                        if (curIndex == -1)
                            break;
                        results.Add (curIndex++);
                    }
                }
                results.Sort ();
                e.Args.AddRange (results.Select (ix => new Node ("", ix)));
            }
        }
    }
}
