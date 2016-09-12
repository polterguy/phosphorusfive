/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Configuration;

namespace p5.webapp.code.configuration
{
    /// <summary>
    ///     Active event assembly
    /// </summary>
    public class ActiveEventAssembly : ConfigurationElement
    {
        /// <summary>
        ///     Gets the assembly for this instance
        /// </summary>
        /// <value>The assembly</value>
        [ConfigurationProperty ("assembly", IsRequired = true)]
        public string Assembly
        {
            get { return this ["assembly"] as string; }
        }
    }
}