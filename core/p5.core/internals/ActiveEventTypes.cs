/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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
    /*
     * Wrapper for types that contains Active Event handlers.
     * 
     * Class is basically a thin layer around a Dictionary instance, and really nothing more.
     */
    class ActiveEventTypes
    {
        // Contains the list af Active Events/Methods for each Type registered in current instance.
        readonly Dictionary<Type, List<Tuple<string, MethodInfo>>> _types = new Dictionary<Type, List<Tuple<string, MethodInfo>>> ();

        /*
         * Adds an Active Event for given type.
         */
        internal void AddActiveEvent (Type type, string activeEventName, MethodInfo method)
        {
            // Making sure there exists an entry for type
            if (!_types.ContainsKey (type))
                _types [type] = new List<Tuple<string, MethodInfo>> ();

            // Adding Active Event and its associated method
            _types [type].Add (new Tuple<string, MethodInfo> (activeEventName, method));
        }

        /*
         * Deletes a type entirely from being able to handle Active Events.
         */
        internal void DeleteType (Type type)
        {
            if (_types.ContainsKey (type))
                _types.Remove (type);
        }

        /*
         * Operator overload to make class behave like a Dictionary.
         */
        internal IEnumerable<Tuple<string, MethodInfo>> this [Type key]
        {
            get { return _types [key]; }
        }

        /*
         * Returns true it the given Type exists in instance.
         */
        internal bool ContainsKey (Type key)
        {
            return _types.ContainsKey (key);
        }

        /*
         * Returns all Type keys for instance.
         */
        internal IEnumerable<Type> Keys
        {
            get { return _types.Keys; }
        }
    }
}
