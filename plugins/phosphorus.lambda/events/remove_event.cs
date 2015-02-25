
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.lambda
{
    /// <summary>
    /// class wrapping [remove-event] keyword
    /// </summary>
    public static class delete_event
    {
        /// <summary>
        /// removes zero or more dynamically created Active Events
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "remove-event")]
        private static void lambda_remove_event (ApplicationContext context, ActiveEventArgs e)
        {
            // iterating through all events to delete
            foreach (var idxName in XUtil.Iterate<string> (e.Args, context)) {

                // deleting event
                events_common.RemoveEvent (idxName);
            }
        }
    }
}
