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

namespace p5.ajax.core.internals
{
    /// <summary>
    ///     Class encapsulating one attribute for Ajax widgets.
    /// </summary>
    internal class Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the Attribute class.
        /// </summary>
        /// <param name="name">Name of your attribute</param>
        internal Attribute (string name)
        {
            Name = name;
        }

        /// <summary>
        ///     Initializes a new instance of the Attribute class.
        /// </summary>
        /// <param name="name">Name of your attribute</param>
        /// <param value="name">Value of your attribute</param>
        internal Attribute (string name, string value)
            : this (name)
        {
            Value = value;
        }

        /// <summary>
        ///     Gets the name of your attribute.
        /// </summary>
        /// <value>The name</value>
        internal string Name
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the value of your attribute.
        /// </summary>
        /// <value>The value</value>
        internal string Value
        {
            get;
            private set;
        }
    }
}
