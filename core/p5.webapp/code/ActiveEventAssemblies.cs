/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Configuration;

/// <summary>
///     Main namespace for all code that is specific to your application-pool
/// </summary>
namespace p5.webapp.code
{
    /// <summary>
    ///     Class wrapping your configuration section from your web.config that defines which assemblies to use as Active Event plugins,
    ///     and which default user to raise Active Events as, unless another user is explicitly logged in
    /// </summary>
    public class ActiveEventAssemblies : ConfigurationSection
    {
        /// <summary>
        ///     Gets the plugin directory
        /// </summary>
        /// <value>The plugin directory.</value>
        [ConfigurationProperty ("assemblyDirectory", DefaultValue = "~/plugins/", IsRequired = false)]
        public string PluginDirectory
        {
            get { return this ["assemblyDirectory"] as string; }
        }

        /// <summary>
        ///     Gets the default username used to raise Active Events on behalf of
        /// </summary>
        /// <value>The plugin directory.</value>
        [ConfigurationProperty ("defaultContextUsername", IsRequired = true)]
        public string DefaultContextUsername
        {
            get { return this ["defaultContextUsername"] as string; }
        }

        /// <summary>
        ///     Gets the default role used to raise Active Events as
        /// </summary>
        /// <value>Default context role</value>
        [ConfigurationProperty ("defaultContextRole", IsRequired = true)]
        public string DefaultContextRole
        {
            get { return this ["defaultContextRole"] as string; }
        }

        /// <summary>
        ///     Gets the default role used to raise Active Events as
        /// </summary>
        /// <value>The plugin directory.</value>
        [ConfigurationProperty ("passwordFile", IsRequired = true)]
        public string PasswordFile
        {
            get { return this ["passwordFile"] as string; }
        }

        /// <summary>
        ///     Gets the assemblies
        /// </summary>
        /// <value>The assemblies.</value>
        [ConfigurationProperty ("assemblies")]
        public ActiveEventAssemblyCollection Assemblies
        {
            get { return this ["assemblies"] as ActiveEventAssemblyCollection; }
        }
    }
}