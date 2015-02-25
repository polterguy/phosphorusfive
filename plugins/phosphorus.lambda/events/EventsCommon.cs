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

namespace phosphorus.lambda.events
{
    /// <summary>
    ///     contains common stuff for global events and global overrides,
    ///     created using the [event] keyword and the [override] keyword
    /// </summary>
    public static class EventsCommon
    {
        // contains our list of dynamically created Active Events
        private static readonly Dictionary<string, Node> Events = new Dictionary<string, Node> ();
        // used to remember the overrides across Application Context instances
        private static readonly Dictionary<string, List<string>> Overrides = new Dictionary<string, List<string>> ();
        // used to create lock when creating, deleting and consuming events
        private static readonly object Lock;
        // necessary to make sure we have a global "lock" object
        static EventsCommon () { Lock = new object (); }

        /// <summary>
        ///     lists all dynamically create Active Events
        /// </summary>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext" /> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.meta.list-events")]
        private static void pf_meta_list_events (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving filter, if any
            // ps, we cannot use the default XUtil.Iterate here, since it'll start iterating children
            // if there's no expression in value of node, and since this event have multiple handlers,
            // then whatever handler comes in last, basically "looses out", and won't get to return anything,
            // since filters then being used for second handler, becomes the return values from the first handler
            var filter = new List<string> ();
            if (e.Args.Value != null) {
                if (XUtil.IsExpression (e.Args.Value)) {
                    filter.AddRange (XUtil.Iterate<string> (e.Args.Get<string> (context), e.Args, context));
                }
                else {
                    filter.Add (e.Args.Get<string> (context));
                }
            }

            // looping through each Active Event from core
            foreach (var idx in Events.Keys) {
                // checking to see if we have any filter
                if (filter.Count == 0) {
                    // no filter(s) given, slurping up everything
                    e.Args.Add (new Node ("dynamic", idx));
                } else {
                    // we have filter(s), checking to see if Active Event name matches at least one of our filters
                    if (filter.Any (idxFilter => idx.IndexOf (idxFilter, StringComparison.InvariantCulture) != -1)) {
                        e.Args.Add (new Node ("dynamic", idx));
                    }
                }
            }
        }

        /// <summary>
        ///     creates a dynamic Active Event
        /// </summary>
        /// <param name="name">name of Active Event</param>
        /// <param name="lambdas">lambda objects</param>
        public static void CreateEvent (string name, IEnumerable<Node> lambdas)
        {
            // acquiring lock since we're consuming object shared amongst more than one thread (_events)
            lock (Lock) {
                // making sure we have a key for Active Event name
                if (!Events.ContainsKey (name))
                    Events [name] = new Node ();

                // looping through each "lambda.xxx" node inside of event creation node, appending these
                // into our event node
                foreach (var idxLambda in lambdas) {
                    Events [name].Add (idxLambda.Clone ());
                }
            }
        }

        /// <summary>
        ///     removes all dynamically Active Events created with the [event] keyword with the given name
        /// </summary>
        /// <param name="name">name of Active Event(s) to delete</param>
        public static void RemoveEvent (string name)
        {
            // acquiring lock since we're consuming object shared amongst more than one thread (_events)
            lock (Lock) {
                // removing event, if it exists
                if (Events.ContainsKey (name))
                    Events.Remove (name);
            }
        }

        /// <summary>
        ///     deletes given override
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="baseEvent">name of base (overridden) event</param>
        /// <param name="superEvent">name of super (overriding) event</param>
        public static void RemoveOverride (ApplicationContext context, string baseEvent, string superEvent)
        {
            // acquiring lock since we're consuming object shared amongst more than one thread (_override)
            lock (Lock) {
                // removing override, if it exists
                if (Overrides.ContainsKey (baseEvent)) {
                    // removing override from internal list of overrides
                    Overrides [baseEvent].Remove (superEvent);
                    if (Overrides [baseEvent].Count == 0)
                        Overrides.Remove (baseEvent);

                    // removing override from core, to make sure current context has override removed
                    context.RemoveOverride (baseEvent, superEvent);
                }
            }
        }

        /// <summary>
        ///     creates an override from baseEvent to superEvent
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="baseEvent">event to override</param>
        /// <param name="superEvent">event supposed to override baseEvent</param>
        public static void CreateOverride (ApplicationContext context, string baseEvent, string superEvent)
        {
            // acquiring lock since we're consuming object shared amongst more than one thread (_overrides)
            lock (Lock) {
                // checking to see if dictionary of overrides contains base event, and if not, we create it
                if (!Overrides.ContainsKey (baseEvent)) {
                    // this event has not been overriddet yet
                    Overrides [baseEvent] = new List<string> ();
                }

                // creating a mapping from "base" to "super"
                Overrides [baseEvent].Add (superEvent);

                // making sure core actually creates our override for current context
                context.Override (baseEvent, superEvent);
            }
        }

        /*
         * responsible for re-mapping our overrides when Application Context is initialized
         */

        [ActiveEvent (Name = "pf.core.initialize-application-context")]
        // ReSharper disable once UnusedParameter.Local
        private static void pf_core_initialize_application_context (ApplicationContext context, ActiveEventArgs e)
        {
            // acquiring lock since we're consuming object shared amongst more than one thread (_overrides)
            lock (Lock) {
                // looping through each base events first
                foreach (var idxBase in Overrides.Keys) {
                    // looping through each super events for base instance
                    foreach (var idxSuper in Overrides [idxBase]) {
                        // creating override from base to super
                        context.Override (idxBase, idxSuper);
                    }
                }
            }
        }

        /*
         * responsible for executing all dynamically created Active Events or lambda objects
         */

        [ActiveEvent (Name = "")]
        private static void _pf_core_null_active_event (ApplicationContext context, ActiveEventArgs e)
        {
            // checking if there's an event with given name in dynamically created events
            // to avoid creating a lock on every single event invocation in system, we create a "double check"
            // here, first checking for existance of key, then to create lock, for then to re-check again, which
            // should significantly improve performance of event invocations in system
            if (Events.ContainsKey (e.Name)) {
                // keep a reference to all lambda objects in current event, such that we can later delete them
                var lambdas = new List<Node> ();

                // acquiring lock to make sure we're thread safe,
                // this lock must be released before event is invoked, and is only here since we're consuming
                // an object shared among different threads (_events)
                lock (Lock) {
                    // then re-checking after lock is acquired, to make sure event is still around
                    // note, we could acquire lock before checking first time, but that would impose
                    // a significant overhead on all Active Event invocations, since "" (null Active Events)
                    // are invoked for every single Active Event raised in system
                    if (Events.ContainsKey (e.Name)) {
                        // looping through all [lambda.xxx] objects in current event, concatenating these into
                        // event invocation statement, storing a reference to each lambda object,
                        // before we release lock, and execute event invocation node
                        var idxLambdaParent = Events [e.Name];
                        foreach (var idxLambda in idxLambdaParent.Children) {
                            // appending lambda nodes into current Active Event node, and storing lambda such that we can
                            // later remove it from current node
                            var tmp = idxLambda.Clone ();
                            e.Args.Add (tmp);
                            lambdas.Add (tmp);
                        }
                    }
                }

                // in case event was deleted after first check, but before nodes were appended,
                // we double check here, to be bullet proof in regards to thread safety
                if (lambdas.Count > 0) {
                    // this event was still around after acquiring lock
                    context.Raise ("lambda", e.Args);

                    // cleaning up after ourselves, deleting only the lambda objects that came
                    // from our dynamically created event
                    foreach (var idxLambda in lambdas) {
                        idxLambda.UnTie ();
                    }
                }
            }
        }
    }
}