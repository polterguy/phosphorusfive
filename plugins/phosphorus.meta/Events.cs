/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Linq;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.meta
{
    /// <summary>
    ///     contains meta active events to retrieve all Active Events and overrides within system
    /// </summary>
    public static class Events
    {
        /// <summary>
        ///     returns all active events, alternatively containing the string given through [pf.meta.list-events]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext" /> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.meta.list-events")]
        private static void pf_meta_list_events (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving filter, if any
            var filter = new List<string> (e.Args.Value == null ? new string[] {} : XUtil.Iterate<string> (e.Args, context));

            // looping through each Active Event from core
            foreach (var idx in context.ActiveEvents) {
                // checking to see if we have any filter
                if (filter.Count == 0) {
                    // no filter(s) given, slurping up everything
                    e.Args.Add (new Node (string.Empty, idx));
                } else {
                    // we have filter(s), checking to see if Active Event name matches at least one of our filters
                    if (filter.Any (idxFilter => idx.IndexOf (idxFilter, StringComparison.InvariantCulture) != -1)) {
                        e.Args.Add (new Node (string.Empty, idx));
                    }
                }
            }
        }

        /// <summary>
        ///     returns all overrides in system, alternatively containing the string given through [pf.meta.list-overrides]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext" /> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.meta.list-overrides")]
        private static void pf_meta_list_overrides (ApplicationContext context, ActiveEventArgs e)
        {
            var query = e.Args.Get<string> (context);
            foreach (var idx in context.Overrides) {
                if (query == null || idx.Item1.Contains (query)) {
                    var over = new Node (string.Empty, idx.Item1);
                    foreach (var idxStr in idx.Item2) {
                        over.Add (new Node (string.Empty, idxStr));
                    }
                    e.Args.Add (over);
                }
            }
        }
    }
}