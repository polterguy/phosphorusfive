
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

        /// <summary>
        /// creates a dynamic Active Event
        /// </summary>
        /// <param name="name">name of Active Event</param>
        /// <param name="lambdas">lambda objects</param>
        public static void CreateEvent (string name, IEnumerable<Node> lambdas)
        {
            // making sure we have a key for Active Event name
            if (!_events.ContainsKey (name))
                _events [name] = new Node ();

            // looping through each "lambda.xxx" node inside of event creation node, appending these
            // into our event node
            foreach (Node idxLambda in lambdas) {
                _events [name].Add (idxLambda.Clone ());
            }
        }

        /// <summary>
        /// deletes all dynamically Active Events created with the [event] keyword with the given name
        /// </summary>
        /// <param name="name">name of Active Event(s) to delete</param>
        public static void DeleteEvent (string name)
        {
            if (_events.ContainsKey (name))
                _events.Remove (name);
        }

        /*
         * responsible for executing all dynamically created Active Events or lambda objects
         */
        [ActiveEvent (Name = "")]
        private static void _pf_core_null_active_event (ApplicationContext context, ActiveEventArgs e)
        {
            if (_events.ContainsKey (e.Name)) {

                // looping through all [lambda.xxx] objects in event node, concatenating these into
                // event invocation statement, before we execute event invocation node
                e.Args.AddRange (_events [e.Name].Clone ().Children);
                context.Raise ("lambda", e.Args);
            }
        }
    }
}
