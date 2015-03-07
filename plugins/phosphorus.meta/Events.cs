/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
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
        ///     returns all active events, alternatively containing the string given through [pf.meta.event.list]
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.meta.event.list")]
        private static void pf_meta_list_events (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving filter, if any
            var filter = new List<string> (XUtil.Iterate<string> (XUtil.TryFormat<object> (e.Args, context, null), e.Args, context));
            if (e.Args.Value != null && filter.Count == 0)
                return; // possibly a filter expression, leading into oblivion

            // looping through each Active Event from core
            foreach (var idx in context.ActiveEvents) {
                // checking to see if we have any filter
                if (filter.Count == 0) {
                    // no filter(s) given, slurping up everything
                    e.Args.Add (new Node ("static", idx));
                } else {
                    // we have filter(s), checking to see if Active Event name matches at least one of our filters
                    if (filter.Any (idxFilter => idx.IndexOf (idxFilter, StringComparison.Ordinal) != -1)) {
                        e.Args.Add (new Node ("static", idx));
                    }
                }
            }
        }

        /// <summary>
        ///     returns all overrides in system, alternatively containing the string given through [pf.meta.list-overrides]
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.meta.list-overrides")]
        private static void pf_meta_list_overrides (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving filter, if any
            var filter = new List<string> (XUtil.Iterate<string> (XUtil.TryFormat<object> (e.Args, context, null), e.Args, context));
            if (e.Args.Value != null && filter.Count == 0)
                return; // possibly a filter expression, leading into oblivion

            // looping through each override base event from core
            foreach (var idxBase in context.Overrides) {
                // checking to see if we have any filter
                bool didAdd = false;
                if (filter.Count == 0) {
                    // no filter(s) given, slurping up everything
                    e.Args.Add (new Node (idxBase.Item1));
                    didAdd = true;
                } else {
                    // we have filter(s), checking to see if Active Event name matches at least one of our filters
                    if (filter.Any (idxFilter => idxBase.Item1.IndexOf (idxFilter, StringComparison.Ordinal) != -1)) {
                        e.Args.Add (new Node (idxBase.Item1));
                        didAdd = true;
                    }
                }
                // looping through all super events associated with base event from core, but only if previous operation actually added base event
                if (didAdd) {
                    foreach (var idxSuper in idxBase.Item2) {
                        e.Args.LastChild.Add (idxSuper);
                    }
                }
            }
        }
    }
}