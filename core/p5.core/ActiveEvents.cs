/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

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
            public ActiveEvent(string name, bool isProtected)
            {
                Name = name;
                Protected = isProtected;
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
            ///     Gets a value indicating whether this <see cref="p5.core.ActiveEvents+ActiveEvent"/> is protected or not
            /// </summary>
            /// <value><c>true</c> if protected; otherwise, <c>false</c>.</value>
            public bool Protected {
                get;
                private set;
            }

            /// <summary>
            ///     Returns the list of methods tied to this specific Active Event
            /// </summary>
            /// <value>The methods.</value>
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
        /// <param name="isProtected">If set to <c>true</c> then Active Event is protected and cannot be overwritten once created</param>
        public void AddMethod (string name, MethodInfo method, object instance, bool isProtected)
        {
            // Verifying we have an entry for event name
            if (!_events.ContainsKey(name)) {

                // Creating event name entry
                _events[name] = new ActiveEvent(name, isProtected);
            } else if (_events[name].Protected) {

                // Oops, event entry existed, and it was protected
                throw new ApplicationException(string.Format("You cannot add to the Active Event '{0}' since it is protected", name));
            } else if (isProtected) {

                // Oops, event entry did not exist, but caller tried to add a protected method, where one which was not protected existed from before
                throw new ApplicationException(string.Format("You cannot add a protected method to the Active Event '{0}' since there already exist one which is not protected", name));
            }

            // Now that we have for sure created an Active Event entry, we can add the actual MethodInfo/Instance-object
            _events[name].Methods.Add(new ActiveEvent.MethodSink (method, instance));
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
        public Node Raise (string name, Node args, ApplicationContext context, ApplicationContext.ContextTicket ticket)
        {
            try
            {
                // Used as buffer to store whether or not Active Event was protected or not
                // This is done since we DO NOT invoke "null handlers" for protected events!
                bool wasProtected = false;

                ActiveEventArgs e = new ActiveEventArgs(name, args ?? new Node(), ticket);

                // Checking if we have any Active Event handlers for given name
                if (_events.ContainsKey (name)) {

                    // Looping through all Active Events handlers for the given Active Event name
                    foreach (var idxMethod in _events [name].Methods) {

                        // Invoking Event Handler
                        idxMethod.Method.Invoke (idxMethod.Instance, new object[] { context, e });
                    }

                    // Storing whether or not event was protected
                    wasProtected = _events[name].Protected;
                }

                // Then looping through all "null Active Event handlers" afterwards
                // ORDER COUNTS. Since most native Active Events are dependent upon arguments
                // being specifically ordered somehow, we must wait until after we have raised
                // all "native Active Events", before we raise all "null Active Event handlers".
                // this is because "null event handlers" might possibly append nodes to the current
                // Active Event's "root node", and hence mess up the parameter passing of native Active
                // Events, that also have "null event handlers", where these null event handlers,
                // are handling events, existing also as "native Active Event handlers"
                // Please also notice that we do NOT raise "null handlers" for "protected" Active Events
                if (!wasProtected && _events.ContainsKey (string.Empty)) {

                    // Active Event was not protected, and we have a "null event handler"
                    foreach (var idxMethod in _events [string.Empty].Methods) {
                        idxMethod.Method.Invoke (idxMethod.Instance, new object[] {context, e});
                    }
                }

                // Returning args to caller
                return e.Args;
            }
            catch (System.Reflection.TargetInvocationException err)
            {
                // Making sure we transform reflection exceptions into actual exceptions thrown
                throw err.InnerException;
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
