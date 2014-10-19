/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Reflection;
using System.Collections.Generic;

namespace phosphorus.core
{
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
                    if (!_registeredActiveEvents.ContainsKey (idxTuple.Item1.Name)) {
                        _registeredActiveEvents [idxTuple.Item1.Name] = new List<Tuple<MethodInfo, object>> ();                    }
                    var methods = _registeredActiveEvents [idxTuple.Item1.Name];
                    methods.Add (new Tuple<MethodInfo, object> (idxTuple.Item2, null));
                }
            }
        }

        public void RegisterListeningObject (object instance)
        {
            if (instance == null)
                throw new ArgumentNullException ();

            bool found = false;

            Type type = instance.GetType ();
            while (type != typeof(object)) {
                if (_typesInstanceActiveEvents.ContainsKey (type)) {
                    found = true;
                    var list = _typesInstanceActiveEvents [type];
                    foreach (var idxTuple in list) {
                        if (!_registeredActiveEvents.ContainsKey (idxTuple.Item1.Name)) {
                            _registeredActiveEvents [idxTuple.Item1.Name] = new List<Tuple<MethodInfo, object>> ();
                        }
                        var methods = _registeredActiveEvents [idxTuple.Item1.Name];
                        bool exist = false;
                        foreach (var idxExisting in methods) {
                            if (instance.Equals (idxExisting.Item2) && idxExisting.Item1 == idxTuple.Item2) {
                                exist = true;
                                break;
                            }
                        }
                        if (!exist)
                            methods.Add (new Tuple<MethodInfo, object> (idxTuple.Item2, instance));
                    }
                }
                type = type.BaseType;
            }

            if (!found)
                throw new ArgumentNullException (string.Format ("object of type '{0}' did not contain any instance active event listeners", 
                                                                instance.GetType ().FullName));
        }

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

        public void Raise (string name, object context, Node args)
        {
            if (!_registeredActiveEvents.ContainsKey (name))
                return;

            ActiveEventArgs e = new ActiveEventArgs (args);
            foreach (var idxMethod in _registeredActiveEvents [name]) {
                idxMethod.Item1.Invoke (idxMethod.Item2, new object[] { context, e });
            }
        }
    }
}

