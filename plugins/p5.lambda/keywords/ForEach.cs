/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
using System.Collections;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.lambda.keywords
{
    /// <summary>
    ///     Class wrapping the p5 lambda [for-each] keyword.
    /// </summary>
    public static class ForEach
    {
        /// <summary>
        ///     The [for-each] keyword allows you to iterate over the results of expressions.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "for-each", Protection = EventProtection.LambdaClosed)]
        public static void lambda_for_each (ApplicationContext context, ActiveEventArgs e)
        {
            // storing old for-each "body"
            Node oldForEach = e.Args.Clone ();

            var nodeValue = e.Args.Value as Node;
            if (nodeValue != null) {

                // Value of node is a Node in itself, iterating its children
                foreach (var idxSource in nodeValue.Children) {

                    if (!IterateForEach (context, idxSource, e.Args, oldForEach))
                        break;
                }
            } else {

                var strValue = e.Args.Value as string;
                if (strValue != null) {

                    // Value of node is a string, converting to node and iterating the converted node's children
                    foreach (var idxSource in Utilities.Convert<Node> (context, strValue).Children) {

                        if (!IterateForEach (context, idxSource, e.Args, oldForEach))
                            break;
                    }
                } else {

                    var expValue = e.Args.Value as Expression;
                    if (expValue != null) {

                        // Value of node is an expression, evaluating that expression, and iterating the evaluated results
                        foreach (var idxSource in e.Args.Get<Expression> (context).Evaluate (context, e.Args, e.Args)) {

                            if (!IterateForEach (context, idxSource.Value, e.Args, oldForEach))
                                break;
                        }
                    } else {

                        var enumerableValue = e.Args as IEnumerable;
                        if (enumerableValue != null) {

                            // Value is a "list of something", iterating each value in the list
                            foreach (var idxSource in enumerableValue) {

                                if (!IterateForEach (context, idxSource, e.Args, oldForEach))
                                    break;
                            }
                        } else {

                            if (e.Args.Value != null) {

                                // value of for-each is "any type of single item object", invoking for-each once and once only on that value
                                IterateForEach (context, e.Args.Value, e.Args, oldForEach);
                            } else if (e.Args.Children.Where (ix => ix.Name != "").GetEnumerator ().MoveNext ()) {

                                // Assuming first non-empty name child node of for-each is "source",
                                // raising that node's name as Active Event, and iterating on the resulting
                                // children from that Event invocation
                                Node sourceNode = e.Args.Children.First(ix=>ix.Name != "");
                                var oldSourceValue = sourceNode.Value;
                                context.RaiseLambda (sourceNode.Name, sourceNode);
                                sourceNode.UnTie (); // removing node that was used as source

                                // Value has presedence
                                if (sourceNode.Value == null || oldSourceValue == sourceNode.Value) {

                                    // Source was returned as nodes
                                    foreach (var idxSource in sourceNode.Children) {

                                        // Iterating on the values returned from Active Event invocation
                                        if (!IterateForEach (context, idxSource, e.Args, oldForEach))
                                            break;
                                    }
                                } else {

                                    // Source was returned as object in value of Active Event invocation
                                    IterateForEach (context, sourceNode.Value, e.Args, oldForEach);
                                }
                            }
                        }
                    }
                }
            }
        }

        /*
         * Invokes [for-each] scope once, setting the [_dp] correctly and resetting the for-each scope afterwards.
         * Returns true if iteration should continue
         */
        private static bool IterateForEach (ApplicationContext context, object source, Node args, Node oldForEach)
        {
            var dp = new Node ("_dp", source);
            args.Insert (0, dp);

            context.RaiseLambda ("eval-mutable", args);
            var rootChildName = args.Root.FirstChild?.Name;
            bool shouldStop = rootChildName == "_return" || rootChildName == "_break";
            if (!shouldStop && args.Root.FirstChild.Name == "_break") {
                args.Root.FirstChild.UnTie ();
            }
            args.Clear ();
            args.AddRange (oldForEach.Clone ().Children);
            return !shouldStop;
        }
    }
}