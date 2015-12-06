/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace p5.core
{
    /// <summary>
    ///     Loads up assemblies for handling Active Events.
    /// </summary>
    public class Loader
    {
        /// <summary>
        ///     Wrapper for all type in AppDomain that contains Active Event handlers
        /// </summary>
        internal class ActiveEventTypes
        {
            /// <summary>
            ///     Wrapper for a single type in AppDomain that contains Active Event handlers
            /// </summary>
            internal class ActiveEventType
            {
                /// <summary>
                ///     One single Active Event
                /// 
                ///     Notice, there might exist several Active Events for a single method
                /// </summary>
                internal class ActiveEvent
                {
                    /// <summary>
                    ///     Initializes a new instance of the
                    /// <see cref="p5.core.Loader+ActiveEventTypes+ActiveEventType+ActiveEvent"/> class
                    /// </summary>
                    /// <param name="atr">Atr.</param>
                    /// <param name="method">Method.</param>
                    public ActiveEvent(ActiveEventAttribute atr, MethodInfo method)
                    {
                        Attribute = atr;
                        Method = method;
                    }

                    /// <summary>
                    ///     Gets the ActiveEventAttribute for given Active Event
                    /// </summary>
                    /// <value>The attribute.</value>
                    public ActiveEventAttribute Attribute {
                        get;
                        private set;
                    }

                    /// <summary>
                    ///     Returns the Method for Active Event
                    /// </summary>
                    /// <value>The method.</value>
                    public MethodInfo Method {
                        get;
                        private set;
                    }
                }

                /// <summary>
                ///     Initializes a new instance of the <see cref="p5.core.Loader+ActiveEventTypes+ActiveEventType"/> class.
                /// </summary>
                public ActiveEventType()
                {
                    Events = new List<ActiveEvent> ();
                }

                /// <summary>
                ///     Gets the list of Active Events for given Type
                /// </summary>
                /// <value>The events.</value>
                public List<ActiveEvent> Events {
                    get;
                    private set;
                }

                /// <summary>
                ///     Adds an Active Event for given type
                /// </summary>
                /// <param name="atr">Atr.</param>
                /// <param name="method">Method.</param>
                public void AddActiveEvent (ActiveEventAttribute atr, MethodInfo method)
                {
                    Events.Add(new ActiveEvent(atr, method));
                }
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="p5.core.Loader+ActiveEventTypes"/> class.
            /// </summary>
            public ActiveEventTypes()
            {
                Types = new Dictionary<Type, ActiveEventType>();
            }

            /// <summary>
            ///     Returns all Types in AppDomain that has Active Event handlers
            /// </summary>
            /// <value>The types.</value>
            public Dictionary<Type, ActiveEventType> Types {
                get;
                private set;
            }

            /// <summary>
            ///     Adds an Active Event for given type
            /// </summary>
            /// <param name="type">Type with Active Event handlers</param>
            /// <param name="atr">Attribute describing Active Event handler</param>
            /// <param name="method">Method that implements handler</param>
            public void AddActiveEvent (Type type, ActiveEventAttribute atr, MethodInfo method)
            {
                // Making sure there exists an entry for type
                if (!Types.ContainsKey(type))
                    Types[type] = new ActiveEventType();

                // Adding Active Event and its associated method
                Types[type].AddActiveEvent(atr, method);
            }

            /// <summary>
            ///     Removes a type entirely from being able to handle Active Events
            /// </summary>
            /// <param name="type">Type.</param>
            public void RemoveType (Type type)
            {
                if (Types.ContainsKey(type))
                    Types.Remove(type);
            }
        }

        /// <summary>
        ///     Returns the singleton instance
        /// </summary>
        /// <value>The singleton instance</value>
        public static readonly Loader Instance = new Loader ();

        private readonly List<Assembly> _assemblies = new List<Assembly> ();
        private readonly ActiveEventTypes _instanceActiveEvents = new ActiveEventTypes();
        private readonly ActiveEventTypes _staticActiveEvents = new ActiveEventTypes();

        /// <summary>
        ///     Creates a new ApplicationContext for you
        /// </summary>
        /// <returns>The newly created context</returns>
        public ApplicationContext CreateApplicationContext (
            ApplicationContext.ContextTicket ticket = null)
        {
            var context = new ApplicationContext (_instanceActiveEvents, _staticActiveEvents, ticket);
            return context;
        }

        /// <summary>
        ///     Loads an assembly for handling Active Events
        /// </summary>
        /// <param name="assembly">Assembly to register as Active event handler.</param>
        public void LoadAssembly (Assembly assembly)
        {
            // Checking to see if assembly is already loaded up, to avoid initializing the same assembly twice
            if (_assemblies.Exists (idx => idx == assembly))
                return;

            // Looking for assembly in our current AppDomain, for then to initialize it
            foreach (var idxAsm in AppDomain.CurrentDomain.GetAssemblies ()) {

                // Checking if this is the requested assembly
                if (idxAsm == assembly) {

                    // This is our assembly
                    InitializeAssembly (idxAsm);
                    _assemblies.Add (idxAsm);
                }
            }
        }

        /// <summary>
        ///     Loads and registers the assembly, containing the given type, for handling Active Events
        /// </summary>
        /// <param name="type">Type declared in assembly you wish to load</param>
        public void LoadAssembly (Type type)
        {
            LoadAssembly (type.Assembly);
        }

        /// <summary>
        ///     Loads a named assembly for handling Active Events
        /// </summary>
        /// <param name="name">The name of the assembly you wish to load</param>
        public void LoadAssembly (string name)
        {
            LoadAssembly (string.Empty, name);
        }

        /// <summary>
        ///     Loads an assembly for handling Active Events
        /// </summary>
        /// <param name="path">Folder where assembly exists</param>
        /// <param name="name">Name of your assembly</param>
        public void LoadAssembly (string path, string name)
        {
            // "Normalizing" name of assembly
            name = name.ToLower();
            if (!name.EndsWith (".dll"))
                name += ".dll";

            // Checking to see if assembly is already loaded
            if (_assemblies.Exists (idx => idx.ManifestModule.Name.ToLower () == name))
                return;

            // Checking our current AppDomain to see if assembly is already a part of our AppDomain
            foreach (var idxAsm in AppDomain.CurrentDomain.GetAssemblies ()) {

                // Checking name of currently iterated assembly, to see if it's the assembly we wish to load
                if (idxAsm.ManifestModule.Name.ToLower () == name) {

                    // Initializing assembly for handling Active Events that is already loaded in AppDomain
                    InitializeAssembly (idxAsm);
                    _assemblies.Add (idxAsm);
                    return;
                }
            }

            // We must dynamically load assembly and initialize it, since it is not registered in our current AppDomain
            var assembly = Assembly.LoadFile (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, path + name));
            InitializeAssembly (assembly);
            _assemblies.Add (assembly);
        }

        /// <summary>
        ///     Unloads the assembly with the given name
        /// </summary>
        /// <param name="name">Name of assembly to unload</param>
        public void UnloadAssembly (string name)
        {
            // "Normalizing" assembly name
            name = name.ToLower();
            if (!name.EndsWith (".dll"))
                name += ".dll";

            // Finding the assembly in our list of initialized assemblies
            var assembly = _assemblies.Find (idx => idx.ManifestModule.Name.ToLower () == name);
            if (assembly != null) {

                // Removing assembly, and making sure all Active Events are "unregistered"
                // please notice that assembly is still in AppDomain, but will no longer handle Active Events
                _assemblies.Remove (assembly);
                RemoveAssembly (assembly);
            }
        }

        /*
         * removes an assembly such that all Active Events from given assembly will no longer
         * be a part of our list of potential invocation objects for Active Events
         */
        private void RemoveAssembly (Assembly assembly)
        {
            // Looping through all types from assembly, to see if they're handling Active Events
            foreach (var idxType in assembly.GetTypes ()) {

                // Removing type from instance Active Event list
                _instanceActiveEvents.RemoveType (idxType);

                // Removing type from static Active Event list
                _staticActiveEvents.RemoveType (idxType);
            }
        }

        /*
         * initializes an assembly by looping through all types from it, and see if type has
         * Active Event attributes for one or more of its methods, and if it does, we register
         * type as Active Event sink
         */
        private void InitializeAssembly (Assembly assembly)
        {
            // Looping through all types in assembly
            foreach (var idxType in assembly.GetTypes ()) {

                // Adding instance Active Events
                var instanceMethods = idxType.GetMethods (
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Instance |
                    BindingFlags.NonPublic |
                    BindingFlags.Public);
                AddActiveEventsForType (idxType, instanceMethods, _instanceActiveEvents);

                // Adding static Active Events
                var staticMethods = idxType.GetMethods (
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Static |
                    BindingFlags.NonPublic |
                    BindingFlags.Public);
                AddActiveEventsForType (idxType, staticMethods, _staticActiveEvents);
            }
        }

        /*
         * loops through all MethodInfo objects given, and adds them to the associated dictionary with type as key,
         * if they have Active Event attributes declared
         */
        private void AddActiveEventsForType (
            Type type,
            MethodInfo[] methods,
            ActiveEventTypes activeEventTypes)
        {
            // Looping through all MethodInfo from type we currently are iterating
            foreach (var idxMethod in methods) {

                // Checking to see if current MethodInfo has our Active Event attribute, and if it does, we check if it has
                // the right signature, before we add it to our list of Active Event sinks
                var atrs = idxMethod.GetCustomAttributes (typeof (ActiveEventAttribute), true) as ActiveEventAttribute[];
                if (atrs != null && atrs.Length > 0) {

                    // Checking if Active Event has a valid signature
                    VerifyActiveEventSignature (idxMethod);

                    // Looping through each Active Event attribute for method
                    foreach (var idxAtr in atrs) {

                        // Adding currently iterated Active Event attribute
                        activeEventTypes.AddActiveEvent (type, idxAtr, idxMethod);
                    }
                }
            }
        }

        /*
         * verifies that the signature of our Active Event is correct
         */
        private static void VerifyActiveEventSignature (MethodInfo method)
        {
            var pars = method.GetParameters ();

            // An Active Event must take two arguments, the first argument must be of type "ApplicationContext", and the
            // second argument must be of type "ActiveEventArgs"
            if (pars.Length != 2 ||
                pars [0].ParameterType != typeof (ApplicationContext) ||
                pars [1].ParameterType != typeof (ActiveEventArgs))

                // Oops, signature didn't match
                throw new ArgumentException (
                    string.Format ("method '{0}.{1}' is not a valid active event, parameters of method is wrong. all Active Events must take an ApplicationContext and an ActiveEventArgs object",
                        method.DeclaringType.FullName,
                        method.Name));
        }
    }
}
