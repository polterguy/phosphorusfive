/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using p5.core.internals;

namespace p5.core
{
    /// <summary>
    ///     Loads up assemblies for handling Active Events.
    /// 
    ///     Class is a singleton, use Instance to retrieve its instance.
    /// </summary>
    public class Loader
    {
        /// <summary>
        ///     Returns the singleton instance.
        /// </summary>
        /// <value>The singleton instance</value>
        public static readonly Loader Instance = new Loader ();

        // List of all assmeblies that has been loaded and registered for handling Active Events.
        private readonly List<Assembly> _assemblies = new List<Assembly> ();

        // Types that have instance Active Event handlers.
        private readonly ActiveEventTypes _instanceActiveEvents = new ActiveEventTypes();

        // Types that have static Active Event handlers.
        private readonly ActiveEventTypes _staticActiveEvents = new ActiveEventTypes();

        /*
         * Private constructor, to prevent other classes from instantiating this class.
         */
        private Loader ()
        { }

        /// <summary>
        ///     Creates a new ApplicationContext for you.
        /// 
        ///     This is the only way to actually create an ApplicationContext, which again is used for raising Active Events.
        /// </summary>
        /// <returns>The newly created context</returns>
        public ApplicationContext CreateApplicationContext (ContextTicket ticket = null)
        {
            return new ApplicationContext (_instanceActiveEvents, _staticActiveEvents, ticket);
        }

        /// <summary>
        ///     Registers an assembly for handling Active Events.
        /// 
        ///     This must be done before you create your ApplicationContext.
        ///     Will find all types in specified assembly, that have Active Event methods, and store their types as Active Event handlers, using
        ///     them later when creating our ApplicationContext, to create Active Event handlers, both static and instance handlers.
        /// </summary>
        /// <param name="assembly">Assembly to register as Active event handler</param>
        public void RegisterAssembly (Assembly assembly)
        {
            // Checking to see if assembly is already loaded, to avoid initializing the same assembly twice.
            if (_assemblies.Exists (idx => idx == assembly))
                return;

            // Looking for assembly in our current AppDomain, for then to initialize it.
            foreach (var idxAsm in AppDomain.CurrentDomain.GetAssemblies ()) {

                // Checking if this is the requested assembly.
                if (idxAsm == assembly) {

                    // This is the requested assembly, registering it, and initializing types from it.
                    RegisterTypesFromAssembly (idxAsm);
                    _assemblies.Add (idxAsm);
                }
            }
        }

        /// <summary>
        ///     Registers the specified assembly, containing the given type, for handling Active Events.
        /// 
        ///     This must be done before you create your ApplicationContext.
        ///     Will find all types in specified assembly, that have Active Event methods, and store their types as Active Event handlers, using
        ///     them later when creating our ApplicationContext, to create Active Event handlers, both static and instance handlers.
        /// </summary>
        /// <param name="type">Type from assembly you wish to register</param>
        public void RegisterAssembly (Type type)
        {
            RegisterAssembly (type.Assembly);
        }

        /// <summary>
        ///     Loads the specified assembly, and registers it for handling Active Events.
        /// 
        ///     This must be done before you create your ApplicationContext.
        ///     Will load Assembly into AppDomain, and find all types in it, that have Active Event methods, and store their types as 
        ///     Active Event handlers, using them later when creating our ApplicationContext, to create Active Event handlers.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly you wish to load</param>
        public void LoadAssembly (string assemblyName)
        {
            LoadAssembly ("", assemblyName);
        }

        /// <summary>
        ///     Loads and registers an assembly for handling Active Events.
        /// 
        ///     Will use the specified path, instead of resolving the path automatically.
        ///     Allows you to load up assemblies, with an explicit path, inject them into AppDomain, and register them as 
        ///     Active Event handlers.
        ///     Will load Assembly into AppDomain, and find all types in it, that have Active Event methods, and store their types as 
        ///     Active Event handlers, using them later when creating our ApplicationContext, to create Active Event handlers.
        /// </summary>
        /// <param name="assemblyPath">Folder where assembly exists</param>
        /// <param name="assemblyName">Name of your assembly</param>
        public void LoadAssembly (string assemblyPath, string assemblyName)
        {
            // Making sure assemblyName ends with ".dll", unless caller already did this for us.
            if (!assemblyName.ToLower ().EndsWith (".dll"))
                assemblyName += ".dll";

            // Checking to see if assembly is already loaded, at which point we return early.
            if (_assemblies.Exists (idx => idx.ManifestModule.Name.ToLower () == assemblyName.ToLower ()))
                return;

            // Checking our current AppDomain, to see if assembly is already a part of our AppDomain, at which point, 
            // all we have to do is to "register" it as an Active event assembly.
            foreach (var idxAsm in AppDomain.CurrentDomain.GetAssemblies ()) {

                // Checking name of currently iterated assembly, to see if it's the assembly we wish to load
                if (idxAsm.ManifestModule.Name.ToLower () == assemblyName.ToLower ()) {

                    // Initializing assembly for handling Active Events that is already loaded in AppDomain
                    RegisterTypesFromAssembly (idxAsm);
                    _assemblies.Add (idxAsm);
                    return;
                }
            }

            // We must dynamically load assembly, and initialize it, since it is not previously loaded into our AppDomain.
            var assembly = Assembly.LoadFrom (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, assemblyPath + assemblyName));
            RegisterTypesFromAssembly (assembly);
            _assemblies.Add (assembly);
        }

        /// <summary>
        ///     Unregister the assembly with the given name.
        /// 
        ///     No types in assembly will be able to handle Active Events after you've invoked this method.
        /// </summary>
        /// <param name="name">Name of assembly to unregister</param>
        public void UnregisterAssembly (string name)
        {
            // Making sure assemblyName ends with ".dll", unless caller already did this for us.
            if (!name.ToLower ().EndsWith (".dll"))
                name += ".dll";

            // Finding the assembly in our list of initialized assemblies.
            var assembly = _assemblies.Find (idx => idx.ManifestModule.Name.ToLower () == name.ToLower ());
            if (assembly != null) {

                // Removing assembly, and making sure all Active Events are "unregistered".
                // Please notice, that assembly is still in AppDomain, but will no longer handle Active Events in any ways, 
                // unless it is later registered back into our Loader.
                UnregisterAssembly (assembly);
            }
        }

        /// <summary>
        ///     Unregister the specified assembly.
        /// 
        ///     No types in assembly will be able to handle Active Events after you've invoked this method.
        /// </summary>
        /// <param name="assembly">Assembly to unregister</param>
        public void UnregisterAssembly (Type type)
        {
            UnregisterAssembly (type.Assembly);
        }

        /// <summary>
        ///     Unregister the specified assembly.
        /// 
        ///     No types in assembly will be able to handle Active Events after you've invoked this method.
        /// </summary>
        /// <param name="assembly">Assembly to unregister</param>
        public void UnregisterAssembly (Assembly assembly)
        {
            // Verifying assemly actually is registered from before, and if not, returning early.
            if (_assemblies.Find (ix => ix == assembly) == null)
                return;

            // Removing assembly from list of Active Event assemblies.
            _assemblies.Remove (assembly);

            // Looping through all types from assembly, and removing them as Active Event handlers.
            foreach (var idxType in assembly.GetTypes ()) {

                // Removing type from instance Active Events list.
                _instanceActiveEvents.RemoveType (idxType);

                // Removing type from static Active Events list.
                _staticActiveEvents.RemoveType (idxType);
            }
        }

        /*
         * Initializes an assembly, by looping through all types from it, and see if type has Active Event attributes for one or more of its methods.
         * If it does, we register type as Active Event handler, for all future ApplicationContext objects created from Loader.
         */
        private void RegisterTypesFromAssembly (Assembly assembly)
        {
            // Looping through all types in assembly.
            foreach (var idxType in assembly.GetTypes ()) {

                // Adding instance Active Events.
                var instanceMethods = idxType.GetMethods (
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Instance |
                    BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly |
                    BindingFlags.Public);
                AddActiveEventsForType (idxType, instanceMethods, _instanceActiveEvents);

                // Adding static Active Events.
                var staticMethods = idxType.GetMethods (
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Static |
                    BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly |
                    BindingFlags.Public);
                AddActiveEventsForType (idxType, staticMethods, _staticActiveEvents);
            }
        }

        /*
         * Loops through all MethodInfo objects given, and adds them to the associated dictionary with type as key, 
         * if they have ActiveEventAttribute declared, once or more.
         */
        private void AddActiveEventsForType (Type type, MethodInfo[] methods, ActiveEventTypes activeEventTypes)
        {
            // Looping through all MethodInfos from type we currently are iterating.
            foreach (var idxMethod in methods) {

                // Checking to see if current MethodInfo has our Active Event attribute, and if it does, we check if it has
                // the right signature, before we add it to our list of Active Event sinks.
                var atrs = idxMethod.GetCustomAttributes (typeof (ActiveEventAttribute), true) as ActiveEventAttribute [];
                if (atrs != null && atrs.Length > 0) {

                    // Checking if Active Event method has a valid signature.
                    VerifyActiveEventSignature (idxMethod);

                    // Looping through each Active Event attribute for method.
                    foreach (var idxAtr in atrs) {

                        // Adding currently iterated Active Event attribute
                        activeEventTypes.AddActiveEvent (type, idxAtr, idxMethod);
                    }
                }
            }
        }

        /*
         * Verifies that the signature of our Active Event is correct.
         */
        private static void VerifyActiveEventSignature (MethodInfo method)
        {
            var pars = method.GetParameters ();

            // An Active Event must take two arguments, the first argument must be of type "ApplicationContext", and the
            // second argument must be of type "ActiveEventArgs".
            // Optionally, to conform to a default event in the CLR, we also allow (object, EventArgs) as signature.
            // However, the latter is not encouraged to use in code, except for places where you need to explicitly interact with legacy code, where
            // your event handler is an old event, which you need to give support for handling Active Events.
            if (pars.Length != 2 ||
                (pars [0].ParameterType != typeof (ApplicationContext) &&
                pars [0].ParameterType != typeof (object)) ||
                (pars [1].ParameterType != typeof (ActiveEventArgs) &&
                 pars [1].ParameterType != typeof (EventArgs)))

                // Oops, signature didn't match.
                throw new ArgumentException (
                    string.Format ("Method '{0}.{1}' is not a valid Active Event, parameters of method is wrong.",
                        method.DeclaringType.FullName,
                        method.Name));
        }
    }
}
