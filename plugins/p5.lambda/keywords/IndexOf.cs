/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.keywords {
    /// <summary>
    ///     Class wrapping the [find] keyword in p5 lambda.
    /// </summary>
    public static class IndexOf
    {
        /// <summary>
        ///     The [index-of] keyword, retrieves the index of the specified string(s) and/or regular expression(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "index-of", Protection = EventProtection.LambdaClosed)]
        public static void lambda_index_of (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Figuring out source value for [find]
                string source = XUtil.Single<string> (context, e.Args, true);
                if (source == null)
                    return; // Nothing to check

                // Retrieving what to look for in source
                var sepObject = e.Args.GetExChildValue<object> ("what", context, null);
                if (sepObject == null || (sepObject is string && string.IsNullOrEmpty (sepObject as string)))
                    throw new LambdaException ("No [what] supplied to [index-of]", e.Args, context);

                // Checking type of find object
                if (sepObject is Regex) {

                    // Regex type of find
                    e.Args.AddRange (IndexOfRegex (source, sepObject as Regex).Select (ix => new Node ("", ix)));
                } else {

                    // Simple string type of find
                    e.Args.AddRange (IndexOfString (source, sepObject as string).Select (ix => new Node ("", ix)));
                }
            }
        }

        /*
         * Evaluates the given regular expression and yields all index results
         */
        private static IEnumerable<int> IndexOfRegex (string source, Regex regex)
        {
            // Evaluating regex and looping through each result
            foreach (System.Text.RegularExpressions.Match idxMatch in regex.Matches (source)) {

                // Returning currently iterated result
                yield return idxMatch.Index;
            }
        }

        /*
         * Simple string find
         */
        private static IEnumerable<int> IndexOfString (string source, string search)
        {
            // Looping until we don't find anymore occurrencies
            var curIndex = 0;
            while (true) {
                curIndex = source.IndexOf (search, curIndex);
                if (curIndex == -1)
                    yield break;
                yield return curIndex++;
            }
        }
    }
}
