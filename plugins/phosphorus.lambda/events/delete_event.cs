
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
    /// class wrapping [delete-event] keyword
    /// </summary>
    public static class delete_event
    {
        /// <summary>
        /// deletes one or more dynamically created Active Events
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-event")]
        private static void lambda_delete_event (ApplicationContext context, ActiveEventArgs e)
        {
            // iterating through all events to delete
            foreach (var idxName in XUtil.Iterate<string> (e.Args, context)) {

                // deleting event
                events_common.DeleteEvent (idxName);
            }
        }
    }
}
