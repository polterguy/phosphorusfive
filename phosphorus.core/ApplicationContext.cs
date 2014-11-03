/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Reflection;
using System.Collections.Generic;

namespace phosphorus.core
{
    /// <summary>
    /// the application context allows your to register instance event listeners and raise Active Events. there should be
    /// one application context for every user in your system
    /// </summary>
    public class ApplicationContext
    {
        private Dictionary<string, List<Tuple<MethodInfo, object>>> _registeredActiveEvents;
        private Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> _typesInstanceActiveEvents;

        internal ApplicationContext (
            Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> instanceEvents,
            Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> staticEvents)
        {
            _registeredActiveEvents = new Dictionary<string, List<Tuple<MethodInfo, object>>> ();
            _typesInstanceActiveEvents = instanceEvents;

            // looping through, creating active event mappings for all static active events
            foreach (var idxType in staticEvents.Keys) {
                foreach (var idxTuple in staticEvents [idxType]) {
                    if (!_registeredActiveEvents.ContainsKey (idxTuple.Item1.Name))
                        _registeredActiveEvents [idxTuple.Item1.Name] = new List<Tuple<MethodInfo, object>> ();
                    var methods = _registeredActiveEvents [idxTuple.Item1.Name];
                    methods.Add (new Tuple<MethodInfo, object> (idxTuple.Item2, null));
                }
            }
        }

        /// <summary>
        /// registers an instance Active Event listening object
        /// </summary>
        /// <param name="instance">object to register for Active Event handling</param>
        public void RegisterListeningObject (object instance)
        {
            if (instance == null)
                throw new ArgumentNullException ("instance");

            Type type = instance.GetType ();
            while (type != typeof (object)) {
                if (_typesInstanceActiveEvents.ContainsKey (type)) {
                    var list = _typesInstanceActiveEvents [type];
                    foreach (var idxTuple in list) {
                        if (!_registeredActiveEvents.ContainsKey (idxTuple.Item1.Name))
                            _registeredActiveEvents [idxTuple.Item1.Name] = new List<Tuple<MethodInfo, object>> ();
                        _registeredActiveEvents [idxTuple.Item1.Name].Add (new Tuple<MethodInfo, object> (idxTuple.Item2, instance));
                    }
                }
                type = type.BaseType;
            }
        }

        /// <summary>
        /// unregisters an instance listening object
        /// </summary>
        /// <param name="instance">object to unregister</param>
        public void UnregisterListeningObject (object instance)
        {
            if (instance == null)
                throw new ArgumentNullException ();

            Type type = instance.GetType ();
            while (type != typeof(object)) {
                if (_typesInstanceActiveEvents.ContainsKey (type)) {
                    var list = _typesInstanceActiveEvents [type];
                    foreach (var idxTuple in list) {
                        if (!_registeredActiveEvents.ContainsKey (idxTuple.Item1.Name))
                            continue;
                        var methods = _registeredActiveEvents [idxTuple.Item1.Name];
                        methods.RemoveAll (
                            delegate(Tuple<MethodInfo, object> idxTupleToRemove) {
                            return idxTupleToRemove.Item2 != null && idxTupleToRemove.Item2 == instance;
                        });
                        if (methods.Count == 0)
                            _registeredActiveEvents.Remove (idxTuple.Item1.Name);
                    }
                }
                type = type.BaseType;
            }
        }

        /// <summary>
        /// raise the specified Active Event with the given arguments
        /// </summary>
        /// <param name="name">name of Active Event to raise</param>
        /// <param name="args">arguments to pass into the Active Event</param>
        public void Raise (string name, Node args)
        {
            if (!_registeredActiveEvents.ContainsKey (name))
                return;

            ActiveEventArgs e = new ActiveEventArgs (args);
            foreach (var idxMethod in _registeredActiveEvents [name]) {
                idxMethod.Item1.Invoke (idxMethod.Item2, new object[] { this, e });
            }
        }
    }
}

