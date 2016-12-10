/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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
    ///     Class wrapping the [p5.string.index-of] Active Event.
    /// </summary>
    public static class IndexOf
    {
        /// <summary>
        ///     The [p5.string.index-of] event, retrieves the index of the specified string(s) and/or regular expression(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.string.index-of")]
        public static void p5_string_index_of (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check.
            if (e.Args.Value == null)
                throw new LambdaException ("[p5.string.index-of] requires an expression or constant as its value", e.Args, context);

            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Figuring out source value for [p5.string.index-of], and returning early if there is no source.
                string source = XUtil.Single<string> (context, e.Args);
                if (source == null)
                    return;

                // Retrieving what to look for, and returning early if we have no source.
                var src = XUtil.Source (context, e.Args);
                if (src == null)
                    return;

                // Checking what type of value we're looking for, and acting accordingly.
                if (src is Regex) {

                    // Regex type of find.
                    e.Args.AddRange (IndexOfRegex (source, src as Regex).Select (ix => new Node ("", ix)));

                } else {

                    // Simple string type of find.
                    e.Args.AddRange (IndexOfString (source, Utilities.Convert<string> (context, src)).Select (ix => new Node ("", ix)));
                }
            }
        }

        /*
         * Evaluates the given regular expression and yields all its results.
         */
        private static IEnumerable<int> IndexOfRegex (string source, Regex regex)
        {
            // Evaluating regex and returning all results.
            foreach (System.Text.RegularExpressions.Match idxMatch in regex.Matches (source)) {
                yield return idxMatch.Index;
            }
        }

        /*
         * Simple string find.
         */
        private static IEnumerable<int> IndexOfString (string source, string search)
        {
            // Returning all occurrences of specified string.
            var idx = source.IndexOf (search);
            while (idx != -1) {
                yield return idx++;
                idx = source.IndexOf (search, idx);
            }
        }
    }
}
