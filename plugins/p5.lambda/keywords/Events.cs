/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;

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
        ///     Creates one Active Event
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "set-event")]
        private static void set_event (ApplicationContext context, ActiveEventArgs e)
        {
            // creating event
            CreateEvent (XUtil.Single<string> (e.Args, context), e.Args.Clone ());
        }

        /// <summary>
        ///     Removes dynamically created Active Events
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-events")]
        private static void delete_events (ApplicationContext context, ActiveEventArgs e)
        {
            // iterating through all events to delete
            foreach (var idxName in XUtil.Iterate<string> (e.Args, context)) {

                // deleting event
                DeleteEvent (idxName);
            }
        }

        /// <summary>
        ///     Retrieves dynamically created Active Events
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "get-events")]
        private static void get_event (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover args = new Utilities.ArgsRemover (e.Args, true)) {

                // looping through all events caller wish to retrieve
                foreach (var idxEventName in XUtil.Iterate<string> (e.Args, context)) {

                    // looping through all existing event keys
                    foreach (var idxKey in _events.Keys) {

                        // checking is current event name contains current filter
                        if (idxKey.Contains (idxEventName)) {

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
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "list-events")]
        private static void list_events (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover args = new Utilities.ArgsRemover (e.Args, true)) {

                // retrieving filter, if any
                var filter = new List<string> (XUtil.Iterate<string> (e.Args, context));
                if (e.Args.Value != null && filter.Count == 0)
                    return; // possibly a filter expression, leading into oblivion, since filter still was given, we return "nothing"

                // Getting all dynamic Active Events
                ListActiveEvents (_events.Keys, e.Args, filter, "dynamic");

                // Getting all core Active Events
                ListActiveEvents (context.ActiveEvents, e.Args, filter, "static");
            }
        }

        /*
         * Creates a new Active Event
         */
        internal static void CreateEvent (string name, Node lambda)
        {
            // acquiring lock since we're consuming object shared amongst more than one thread (_events)
            lock (Lock) {

                // making sure we have a key for Active Event name
                _events [name] = new Node ();

                // adding event to dictionary
                _events [name].AddRange (lambda.Children);
            }
        }

        /*
         * removes the given dynamically created Active Event(s)
         */
        internal static void DeleteEvent (string name)
        {
            // acquiring lock since we're consuming object shared amongst more than one thread (_events)
            lock (Lock) {

                // removing event, if it exists
                if (_events.ContainsKey (name))
                    _events.Remove (name);
            }
        }

        /*
         * Returns Active Events from source given, using name as type of Active Event
         */
        private static void ListActiveEvents (
            IEnumerable<string> source, 
            Node node, 
            List<string> filter,
            string name)
        {
            // looping through each Active Event from IEnumerable
            foreach (var idx in source) {

                // checking to see if this is "private" active event
                if (idx.StartsWith ("_"))
                    continue;

                // checking to see if we have any filter
                if (filter.Count == 0) {

                    // no filter(s) given, slurping up everything
                    node.Add (new Node (name, idx));
                } else {

                    // we have filter(s), checking to see if Active Event name matches at least one of our filters
                    if (filter.Any (ix => idx.IndexOf (ix) != -1)) {
                        node.Add (new Node (name, idx));
                    }
                }
            }
        }

        /*
         * responsible for executing all dynamically created Active Events or lambda objects
         */
        [ActiveEvent (Name = "")]
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
                        lambda = _events [e.Name].Clone ();
                    }
                }

                // executing if lambda is around
                if (lambda != null) {

                    // adding up arguments, no need to clone, they should be gone after execution anyway
                    lambda.AddRange (e.Args.Children);
                    
                    // storing lambda block and value, such that we can "diff" after execution, 
                    // and only return the nodes added during execution, and the value if it was changed
                    var oldLambdaNode = new List<Node> (lambda.Children);
                    var oldValue = e.Args.Value;

                    if (e.Args.Value != null) {

                        // evaluating node, and stuffing results into arguments
                        foreach (var idx in XUtil.Iterate<object> (e.Args, context)) {

                            // adding currently iterated results into execution object
                            var idxNode = idx as Node;
                            if (idxNode != null) {

                                // appending node into execution object
                                lambda.Add (idxNode.Clone ());
                            } else {

                                // appending simple value into execution object with empty name
                                lambda.Add (string.Empty, idx);
                            }
                        }
                    }

                    // executing lambda children, and not evaluating any expression in evaluated node!
                    context.Raise ("eval-mutable", lambda);

                    // making sure we return all nodes that was created during execution of event back to caller
                    // in addition to value, but only if it was changed
                    e.Args.AddRange (lambda.Children.Where (ix => oldLambdaNode.IndexOf (ix) == -1));
                    if (oldValue != lambda.Value)
                        e.Args.Value = lambda.Value;
                    else
                        e.Args.Value = null; // removing value, since it was not changed, and hence is not part of the result
                }
            }
        }
    }
}
