/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;

namespace p5.lambda.keywords.extras
{
    /// <summary>
    ///     Class wrapping the [sort] keyword in p5 lambda.
    /// </summary>
    public static class Sort
    {
        /// <summary>
        ///     The [sort] keyword, allows you to sort a node list
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "sort")]
        [ActiveEvent (Name = "sort-desc")]
        public static void lambda_sort (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Creating copy of node list to insert as result of [sort] after sorting nodelist
                var nodeList = new List<Node> (XUtil.Iterate<Node> (context, e.Args, true).Select (ix => ix.Clone ()));

                // Special case for empty [sort] node, at which case we use "default sorting", sorting by value
                if (e.Args.Children.Count (ix => ix.Name != "") == 0) {

                    // Defaulting to sorting nodes by "value" casted to IComparable
                    nodeList.Sort (delegate(Node lhs, Node rhs) {
                        if (lhs.Value == null && rhs.Value != null)
                            return e.Name == "sort" ? -1 : 1;
                        else if (lhs.Value != null && rhs.Value == null)
                            return e.Name == "sort" ? 1 : -1;
                        else if (lhs.Value == null && rhs.Value == null)
                            return 0;

                        // Assuming value somehow implements IComparable
                        return (lhs.Value as IComparable).CompareTo (rhs.Value) * (e.Name == "sort" ? 1 : -1);
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
                        context.Raise ("eval", exeNode);

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
