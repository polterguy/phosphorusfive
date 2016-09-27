/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
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
                            return -1;
                        else if (lhs.Value != null && rhs.Value == null)
                            return 1;
                        else if (lhs.Value == null && rhs.Value == null)
                            return 0;

                        // Assuming value somehow implements IComparable
                        return (lhs.Value as IComparable).CompareTo (rhs.Value);
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
                        return exeNode.Get<int> (context, 0);
                    });
                }

                // Returning sorted list to caller
                e.Args.AddRange (nodeList);
            }
        }
    }
}
