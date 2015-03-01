/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local

namespace phosphorus.lambda.events
{
    /// <summary>
    ///     class wrapping [remove-event] keyword
    /// </summary>
    public static class RemoveEvent
    {
        /// <summary>
        ///     removes zero or more dynamically created Active Events
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "remove-event")]
        private static void lambda_remove_event (ApplicationContext context, ActiveEventArgs e)
        {
            // iterating through all events to delete
            foreach (var idxName in XUtil.Iterate<string> (e.Args, context)) {
                // deleting event
                EventsCommon.RemoveEvent (idxName);
            }
        }
    }
}