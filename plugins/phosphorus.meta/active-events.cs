
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
    /// contains meta active events to retrieve all Active Events and overrides within system
    /// </summary>
    public static class activeEvents
    {
        // TODO: too large, refactor ...
        /// <summary>
        /// returns all active events, alternatively containing the string given through [pf.meta.list-events]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.meta.list-events")]
        private static void pf_meta_list_events (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving filter, if any
            List<string> filter = new List<string>(XUtil.Iterate<string>(e.Args, context));

            // looping through each Active Event from core
            foreach (var idx in context.ActiveEvents) {

                // checking to see if we have any filter
                if (filter.Count == 0) {

                    // no filter(s) given, slurping up everything
                    e.Args.Add (new Node (string.Empty, idx));
                } else {

                    // we have filter(s), checking to see if Active Event name matches at least one of our filters
                    foreach (var idxFilter in filter) {
                        if (idx.IndexOf (idxFilter) != -1) {

                            // yup, it matched, adding current Active Event as result, and ending our filter foreach loop
                            e.Args.Add (new Node (string.Empty, idx));
                            break;
                        }
                    }
                }
            }

            // TODO: verify we actually need this bugger, and that it is semantically correct to return
            // all overrides, when asking for Active Events ...?
            // then looping through each override, since each override potentially creates a new Active Event
            foreach (var idx in context.Overrides) {
                if (filter.Count == 0) {

                    // no filter(s) given, slurping up everything
                    e.Args.Add (new Node (string.Empty, idx.Item1));
                } else {

                    // we have filter(s), checking to see if override name matches at least one of our filters
                    foreach (var idxFilter in filter) {
                        if (idx.Item1.IndexOf (idxFilter) != -1) {

                            // yup, it matched, adding current Active Event as result, and ending our filter foreach loop
                            e.Args.Add (new Node (string.Empty, idx.Item1));
                            break;
                        }
                    }
                }
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
