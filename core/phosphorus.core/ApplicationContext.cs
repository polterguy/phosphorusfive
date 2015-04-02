/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace phosphorus.core
{
    public class ApplicationContext
    {
        private readonly Dictionary<string, List<string>> _overrides;
        private readonly Dictionary<string, List<Tuple<MethodInfo, object>>> _registeredActiveEvents;
        private readonly Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> _typesInstanceActiveEvents;

        internal ApplicationContext (
            Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> instanceEvents,
            Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> staticEvents)
        {
            _registeredActiveEvents = new Dictionary<string, List<Tuple<MethodInfo, object>>> ();
            _typesInstanceActiveEvents = instanceEvents;
            _overrides = new Dictionary<string, List<string>> ();
            InitializeApplicationContext (staticEvents);
        }

        public IEnumerable<string> ActiveEvents
        {
            get { return _registeredActiveEvents.Select (idx => idx.Key); }
        }

        public IEnumerable<Tuple<string, List<string>>> Overrides
        {
            get { return _overrides.Select (idx => new Tuple<string, List<string>> (idx.Key, idx.Value)); }
        }

        public void RegisterListeningObject (object instance)
        {
            if (instance == null)
                throw new ArgumentNullException ("instance");

            // recursively iterating the Type of the object given, until we reach an object where we know there won't exist
            // any Active Events
            var type = instance.GetType ();
            while (!type.FullName.StartsWith ("System.")) {

                // checking to see if this type is registered in our list of types that has Active Events
                if (_typesInstanceActiveEvents.ContainsKey (type)) {

                    // retrieving the list of ActiveEvent/MethodInfo  for the currently iterated Type
                    var listAvMethodInfo = _typesInstanceActiveEvents [type];
                    foreach (var idxTupleAvMethodInfo in listAvMethodInfo) {

                        // checking to see if our registered active events list contains the key for the 
                        // currently iterated Active Event, and if not, we create a list with the given Active Event name
                        if (!_registeredActiveEvents.ContainsKey (idxTupleAvMethodInfo.Item1.Name))
                            _registeredActiveEvents [idxTupleAvMethodInfo.Item1.Name] = new List<Tuple<MethodInfo, object>> ();

                        // adding a MethodInfo/InstanceObject tuple to our list of registered Active Events with 
                        // the Active Event name as Key
                        _registeredActiveEvents [idxTupleAvMethodInfo.Item1.Name].Add (
                            new Tuple<MethodInfo, object> (idxTupleAvMethodInfo.Item2, instance));
                    }
                }

                // continue iteration over Types until we reach a type we know does not have Active Events
                type = type.BaseType;
            }
        }

        public void UnregisterListeningObject (object instance)
        {
            if (instance == null)
                throw new ArgumentNullException ("instance");

            // iterating over the Type until we find a type we know for a fact won't contains Active Events
            var type = instance.GetType ();
            while (!type.FullName.StartsWith ("System.")) {

                // checking to see if our list of instance Active Events contains the currently iterated Type
                if (_typesInstanceActiveEvents.ContainsKey (type)) {

                    // retrieving the list of ActiveEvent/MethodInfo for the currently iterated type
                    var list = _typesInstanceActiveEvents [type];
                    foreach (var idxTupleAvMethodInfo in list) {

                        // checking to see if our registered Active Events contains the Name of the currently iterated Active Event
                        // and if not, we continue iteration. This allows us to "unregister" an object that's not registered
                        if (!_registeredActiveEvents.ContainsKey (idxTupleAvMethodInfo.Item1.Name))
                            continue;

                        // retrieving our list of MethodInfo/ObjectInstance for the currently iterated Active Event
                        var methods = _registeredActiveEvents [idxTupleAvMethodInfo.Item1.Name];

                        // removing all methods from our list of MethodInfo/ObjectInstance that matches the instance object
                        // we were given as input, that the client code wanted to remove or "unregister"
                        methods.RemoveAll (idxTupleToRemove => idxTupleToRemove.Item2 == instance);

                        // checking to see if this was the last Active Event with the given Active Event Name, and if so,
                        // we entirely remove the entire mapping from our list of Active Events to clean up after ourselves
                        if (methods.Count == 0)
                            _registeredActiveEvents.Remove (idxTupleAvMethodInfo.Item1.Name);
                    }
                }

                // finding base type, to continue iteration over its Type until we find a Type we're sure of does not 
                // contain Active Event declarations
                type = type.BaseType;
            }
        }

        public void Override (string baseEvent, string superEvent)
        {
            // checking to see if this is our first override of the given base Active Event, and if so, we create our list of 
            // overridden Active Events for the given key of the base Active Event
            if (!_overrides.ContainsKey (baseEvent))
                _overrides [baseEvent] = new List<string> ();

            // this might produce multiple results for the same override, hence we must check during invocation
            // that we only raise the same override once!
            // However, since we're using the _overrides as a "stack", when removing overrides, this is the correct
            // semantic way to do this
            _overrides [baseEvent].Add (superEvent);
        }

        public void RemoveOverride (string baseEvent, string superEvent)
        {
            if (_overrides.ContainsKey (baseEvent)) {

                // removing first occurency of newActiveEvent, which might not necessarily remove ALL entries with same name
                _overrides [baseEvent].Remove (superEvent);

                // checking to see if this was the last override, and if so, remove the dictionary item all together to clean up
                if (_overrides [baseEvent].Count == 0)
                    _overrides.Remove (baseEvent);
            }
        }

        public Node Raise (string name, Node pars = null)
        {
            if (pars == null)
                pars = new Node ();
            var e = new ActiveEventArgs (name, pars);
            RaiseImplementation (e);
            return e.Args;
        }

        public Node RaiseDirectly (string name, Node pars = null)
        {
            if (pars == null)
                pars = new Node ();
            var e = new ActiveEventArgs (name, pars);
            RaiseDirectly (e);
            return e.Args;
        }

        public Node CallBase (ActiveEventArgs e)
        {
            if (e.Base != null) {
                RaiseDirectly (e.Base);
            }
            return e.Args;
        }

        private void InitializeApplicationContext (Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> staticEvents)
        {
            // looping through each Type in static Active Events given
            foreach (var idxType in staticEvents.Keys) {

                // looping through each ActiveEvent/MethodInfo tuple in Type
                foreach (var idxTupleAvMethodInfo in staticEvents [idxType]) {

                    // checking to see if there exist a string Key for the name of our current iterated Active Event
                    // and if not, we create a string key, with a list of MethodInfo/InstanceObject tuples
                    // before we add the current MethodInfo as a handler of that given Active Event name
                    if (!_registeredActiveEvents.ContainsKey (idxTupleAvMethodInfo.Item1.Name))
                        _registeredActiveEvents [idxTupleAvMethodInfo.Item1.Name] = new List<Tuple<MethodInfo, object>> ();

                    // retrieving the list of MethodInfo/InstanceObject typles for Active Event name,
                    // and adding currently iterated method, with a "null instance object" since it's static
                    var methods = _registeredActiveEvents [idxTupleAvMethodInfo.Item1.Name];
                    methods.Add (new Tuple<MethodInfo, object> (idxTupleAvMethodInfo.Item2, null));
                }
            }

            // raising "initialize" Active Event
            Raise ("pf.core.initialize-application-context");
        }

        private void RaiseImplementation (ActiveEventArgs e)
        {
            // checking to see if the Active Event currently raised, is overridden, and if it is
            // we invoke the override, and not the given Active Event, recursively, coming to the 
            // "outer most derived" Active Event
            if (_overrides.ContainsKey (e.Name)) {

                // raising the overridden Active Events, and not the given Active Event,
                foreach (var idxActiveEventName in _overrides [e.Name]) {

                    // creating a "derived" Active Event invocation, storing the base Active Event in our derived ActiveEventArgs
                    var derived = new ActiveEventArgs (idxActiveEventName, e.Args, e);

                    // recursively calling "self", in case the derived Active Event is overridden too
                    RaiseImplementation (derived);
                }
            } else {

                // raising the Active Event given, since there are no overridden Active Events overriding the current Active Event
                RaiseDirectly (e);
            }
        }

        private void RaiseDirectly (ActiveEventArgs e)
        {
            // checking if we have any handlers
            if (_registeredActiveEvents.ContainsKey (e.Name)) {

                // looping through all Active Events handlers for the given Active Event name, and invoking them
                foreach (var idxMethod in _registeredActiveEvents [e.Name]) {
                    idxMethod.Item1.Invoke (idxMethod.Item2, new object[] { this, e });
                }
            }

            // then looping through all "null Active Event handlers" afterwards
            if (_registeredActiveEvents.ContainsKey (string.Empty)) {
                foreach (var idxMethod in _registeredActiveEvents [string.Empty]) {
                    idxMethod.Item1.Invoke (idxMethod.Item2, new object[] {this, e});
                }
            }
        }
    }
}
