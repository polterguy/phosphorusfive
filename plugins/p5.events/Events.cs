/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.events
{
    /// <summary>
    ///     Common helper methods for dynamically created Active Events.
    /// </summary>
    public static class Events
    {
        // Contains our list of dynamically created Active Events
        private static readonly Dictionary<string, Node> _events = new Dictionary<string, Node> ();

        // Used to create lock when creating, deleting and consuming events
        private static readonly object Lock;

        // Necessary to make sure we have a global "lock" object
        static Events ()
        {
            Lock = new object ();
        }

        /// <summary>
        ///     Creates (or deletes) an Active Event
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "create-event")]
        [ActiveEvent (Name = ".create-event")]
        public static void create_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Checking to see if this event has no lambda objects, and is not protected, at which case it is a "delete event" invocation
            if (e.Args.Children.Count (ix => ix.Name != "") == 0) {

                // Deleting event, if existing, since it doesn't have any lambda objects associated with it
                DeleteEvent (XUtil.Single<string> (context, e.Args), context, e.Args, e.Name.StartsWith ("."));
            } else {

                // Creating new event
                CreateEvent (XUtil.Single<string> (context, e.Args), e.Args, context, e.Name.StartsWith ("."));
            }
        }

        /// <summary>
        ///     Removes dynamically created Active Events
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-event")]
        public static void delete_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Iterating through all events to delete
            foreach (var idxName in XUtil.Iterate<string> (context, e.Args)) {

                // Deleting event
                DeleteEvent (idxName, context, e.Args, e.Name.StartsWith ("."));
            }
        }

        /// <summary>
        ///     Lists all dynamically created Active Events.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "vocabulary")]
        [ActiveEvent (Name = ".vocabulary")]
        public static void vocabulary (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving filter, if any
                var filter = new List<string> (XUtil.Iterate<string> (context, e.Args));

                // Getting all dynamic Active Events
                ListActiveEvents (_events.Keys, e.Args, filter, "dynamic", context, e.Name.StartsWith ("."));

                // Getting all core Active Events
                ListActiveEvents (context.ActiveEvents, e.Args, filter, "static", context, e.Name.StartsWith ("."));
            }
        }

        /*
         * Creates a new Active Event
         */
        internal static void CreateEvent (string name, Node args, ApplicationContext context, bool isNative)
        {
            // Sanity check
            if (!isNative && (name.StartsWith ("_") || name.StartsWith (".")))
                throw new LambdaException ("Tried to create a 'protected event'", args, context);

            // Acquire lock since we're consuming object shared amongst more than one thread (_events)
            lock (Lock) {

                // Making sure we have a key for Active Event name
                _events [name] = new Node ("");

                // Adding event to dictionary
                _events [name].AddRange (args.Clone ().Children);
            }
        }

        /*
         * Removes the given dynamically created Active Event(s)
         */
        internal static void DeleteEvent (string name, ApplicationContext context, Node args, bool isNative)
        {
            // Sanity check
            if (!isNative && (name.StartsWith ("_") || name.StartsWith (".")))
                throw new LambdaException ("Tried to create a 'protected event'", args, context);

            // Acquire lock since we're consuming object shared amongst more than one thread (_events)
            lock (Lock) {

                // Removing event, if it exists
                if (_events.ContainsKey (name)) {
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
            ApplicationContext context,
            bool isNative)
        {
            // Looping through each Active Event from IEnumerable
            foreach (var idx in source) {

                if (!isNative && (idx.StartsWith (".") || idx.StartsWith ("_")))
                    continue;

                // Checking to see if we have any filter
                if (filter.Count == 0) {

                    // No filter(s) given, slurping up everything
                    node.Add (new Node (eventTypeName, idx));
                } else {

                    // We have filter(s), checking to see if Active Event name matches at least one of our filters
                    if (filter.Any (ix => idx.Contains (ix))) {
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
         * Responsible for executing all dynamically created Active Events or lambda objects
         */
        [ActiveEvent (Name = "")]
        private static void _p5_core_null_active_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Checking if there's an event with given name in dynamically created events.
            // To avoid creating a lock on every single event invocation in system, we create a "double check"
            // here, first checking for existance of key, then to create lock, for then to re-check again, which
            // should significantly improve performance of event invocations in the system
            if (_events.ContainsKey (e.Name)) {

                // Keep a reference to all lambda objects in current event, such that we can later delete them
                Node lambda = null;

                // Acquire lock to make sure we're thread safe.
                // This lock must be released before event is invoked, and is only here since we're consuming
                // an object shared among different threads (_events)
                lock (Lock) {

                    // Then re-checking after lock is acquired, to make sure event is still around.
                    // Note, we could acquire lock before checking first time, but that would impose
                    // a significant overhead on all Active Event invocations, since "" (null Active Events)
                    // are invoked for every single Active Event raised in system.
                    // In addition, by releasing the lock before we invoke the Active Event, we further
                    // avoid deadlocks by dynamically created Active Events invoking other dynamically
                    // created Active Events
                    if (_events.ContainsKey (e.Name)) {

                        // Adding event into execution lambda
                        lambda = _events [e.Name];
                    }
                }

                // Executing if lambda is around
                if (lambda != null) {

                    // Raising Active Event
                    XUtil.RaiseEvent (context, e.Args, lambda.Clone (), e.Name);
                }
            }
        }
    }
}
