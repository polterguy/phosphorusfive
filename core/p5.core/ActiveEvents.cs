/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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
using System.Security;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace p5.core
{
    /// <summary>
    ///     Active Events wrapper class
    /// </summary>
    internal class ActiveEvents
    {
        /// <summary>
        ///     One single Active Event
        /// </summary>
        internal class ActiveEvent
        {
            /// <summary>
            ///     One single Active Event handler (method) with its associated instance (unless method is static)
            /// </summary>
            internal class MethodSink
            {
                public MethodSink(MethodInfo method, object instance)
                {
                    Method = method;
                    Instance = instance;
                }

                public MethodInfo Method {
                    get;
                    private set;
                }

                public object Instance {
                    get;
                    private set;
                }
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="p5.core.ActiveEvents+ActiveEvent"/> class.
            /// </summary>
            /// <param name="name">Name of Active Event</param>
            /// <param name="isProtected">If set to <c>true</c> is protected, otherwise it is not protected</param>
            public ActiveEvent(string name)
            {
                Name = name;
                Methods = new List<MethodSink> ();
            }

            /// <summary>
            ///     Name of Active Event
            /// </summary>
            /// <value>The name</value>
            public string Name {
                get;
                private set;
            }

            /// <summary>
            ///     Returns the list of methods tied to this specific Active Event
            /// </summary>
            /// <value>The methods</value>
            public List<MethodSink> Methods {
                get;
                private set;
            }
        }

        private Dictionary<string, ActiveEvent> _events = new Dictionary<string, ActiveEvent> ();

        /// <summary>
        ///     Creates an Active Event handler
        /// </summary>
        /// <param name="name">Name of Active Event</param>
        /// <param name="method">Event handler for Active Event</param>
        /// <param name="instance">Instance, meaning the object that is registered as a listener object. Null if event handler is static</param>
        public void AddMethod (
            string name, 
            MethodInfo method, 
            object instance)
        {
            // Verifying we have an entry for event name
            if (!_events.ContainsKey(name)) {

                // No existing event exist, create new event
                _events[name] = new ActiveEvent(name);
            }

            // Now that we have for sure created an Active Event entry, 
            // we can add the actual MethodInfo/Instance-object, but only
            // if listener is not registered from before
            if (!_events[name].Methods.Exists(ix => ix.Instance == instance && ix.Method == method))
                _events[name].Methods.Add(new ActiveEvent.MethodSink(method, instance));
        }

        /// <summary>
        ///     Removes an event handler instance for the given Active Event
        /// </summary>
        /// <param name="name">Name of Active Event</param>
        /// <param name="instance">Instance or registered listening object</param>
        public void RemoveMethod (string name, object instance)
        {
            if (_events.ContainsKey(name)) {

                // This Active Event exists, removing the Method Info associated with the given instance
                _events[name].Methods.RemoveAll(ix => ix.Instance == instance);

                // Checking if this was the only remaining MethodInfo/instance-object for given Active Event,
                // and if so, removing the Active Event entirely
                if (_events[name].Methods.Count == 0)
                    _events.Remove(name);
            }
        }

        /// <summary>
        ///     Determines whether this instance has the event with the specified name.
        /// </summary>
        /// <returns><c>true</c> if this instance has event with the specified name; otherwise, <c>false</c></returns>
        /// <param name="name">Name</param>
        public bool HasEvent (string name)
        {
            return _events.ContainsKey(name);
        }

        /// <summary>
        ///     Removes an event handler instance
        /// </summary>
        /// <param name="name">Name of Active Event</param>
        /// <param name="instance">Instance or registered listening object</param>
        public void RemoveMethod (object instance)
        {
            foreach (var name in _events.Keys.ToList ()) {

                // This Active Event exists, removing the Method Info associated with the given instance
                _events[name].Methods.RemoveAll(ix => ix.Instance == instance);

                // Checking if this was the only remaining MethodInfo/instance-object for given Active Event,
                // and if so, removing the Active Event entirely
                if (_events[name].Methods.Count == 0)
                    _events.Remove(name);
            }
        }

        /// <summary>
        ///     Raise the specified Active Event with the given name
        /// </summary>
        /// <param name="name">Name of Active Event to raise</param>
        /// <param name="args">Arguments to pass into Event Handlers</param>
        /// <param name="context">Application Context</param>
        public Node Raise (
            string name, 
            Node args, 
            ApplicationContext context, 
            ApplicationContext.ContextTicket ticket)
        {
            try
            {
                // Constructing EventArgs
                ActiveEventArgs e = new ActiveEventArgs (name, args ?? new Node());

                // Checking if we have any Active Event handlers for given name
                if (_events.ContainsKey (name)) {

                    // Looping through all Active Events handlers for the given Active Event name
                    foreach (var idxMethod in _events [name].Methods.ToList ()) {

                        // Invoking Event Handler
                        idxMethod.Method.Invoke (idxMethod.Instance, new object[] { context, e });
                    }
                }

                // Then looping through all "null Active Event handlers" afterwards
                if (_events.ContainsKey ("")) {

                    // Active Event was not protected, and we have a "null event handler"
                    foreach (var idxMethod in _events [""].Methods) {
                        idxMethod.Method.Invoke (idxMethod.Instance, new object[] {context, e});
                    }
                }

                // Returning args to caller
                return e.Args;
            }
            catch (TargetInvocationException err)
            {
                // Making sure we transform reflection exceptions into actual exceptions thrown
                ExceptionDispatchInfo.Capture(err.InnerException).Throw();
                throw; // Never reached, needed for compiler to not choke ...!!
            }
        }

        /// <summary>
        ///     Returns all registered Active Events
        /// </summary>
        /// <returns>All Active Events registered in the system</returns>
        public IEnumerable<ActiveEvent> GetEvents ()
        {
            foreach (var idx in _events.Values) {
                yield return idx;
            }
        }
    }
}
