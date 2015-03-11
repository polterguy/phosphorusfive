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

/// <summary>
///     Main namespace for "meta" Active Events.
/// 
///     Meta Active Events are Active Events that doesn't necessarily produce a result, or executes code that does anything, but rather
///     returns information about the system's <em>"state"</em>. A good exampe is [pf.meta.event.list], which returns all Active Events
///     registered in the system.
/// 
///     To some extent, the "meta" Active Events, are actually scattered around the entire system. This is because the
///     different modules, will add their own meta Active Events.
/// </summary>
namespace phosphorus.meta
{
    /// <summary>
    ///     Wraps necessary Active Events to retrieve information about Active Events.
    /// 
    ///     Contains meta Active Events to retrieve information about all Active Events, and overrides within system.
    /// </summary>
    public static class Events
    {
        /// <summary>
        ///     Wraps Active Events that returns all registered Active Events and overrides within the system.
        /// 
        ///     Returns all active events. You can optionally add a filter criteria, that will only list Active Events containing
        ///     the string(s) passed in as an argument.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
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
        ///     Returns all overrides in the system.
        /// 
        ///     Optionally supply a filter criteria, or string(s), that each Active Event must contain in order to be defined as a match.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
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
