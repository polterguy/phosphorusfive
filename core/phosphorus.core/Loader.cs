/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace phosphorus.core
{
    public class Loader
    {
        public static readonly Loader Instance = new Loader ();

        private readonly List<Assembly> _assemblies = new List<Assembly> ();
        private readonly Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> _instanceActiveEvents;
        private readonly Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> _staticActiveEvents;

        private Loader ()
        {
            _instanceActiveEvents = new Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> ();
            _staticActiveEvents = new Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> ();
        }

        public ApplicationContext CreateApplicationContext ()
        {
            var context = new ApplicationContext (_instanceActiveEvents, _staticActiveEvents);
            return context;
        }

        public void LoadAssembly (Assembly assembly)
        {
            // checking to see if assembly is already loaded up, to avoid initializing the same assembly twice
            if (_assemblies.Exists (idx => idx == assembly))
                return;

            // finding the assembly in our current AppDomain, for then to initialize it
            foreach (var idxAsm in AppDomain.CurrentDomain.GetAssemblies ()) {
                if (idxAsm == assembly) {
                    InitializeAssembly (idxAsm);
                    _assemblies.Add (idxAsm);
                }
            }
        }

        public void LoadAssembly (Type type)
        {
            // checking to see if assembly is already loaded up, to avoid initializing the same assembly twice
            var assembly = type.Assembly;
            if (_assemblies.Exists (idx => idx == assembly))
                return;

            // finding the assembly in our current AppDomain, for then to initialize it
            foreach (var idxAsm in AppDomain.CurrentDomain.GetAssemblies ()) {
                if (idxAsm == assembly) {
                    InitializeAssembly (idxAsm);
                    _assemblies.Add (idxAsm);
                }
            }
        }

        public void LoadAssembly (string name)
        {
            LoadAssembly (string.Empty, name);
        }

        public void LoadAssembly (string path, string name)
        {
            // "normalizing" name of assembly
            if (!name.ToLower ().EndsWith (".dll"))
                name += ".dll";

            // checking to see if assembly is already loaded
            if (_assemblies.Exists (idx => string.Equals (idx.ManifestModule.Name, name, StringComparison.Ordinal)))
                return;

            // checking our current AppDomain to see if assembly is already a part of our AppDomain
            foreach (var idxAsm in AppDomain.CurrentDomain.GetAssemblies ()) {
                if (idxAsm.ManifestModule.Name.ToLower () == name.ToLower ()) {
                    InitializeAssembly (idxAsm);
                    _assemblies.Add (idxAsm);
                    return;
                }
            }

            // we must dynamically load assembly and initialize it
            var assembly = Assembly.LoadFile (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, path + name));
            InitializeAssembly (assembly);
            _assemblies.Add (assembly);
        }

        public void UnloadAssembly (string name)
        {
            // "normalizing" assembly name
            if (!name.ToLower ().EndsWith (".dll"))
                name += ".dll";

            // finding the assembly in our list of initialized assemblies
            var assembly = _assemblies.Find (idx => idx.ManifestModule.Name.ToLower () == name);

            if (assembly != null) {

                // removing assembly, and making sure all Active Events are "unregistered"
                // please notice that assembly is still in AppDomain, but will no longer handle Active Events
                _assemblies.Remove (assembly);
                RemoveAssembly (assembly);
            }
        }

        private void RemoveAssembly (Assembly assembly)
        {
            // looping through all types from assembly, to see if they're handling Active Events
            foreach (var idxType in assembly.GetTypes ()) {
                if (_instanceActiveEvents.ContainsKey (idxType))
                    _instanceActiveEvents.Remove (idxType);
                if (_staticActiveEvents.ContainsKey (idxType))
                    _staticActiveEvents.Remove (idxType);
            }
        }

        private void InitializeAssembly (Assembly assembly)
        {
            // looping through all types in assembly
            foreach (var idxType in assembly.GetTypes ()) {

                // adding instance Active Events
                var instanceMethods = idxType.GetMethods (
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Instance |
                    BindingFlags.NonPublic |
                    BindingFlags.Public);
                AddActiveEventsForType (idxType, instanceMethods, _instanceActiveEvents);

                // adding static Active Events
                var staticMethods = idxType.GetMethods (
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Static |
                    BindingFlags.NonPublic |
                    BindingFlags.Public);
                AddActiveEventsForType (idxType, staticMethods, _staticActiveEvents);
            }
        }

        private void AddActiveEventsForType (
            Type type,
            MethodInfo[] methods,
            Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> dictionary)
        {
            // creating a list of Active Events for our type, which we check later if it contains any items, and if it does, we
            // associate it with our type
            var activeEvents = new List<Tuple<ActiveEventAttribute, MethodInfo>> ();

            // looping through all MethodInfo from type we currently are iterating
            foreach (var idxMethod in methods) {

                // checking to see if current MethodInfo has our Active Event attribute, and if it does, we check if it has
                // the right signature before we add it to our list of Active Event sinks
                var atrs = idxMethod.GetCustomAttributes (typeof (ActiveEventAttribute), true) as ActiveEventAttribute[];
                if (atrs != null && atrs.Length > 0) {

                    // checking if Active Event has a valid signature
                    VerifyActiveEventSignature (idxMethod);

                    // verifying validity of name of Active Event
                    foreach (var idxAtr in atrs) {
                        if (idxAtr.Name.StartsWith ("@") || idxAtr.Name.Contains (" "))
                            throw new CoreException (string.Format ("An Active event cannot start with '@' or contain a space. Active Event '{0}' in '{1}' is a violation of rules.", idxAtr.Name, type.FullName));
                    }

                    // adding all Active Event attributes such that they become associate with our MethodInfo, to our list of Active Events
                    activeEvents.AddRange (atrs.Select (idxAtr => new Tuple<ActiveEventAttribute, MethodInfo> (idxAtr, idxMethod)));
                }
            }

            // making sure we only add type as Active Event sinks, if it actually has Active Events declared through ActiveEventAttribute
            if (activeEvents.Count > 0)
                dictionary [type] = activeEvents;
        }

        private static void VerifyActiveEventSignature (MethodInfo method)
        {
            var pars = method.GetParameters ();
            if (pars.Length != 2 ||
                (pars [0].ParameterType != typeof (ApplicationContext) && pars [0].ParameterType != typeof (object)) ||
                (pars [1].ParameterType != typeof (ActiveEventArgs) && pars [1].ParameterType != typeof (EventArgs)))
                throw new CoreException (string.Format ("Method '{0}.{1}' is not a valid active event.", method.DeclaringType.FullName, method.Name));
        }
    }
}
