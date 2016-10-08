/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
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
    ///     Class wrapping the [find] keyword in p5 lambda.
    /// </summary>
    public static class IndexOf
    {
        /// <summary>
        ///     The [index-of] keyword, retrieves the index of the specified string(s) and/or regular expression(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "index-of")]
        public static void lambda_index_of (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Sanity check
                List<Node> sourceNodes = e.Args.Children.Where (ix => ix.Name != "").ToList ();
                if (sourceNodes.Count == 0)
                    throw new LambdaException ("No source given to [index-of]", e.Args, context);

                // Figuring out source value for [index-of]
                string source = XUtil.Single<string> (context, e.Args, true);
                if (source == null)
                    return; // Nothing to check

                // Retrieving what source to look for
                var src = XUtil.Source (context, e.Args, e.Args);

                // Making sure there is a source
                if (src.Count == 0) {
                    e.Args.Clear ();
                    return;
                }

                // Looping through each source
                foreach (var idxSrc in src) {
                    if (idxSrc is Regex) {

                        // Regex type of find
                        e.Args.AddRange (IndexOfRegex (source, idxSrc as Regex).Select (ix => new Node (Utilities.Convert<string> (context, idxSrc), ix)));
                    } else if (idxSrc is Node) {

                        // Simple string type of find
                        var objIdxSrc = (idxSrc as Node).Value;
                        if (objIdxSrc is Regex) {
                            e.Args.AddRange (IndexOfRegex (source, objIdxSrc as Regex).Select (ix => new Node ((idxSrc as Node).Name, ix)));
                        } else {
                            var strIdxSrc = Utilities.Convert<string> (context, objIdxSrc);
                            e.Args.AddRange (IndexOfString (source, strIdxSrc).Select (ix => new Node ((idxSrc as Node).Name, ix)));
                        }
                    } else {

                        // Simple string type of find
                        var strIdxSrc = Utilities.Convert<string> (context, idxSrc);
                        e.Args.AddRange (IndexOfString (source, strIdxSrc).Select (ix => new Node (strIdxSrc, ix)));
                    }
                }

                // Since our "args remover" won't remove the [src] argument, we explicitly remove it here
                sourceNodes.ForEach (ix => e.Args [ix.Name].UnTie ());
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
