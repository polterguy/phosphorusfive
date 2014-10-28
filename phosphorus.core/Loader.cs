/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Reflection;
using System.Collections.Generic;

namespace phosphorus.core
{
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

        public static Loader Instance {
            get {
                return _instance;
            }
        }

        public ApplicationContext CreateApplicationContext ()
        {
            ApplicationContext context = new ApplicationContext (_instanceActiveEvents, _staticActiveEvents);
            return context;
        }

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

        public void LoadAssembly (string path, string name)
        {
            if (_assemblies.Exists (
                delegate(Assembly idx) {
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

            // loading assembly
            Assembly assembly = AppDomain.CurrentDomain.Load (path + name);
            InitializeAssembly (assembly);
            _assemblies.Add (assembly);
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
                            if (pars.Length != 2 || pars [0].ParameterType != typeof(object) || pars [1].ParameterType != typeof(ActiveEventArgs))
                                throw new ArgumentException (
                                    string.Format("method '{0}.{1}' is not a valid active event", 
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

