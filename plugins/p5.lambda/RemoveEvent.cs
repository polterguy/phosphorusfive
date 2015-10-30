/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;
using p5.exp;

namespace p5.lambda
{
    /// <summary>
    ///     Class wrapping [remove-event] keyword.
    /// 
    ///     Class encapsulating [remove-event], and its associated helper methods.
    /// </summary>
    public static class RemoveEvent
    {
        /// <summary>
        ///     Removes zero or more dynamically created Active Events.
        /// 
        ///     Will remove all dynamically created Active Events with the given name(s).
        /// 
        ///     Example;
        /// 
        ///     <pre>event-remove:foo</pre>
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "event-remove")]
        private static void event_remove (ApplicationContext context, ActiveEventArgs e)
        {
            // iterating through all events to delete
            foreach (var idxName in XUtil.Iterate<string> (e.Args, context)) {
                // deleting event
                EventsCommon.RemoveEvent (idxName);
            }
        }
    }
}
