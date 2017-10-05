/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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
using System.Security;
using System.Collections.Generic;
using p5.core.internals;

namespace p5.core
{
    /// <summary>
    ///     Application context, which your Active Events are raised through.
    /// </summary>
    public class ApplicationContext
    {
        // Wraps all registered Active Events for the given context.
        readonly ActiveEventMethods _registeredActiveEvents = new ActiveEventMethods ();

        // Wraps all types that have instance events, not necessarily registered as events, but that might be registered later, if an
        // instance event handler is registered as a listener object.
        readonly ActiveEventTypes _instanceHandlerTypes;

        // The current "ticket", authorization/authentication object for the context.
        ContextTicket _ticket;

        /*
         * Creates a new application context.
         * This must be done through the Loader class, hence the constructor is internal.
         */
        internal ApplicationContext (ActiveEventTypes instanceHandlerTypes, ActiveEventTypes staticHandlerTypes, ContextTicket ticket)
        {
            _ticket = ticket;

            // Each type that can handle instance Active Events neds to be stored, such that we can later register instance listeners, 
            // according to their type.
            _instanceHandlerTypes = instanceHandlerTypes;

            // However, the static event handlers, are processed when we create our application context, and not stored for later.
            // Which means, you can register instance listeners on the ApplicationContext object, but if you register new assemblies through
            // your Loader singletone instance, these will not affect existing ApplicationContext instances.
            // While registering listener instances for a single ApplicationContext instance, will only affect that single context, and not
            // any other contexts.
            InitializeApplicationContext (staticHandlerTypes);
        }

        /// <summary>
        ///     Returns the context ticket for this instance.
        /// </summary>
        /// <value>The context ticket</value>
        public ContextTicket Ticket {
            get { return _ticket; }
        }

        /// <summary>
        ///     Returns all Active Events registered within the current ApplicationContext object.
        /// </summary>
        /// <value>The active events</value>
        public IEnumerable<string> ActiveEvents {
            get { return _registeredActiveEvents.ActiveEventNames; }
        }

        /// <summary>
        ///     Returns true if the specified Active Event is registered.
        /// </summary>
        /// <value>True if Active Event is registered in context</value>
        public bool HasActiveEvent (string name)
        {
            return _registeredActiveEvents.HasActiveEvent (name);
        }

        /// <summary>
        ///     Changes the ticket for the context.
        /// </summary>
        /// <param name="ticket">New ticket</param>
        public void UpdateTicket (ContextTicket ticket)
        {
            _ticket = ticket;
        }

        /// <summary>
        ///     Registers an instance Active Event listening object.
        /// </summary>
        /// <param name="instance">Object to register as an instance Active Event handling.</param>
        public void RegisterListeningInstance (object instance)
        {
            // Sanity check.
            if (instance == null)
                throw new ArgumentNullException (nameof (instance));

            // Recursively iterating the Type of the type of the object given, in its inheritance chain, 
            // until we reach a type where we know for a fact, there won't exist any handlers.
            for (var idxType = instance.GetType (); !idxType.FullName.StartsWith ("System.", StringComparison.InvariantCulture); idxType = idxType.BaseType) {

                // Checking to see if this type is registered in our list of types that contains Active Events.
                if (_instanceHandlerTypes.ContainsKey (idxType)) {

                    // Retrieving the list of ActiveEvent/MethodInfo for the currently iterated Type.
                    foreach (var idxActiveEvent in _instanceHandlerTypes [idxType]) {

                        // Registering Active Event, with specified instance.
                        _registeredActiveEvents.AddEventMethod (idxActiveEvent.Item1, idxActiveEvent.Item2, instance);
                    }
                }
            }
        }

        /// <summary>
        ///     Unregisters an instance listening object.
        /// </summary>
        /// <param name="instance">object to unregister</param>
        public void UnregisterListeningInstance (object instance)
        {
            // Sanity check.
            if (instance == null)
                throw new ArgumentNullException (nameof (instance));

            // Deleting all associated Active Events for specified instance.
            _registeredActiveEvents.DeleteEventsForInstance (instance);
        }

        /// <summary>
        ///     Raises the specified Active Event, with the given arguments, if any.
        /// </summary>
        /// <param name="activeEventName">Name of Active Event to raise</param>
        /// <param name="arguments">Arguments to pass into the Active Event</param>
        public Node RaiseEvent (string activeEventName, Node arguments = null)
        {
            // Checking if we have a whitelist definition on ticket or not, to determine how to raise Active Event, if at all.
            // Notice, to not mess up internal Active Events, necessary to for instance raise pre-condition active events, and other similar
            // internal system events, we do not consider whitelist definition, if Active Event starts with a "_" or an ".", since these
            // events are anyways impossible to raise from lambda, and only C# code is able to raise them.
            if (Ticket != null && Ticket.Whitelist != null && !activeEventName.StartsWith (".", StringComparison.InvariantCulture) && !activeEventName.StartsWith ("_", StringComparison.InvariantCulture)) {

                // Considering our whitelist before we raise event.
                return RaiseWithWhitelist (activeEventName, arguments);

            }

            // No whitelist definition.
            return _registeredActiveEvents.RaiseEvent (this, arguments, activeEventName);
        }

        /*
         * Raises a single Active Event, making sure it exists in our whitelist, before we allow it to be raised.
         */
        Node RaiseWithWhitelist (string activeEventName, Node arguments)
        {
            // Retrieving definition, and throwing an exception, unless Active Event is explicitly legalized in whitelist.
            var definition = Ticket.Whitelist [activeEventName];
            if (definition == null)
                throw new SecurityException (string.Format ("Caller tried to invoke illegal Active Event [{0}] according to whitelist definition", activeEventName));

            // Looping through all [pre-condition]s for Active Event, and raising them as Active Events, to verify activeEventName can be legally raised by caller.
            foreach (var idxCondition in definition.Children.Where (ix => ix.Name == "pre-condition")) {

                // Raising [pre-condition] Active Event, which will throw an exception, if condition is not met.
                var conditionArgs = new Node ("", null, new Node ("pre-condition", idxCondition), new Node ("lambda", arguments));
                RaiseEvent (".p5.lambda.whitelist.pre-condition." + idxCondition.Get<string> (this), conditionArgs);
            }

            // OK, so far, so good, now we can raise the actual Active Event caller attempts to raise.
            var retVal = _registeredActiveEvents.RaiseEvent (this, arguments, activeEventName);

            // Looping through all [post-conditions] for Active Event.
            foreach (var idxCondition in definition.Children.Where (ix => ix.Name == "post-condition")) {

                // Raising [post-condition] Active Event, which will throw if condition is not met.
                var conditionArgs = new Node ("", null, new Node ("post-condition", idxCondition), new Node ("lambda", retVal));
                RaiseEvent (".p5.lambda.whitelist.post-condition." + idxCondition.Get<string> (this), conditionArgs);
            }

            // Success, returning results to caller.
            return retVal;
        }

        /*
         * Initializes our ApplicationContext instance.
         */
        void InitializeApplicationContext (ActiveEventTypes staticEventTypes)
        {
            // Looping through each Type in Active Events given.
            foreach (var idxType in staticEventTypes.Keys) {

                // Looping through each ActiveEvent in Type.
                foreach (var idxAVTypeEvent in staticEventTypes [idxType]) {

                    // Registering Active Event as a static Active Event handler.
                    _registeredActiveEvents.AddEventMethod (idxAVTypeEvent.Item1, idxAVTypeEvent.Item2);
                }
            }

            // Raising "initialize" Application Context Active Event, in case there are any listeners being interested in such things.
            RaiseEvent (".p5.core.initialize-application-context");
        }
    }
}
