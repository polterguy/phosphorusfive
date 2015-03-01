/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.expressions.exceptions;

// ReSharper disable UnusedMember.Local

namespace phosphorus.lambda.events
{
    /// <summary>
    ///     class wrapping [event] keyword
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public static class Event
    {
        /// <summary>
        ///     creates zero or more active events with the given name, where body is taken from
        ///     all [lambda.xxx] children nodes
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
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