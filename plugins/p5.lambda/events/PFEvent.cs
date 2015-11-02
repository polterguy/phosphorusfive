/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.lambda.events
{
    /// <summary>
    ///     Class wrapping [event] Active Event.
    /// 
    ///     Class encapsulating the Active Events that are necessary to create and manipulate dynamically created
    ///     Active Events.
    /// </summary>
    public static class PFEvent
    {
        /// <summary>
        ///     Creates zero or more active events.
        /// 
        ///     Will create zero or more dynamic Active Events, where each [lambda.xxx] node beneath the [event] keyword,
        ///     becomes the p5.lambda code executed when the event is raised. The name(s) of your Active Events,
        ///     are given as the value of the main [event] node.
        /// 
        ///     Example that creates, and invokes, an event called <em>"foo"</em>;
        /// 
        ///     <pre>event:foo
        ///   lambda
        ///     set:@/./.?value
        ///       source:Hello {0}
        ///         :@/./././.?value
        /// foo:Thomas Hansen</pre>
        /// 
        /// Dynamically created Active Events in Phosphorus basically appends all lambda objects beneath
        ///     the event creation statement into the node that invokes the Active Event upon execution. This means that
        ///     every time you invoke an Active Event, then each lambda statement added when creating your Active Event will
        ///     be logically appended beneath your Active Event invocation statement.
        /// 
        ///     Consider the following;
        /// 
        ///     <pre>event:foo
        ///   lambda
        ///     set:@/./.?value
        ///       source:Hello {0}
        ///         :@/./././.?value
        /// event:foo
        ///   lambda
        ///     set:@/./.?value
        ///       source:{0}. YO - {1}
        ///         :@/./././.?value
        ///         :@/./././.?value
        /// foo:Thomas Hansen
        /// sys42.show-tree</pre>
        /// 
        /// As you can see above, the set statement references nodes and value from apparently the "event:foo" node.
        ///     However this node will actually be your Active Event invocation node when invoked by the second last line
        ///     of Hyperlisp. Since the above code creates two Active Events with the same name, then both of these Active 
        ///     Events will be executed in the order they are created. Or to be more specific; all lambda objects, from all
        ///     Active Events with the same name, will be executed in the order they are created when the Active Event is 
        ///     executed.
        /// 
        ///     This allows you to "append" logic to an existing Active Event, to intersect each Active Event invocation,
        ///     which becomes kind of like the <em>"aspect oriented programming constructs"</em> of Phosphorus Five.
        /// 
        ///     Active Events in Phosphorus Five, to a large extent logically replaces conventional functions and methods, 
        ///     with a mechanism that allows for encapsulation on <em>"function level"</em>. This has the advantage that 
        ///     modules does not need to know anything about the internals of other modules.
        /// 
        ///     One of the advantages about this construct, is that you can have two completely independent modules handle the same event,
        ///     without even knowing anything about each other. This does however create some <em>"weird"</em> consequence. For instance, 
        ///     if you execute the above code twice, then you will double the size of your event, creating two duplicate [lambda] nodes, 
        ///     doing the exact same thing. In addition, you might have one handler for your event, taking one set of arguments, while the
        ///     other event implementation expecting another set or type of arguments.
        /// 
        ///     To eliminate that problem, you might want to use [get-event] and/or [event-remove] when creating your events, 
        ///     unless you are explicitly appending to an existing event.
        /// 
        ///     For instance, consider the following code;
        /// 
        ///     <pre>event:bar
        ///   lambda
        ///     set:@/./.?value
        ///       source:Hello {0}
        ///         :@/././././"*"/_input1?value
        /// event:bar
        ///   lambda
        ///     set:@/./.?value
        ///       source:Hello {0}
        ///         :@/././././"*"/_input2?value
        /// bar
        ///   _input2:Thomas</pre>
        /// 
        ///     There are two problems with the above code. The first problem, is that neither of the Active Event handlers checks for the existence
        ///     of the parameter they expect. The second problem, is that they both overwrite the event's root node's value, which means they will
        ///     overwrite the output, depending upon which handler is invoked first. To avoid these problems, we might change the above code to 
        ///     something like this;
        /// 
        ///     <pre>event:bar
        ///   lambda
        ///     if:@/././"*"/_input1?node
        ///       _retVal1
        ///       set:@/-?value
        ///         source:YO {0}
        ///           :@/./././././"*"/_input1?value
        ///       append:@/././.?node
        ///         source:@/./-2?node
        /// event:bar
        ///   lambda
        ///     if:@/././"*"/_input2?node
        ///       _retVal2
        ///       set:@/-?value
        ///         source:Hello {0}
        ///           :@/./././././"*"/_input2?value
        ///       append:@/././.?node
        ///         source:@/./-2?node
        /// bar
        ///   _input2:Thomas</pre>
        /// 
        ///     The above piece of code, will execute a different Active Event, depending upon which input parameters you pass in. If you
        ///     pass in [_input1], then the logic of the first [bar] Active Event will be executed, while if you pass in [_input2], then the
        ///     logic of the second [bar] Active Event will be executed. In addition, if you pass in both [_input1] and [_input2], then they
        ///     will both execute, and return their values in different nodes back to the caller.
        /// 
        ///     If you execute the above code twice though, you'll still end up creating a new set of identical Active Events. However, most 
        ///     of the time, your Active Event names will be unique, at least within your application, and you can therefor simply make sure you
        ///     remove any existing Active Events, before you create them, by using for instance;
        /// 
        ///     <pre>remove-event:bar</pre>
        /// 
        ///     Before you create your Active Events ...
        /// 
        ///     Due to the above reasons, it is crucial that you properly <em>"namespace"</em> your Active Events. Our suggestion is to use the
        ///     name of your company or organization as the <em>"root namespace"</em> for all your Active Events. Such as for instance;
        /// 
        ///     <pre>event:acme-inc.cool-stuff.foo</pre>
        /// 
        /// In the above piece of code, the <em>"acme-inc"</em> is the root namespace of your Active Event, while the <em>"cool-stuff"</em>
        ///     is another namespace inside of acme-inc. <em>"foo"</em> in the above piece of code, is the actual name of your Active Event, 
        ///     though the entire namespace(s), in addition to the name of the Active Event is necessary to invoke your Active Event.
        ///     To invoke the above Active Event, use something like this;
        /// 
        ///     <pre>acme-inc.cool-stuff.foo</pre>
        /// 
        /// The usage of namespaces in your Active Event is purely a convention, and carries no semantic difference for the behavior
        ///     of your Active Events.
        /// 
        ///     All parameters passed into an Active Event will be automatically de-referencable from inside your Active Event, in
        ///     addition to that the Active Event will by default have access to any Node from your execution tree. Consider the 
        ///     following code;
        /// 
        ///     <pre>_foo-data:"Text - "
        /// event-remove:foo
        ///   event:foo
        ///     lambda
        ///       set:@/./.?value
        ///         source:@/../0?value
        ///     for-each:@/././*(!/lambda)?value
        ///       set:@/././.?value
        ///         source:{0}{1}
        ///           :@/././././.?value
        ///           :@/./././*/__dp?value
        /// foo
        ///   :Howdy
        ///   :" "
        ///   :World</pre>
        /// 
        /// The above piece of code, will first set the foo invocation node's value to the value of <em>"_foo-data"</em>, then
        ///     it will concatenate each child node's value of the foo invocation node into the foo node's value, the result becoming
        ///     <em>"Text - Howdy World"</em>. Notice how the <em>"(!/lambda)"</em> parts of the expression above removes the actual
        ///     lambda node from the nodes traversed in the for-each.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "event")]
        private static void lambda_event (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure there's at least one actual [lambda] object within Active Event creation statement
            var lambdas = new List<Node> (e.Args.FindAll (idx => idx.Name.StartsWith ("lambda")));
            if (lambdas.Count == 0)
                throw new LambdaException ("[event] requires at least one [lambda.xxx] child", e.Args, context);

            // iterating through each name for new Active Event(s)
            foreach (var idxName in XUtil.Iterate<string> (e.Args, context)) {
                // creating event
                EventsCommon.CreateEvent (idxName, lambdas);
            }
        }
    }
}
