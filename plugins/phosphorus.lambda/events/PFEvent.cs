/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.expressions.exceptions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

/// <summary>
///     Main namespace for everything regarding dynamically created Active Events.
/// 
///     A dynamic Active Event is created using the [event] keyword. You can also override dynamically created
///     Active Events by using the [override] keyword. This namespace, contains the necessary Active Events to handle
///     dynamically created Active Events.
/// </summary>
namespace phosphorus.lambda.events
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
        ///     becomes the pf.lambda code executed when the event is raised. The name(s) of your Active Events,
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
        /// Active Events in Phosphorus.Five, to a large extent replaces conventional functions and methods, with a mechanism that
        ///     allows for polymorphism and encapsulation on <em>"function level"</em>. This has the advantage that modules does not
        ///     need to know anything about the internals of other modules.
        /// 
        ///     One of the advantages about this construct, is that you can have two completely independent modules handle the same event,
        ///     without even knowing anything about each other. This does however create some <em>"weird"</em> consequence. For instance, 
        ///     if you execute the above code twice, then you will double the size of your event, creating two duplicate [lambda] nodes, 
        ///     doing the exact same thing. In addition, you might have one handler for your event, taking one set of arguments, while the
        ///     other event implementation expecting another set of arguments.
        /// 
        ///     To eliminate that problem, you might want to use [pf.meta.event.get] and/or [remove-event] when creating your events, 
        ///     unless you are explicitly appending to an existing event. In addition, you should avoid raising exceptions inside of your
        ///     Active Events, unless you really have to, since even though one set of arguments does not in any ways create a sane execution
        ///     tree for one handler, it might still provide a perfectly valid piece of input to another handler, handling the same event.
        /// 
        ///     For instance, consider the following;
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
        ///     of the parameter they expect. The second problem, is that they both write to the event's root node's value, which means they will
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
        ///     Due to the above reasons, it is crucial that you properly <em>"namespace"</em> your Active Events. My suggestion is to use the
        ///     name of your company or organization as the <em>"root namespace"</em> for all your Active Events.
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
