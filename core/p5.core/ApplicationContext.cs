/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

/// <summary>
///     Main namespace for Phosphorus core functionality.
/// </summary>
namespace p5.core
{
    /// <summary>
    ///     Application context, which your Active Events are raised through.
    /// </summary>
    public class ApplicationContext
    {
        private readonly ActiveEvents _registeredActiveEvents = new ActiveEvents();
        private readonly Loader.ActiveEventTypes _typesInstanceActiveEvents;

        internal ApplicationContext (Loader.ActiveEventTypes instanceEvents, Loader.ActiveEventTypes staticEvents)
        {
            _typesInstanceActiveEvents = instanceEvents;
            InitializeApplicationContext (staticEvents);
        }

        /// <summary>
        ///     Returns all Active Events registered within the current ApplicationContext object.
        /// </summary>
        /// <value>The active events.</value>
        public IEnumerable<string> ActiveEvents
        {
            get { return _registeredActiveEvents.GetEvents ().Select (idx => idx.Name); }
        }

        /// <summary>
        ///     Registers an instance Active Event listening object.
        /// </summary>
        /// <param name="instance">object to register for Active Event handling</param>
        public void RegisterListeningObject (object instance)
        {
            if (instance == null)
                throw new ArgumentNullException ("instance");

            // Recursively iterating the Type of the object given, until we reach an object where we know there won't exist
            // any Active Events
            var idxType = instance.GetType ();
            while (!idxType.FullName.StartsWith ("System.")) {

                // Checking to see if this type is registered in our list of types that has Active Events
                if (_typesInstanceActiveEvents.Types.ContainsKey (idxType)) {

                    // Retrieving the list of ActiveEvent/MethodInfo  for the currently iterated Type
                    foreach (var idxActiveEvent in _typesInstanceActiveEvents.Types [idxType].Events) {

                        // Adding Active Event
                        _registeredActiveEvents.AddMethod(
                            idxActiveEvent.Attribute.Name,
                            idxActiveEvent.Method,
                            instance,
                            idxActiveEvent.Attribute.Protected);
                    }
                }

                // Continue iteration over Types until we reach a type we know does not have Active Events
                idxType = idxType.BaseType;
            }
        }

        /// <summary>
        ///     Unregisters an instance listening object.
        /// </summary>
        /// <param name="instance">object to unregister</param>
        public void UnregisterListeningObject (object instance)
        {
            if (instance == null)
                throw new ArgumentNullException ("instance");

            // Iterating over the Type until we find a type we know for a fact won't contains Active Events
            var type = instance.GetType ();
            while (!type.FullName.StartsWith ("System.")) {

                // Checking to see if our list of instance Active Events contains the currently iterated Type
                _registeredActiveEvents.RemoveMethod (instance);

                // finding base type, to continue iteration over its Type until we find a Type we're sure of does not 
                // contain Active Event declarations
                type = type.BaseType;
            }
        }

        /// <summary>
        ///     Raises one Active Event.
        /// </summary>
        /// <param name="name">name of Active Event to raise</param>
        /// <param name="pars">arguments to pass into the Active Event</param>
        public Node Raise (string name, Node pars = null)
        {
            return _registeredActiveEvents.Raise (name, pars, this);
        }

        /*
         * initializes app context
         */
        private void InitializeApplicationContext (Loader.ActiveEventTypes staticEvents)
        {
            // Looping through each Type in static Active Events given
            foreach (var idxType in staticEvents.Types.Keys) {

                // Looping through each ActiveEvent/MethodInfo tuple in Type
                foreach (var idxAVTypeEvent in staticEvents.Types [idxType].Events) {

                    // Registering Active Event
                    _registeredActiveEvents.AddMethod (
                        idxAVTypeEvent.Attribute.Name, 
                        idxAVTypeEvent.Method, 
                        null, 
                        idxAVTypeEvent.Attribute.Protected);
                }
            }

            // Raising "initialize" Active Event
            Raise ("p5.core.initialize-application-context");
        }
    }
}
