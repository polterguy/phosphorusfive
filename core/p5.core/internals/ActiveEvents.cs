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

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace p5.core.internals
{
    /*
     * Class wrapping all registered Active Events in your ApplicationContext.
     */
    internal class ActiveEvents
    {
        // Maps between an Active Event name, and its Active Event methods.
        // Each Active Event can have multiple handlers (List), and each method that handles the Active Event, optionally has an instance,
        // if the method is an instance method.
        private Dictionary<string, List<Tuple<MethodInfo, object>>> _events = new Dictionary<string, List<Tuple<MethodInfo, object>>> ();

        /*
         * Creates an Active Event handler.
         * 
         * If listenerInstance is null, it's assumed to be a static event handler, otherwise an instance event handler.
         */
        internal void AddEventMethod (string activeEventName, MethodInfo method, object listenerObject = null)
        {
            // Verifying we have an entry for Active Event name.
            if (!_events.ContainsKey (activeEventName)) {

                // No existing Active Events for given name, create new event.
                _events [activeEventName] = new List<Tuple<MethodInfo, object>> ();
            }

            // Now that we have for sure created an Active Event entry, we can add the actual MethodInfo/instance.
            // Notice, in case for some reasons, the same listener object is added twice, or the same assembly is loaded twice, we verify that
            // this handler has not been previously added to our list of events.
            if (!_events [activeEventName].Exists (ix => ix.Item2 == listenerObject && ix.Item1 == method))
                _events [activeEventName].Add (new Tuple<MethodInfo, object> (method, listenerObject));
        }

        /*
         * Deletes all event handlers for the given object instance.
         */
        internal void DeleteEventsForInstance (object listenerObject)
        {
            // Sanity check.
            if (listenerObject == null)
                throw new ArgumentException (nameof (listenerObject));

            // Iterating through each key, for then to check if there's a registered listening object in events, for the given listenerObject,
            // for then to, if it exists, deleting it from events dictionary.
            // Notice our "ToList" invocation, to not invalidate IEnumerable, since we're modifying dictionary during enumeration.
            foreach (var activeEventName in _events.Keys.ToList ()) {

                // Removing all events associated with the specified listening object.
                _events [activeEventName].RemoveAll (ix => ix.Item2 == listenerObject);

                // Checking if this was the only remaining MethodInfo/instance-object for given Active Event, and if so, 
                // removing the Active Event entirely.
                if (_events [activeEventName].Count == 0)
                    _events.Remove (activeEventName);
            }
        }

        /*
         * Raise the Active Event with the specified name, with the given args (if any), within the given application context.
         */
        internal Node RaiseEvent (ApplicationContext context, Node args, string activeEventName)
        {
            try {
                // Constructing EventArgs to pass into event handler.
                // If no Node is null, we create a new Node, such that event handlers are never given null node, and can return
                // values, even though caller did not pass in a Node into handler.
                var e = new ActiveEventArgs (activeEventName, args ?? new Node());

                // Checking if we have any Active Event handlers for the given name.
                if (_events.ContainsKey (activeEventName)) {

                    // Looping through all Active Events handlers for the given Active Event name, invoking all methods handling event.
                    // Notice, we must iterate the events using "ToList", since an Active Event, theoretically, might create a new Active Event,
                    // invalidating the enumerator for our foreach loop.
                    foreach (var idxMethod in _events [activeEventName].ToList ()) {
                        idxMethod.Item1.Invoke (idxMethod.Item2, new object[] { context, e });
                    }
                }

                // Then iterating through all "null Active Event handlers" afterwards, and invoking these handlers.
                if (_events.ContainsKey ("")) {

                    // Active Event was not protected, and we have a "null event handler".
                    foreach (var idxMethod in _events [""].ToList ()) {
                        idxMethod.Item1.Invoke (idxMethod.Item2, new object[] { context, e });
                    }
                }

                // Returning results of invocation of event to caller.
                return e.Args;

            } catch (TargetInvocationException err) {

                // Making sure we transform reflection exceptions into actual exceptions thrown.
                ExceptionDispatchInfo.Capture (err.InnerException).Throw();

                // Never reached, but needed for compiler to not choke, since it doesn't realize the above invocation to Capture will always throw ...!!
                throw;
            }
        }

        /*
         * Returns all registered Active Event names in the given instance.
         */
        internal IEnumerable<string> ActiveEventNames
        {
            get {
                return _events.Keys;
            }
        }
    }
}
