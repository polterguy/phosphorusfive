
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.lambda
{
    /// <summary>
    /// class wrapping [event] keyword
    /// </summary>
    public static class pfEvent
    {
        /// <summary>
        /// creates an active event with the given name, where body is taken from children [lambda.xxx] nodes
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "event")]
        private static void lambda_event (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving name
            string name = XUtil.Single<string> (e.Args, context);
            if (string.IsNullOrEmpty (name))
                throw new ArgumentException ("no event name given to [event] statement");

            // making sure there's at least one actual [lambda] object within Active Event creation statement
            List<Node> lambdas = new List<Node> (e.Args.FindAll (
                delegate (Node idx) {
                    return idx.Name.StartsWith ("lambda");
            }));
            if (lambdas.Count == 0)
                throw new ArgumentException ("[event] requires at least one [lambda.xxx] child");

            // creating event
            events_common.CreateEvent (name, lambdas);
        }
    }
}
