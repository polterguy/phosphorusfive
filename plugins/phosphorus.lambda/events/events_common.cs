
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
    /// contains common stuff for events
    /// </summary>
    public static class events_common
    {
        // contains our list of dynamically created Active Events
        private static Dictionary<string, Node> _events = new Dictionary<string, Node> ();

        // used to remember the overrides across Application Context instances
        private static Dictionary<string, List<string>> _overrides = new Dictionary<string, List<string>> ();

        // used to create lock when creating, deleting and consuming events
        private static object _lock;

        static events_common ()
        {
            _lock = new object ();
        }

        /// <summary>
        /// creates a dynamic Active Event
        /// </summary>
        /// <param name="name">name of Active Event</param>
        /// <param name="lambdas">lambda objects</param>
        public static void CreateEvent (string name, IEnumerable<Node> lambdas)
        {
            // acquiring lock, in case multiple threads creates the same event, or event is consumed by another
            // thread simultaneously
            lock (_lock) {

                // making sure we have a key for Active Event name
                if (!_events.ContainsKey (name))
                    _events [name] = new Node ();

                // looping through each "lambda.xxx" node inside of event creation node, appending these
                // into our event node
                foreach (Node idxLambda in lambdas) {
                    _events [name].Add (idxLambda.Clone ());
                }
            }
        }

        /// <summary>
        /// deletes all dynamically Active Events created with the [event] keyword with the given name
        /// </summary>
        /// <param name="name">name of Active Event(s) to delete</param>
        public static void DeleteEvent (string name)
        {
            // acquiring lock
            lock (_lock) {

                // removing event, if it exists
                if (_events.ContainsKey (name))
                    _events.Remove (name);
            }
        }

        /// <summary>
        /// creates an override from baseEvent to superEvent
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="baseEvent">event to override</param>
        /// <param name="superEvent">event supposed to override baseEvent</param>
        public static void CreateOverride (ApplicationContext context, string baseEvent, string superEvent)
        {
            lock (_lock) {

                // making sure we can remember override across context objects
                if (!_overrides.ContainsKey (baseEvent))
                    _overrides [baseEvent] = new List<string> ();
                _overrides [baseEvent].Add (superEvent);

                // making sure core actually creates our override for current context
                context.Override (baseEvent, superEvent);
            }
        }

        /*
         * responsible for re-mapping our overrides when Application Context is initialized
         */
        [ActiveEvent (Name = "pf.core.initialize-application-context")]
        private static void pf_core_initialize_application_context (ApplicationContext context, ActiveEventArgs e)
        {
            // looping through each override previously created, making sure they're re-mapped
            foreach (var idxBase in _overrides.Keys) {
                foreach (var idxSuper in _overrides [idxBase]) {
                    context.Override (idxBase, idxSuper);
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

                // used to signal whether or not we should actually raise the event
                bool shouldRaise = false;

                // acquiring lock
                lock (_lock) {

                    // then re-checking after lock is acquired, to make sure event is still around
                    if (_events.ContainsKey (e.Name)) {

                        // looping through all [lambda.xxx] objects in event node, concatenating these into
                        // event invocation statement, before we release lock, and execute event invocation node
                        e.Args.AddRange (_events [e.Name].Clone ().Children);

                        // signaling to outer parts that we should raise event
                        shouldRaise = true;
                    }
                }

                // in case event was deleted efter first check, but before nodes were appended, we double check here ...
                if (shouldRaise)
                    context.Raise ("lambda", e.Args);
            }
        }
    }
}
