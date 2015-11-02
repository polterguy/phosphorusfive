/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.lambda
{
    /// \todo [set] should probably be allowed to create new nodes, think about it ...?
    /// <summary>
    ///     Class wrapping the [set] keyword in p5.lambda.
    /// 
    ///     The [set] keyword allows you to change nodes through p5.lambda.
    /// </summary>
    public static class Set
    {
        /// <summary>
        ///     The [set] keyword, allows you to change nodes through p5.lambda.
        /// 
        ///     The [set] keyword is your main mechanism to change nodes, values and names of nodes, and allows you to change
        ///     nodes through clever usage of <see cref="phosphorus.expressions.Expression">Expressions</see>.
        /// 
        ///     [set] takes the destination expression as its value. The [source] for your expression, must be its
        ///     last child node. You can use a relative source for [set], the same way you can with 
        ///     <see cref="p5.lambda.keywords.Append.lambda_append">[append]</see>.
        /// 
        ///     Example;
        /// 
        ///     <pre>_source:foo
        /// _destination
        /// set:@/-?value
        ///   source:@/./-2?value</pre>
        /// 
        ///      The above code will change the [_destination] node such that its value becomes that of the value of the [_source] node.
        /// 
        ///     [set] can also use constant values as its [source], and any types your [source] have, will follow along to its destination 
        ///     automatically. Consider the following code;
        /// 
        ///     <pre>_destination
        /// set:@/-?value
        ///   source:int:5</pre>
        /// 
        ///     [set] can also change multiple destinations at the same time. Every result your destination expression yields, will have its
        ///     value changed according to the [source] or [rel-source] given. This code changes the value of all children of [_dest] to
        ///     <em>"foo"</em> for instance
        /// 
        ///     <pre>_dest
        ///   bar1
        ///   bar2
        /// set:@/-/"*"?value
        ///   source:foo</pre>
        /// 
        ///     You can also use [src] and [rel-src] as synonyms to [source] and [rel-source]. To see an example of how to use a relative source,
        ///     read the documentation for the <see cref="p5.lambda.keywords.Append.lambda_append">[append]</see> keyword. [set] uses its
        ///     relative source features the same way as [append].
        /// 
        ///     If your [set] statement has no source, then the execution of your [set] will <em>"nullify"</em> your destination. Exactly what it 
        ///     means to nullify a result, depends upon what type of expression your destination is. If your destination expression is of type 'node',
        ///     then your node(s) will be removed entirely from the execution tree. If your expression is of type 'value', then the value of all nodes
        ///     matching your expression will have its value set to <em>"null"</em>. If your expression is of type 'name', then the name(s) of your
        ///     nodes matching your expression will be set to "", or string.Empty from C#. This code sets the value of all children nodes from
        ///     [_dest] to null for instance;
        /// 
        ///     <pre>
        /// _dest
        ///   foo1:bar1
        ///   foo2:bar2
        /// set:@/-/"*"?value
        ///     </pre>
        /// 
        ///     You can also set nodes themselves with [set]. Below is an example that entirely changes all nodes beneath
        ///     the [_dest] node, to a node created from its source;
        /// 
        ///     <pre>_dest
        ///   foo1:bar1
        ///   foo2:bar2
        /// set:@/-/"*"?node
        ///   source:howdy</pre>
        /// 
        ///     Notice how the string <em>"howdy"</em> will be converted into a node, for then to have that node replace both the [foo1] node, in
        ///     addition to the [foo2] node. If you replaces the value of <em>howdy</em> with for instance <em>"howdy:world"</em>, then you would
        ///     end up with two children nodes beneath [_dest] having both a name, in addition to a value.
        /// 
        ///     In general terms, Phosphorus Five will attempt to convert anything necessary to be converted before applying the [set] operation,
        ///     in addition to that you can of course explicitly convert yourself, such as the example below shows you;
        /// 
        ///     <pre>_source:555
        /// _dest
        /// set:@/-?value
        ///   source:@/./-2?value.int</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "set")]
        private static void lambda_set (ApplicationContext context, ActiveEventArgs e)
        {
            // asserting destination is expression
            if (!(e.Args.Value is Expression))
                throw new LambdaException ("Not a valid destination expression given, make sure you set [set]'s value to an expression using :x:", e.Args, context);

            // figuring out source type, for then to execute the corresponding logic
            if (e.Args.Count > 0 && e.Args.LastChild.Name == "rel-src") {

                // asserting destination is expression
                if (!(e.Args.LastChild.Value is Expression))
                    throw new LambdaException ("Not a valid relative source expression given, make sure you set [rel-src]'s value to an expression using :x:", e.Args, context);

                // iterating through all destinations, figuring out source relative to each destinations
                foreach (var idxDestination in e.Args.Get<Expression> (context).Evaluate (e.Args, context, e.Args)) {

                    // source is relative to destination, postponing figuring it out, until we're inside 
                    // our destination nodes, on each iteration, passing in destination node as data source
                    idxDestination.Value = XUtil.SourceSingle (e.Args.LastChild, Utilities.Convert<Node> (idxDestination.Node, context), context);
                }
            } else if (e.Args.Count > 0 && e.Args.LastChild.Name == "src") {

                // static source, hence retrieving source before iteration starts
                var source = XUtil.SourceSingle (e.Args.LastChild, context);

                // iterating through all destinations, updating with source
                foreach (var idxDestination in e.Args.Get<Expression> (context).Evaluate (e.Args, context, e.Args)) {
                    idxDestination.Value = source;
                }
            } else if (e.Args.Count == 0 || e.Args.LastChild.Name == "") {

                // "null source", iterating through all destinations, updating with null
                foreach (var idxDestination in e.Args.Get<Expression> (context).Evaluate (e.Args, context, e.Args)) {
                    idxDestination.Value = null;
                }
            } else {

                // Active Event invocation source, iterating through all destinations, after invocting Active event, updating
                // with result from Active Event invocation
                context.Raise (e.Args.LastChild.Name, e.Args.LastChild);
                foreach (var idxDestination in e.Args.Get<Expression> (context).Evaluate (e.Args, context, e.Args)) {
                    idxDestination.Value = e.Args.LastChild.Value;
                }
            }
        }
    }
}