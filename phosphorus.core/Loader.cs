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
    /// loads up assemblies for handling Active Events. class is a natural singleton, use the Instance static member to access
    /// the singleton instance
    /// </summary>
    public class Loader
    {
        private static Loader _instance = new Loader ();
        private List<Assembly> _assemblies = new List<Assembly> ();
        private Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> _instanceActiveEvents;
        private Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> _staticActiveEvents;

        private Loader ()
        {
            _instanceActiveEvents = new Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> ();
            _staticActiveEvents = new Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>> ();
        }

        /// <summary>
        /// gets the instance
        /// </summary>
        /// <value>the singleton instance</value>
        public static Loader Instance {
            get {
                return _instance;
            }
        }

        /// <summary>
        /// creates an application context. there should be one application context for every user in the system. an application
        /// context is being used for registering instance Active Event handlers and raising Active Events
        /// </summary>
        /// <returns>the application context</returns>
        public ApplicationContext CreateApplicationContext ()
        {
            ApplicationContext context = new ApplicationContext (_instanceActiveEvents, _staticActiveEvents);
            return context;
        }

        /// <summary>
        /// loads an assembly for handling Active Events
        /// </summary>
        /// <param name="assembly">assembly to load</param>
        public void LoadAssembly (Assembly assembly)
        {
            if (_assemblies.Exists (
                delegate(Assembly idx) {
                return idx == assembly;
            }))
                return;

            foreach (var idxAsm in AppDomain.CurrentDomain.GetAssemblies ()) {
                if (idxAsm == assembly) {
                    InitializeAssembly (idxAsm);
                    _assemblies.Add (idxAsm);
                }
            }
        }
        
        /// <summary>
        /// loads an assembly for handling Active Events using the current directory as the directory
        /// for the assembly to load
        /// </summary>
        /// <param name="name">name of assembly</param>
        public void LoadAssembly (string name)
        {
            LoadAssembly (string.Empty, name);
        }

        /// <summary>
        /// loads an assembly for handling Active Events
        /// </summary>
        /// <param name="path">directory of assembly</param>
        /// <param name="name">name of assembly</param>
        public void LoadAssembly (string path, string name)
        {
            if (!name.ToLower ().EndsWith (".dll"))
                name += ".dll";
            if (_assemblies.Exists (
                delegate (Assembly idx) {
                    return idx.ManifestModule.Name.ToLower () == name.ToLower ();
            }))
                return;

            foreach (var idxAsm in AppDomain.CurrentDomain.GetAssemblies ()) {
                if (idxAsm.ManifestModule.Name.ToLower () == name.ToLower ()) {
                    InitializeAssembly (idxAsm);
                    _assemblies.Add (idxAsm);
                    return;
                }
            }

            // we must load assembly and link in
            Assembly assembly = Assembly.LoadFile (path + name);
            InitializeAssembly (assembly);
            _assemblies.Add (assembly);
        }

        /// <summary>
        /// unloads the assembly with the given name
        /// </summary>
        /// <param name="name">name of assembly to unload</param>
        public void UnloadAssembly (string name)
        {
            if (!name.ToLower ().EndsWith (".dll"))
                name += ".dll";
            Assembly assembly = _assemblies.Find (
                delegate (Assembly idx) {
                    return idx.ManifestModule.Name.ToLower () == name;
            });
            _assemblies.Remove (assembly);
            RemoveAssembly (assembly);
        }
        
        private void RemoveAssembly (Assembly assembly)
        {
            foreach (Type idxType in assembly.GetTypes ())
            {
                MethodInfo[] instanceMethods = idxType.GetMethods (
                    BindingFlags.FlattenHierarchy | 
                    BindingFlags.Instance | 
                    BindingFlags.NonPublic | 
                    BindingFlags.Public);
                foreach (var idxMethod in instanceMethods) {
                    var atrs = idxMethod.GetCustomAttributes (typeof(ActiveEventAttribute), true) as ActiveEventAttribute[];
                    if (atrs != null && atrs.Length > 0)
                        _instanceActiveEvents.Remove (idxType);
                }

                MethodInfo[] staticMethods = idxType.GetMethods (
                    BindingFlags.FlattenHierarchy | 
                    BindingFlags.Static | 
                    BindingFlags.NonPublic | 
                    BindingFlags.Public);
                foreach (var idxMethod in staticMethods) {
                    var atrs = idxMethod.GetCustomAttributes (typeof(ActiveEventAttribute), true) as ActiveEventAttribute[];
                    if (atrs != null && atrs.Length > 0)
                        _staticActiveEvents.Remove (idxType);
                }
            }
        }

        private void InitializeAssembly (Assembly assembly)
        {
            foreach (Type idxType in assembly.GetTypes ())
            {
                MethodInfo[] instanceMethods = idxType.GetMethods (
                    BindingFlags.FlattenHierarchy | 
                    BindingFlags.Instance | 
                    BindingFlags.NonPublic | 
                    BindingFlags.Public);
                MethodInfo[] staticMethods = idxType.GetMethods (
                    BindingFlags.FlattenHierarchy | 
                    BindingFlags.Static | 
                    BindingFlags.NonPublic | 
                    BindingFlags.Public);

                foreach (var idx in 
                         new Tuple<MethodInfo[], Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>>>[] { 
                            new Tuple<MethodInfo[], Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>>> (instanceMethods, _instanceActiveEvents), 
                            new Tuple<MethodInfo[], Dictionary<Type, List<Tuple<ActiveEventAttribute, MethodInfo>>>> (staticMethods, _staticActiveEvents) }) {
                    var activeEvents = new List<Tuple<ActiveEventAttribute, MethodInfo>> ();
                    foreach (var idxMethod in idx.Item1) {
                        var atrs = idxMethod.GetCustomAttributes (typeof(ActiveEventAttribute), true) as ActiveEventAttribute[];
                        if (atrs != null && atrs.Length > 0) {
                            ParameterInfo[] pars = idxMethod.GetParameters ();
                            if (pars.Length != 2 || 
                                pars [0].ParameterType != typeof(ApplicationContext) || 
                                pars [1].ParameterType != typeof(ActiveEventArgs))
                                throw new ArgumentException (
                                    string.Format("method '{0}.{1}' is not a valid active event, parameters of method is wrong", 
                                              idxMethod.DeclaringType.FullName,
                                              idxMethod.Name));
                            foreach (var idxAtr in atrs) {
                                var tuple = new Tuple<ActiveEventAttribute, MethodInfo> (idxAtr, idxMethod);
                                activeEvents.Add (tuple);
                            }
                        }
                    }
                    if (activeEvents.Count > 0)
                        idx.Item2 [idxType] = activeEvents;
                }
            }
        }
    }
}

