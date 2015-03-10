/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Configuration;

namespace phosphorus.applicationpool.code
{
    /// <summary>
    ///     Active event assembly.
    /// 
    ///     This defines one Assembly to use as an Active Event plugin assembly. Add a key in your web.config for your assemblies, 
    ///     if you wish for the framework to automatically load it up, and use your assembly as a source for Active Event handlers.
    /// </summary>
    public class ActiveEventAssembly : ConfigurationElement
    {
        /// <summary>
        ///     Gets the assembly.
        /// 
        ///     The assembly you wish to use as an Active Event handler assembly.
        /// </summary>
        /// <value>The assembly.</value>
        [ConfigurationProperty ("assembly", IsRequired = true)]
        public string Assembly
        {
            get { return this ["assembly"] as string; }
        }
    }
}