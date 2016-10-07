/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.Configuration;

/// <summary>
///     Main namespace for all configuration code that is specific to your application-pool
/// </summary>
namespace p5.webapp.code.configuration
{
    /// <summary>
    ///     Class wrapping your Phosphorus Five configuration section from web.config
    /// </summary>
    public class PhosphorusConfiguration : ConfigurationSection
    {
        /// <summary>
        ///     Gets the plugin directory
        /// </summary>
        /// <value>The plugin directory</value>
        [ConfigurationProperty ("assemblyDirectory", DefaultValue = "/bin/", IsRequired = false)]
        public string PluginDirectory
        {
            get { return this ["assemblyDirectory"] as string; }
        }

        /// <summary>
        ///     Gets the default username used to raise Active Events on behalf of
        /// </summary>
        /// <value>The plugin directory</value>
        [ConfigurationProperty ("defaultContextUsername", IsRequired = false, DefaultValue = "guest")]
        public string DefaultContextUsername
        {
            get { return this ["defaultContextUsername"] as string; }
        }

        /// <summary>
        ///     Gets the default role used to raise Active Events as
        /// </summary>
        /// <value>Default context role</value>
        [ConfigurationProperty ("defaultContextRole", IsRequired = false, DefaultValue = "guest")]
        public string DefaultContextRole
        {
            get { return this ["defaultContextRole"] as string; }
        }

        /// <summary>
        ///     Gets the path to the file on disc that is used for authenticating and authorizing users
        /// </summary>
        /// <value>Path to auth file</value>
        [ConfigurationProperty ("authFile", IsRequired = false, DefaultValue = "/auth.hl")]
        public string AuthFile
        {
            get { return this ["authFile"] as string; }
        }

        /// <summary>
        ///     Gets the assemblies
        /// </summary>
        /// <value>The assemblies</value>
        [ConfigurationProperty ("assemblies")]
        public ActiveEventAssemblyCollection Assemblies
        {
            get { return this ["assemblies"] as ActiveEventAssemblyCollection; }
        }
    }
}