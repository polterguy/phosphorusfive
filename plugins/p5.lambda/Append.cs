/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.lambda
{
    /// <summary>
    ///     Class wrapping execution engine keyword [append].
    /// 
    ///     The [append] keyword, allows you to append nodes to another node-set. Either by using expressions, or constants as your source.
    ///     Destination must be an <see cref="phosphorus.expressions.Expression">Expression</see>.
    /// </summary>
    public static class Append
    {
        /// <summary>
        ///     The [append] keyword allows you to append a node-set into another node-set.
        /// 
        ///     The [append] keyword, allows you to append a node-set into another node-set. Either by using a constant list of nodes, beneath
        ///     your [source] parameter, or by having the value of your [source] or [src] parameter being an 
        ///     <see cref="phosphorus.expressions.Expression">Expression</see>.
        /// 
        ///     You can also use a relative source, by supplying a [rel-src] or [rel-source], instead of a [source] or [src] parameter.
        ///     This allows you to append into your destination, a node-set that is somehow relative to each of your destinations.
        /// 
        ///     The value of your [append] node is its destination, and its [source], [rel-source], and so on, must be its last child node.
        /// 
        ///     Example that will append all children of [_source] into [_destination];
        /// 
        ///     <pre>_source
        ///   foo1:bar1
        ///   foo2:bar2
        /// _destination
        /// append:@/-?node
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
        /// append:@/-/"*"/1?node
        ///   rel-source:@/-?node</pre>
        /// 
        ///     The [append] Active Event, will also automatically convert any value to a node-set, if necessary, such as the example beneath
        ///     is an example of;
        /// 
        ///     <pre>_source
        ///   foo1:bar1
        ///   foo2:bar2
        /// _destination
        /// append:@/-?node
        ///   source:@/./-2/"*"?value</pre>
        /// 
        ///     The next piece of code, shows an example of how to append a constant list of nodes;
        /// 
        ///     <pre>_destination
        /// append:@/-?node
        ///   source
        ///     foo1:bar1
        ///     foo2:bar2</pre>
        /// 
        ///     The [append] Active Event will also automatically convert any value it is given as its [source], for instance;
        /// 
        ///     <pre>_destination
        /// append:@/-?node
        ///   source:"foo1:bar1\nfoo2:bar2"</pre>
        /// 
        ///     The [append] keyword can also be given multiple destinations, for instance;
        ///     <pre>_dest1
        /// _dest2
        /// append:@/-1|/-2?node
        ///   source
        ///     foo1:bar1</pre>
        /// 
        ///     In addition, [append] can also of course have multiple sources, such as for instance;
        /// 
        ///     <pre>_destination
        /// _src1
        ///   foo1:bar1
        /// _src2
        ///   foo2:bar2
        /// append:@/-3?node
        ///   source:@/.(/-1|/-2)/"*"?node</pre>
        /// 
        ///     The [append] keyword can also append into reference nodes, for instance;
        /// 
        ///     <pre>_node:node:"_foo"
        /// append:@/-/#?node
        ///   source
        ///     foo1:bar1</pre>
        /// 
        ///     The [append] keyword has probably dozens of other legal permutations, and combinations. Meaning, if you can think about it, 
        ///     then [append] can probably do it. The [append] keyword, combined with 
        ///     <see cref="phosphorus.expressions.Expression">Expressions</see>, is probably one of your most flexible keywords in p5.lambda.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "append")]
        private static void lambda_append (ApplicationContext context, ActiveEventArgs e)
        {
            // figuring out source type, for then to execute the corresponding logic
            if (e.Args.Count > 0 && (e.Args.LastChild.Name == "rel-source" || e.Args.LastChild.Name == "rel-src")) {
                // relative source
                AppendRelativeSource (e.Args, context);
            } else {
                // static source
                AppendStaticSource (e.Args, context);
            }
        }

        /*
         * source is static
         */
        private static void AppendStaticSource (Node node, ApplicationContext context)
        {
            // retrieving source before we start iterating destination,
            // in case destination and source overlaps
            var sourceNodes = XUtil.SourceNodes (node, context);

            // making sure there is a source
            if (sourceNodes == null)
                return;

            // looping through every destination node
            var isFirst = true; // since source is already cloned, we avoid cloning on our first run
            foreach (var idxDestination in XUtil.Iterate (node, context)) {
                // verifying destination actually is a node
                var curDest = idxDestination.Value as Node;
                if (curDest == null)
                    throw new LambdaException ("cannot [append] into something that's not a node", node, context);

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
            foreach (var idxDestination in XUtil.Iterate (node, context)) {
                // verifying destination actually is a node
                var curDest = idxDestination.Value as Node;
                if (curDest == null)
                    throw new LambdaException ("cannot [append] into something that's not a node", node, context);

                foreach (var idxSource in XUtil.SourceNodes (node, idxDestination.Node, context)) {
                    curDest.Add (idxSource.Clone ());
                }
            }
        }
    }
}
