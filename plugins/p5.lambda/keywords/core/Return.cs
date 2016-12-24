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
using p5.exp;
using p5.core;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [return] Active Event.
    /// </summary>
    public static class Return
    {
        /// <summary>
        ///     The [return] event, allows you to return immediately from the evaluation of some lambda, with or without a return value/lambda.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "return")]
        public static void lambda_return (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving root for efficiency reasons, since everything we do from here, we do to root.
            var root = e.Args.Root;

            // Inserting "return signaling node", such that [eval] and similar constructs will break out of their current execution.
            root.Insert (0, new Node ("_return"));

            // Checking value of return, and acting accordingly, making sure we return simple values as such, and node expression results as nodes.
            var x = e.Args.Value as Expression;
            if (x != null) {

                // Source is an expression, evaluating it, and returning results back to caller.
                var match = x.Evaluate (context, e.Args, e.Args);
                if (match.TypeOfMatch == Match.MatchType.count) {

                    // Simple count expression.
                    root.Value = match.Count;

                } else if (match.TypeOfMatch == Match.MatchType.node) {

                    // Node values, single or multiple is irrelevant, still need to clone them, and insert them into root.
                    root.AddRange (match.Select (ix => ix.Node.Clone ()));

                } else if (match.Count == 1) {

                    // Single value, name or value type of value is irrelevant, adding to value anyways.
                    root.Value = match[0].Value;

                } else if (match.TypeOfMatch == Match.MatchType.name) {

                    // Multiple name values.
                    root.AddRange (match.Select (ix => new Node (ix.Node.Name)));

                } else {

                    // Multiple value values.
                    root.AddRange (match.Select (ix => new Node ("", ix.Value)));

                }
            } else if (e.Args.Value != null) {

                // Returning formatted value.
                root.Value = XUtil.FormatNode (context, e.Args);
            }

            // Adding all children of [return] as result to evaluated rooot node, no need to clone.
            root.InsertRange (1, e.Args.Children);
        }
    }
}
