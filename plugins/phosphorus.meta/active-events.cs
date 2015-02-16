
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.lambda
{
    /// <summary>
    /// contains meta active events to retrieve all Active Events and overrides within system
    /// </summary>
    public static class activeEvents
    {
        /// <summary>
        /// returns all active events, alternatively containing the string given through [pf.meta.list-events]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.meta.list-events")]
        private static void pf_meta_list_events (ApplicationContext context, ActiveEventArgs e)
        {
            string query = e.Args.Get<string> (context);
            foreach (var idx in context.ActiveEvents) {
                if (string.IsNullOrEmpty (query) || idx.Contains (query))
                    e.Args.Add (new Node (string.Empty, idx));
            }
            foreach (var idx in context.Overrides) {
                if (string.IsNullOrEmpty (query) || idx.Item1.Contains (query))
                    e.Args.Add (new Node (string.Empty, idx.Item1));
            }
        }

        /// <summary>
        /// returns all overrides in system, alternatively containing the string given through [pf.meta.list-overrides]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.meta.list-overrides")]
        private static void pf_meta_list_overrides (ApplicationContext context, ActiveEventArgs e)
        {
            string query = e.Args.Get<string> (context);
            foreach (var idx in context.Overrides) {
                if (query == null || idx.Item1.Contains (query)) {
                    Node over = new Node (string.Empty, idx.Item1);
                    foreach (string idxStr in idx.Item2) {
                        over.Add (new Node (string.Empty, idxStr));
                    }
                    e.Args.Add (over);
                }
            }
        }
    }
}
