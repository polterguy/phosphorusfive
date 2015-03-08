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
    public static class Event
    {
        /// <summary>
        ///     Creates zero or more active events.
        /// 
        ///     Will create zero or more dynamic Active Events, where each [lambda.xxx] node beneath the [event] keyword
        ///     becomes the pf.lambda code executed when the Active Event is raised. The name(s) of your Active Events,
        ///     as given as the value of the main [event] node.
        /// 
        ///     Example that creates and invokes an Active Event called "foo";
        /// 
        ///     <pre>event:foo
        ///   lambda
        ///     set:@/./.?value
        ///       source:Hello {0}
        ///         :@/./././.?value
        /// foo:Thomas Hansen</pre>
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
