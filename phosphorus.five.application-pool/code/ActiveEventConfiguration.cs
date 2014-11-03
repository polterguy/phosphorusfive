/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Configuration;

namespace phosphorus.five.applicationpool
{
    public class ActiveEventConfiguration : ConfigurationSection
    {
        [ConfigurationProperty ("pluginDirectory", DefaultValue="plugins", IsRequired = false)]
        public string PluginDirectory
        {
            get
            {
                return this ["pluginDirectory"] as string;
            }
        }

        [ConfigurationProperty ("assemblies")]
        public ActiveEventAssemblyCollection Assemblies {
            get {
                return this ["assemblies"] as ActiveEventAssemblyCollection; 
            }
        }
    }
}

