/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Configuration;

// ReSharper disable ClassNeverInstantiated.Global

namespace phosphorus.applicationpool.code
{
    /// <summary>
    ///     Active event assembly collection.
    /// 
    ///     List of Assemblies to use as Active Event handlers.
    /// </summary>
    public class ActiveEventAssemblyCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement () { return new ActiveEventAssembly (); }
        protected override object GetElementKey (ConfigurationElement element) { return ((ActiveEventAssembly) element).Assembly; }
    }
}