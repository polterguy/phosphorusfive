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

namespace p5.strings.keywords
{
    /// <summary>
    ///     Class wrapping the [regex] keyword in p5 lambda.
    /// </summary>
    public static class Match
    {
        /// <summary>
        ///     The [match] keyword, allows you to find occurrencies of regular expressions in a string
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "match")]
        public static void lambda_match (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Figuring out source value
                string source = XUtil.Single<string> (context, e.Args, true);
                if (source == null)
                    return; // Nothing to check

                // Retrieving explicitly named captures, if any
                var captures = new List<string> (e.Args.Children.Where (ix => ix.Name != "").Select (ix => ix.Name));
                captures.RemoveAt (0);

                // Retrieving what source to look for
                var src = XUtil.Source (context, e.Args, e.Args, "src", captures);

                // Making sure there is a source
                if (src.Count == 0) {
                    e.Args.Clear ();
                    return;
                }

                // Running regular expression(s)
                foreach (var idxSrc in src) {
                    var matches = (idxSrc as Regex).Matches (source);
                    foreach (System.Text.RegularExpressions.Match idxMatch in matches) {

                        // Creating result node
                        var resultNode = e.Args.Add ("result").LastChild;

                        // Checking if caller supplied explicit capture group names
                        if (captures.Count > 0) {

                            // Looping through all group names requested by caller to be returned
                            foreach (var idxCapture in captures) {
                                resultNode.Add (idxCapture, idxMatch.Groups[idxCapture].Value);
                                resultNode.LastChild.Add ("start", idxMatch.Groups[idxCapture].Index);
                                resultNode.LastChild.Add ("length", idxMatch.Groups[idxCapture].Length);
                            }

                        } else {

                            // Returning all groups, including outer most
                            foreach (Group idxGroup in idxMatch.Groups) {
                                resultNode.Add (idxGroup.Value);
                            }
                        }
                    }
                }
            }
        }
    }
}
