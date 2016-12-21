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

using System;
using System.Reflection;
using System.Collections.Generic;

namespace p5.core.internals
{
    /// <summary>
    ///     Wrapper for all type in AppDomain that contains Active Event handlers
    /// </summary>
    internal class ActiveEventTypes
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.core.Loader+ActiveEventTypes"/> class.
        /// </summary>
        public ActiveEventTypes()
        {
            Types = new Dictionary<Type, ActiveEventType>();
        }

        /// <summary>
        ///     Gets or sets all Types in this instance that has Active Event handlers.
        /// </summary>
        /// <value>The types</value>
        public Dictionary<Type, ActiveEventType> Types
        {
            get;
            private set;
        }

        /// <summary>
        ///     Adds an Active Event for given type
        /// </summary>
        /// <param name="type">Type with Active Event handlers</param>
        /// <param name="atr">Attribute describing Active Event handler</param>
        /// <param name="method">Method that implements handler</param>
        public void AddActiveEvent (Type type, ActiveEventAttribute atr, MethodInfo method)
        {
            // Making sure there exists an entry for type
            if (!Types.ContainsKey(type))
                Types[type] = new ActiveEventType();

            // Adding Active Event and its associated method
            Types[type].AddActiveEvent(atr, method);
        }

        /// <summary>
        ///     Removes a type entirely from being able to handle Active Events
        /// </summary>
        /// <param name="type">Type</param>
        public void RemoveType (Type type)
        {
            if (Types.ContainsKey(type))
                Types.Remove(type);
        }
    }
}
