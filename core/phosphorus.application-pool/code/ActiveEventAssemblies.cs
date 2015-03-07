/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Configuration;

/// <summary>
///     Main namespace for all code that is specific to your application-pool.
/// 
///     Contains all code that is necessary to glue together your application-pool.
/// </summary>
namespace phosphorus.five.applicationpool.code
{
    /// <summary>
    ///     Class wrapping your configuration section from your web.config that defines which assemblies to use as Active Event plugins.
    /// 
    ///     To add your own plugins to your application pool, add one key for the assembly in your web.config at the appropriate place.
    /// </summary>
    public class ActiveEventAssemblies : ConfigurationSection
    {
        /// <summary>
        ///     Gets the plugin directory.
        /// 
        ///     By default, Phosphorus will use your "bin" folder inside of your application pool to resolve its plugin asemblies.
        ///     You can however override this behavior, by explicitly changing the folder it looks in by changing this property through
        ///     your web.config file.
        /// </summary>
        /// <value>The plugin directory.</value>
        [ConfigurationProperty ("assemblyDirectory", DefaultValue = "~/plugins/", IsRequired = false)]
        public string PluginDirectory
        {
            get { return this ["assemblyDirectory"] as string; }
        }

        /// <summary>
        ///     Gets the assemblies.
        /// 
        ///     These are your assemblies that you wish to use as Phosphorus.Five plugins in your application.
        /// </summary>
        /// <value>The assemblies.</value>
        [ConfigurationProperty ("assemblies")]
        public ActiveEventAssemblyCollection Assemblies
        {
            get { return this ["assemblies"] as ActiveEventAssemblyCollection; }
        }
    }
}