/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Configuration;

namespace p5.webapp.code
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