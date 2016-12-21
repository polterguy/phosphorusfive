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
using System.Collections.Generic;

namespace p5.core.internals
{
    /// <summary>
    ///     Wrapper for a single type in AppDomain that contains Active Event handlers.
    /// </summary>
    internal class ActiveEventType
    {
        /// <summary>
        ///     One single Active Event
        /// 
        ///     Notice, there might exist several Active Events for a single method
        /// </summary>
        internal class ActiveEvent
        {
            /// <summary>
            ///     Initializes a new instance of the
            /// <see cref="p5.core.Loader+ActiveEventTypes+ActiveEventType+ActiveEvent"/> class
            /// </summary>
            /// <param name="atr">Atr</param>
            /// <param name="method">Method</param>
            public ActiveEvent(ActiveEventAttribute atr, MethodInfo method)
            {
                Attribute = atr;
                Method = method;
            }

            /// <summary>
            ///     Gets the ActiveEventAttribute for given Active Event
            /// </summary>
            /// <value>The attribute</value>
            public ActiveEventAttribute Attribute {
                get;
                private set;
            }

            /// <summary>
            ///     Returns the Method for Active Event
            /// </summary>
            /// <value>The method</value>
            public MethodInfo Method {
                get;
                private set;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.core.Loader+ActiveEventTypes+ActiveEventType"/> class.
        /// </summary>
        public ActiveEventType()
        {
            Events = new List<ActiveEvent> ();
        }

        /// <summary>
        ///     Gets the list of Active Events for given Type
        /// </summary>
        /// <value>The events</value>
        public List<ActiveEvent> Events {
            get;
            private set;
        }

        /// <summary>
        ///     Adds an Active Event for given type
        /// </summary>
        /// <param name="atr">Atr</param>
        /// <param name="method">Method</param>
        public void AddActiveEvent (ActiveEventAttribute atr, MethodInfo method)
        {
            Events.Add(new ActiveEvent(atr, method));
        }
    }
}
