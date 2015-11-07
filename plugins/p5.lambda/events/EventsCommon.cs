/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Linq;
using p5.core;
using p5.exp;

namespace p5.lambda.events
{
    /// <summary>
    ///     Common helper methods for dynamically created Active Events.
    /// 
    ///     Contains helper methods for creating and manipulating dynamically created Active Events.
    /// </summary>
    public static class EventsCommon
    {
        // contains our list of dynamically created Active Events
        private static readonly Dictionary<string, Node> Events = new Dictionary<string, Node> ();

        // used to create lock when creating, deleting and consuming events
        private static readonly object Lock;

        // necessary to make sure we have a global "lock" object
        static EventsCommon ()
        {
            Lock = new object ();
        }

        /// <summary>
        ///     Retrieves one or more dynamically created Active Events.
        /// 
        ///     Will return all [lambda.xxx] objects for the specified dynamically created Active Event(s).
        /// 
        ///     Example;
        /// 
        ///     <pre>get-event:foo</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "get-event")]
        private static void get_event (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover args = new Utilities.ArgsRemover (e.Args, true)) {

                // syntax checking
                if (e.Args.Value == null)
                    return; // nothing to do here

                // looping through all events caller wish to retrieve
                foreach (var idxEventName in XUtil.Iterate<string> (e.Args, context)) {

                    // looping through all existing event keys
                    foreach (var idxKey in Events.Keys) {

                        // checking is current event name contains current filter
                        if (idxKey.Contains (idxEventName)) {

                            // current Active Event contains current filter value in its name, and we have a match
                            // checking if event is already returned by a previous filter
                            if (!e.Args.Children.Any (idxExisting => idxExisting.Get<string> (context) == idxKey)) {

                                // no previous filter matched Active Event name
                                e.Args.Add (idxKey).LastChild.AddRange (Events [idxKey].Clone ().Children);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all dynamically created Active Events.
        /// 
        ///     Returns the names of all dynamically created Active Events, created through the [event] keyword.
        ///     Optionally, pass in a filter as the value of the main node.
        /// 
        ///     Example;
        /// 
        ///     <pre>list-events:foo</pre>
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
                    return; // possibly a filter expression, leading into oblivion

                // Getting all dynamic Active Events
                GetActiveEvents (Events.Keys, e.Args, filter, "dynamic");
            
                // Getting all core Active Events
                GetActiveEvents (context.ActiveEvents, e.Args, filter, "static");
            }
        }

        /*
         * Creates a new, or appends to an existing, Active Event the given [lambda.xxx] objects,
         * to be executed when event is raised.
         */
        internal static void CreateEvent (string name, Node lambda)
        {
            // acquiring lock since we're consuming object shared amongst more than one thread (_events)
            lock (Lock) {

                // making sure we have a key for Active Event name
                Events [name] = new Node ();

                // adding event to dictionary
                Events [name].AddRange (lambda.Children);
            }
        }

        /*
         * removes the given dynamically created Active Event(s)
         */
        internal static void RemoveEvent (string name)
        {
            // acquiring lock since we're consuming object shared amongst more than one thread (_events)
            lock (Lock) {

                // removing event, if it exists
                if (Events.ContainsKey (name))
                    Events.Remove (name);
            }
        }

        /*
         * Returns Active Events from source given, using name as type of Active Event
         */
        private static void GetActiveEvents (
            IEnumerable<string> source, 
            Node node, 
            List<string> filter,
            string name)
        {
            // looping through each Active Event from IEnumerable
            foreach (var idx in source) {

                // checking to see if we have any filter
                if (filter.Count == 0) {

                    // no filter(s) given, slurping up everything
                    node.Add (new Node (name, idx));
                } else {

                    // we have filter(s), checking to see if Active Event name matches at least one of our filters
                    if (filter.Any (idxFilter => idx.IndexOf (idxFilter, StringComparison.Ordinal) != -1)) {
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
            // should significantly improve performance of event invocations in system
            if (Events.ContainsKey (e.Name)) {

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
                    if (Events.ContainsKey (e.Name)) {

                        // adding event into execution lambda
                        lambda = Events [e.Name].Clone ();
                    }
                }

                // executing if lambda is around
                if (lambda != null) {

                    // copying all nodes from beneath event node into currently executed scope
                    var list = new List<Node> (lambda.Children);
                    e.Args.AddRange (list);

                    // executing lambda children, and not evaluating any expression in evaluated node!
                    context.Raise ("lambda-children", e.Args);

                    // cleaning up after ourselves, deleting entire lambda that was added above
                    foreach (var idxLambda in list) {
                        idxLambda.UnTie ();
                    }
                }
            }
        }
    }
}
