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
    ///     Class wrapping execution engine keyword [add].
    /// 
    ///     The [add] keyword, allows you to append nodes to another node-set. Either by using expressions, or constants as your source.
    ///     Destination must be an <see cref="phosphorus.expressions.Expression">Expression</see>.
    /// </summary>
    public static class Add
    {
        /// <summary>
        ///     The [add] keyword allows you to append a node-set into another node-set.
        /// 
        ///     The [add] keyword, allows you to append a node-set into another node-set. Either by using a constant list of nodes, beneath
        ///     your [source] parameter, or by having the value of your [source] or [src] parameter being an 
        ///     <see cref="phosphorus.expressions.Expression">Expression</see>.
        /// 
        ///     You can also use a relative source, by supplying a [rel-src] or [rel-source], instead of a [source] or [src] parameter.
        ///     This allows you to append into your destination, a node-set that is somehow relative to each of your destinations.
        /// 
        ///     The value of your [add] node is its destination, and its [source], [rel-source], and so on, must be its last child node.
        /// 
        ///     Example that will append all children of [_source] into [_destination];
        /// 
        ///     <pre>_source
        ///   foo1:bar1
        ///   foo2:bar2
        /// _destination
        /// add:@/-?node
        ///   source:@/./-2/"*"?node</pre>
        /// 
        ///     Example of appending the destination node's first child, into its second child;
        ///     <pre>_source
        ///   src1:foo1
        ///     bar1:Howdy
        ///       child:foo1
        ///     dest
        ///   src2:foo2
        ///     bar2:World
        ///       child:foo2
        ///     dest
        /// add:@/-/"*"/1?node
        ///   rel-source:@/-?node</pre>
        /// 
        ///     The [add] Active Event, will also automatically convert any value to a node-set, if necessary, such as the example beneath
        ///     is an example of;
        /// 
        ///     <pre>_source
        ///   foo1:bar1
        ///   foo2:bar2
        /// _destination
        /// add:@/-?node
        ///   source:@/./-2/"*"?value</pre>
        /// 
        ///     The next piece of code, shows an example of how to append a constant list of nodes;
        /// 
        ///     <pre>_destination
        /// add:@/-?node
        ///   source
        ///     foo1:bar1
        ///     foo2:bar2</pre>
        /// 
        ///     The [add] Active Event will also automatically convert any value it is given as its [source], for instance;
        /// 
        ///     <pre>_destination
        /// add:@/-?node
        ///   source:"foo1:bar1\nfoo2:bar2"</pre>
        /// 
        ///     The [add] keyword can also be given multiple destinations, for instance;
        ///     <pre>_dest1
        /// _dest2
        /// add:@/-1|/-2?node
        ///   source
        ///     foo1:bar1</pre>
        /// 
        ///     In addition, [add] can also of course have multiple sources, such as for instance;
        /// 
        ///     <pre>_destination
        /// _src1
        ///   foo1:bar1
        /// _src2
        ///   foo2:bar2
        /// add:@/-3?node
        ///   source:@/.(/-1|/-2)/"*"?node</pre>
        /// 
        ///     The [add] keyword can also append into reference nodes, for instance;
        /// 
        ///     <pre>_node:node:"_foo"
        /// add:@/-/#?node
        ///   source
        ///     foo1:bar1</pre>
        /// 
        ///     The [add] keyword has probably dozens of other legal permutations, and combinations. Meaning, if you can think about it, 
        ///     then [add] can probably do it. The [add] keyword, combined with 
        ///     <see cref="phosphorus.expressions.Expression">Expressions</see>, is probably one of your most flexible keywords in p5.lambda.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "add")]
        private static void lambda_add (ApplicationContext context, ActiveEventArgs e)
        {
            // figuring out source type, for then to execute the corresponding logic, but first asserting destination is expression
            if (!(e.Args.Value is Expression))
                throw new LambdaException ("Not a valid destination expression given, make sure you set [add]'s value to an expression using :x:", e.Args, context);

            if (e.Args.Count > 0 && e.Args.LastChild.Name == "rel-src") {

                // relative source
                AppendRelativeSource (e.Args, context);
            } else if (e.Args.Count > 0 && e.Args.LastChild.Name == "src") {

                // static source
                AppendStaticSource (e.Args, context);
            } else {
                throw new LambdaException ("No [src] or [rel-src] given to [add]", e.Args, context);
            }
        }
        
        /*
         * source is static
         */
        private static void AppendStaticSource (Node addNode, ApplicationContext context)
        {
            // retrieving source before we start iterating destination,
            // in case destination and source overlaps
            var sourceNodes = XUtil.SourceNodes (addNode.LastChild, context);

            // making sure there is a source
            if (sourceNodes == null)
                return;

            // looping through every destination node
            var isFirst = true; // since source is already cloned, we avoid cloning on our first run
            foreach (var idxDestination in addNode.Get<Expression> (context).Evaluate (addNode, context, addNode)) {

                // verifying destination actually is a node
                var curDest = idxDestination.Value as Node;
                if (curDest == null)
                    throw new LambdaException ("cannot [add] into something that's not a node", addNode, context);

                // minor optimization trick, since source already is cloned upon first run
                if (isFirst) {
                    // we don't clone on the first run-through, since node-set is already cloned
                    curDest.AddRange (sourceNodes);
                    isFirst = false;
                } else {
                    // cloning on all consecutive run-throughs
                    foreach (var idxSource in sourceNodes) {
                        curDest.Add (idxSource.Clone ());
                    }
                }
            }
        }

        /*
         * relative source
         */
        private static void AppendRelativeSource (Node node, ApplicationContext context)
        {
            foreach (var idxDestination in node.Get<Expression> (context).Evaluate (node, context, node)) {

                // verifying destination actually is a node
                var curDest = idxDestination.Value as Node;
                if (curDest == null)
                    throw new LambdaException ("cannot [add] into something that's not a node", node, context);

                foreach (var idxSource in XUtil.SourceNodes (node.LastChild, idxDestination.Node, context)) {
                    curDest.Add (idxSource.Clone ());
                }
            }
        }
    }
}
