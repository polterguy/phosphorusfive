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

namespace p5.lambda.keywords.extras
{
    /// <summary>
    ///     Class wrapping the [sort] and [sort-desc] Active Events.
    /// </summary>
    public static class Sort
    {
        /// <summary>
        ///     The [sort] event and [sort-desc] events, allows you to sort a node result, returning the nodes as children.
        ///     Either ascending (default) or descending, if you use [sort-desc].
        ///     You can also provide a lambda block callback, which will be invoked with [_lhs] and [_rhs] to provide your own sorting logic.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "sort")]
        [ActiveEvent (Name = "sort-desc")]
        public static void lambda_sort (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning.
            using (new ArgsRemover (e.Args, true)) {

                // Creating copy of node list to insert as result of [sort] after sorting nodes.
                var nodeList = XUtil.Iterate<Node> (context, e.Args).Where (ix => ix.Name != "").Select (ix => ix.Clone ()).ToList ();

                // Special case for empty [sort] node, at which case we use "default sorting", sorting by value
                if (e.Args.Children.Count (ix => ix.Name != "") == 0) {

                    // Defaulting to sorting nodes by "value" casted to IComparable
                    nodeList.Sort (delegate(Node lhs, Node rhs) {
                        if (e.Name == "sort")
                            return string.Compare (lhs.Name, rhs.Name, System.StringComparison.InvariantCulture);
                        return string.Compare (rhs.Name, lhs.Name, System.StringComparison.InvariantCulture);
                    });
                } else {

                    // We have a lambda delegate here
                    nodeList.Sort (delegate(Node lhs, Node rhs) {

                        // Cloning [sort] node, using clone for [eval] for each node in result
                        var exeNode = e.Args.Clone ();

                        // Making sure exeNode has name of [eval], and forcing children evaluation
                        exeNode.Name = "eval";
                        exeNode.Value = null;

                        // Injecting [_lhs] and [_rhs] to sort callback
                        exeNode.Insert (0, new Node ("_lhs", lhs));
                        exeNode.Insert (1, new Node ("_rhs", rhs));

                        // Invoking [sort] callback
                        context.RaiseEvent ("eval", exeNode);

                        // Retrieving value of [sort] callback, defaulting to "equals"
                        return exeNode.Get (context, 0);
                    });
                }

                // Returning sorted list to caller
                e.Args.AddRange (nodeList);
            }
        }
    }
}
