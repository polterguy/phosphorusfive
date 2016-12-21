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

using System.Reflection;

namespace p5.core.internals
{
    /// <summary>
    ///     One single Active Event handler, with its associated instance, if any.
    /// </summary>
    internal class MethodSink
    {
        /// <summary>
        ///     Creates a new Active Event handler.
        /// 
        ///     If instance is null, it assumes your event handler is a static method.
        /// </summary>
        /// <param name="method">Method that implements Active Event</param>
        /// <param name="instance">Instance to raise method within.</param>
        public MethodSink (MethodInfo method, object instance)
        {
            Method = method;
            Instance = instance;
        }

        /// <summary>
        ///     Returns the method that implements your Active Event.
        /// </summary>
        /// <value>The method</value>
        public MethodInfo Method
        {
            get;
            private set;
        }

        /// <summary>
        ///     The object instance to raise the method within.
        /// 
        ///     If null, your Active Event is assumed to be a static method.
        /// </summary>
        /// <value>The instance.</value>
        public object Instance
        {
            get;
            private set;
        }
    }
}
