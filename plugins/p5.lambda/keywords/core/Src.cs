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

using System.Linq;
using p5.exp;
using p5.core;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [src] Active Event.
    /// </summary>
    public static class Src
    {
        /// <summary>
        ///     The [src] event evaluates an expression, constant or children nodes, and returns the results normalized, in a standard fashion.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "src")]
        [ActiveEvent (Name = "dest")]
        public static void lambda_src_dest (ApplicationContext context, ActiveEventArgs e)
        {
            // Figuring out type of source.
            var x = e.Args.Value as Expression;
            if (x != null) {

                // Source is an expression, evaluating it, and returning results back to caller.
                var match = x.Evaluate (context, e.Args, e.Args);

                // Removing all formatting nodes.
                e.Args.RemoveAll (ix => ix.Name == "");

                // Checking type of match.
                if (match.TypeOfMatch == Match.MatchType.count) {

                    // Simple count expression.
                    e.Args.Value = match.Count;

                } else if (match.TypeOfMatch == Match.MatchType.node) {

                    // Node values, single or multiple is irrelevant, still need to clone them.
                    e.Args.AddRange (match.Select (ix => ix.Node.Clone ()));
                    e.Args.Value = null;

                } else if (match.Count == 1) {

                    // Single value, name or value type of value is irrelevant, adding to value anyways.
                    e.Args.Value = match [0].Value;

                } else if (match.TypeOfMatch == Match.MatchType.name) {

                    // Multiple name values.
                    e.Args.AddRange (match.Select (ix => new Node (ix.Node.Name)));
                    e.Args.Value = null;

                } else {

                    // Multiple value values.
                    e.Args.AddRange (match.Select (ix => new Node ("", ix.Value)));
                    e.Args.Value = null;
                }
            } else if (e.Args.Value != null) {

                // Returning formatted value.
                e.Args.Value = XUtil.FormatNode (context, e.Args);
            } // else, returning children nodes as is
        }
    }
}
