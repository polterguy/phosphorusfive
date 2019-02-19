/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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

using System.Text.RegularExpressions;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.strings.keywords
{
    /// <summary>
    ///     Class wrapping the [p5.string.match] Active Event.
    /// </summary>
    public static class Match
    {
        /// <summary>
        ///     The [p5.string.match] event, returns occurrences of a regular expression in a string.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "match")]
        [ActiveEvent (Name = "p5.string.match")]
        public static void p5_string_match (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args, true)) {

                // Figuring out source value for [match], and returning early if there is no source.
                string source = XUtil.Single<string> (context, e.Args);
                if (source == null)
                    return;

                // Retrieving what source to look for, and returning early if there is none.
                var src = XUtil.Source (context, e.Args);
                if (src == null)
                    return;
                var srcRegex = new Regex (src as string);

                // Evaluating regular expression, and returning results.
                foreach (System.Text.RegularExpressions.Match idxMatch in srcRegex.Matches (source)) {

                    // Returning all groups matches.
                    foreach (Group idxGroup in idxMatch.Groups) {
                        e.Args.Add (idxGroup.Value);
                    }
                }
            }
        }
    }
}
