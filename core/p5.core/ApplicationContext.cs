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
        private readonly Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> _typesInstanceActiveEvents;

        internal ApplicationContext (
            Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> instanceEvents,
            Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> staticEvents)
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
            var type = instance.GetType ();
            while (!type.FullName.StartsWith ("System.")) {

                // Checking to see if this type is registered in our list of types that has Active Events
                if (_typesInstanceActiveEvents.ContainsKey (type)) {

                    // Retrieving the list of ActiveEvent/MethodInfo  for the currently iterated Type
                    foreach (var idxTupleAvMethodInfo in _typesInstanceActiveEvents [type]) {

                        // Adding Active Event
                        _registeredActiveEvents.AddMethod (
                            idxTupleAvMethodInfo.Item1.Name, 
                            idxTupleAvMethodInfo.Item2, 
                            instance, 
                            idxTupleAvMethodInfo.Item1.Protected);
                    }
                }

                // Continue iteration over Types until we reach a type we know does not have Active Events
                type = type.BaseType;
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
                if (_typesInstanceActiveEvents.ContainsKey (type)) {

                    // retrieving the list of ActiveEvent/MethodInfo for the currently iterated type
                    foreach (var idxTupleAvMethodInfo in _typesInstanceActiveEvents [type]) {

                        _registeredActiveEvents.RemoveMethod(idxTupleAvMethodInfo.Item1.Name, instance);
                    }
                }

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
        private void InitializeApplicationContext (Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> staticEvents)
        {
            // Looping through each Type in static Active Events given
            foreach (var idxType in staticEvents.Keys) {

                // Looping through each ActiveEvent/MethodInfo tuple in Type
                foreach (var idxTupleAvMethodInfo in staticEvents [idxType]) {

                    // Registering Active Event
                    _registeredActiveEvents.AddMethod(idxTupleAvMethodInfo.Item1.Name, idxTupleAvMethodInfo.Item2, null, idxTupleAvMethodInfo.Item1.Protected);
                }
            }

            // Raising "initialize" Active Event
            Raise ("p5.core.initialize-application-context");
        }
    }
}
