/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Linq;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.lambda.keywords
{
    /// <summary>
    ///     Class wrapping execution engine keyword [insert-before] and [insert-after].
    /// 
    ///     The [insert-before] and [insert-after] keywords, allows you to insert nodes at a specified position.
    ///     Either by using expressions, or constants as your source. Destination must be an 
    ///     <see cref="phosphorus.expressions.Expression">Expression</see>.
    /// </summary>
    public static class Insert
    {
        /// <summary>
        ///     The [insert-before] keyword allows you to insert nodes before another node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "insert-before")]
        private static void lambda_insert_before (ApplicationContext context, ActiveEventArgs e)
        {
            // figuring out source type, for then to execute the corresponding logic, but first asserting destination is expression
            if (!(e.Args.Value is Expression))
                throw new LambdaException ("Not a valid destination expression given, make sure you set [insert-before]'s value to an expression using :x:", e.Args, context);

            if (e.Args.Count > 0 && e.Args.LastChild.Name == "rel-src") {

                // relative source
                AppendRelativeSource (e.Args, context, false);
            } else {

                // static source
                AppendStaticSource (e.Args, context, false);
            }
        }
        
        /// <summary>
        ///     The [insert-after] keyword allows you to insert nodes after another node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "insert-after")]
        private static void lambda_insert_after (ApplicationContext context, ActiveEventArgs e)
        {
            // figuring out source type, for then to execute the corresponding logic, but first asserting destination is expression
            if (!(e.Args.Value is Expression))
                throw new LambdaException ("Not a valid destination expression given, make sure you set [insert-before]'s value to an expression using :x:", e.Args, context);

            if (e.Args.Count > 0 && e.Args.LastChild.Name == "rel-src") {

                // relative source
                AppendRelativeSource (e.Args, context, true);
            } else {

                // static source
                AppendStaticSource (e.Args, context, true);
            }
        }

        /*
         * source is static
         */
        private static void AppendStaticSource (Node addNode, ApplicationContext context, bool after)
        {
            // retrieving source before we start iterating destination,
            // in case destination and source overlaps
            var sourceNodes = XUtil.SourceNodes (addNode, context);

            // making sure there is a source
            if (sourceNodes == null)
                return;

            // looping through every destination node
            var isFirst = true; // since source is already cloned, we avoid cloning on our first run
            foreach (var idxDestination in addNode.Get<Expression> (context).Evaluate (addNode, context, addNode)) {

                // verifying destination actually is a node
                var curDest = idxDestination.Value as Node;
                if (curDest == null)
                    throw new LambdaException ("cannot [insert-before] or [insert-after] into something that's not a node", addNode, context);

                // minor optimization trick, since source already is cloned upon first run
                if (isFirst) {

                    // we don't clone on the first run-through, since node-set is already cloned
                    foreach (var idxSourceNode in sourceNodes) {
                        curDest.Parent.Insert (curDest.Parent.IndexOf (curDest) + (after ? 1 : 0), idxSourceNode);
                    }
                    isFirst = false;
                } else {

                    // cloning on all consecutive run-throughs
                    foreach (var idxSourceNode in sourceNodes) {
                        curDest.Parent.Insert (curDest.Parent.IndexOf (curDest) + (after ? 1 : 0), idxSourceNode.Clone ());
                    }
                }
            }
        }

        /*
         * relative source
         */
        private static void AppendRelativeSource (Node node, ApplicationContext context, bool after)
        {
            foreach (var idxDestination in node.Get<Expression> (context).Evaluate (node, context, node)) {

                // verifying destination actually is a node
                var curDest = idxDestination.Value as Node;
                if (curDest == null)
                    throw new LambdaException ("cannot [insert-before] or [insert-after] into something that's not a node", node, context);

                foreach (var idxSource in XUtil.SourceNodes (node, idxDestination.Node, context)) {
                    curDest.Parent.Insert (curDest.Parent.IndexOf (curDest) + (after ? 1 : 0), idxSource.Clone ());
                }
            }
        }
    }
}
