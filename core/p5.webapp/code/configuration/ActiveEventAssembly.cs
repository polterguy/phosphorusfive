/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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