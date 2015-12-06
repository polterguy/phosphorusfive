/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.events
{
    /// <summary>
    ///     Common helper methods for dynamically created Active Events.
    /// </summary>
    public static class Events
    {
        // contains our list of dynamically created Active Events
        private static readonly Dictionary<string, Node> _events = new Dictionary<string, Node> ();

        // used to create lock when creating, deleting and consuming events
        private static readonly object Lock;

        // necessary to make sure we have a global "lock" object
        static Events ()
        {
            Lock = new object ();
        }

        /// <summary>
        ///     Creates (or deletes) an Active Event
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-event", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "set-protected-event", Protection = EventProtection.LambdaClosed)]
        private static void set_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Checking to see if this event has no lambda objects, and is not protected, at which case it is a "delete event" invocation
            if (e.Args.Count == 0 && e.Name == "set-event") {

                // Deleting event, if existing, since it doesn't have any lambda objects associated with it
                DeleteEvent (XUtil.Single<string> (context, e.Args), context, e.Args);
            } else {

                // Creating new event
                CreateEvent (XUtil.Single<string> (context, e.Args), e.Args.Clone (), e.Name == "set-protected-event", context);
            }
        }

        /// <summary>
        ///     Removes dynamically created Active Events
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-events", Protection = EventProtection.LambdaClosed)]
        private static void delete_events (ApplicationContext context, ActiveEventArgs e)
        {
            // Iterating through all events to delete
            foreach (var idxName in XUtil.Iterate<string> (context, e.Args)) {

                // Deleting event
                DeleteEvent (idxName, context, e.Args);
            }
        }

        /// <summary>
        ///     Returns all protected dynamically created Active Events
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.lambda.get-protected-events", Protection = EventProtection.NativeClosed)]
        private static void p5_lambda_get_protected_events (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idxEvt in _events.Keys) {
                if (_events [idxEvt].Get<bool> (context))
                    e.Args.Add (idxEvt);
            }
        }

        /// <summary>
        ///     Retrieves dynamically created Active Events
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-events", Protection = EventProtection.LambdaClosed)]
        private static void get_event (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // looping through all events caller wish to retrieve
                foreach (var idxEventName in XUtil.Iterate<string> (context, e.Args)) {

                    // looping through all existing event keys
                    foreach (var idxKey in _events.Keys) {

                        // checking is current event name contains current filter
                        if (idxKey == idxEventName) {

                            // current Active Event contains current filter value in its name, and we have a match
                            // checking if event is already returned by a previous filter
                            if (!e.Args.Children.Any (idxExisting => idxExisting.Get<string> (context) == idxKey)) {

                                // no previous filter matched Active Event name
                                e.Args.Add (idxKey).LastChild.AddRange (_events [idxKey].Clone ().Children);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all dynamically created Active Events.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-events", Protection = EventProtection.LambdaClosed)]
        private static void list_events (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // retrieving filter, if any
                var filter = new List<string> (XUtil.Iterate<string> (context, e.Args));
                if (e.Args.Value != null && filter.Count == 0)
                    return; // possibly a filter expression, leading into oblivion, since filter still was given, we return "nothing"

                // Getting all dynamic Active Events
                ListActiveEvents (_events.Keys, e.Args, filter, "dynamic", context);

                // Getting all core Active Events
                ListActiveEvents (context.ActiveEvents, e.Args, filter, "static", context);
            }
        }

        /*
         * Creates a new Active Event
         */
        internal static void CreateEvent (string name, Node lambda, bool isProtected, ApplicationContext context)
        {
            // acquiring lock since we're consuming object shared amongst more than one thread (_events)
            lock (Lock) {

                // Checking if this is a protected event and there is an existing event with same name, 
                // or there is an existing event, and existing event is protected, and if so, throwing
                if (isProtected && _events.ContainsKey (name) || (_events.ContainsKey (name) && _events[name].Get<bool> (context)))
                    throw new LambdaException (string.Format ("Sorry, '{0}' is a protected event, and cannot be modified", name), lambda, context);

                // making sure we have a key for Active Event name
                _events [name] = new Node (string.Empty, isProtected);

                // adding event to dictionary
                _events [name].AddRange (lambda.Children);
            }
        }

        /*
         * removes the given dynamically created Active Event(s)
         */
        internal static void DeleteEvent (string name, ApplicationContext context, Node args)
        {
            // acquiring lock since we're consuming object shared amongst more than one thread (_events)
            lock (Lock) {

                // removing event, if it exists
                if (_events.ContainsKey (name)) {

                    // Checking if event is protected
                    if (_events [name].Get<bool> (context))
                        throw new LambdaException (string.Format ("You cannot delete '{0}' since it is marked as protected", name), args, context);
                    _events.Remove (name);
                }
            }
        }

        /*
         * Returns Active Events from source given, using name as type of Active Event
         */
        private static void ListActiveEvents (
            IEnumerable<string> source, 
            Node node, 
            List<string> filter,
            string eventTypeName, 
            ApplicationContext context)
        {
            // Looping through each Active Event from IEnumerable
            foreach (var idx in source) {

                // Checking to see if this is Active Event is protected for C# code only, and if so, ignoring it
                if (context.HasEvent (idx)) {

                    // There exist a native Active Event handler for this event, now getting protection level of event
                    EventProtection protection = context.GetEventProtection (idx);

                    // Checking if Active Event is protected for native code only
                    if (protection == EventProtection.NativeClosed || protection == EventProtection.NativeOpen)
                        continue;
                }

                // Checking to see if we have any filter
                if (filter.Count == 0) {

                    // No filter(s) given, slurping up everything
                    node.Add (new Node (eventTypeName, idx));
                } else {

                    // We have filter(s), checking to see if Active Event name matches at least one of our filters
                    if (filter.Any (ix => idx.IndexOf (ix) != -1)) {
                        node.Add (new Node (eventTypeName, idx));
                    }
                }
            }

            // Sorting such that keywords comes first
            node.Sort (delegate(Node x, Node y) {
                if (x.Get<string>(context).Contains (".") && !y.Get<string>(context).Contains ("."))
                    return 1;
                else if (!x.Get<string>(context).Contains (".") && y.Get<string>(context).Contains ("."))
                    return -1;
                else
                    return x.Get<string> (context).CompareTo (y.Value);
            });
        }

        /*
         * responsible for executing all dynamically created Active Events or lambda objects
         */
        [ActiveEvent (Name = "", Protection = EventProtection.NativeOpen)]
        private static void _p5_core_null_active_event (ApplicationContext context, ActiveEventArgs e)
        {
            // checking if there's an event with given name in dynamically created events
            // to avoid creating a lock on every single event invocation in system, we create a "double check"
            // here, first checking for existance of key, then to create lock, for then to re-check again, which
            // should significantly improve performance of event invocations in the system
            if (_events.ContainsKey (e.Name)) {

                // keep a reference to all lambda objects in current event, such that we can later delete them
                Node lambda = null;

                // acquiring lock to make sure we're thread safe,
                // this lock must be released before event is invoked, and is only here since we're consuming
                // an object shared among different threads (_events)
                lock (Lock) {

                    // then re-checking after lock is acquired, to make sure event is still around
                    // note, we could acquire lock before checking first time, but that would impose
                    // a significant overhead on all Active Event invocations, since "" (null Active Events)
                    // are invoked for every single Active Event raised in system
                    // In addition, by releasing the lock before we invoke the Active Event, we further
                    // avoid deadlocks by dynamically created Active Events invoking other dynamically
                    // created Active Events
                    if (_events.ContainsKey (e.Name)) {

                        // adding event into execution lambda
                        lambda = _events [e.Name];
                    }
                }

                // executing if lambda is around
                if (lambda != null) {

                    // Raising Active Event
                    XUtil.RaiseEvent (context, e.Args, lambda, e.Name);
                }
            }
        }
    }
}
