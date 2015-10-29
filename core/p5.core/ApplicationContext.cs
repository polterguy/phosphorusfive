/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
///     Main namespace for Phosphorus core functionality.
/// 
///     This namespace contains all the core mapping functionality of Phosphorus Five, such as the Active Event implementation,
///     the Node structure, in addition to some helper and utilities classes.
/// </summary>
namespace p5.core
{
    /// <summary>
    ///     Application context, which your Active Events are raised through.
    /// 
    ///     The application context allows your to register instance event listeners and raise Active Events. For an executable
    ///     program, you'd probably only have one application context, while for a website project, and similar types of
    ///     projects, you'd probably create an application context for each thread that access your website, by creating your application
    ///     context in the beginning of your page life cycle.
    /// 
    ///     To create your application context, use the <see cref="phosphorus.core.Loader.CreateApplicationContext" /> method. Make sure
    ///     all Active Event assemblies are loaded before you create your application context.
    /// </summary>
    public class ApplicationContext
    {
        private readonly Dictionary<string, List<Tuple<MethodInfo, object>>> _registeredActiveEvents;
        private readonly Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> _typesInstanceActiveEvents;

        internal ApplicationContext (
            Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> instanceEvents,
            Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> staticEvents)
        {
            _registeredActiveEvents = new Dictionary<string, List<Tuple<MethodInfo, object>>> ();
            _typesInstanceActiveEvents = instanceEvents;
            InitializeApplicationContext (staticEvents);
        }

        /// <summary>
        ///     Returns all Active Events registered within the current ApplicationContext object.
        /// 
        ///     Each ApplicationContext has its unique Active Events, depending upon whether or not the current context
        ///     has overridden or created new Active Events.
        /// 
        ///     This property returns all Active Events for the current context.
        /// </summary>
        /// <value>The active events.</value>
        public IEnumerable<string> ActiveEvents
        {
            get { return _registeredActiveEvents.Select (idx => idx.Key); }
        }

        /// <summary>
        ///     Registers an instance Active Event listening object.
        /// 
        ///     If you have instance methods that are Active Event handlers, then you must register your instances in your
        ///     context through this method. This allows you to create Active Event handlers, that are instance methods, in
        ///     your app context.
        /// </summary>
        /// <param name="instance">object to register for Active Event handling</param>
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

        /// <summary>
        ///     Unregisters an instance listening object.
        /// 
        ///     If you no longer want your object to handle Active Events, you must call this method to "unregister" your object
        ///     as an instance listener. Typically, you should let your class implement IDisposable, and unregister your object
        ///     in its Dispose implementation, when you have instance listeners.
        /// </summary>
        /// <param name="instance">object to unregister</param>
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

        /// <summary>
        ///     Raises one Active Event.
        /// 
        ///     Raises the specified Active Event, with the given parameters. Will traverse the "inheritance chain", or
        ///     the overridden Active Events, to check if the Active Event your code wish to raise, is overridden with
        ///     another Active Event.
        /// </summary>
        /// <param name="name">name of Active Event to raise</param>
        /// <param name="pars">arguments to pass into the Active Event</param>
        public Node Raise (string name, Node pars = null)
        {
            if (pars == null)
                pars = new Node ();
            var e = new ActiveEventArgs (name, pars);
            RaiseImplementation (e);
            return e.Args;
        }

        /*
         * initializes app context
         */
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
            Raise ("p5.core.initialize-application-context");
        }

        /*
         * responsible for recursively figuring out the most derived Active Event to actually invoke
         */
        private void RaiseImplementation (ActiveEventArgs e)
        {
            // checking if we have any handlers
            if (_registeredActiveEvents.ContainsKey (e.Name)) {
                // looping through all Active Events handlers for the given Active Event name, and invoking them
                foreach (var idxMethod in _registeredActiveEvents [e.Name]) {
                    idxMethod.Item1.Invoke (idxMethod.Item2, new object[] { this, e });
                }
            }

            // then looping through all "null Active Event handlers" afterwards
            // ORDER COUNTS. Since most native Active Events are dependent upon arguments
            // being specifically ordered somehow, we must wait until after we have raised
            // all "native Active Events", before we raise all "null Active Event handlers".
            // this is because "null event handlers" might possibly append nodes to the current
            // Active Event's "root node", and hence mess up the parameter passing of native Active
            // Events, that also have "null event handlers", where these null event handlers,
            // are handling events, existing also as "native Active Event handlers"
            if (_registeredActiveEvents.ContainsKey (string.Empty)) {
                foreach (var idxMethod in _registeredActiveEvents [string.Empty]) {
                    idxMethod.Item1.Invoke (idxMethod.Item2, new object[] {this, e});
                }
            }
        }
    }
}
