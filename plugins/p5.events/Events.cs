/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using System.Threading;

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
        private static readonly ReaderWriterLockSlim _lock;

        // Necessary to make sure we have a global "lock" object
        static Events ()
        {
            _lock = new ReaderWriterLockSlim ();
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
            // Acquire write lock, since we're consuming object shared amongst more than one thread (_events).
            _lock.EnterWriteLock ();
            try {

                // Checking to see if this event has no lambda objects, and is not protected, at which case it is a "delete event" invocation
                if (e.Args.Children.Count (ix => ix.Name != "") == 0) {

                    // Deleting event, if existing, since it doesn't have any lambda objects associated with it
                    DeleteEvent (XUtil.Single<string> (context, e.Args), context, e.Args, e.Name.StartsWith ("."));

                } else {

                    // Creating new event
                    CreateEvent (XUtil.Single<string> (context, e.Args), e.Args, context, e.Name.StartsWith ("."));
                }
            } finally {

                // Making sure we release lock in a finally, such that we can never exit method, without releasing our lock.
                _lock.ExitWriteLock ();
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
            // Acquire write lock, since we're consuming object shared amongst more than one thread (_events).
            _lock.EnterWriteLock ();
            try {

                // Iterating through all events to delete
                foreach (var idxName in XUtil.Iterate<string> (context, e.Args)) {

                    // Deleting event
                    DeleteEvent (idxName, context, e.Args, e.Name.StartsWith ("."));
                }
            } finally {

                // Making sure we release lock in a finally, such that we can never exit method, without releasing our lock.
                _lock.ExitWriteLock ();
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
            // Retrieving filter, if any
            var filter = e.Args.Value == null ? new List<string> () : new List<string> (XUtil.Iterate<string> (context, e.Args));

            // Acquire read lock, since we're consuming object shared amongst more than one thread (_events).
            _lock.EnterReadLock ();
            try {

                // Getting all dynamic Active Events
                ListActiveEvents (_events.Keys, e.Args, filter, "dynamic", context, e.Name.StartsWith ("."));

            } finally {

                // Making sure we release lock in a finally, such that we can never exit method, without releasing our lock.
                _lock.ExitReadLock ();
            }

            // Getting all core Active Events
            ListActiveEvents (context.ActiveEvents, e.Args, filter, "static", context, e.Name.StartsWith ("."));

            // Sorting such that static events comes first, and then having keywords coming.
            e.Args.Sort (delegate (Node lhs, Node rhs) {
                if (lhs.Name == "static" && rhs.Name == "dynamic")
                    return -1;
                else if (lhs.Name == "dynamic" && rhs.Name == "static")
                    return 1;
                if (!lhs.Get<string> (context).Contains (".") && rhs.Get<string> (context).Contains ("."))
                    return -1;
                else if (lhs.Get<string> (context).Contains (".") && !rhs.Get<string> (context).Contains ("."))
                    return 1;
                else
                    return lhs.Get<string> (context).CompareTo (rhs.Value);
            });
        }

        /*
         * Responsible for executing all dynamically created Active Events or lambda objects
         */
        [ActiveEvent (Name = "")]
        private static void _p5_core_null_active_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Acquire read lock, since we're consuming object shared amongst more than one thread (_events).
            // This lock must be released before event is invoked, and is only here since we're consuming
            Node lambda = null;
            _lock.EnterReadLock ();
            try {

                // Checking if there's an event with given name in dynamically created events.
                // To avoid creating a lock on every single event invocation in system, we create a "double check"
                // here, first checking for existance of key, then to create lock, for then to re-check again, which
                // should significantly improve performance of event invocations in the system
                if (_events.ContainsKey (e.Name)) {

                    // Keep a reference to all lambda objects in current event, such that we can later delete them
                    lambda = _events[e.Name].Clone ();
                }

            } finally {

                // Making sure we release lock in a finally, such that we can never exit method, without releasing our lock.
                _lock.ExitReadLock ();
            }

            // Raising Active Event, if it exists.
            if (lambda != null)
                XUtil.RaiseEvent (context, e.Name, lambda, e.Args);
        }

        /*
         * Creates a new Active Event
         */
        internal static void CreateEvent (string name, Node args, ApplicationContext context, bool isNative)
        {
            // Sanity checks.
            if (!isNative && (name.StartsWith ("_") || name.StartsWith (".") || name == ""))
                throw new LambdaException ("Tried to create a 'protected' event", args, context);

            // Cannot create an event which is already a native event.
            if (context.ActiveEvents.Count (ix => ix == name) > 0)
                throw new LambdaException ("Tried to create an event that is already a system event", args, context);

            // Making sure we have a key for Active Event name.
            _events[name] = new Node (name);

            // Adding event to dictionary.
            _events [name].AddRange (args.Clone ().Children);
        }

        /*
         * Removes the given dynamically created Active Event(s)
         */
        internal static void DeleteEvent (string name, ApplicationContext context, Node args, bool isNative)
        {
            // Sanity check
            if (!isNative && (name.StartsWith ("_") || name.StartsWith (".")))
                throw new LambdaException ("Tried to delete a 'protected event'", args, context);

            // Removing event, if it exists.
            if (_events.ContainsKey (name)) {
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
            string eventTypeName, 
            ApplicationContext context,
            bool isNative)
        {
            // Looping through each Active Event from IEnumerable
            foreach (var idx in source) {

                if (!isNative && (idx.StartsWith (".") || idx.StartsWith ("_") || idx.Contains ("._")))
                    continue;

                // Checking to see if we have any filter
                if (filter.Count == 0) {

                    // No filter(s) given, slurping up everything
                    node.Add (new Node (eventTypeName, idx));
                } else {

                    // We have filter(s), checking to see if Active Event name matches at least one of our filters
                    if (filter.Any (ix => ix.StartsWith ("~") ? idx.Contains (ix.Substring (1)) : idx == ix)) {
                        node.Add (new Node (eventTypeName, idx));
                    }
                }
            }
        }
    }
}
