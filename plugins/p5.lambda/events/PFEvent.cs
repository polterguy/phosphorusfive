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
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "set-event")]
        private static void lambda_set_event (ApplicationContext context, ActiveEventArgs e)
        {
            // creating event(s)
            foreach (var idxEvt in XUtil.Iterate<string> (e.Args, context)) {

                EventsCommon.CreateEvent (idxEvt, e.Args.Clone ());
            }
        }
    }
}
