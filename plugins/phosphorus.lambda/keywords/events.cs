
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
    /// class responsible for creating, deleting, overriding and removing overrides of Active Events
    /// </summary>
    public static class events
    {
        // contains our list of dynamically created Active Events
        private static Dictionary<string, Node> _events = new Dictionary<string, Node> ();

        // contains our list of overrides
        private static Dictionary<string, List<string>> _overrides = new Dictionary<string, List<string>> ();

        /// <summary>
        /// creates a new Active Event. pass in [lambda] and [overrides]. [overrides] can either be a 
        /// string through value, or a list of values as children
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "event")]
        private static void lambda_event (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving [name]
            string name = Expression.Single<string> (e.Args, false);
            if (string.IsNullOrEmpty (name))
                throw new ArgumentException ("no event name given to [event] statement");

            // creating overrides
            Node overrides = e.Args.Find ("overrides");
            if (overrides != null) {

                // adding all overrides from [overrides] node, first "main override"
                string mainBaseEvent = Expression.Single<string> (overrides, false);

                // making sure our override dictionary contains the key for the current Active Event name
                if (!string.IsNullOrEmpty (mainBaseEvent) && !_overrides.ContainsKey (mainBaseEvent)) {
                    _overrides [mainBaseEvent] = new List<string> ();
                }
                if (!string.IsNullOrEmpty (mainBaseEvent)) {
                    _overrides [mainBaseEvent].Add (name);
                    context.Override (mainBaseEvent, name);
                }

                // then all "children" overrides
                foreach (Node idxBaseNode in overrides.Children) {
                    string idxBaseName = Expression.Single<string> (idxBaseNode, true);
                    if (!_overrides.ContainsKey (idxBaseName))
                        _overrides [idxBaseName] = new List<string> ();
                    _overrides [idxBaseName].Add (name);
                    context.Override (idxBaseName, name);
                }
            }

            // creating event, but removing [overrides] node, if it exists
            if (!_events.ContainsKey (name))
                _events [name] = new Node ();
            _events [name].AddRange (e.Args.Clone ().Remove ("overrides").Children);
        }

        /// <summary>
        /// deletes all existing events with the given name given through the value of the [delete-event] node
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-event")]
        private static void lambda_delete_event (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving [name] of event to delete
            string name = Expression.Single<string> (e.Args, true);
            if (string.IsNullOrEmpty (name))
                throw new ArgumentException ("no event name given to [delete-event]");

            // actually deleting event
            _events.Remove (name);
        }

        /// <summary>
        /// [call-base] calls the base Active Event, if any, for the given invocation of the currently
        /// executing Active Event. pass in arguments to base as children, otherwise base event won't
        /// have any arguments during execution
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "call-base")]
        private static void lambda_call_base (ApplicationContext context, ActiveEventArgs e)
        {
            // finding Active Event node, such that we can retrieve base Active Event
            Node current = e.Args.Parent;
            while (current [0].Name != "__base") {
                current = current.Parent;
                if (current == null)
                    return;
            }

            // retrieving base Active Event name
            string activeEventName = current [0] [0].Get<string> ();

            // invoking base event
            if (_events.ContainsKey (activeEventName)) {
                
                // appending base information to current invocation, and removing "this" event from base list
                // of args passed into "this invocation", but only if base has additional base events
                if (current [0].Count > 1) {
                    e.Args.Insert (0, current [0].Clone ());
                    e.Args [0] [0].Untie ();
                }

                // invoking event
                e.Args.AddRange (_events [activeEventName].Clone ().Children);
                context.Raise ("lambda", e.Args);
                
                // cleaning up "base list" after execution of base, but only if there is any "base events" for current base
                if (e.Args.Count > 0 && e.Args [0].Name == "__base")
                    e.Args [0].Untie ();
            } else {

                // leaving it up to core to figure out base events
                context.CallBase (e);
            }
        }

        /// <summary>
        /// dynamically overrides one method with another. pass in the method to override as value, and
        /// the method you wish to override as children's values
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "override")]
        private static void lambda_override (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving base event
            string mainBaseEvt = Expression.Single<string> (e.Args, false);
            if (string.IsNullOrEmpty (mainBaseEvt))
                return;

            // making sure we've got a key for event overridden
            if (!_overrides.ContainsKey (mainBaseEvt)) {
                _overrides [mainBaseEvt] = new List<string> ();
            }

            // adding up every override
            foreach (Node idx in e.Args.Children) {
                string overrideIdx = Expression.Single<string> (idx, true);
                _overrides [mainBaseEvt].Add (overrideIdx);
                context.Override (mainBaseEvt, overrideIdx);
            }
        }

        /// <summary>
        /// dynamically removes an existing override
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-override")]
        private static void lambda_delete_override (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving base event
            string baseEvent = Expression.Single<string> (e.Args, false);

            // looping through all children as overrides
            foreach (Node idxOverrideNode in e.Args.Children) {
                string idxOverrideName = Expression.Single<string> (idxOverrideNode, true);
                _overrides [baseEvent].Remove (idxOverrideName);
                context.RemoveOverride (baseEvent, idxOverrideName);
            }
        }

        /// <summary>
        /// responsible for "re-mapping" all dynamically created Active Events and overrides. automatically
        /// called by the framework when a new <see cref="phosphorus.core.ApplicationContext"/> is created
        /// and initialized
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.core.initialize-application-context")]
        private static void pf_core_initialize_application_context (ApplicationContext context, ActiveEventArgs e)
        {
            // initializing all overrides
            foreach (string idxOverrideBase in _overrides.Keys) {
                foreach (string idxOverridden in _overrides [idxOverrideBase]) {
                    context.Override (idxOverrideBase, idxOverridden);
                }
            }
        }

        /*
         * responsible for executing all dynamically created Active Events or lambda objects
         */
        [ActiveEvent (Name = "")]
        private static void _pf_core_null_active_event (ApplicationContext context, ActiveEventArgs e)
        {
            if (_events.ContainsKey (e.Name)) {

                // making sure "base event" is attached, if there is a base event
                if (e.Base != null) {

                    // attaching all base Active Events in consecutive order
                    e.Args.Insert (0, new Node ("__base"));
                    ActiveEventArgs idxArgs = e.Base;
                    while (idxArgs != null) {
                        e.Args [0].Add (new Node (string.Empty, idxArgs.Name));
                        idxArgs = idxArgs.Base;
                    }

                    // invoking event
                    e.Args.AddRange (_events [e.Name].Clone ().Children);
                    context.Raise ("lambda", e.Args);

                    // removing "base event" arguments
                    e.Args [0].Untie ();
                } else {

                    // no reason to massage this bugger
                    e.Args.AddRange (_events [e.Name].Clone ().Children);
                    context.Raise ("lambda", e.Args);
                }
            }
        }
    }
}
