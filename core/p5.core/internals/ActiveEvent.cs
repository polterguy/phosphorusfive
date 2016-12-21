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

using System.Collections.Generic;

namespace p5.core.internals
{
    /// <summary>
    ///     One single Active Event.
    /// </summary>
    internal class ActiveEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActiveEvents+ActiveEvent"/> class.
        /// </summary>
        /// <param name="name">Name of Active Event</param>
        public ActiveEvent (string name)
        {
            Name = name;
            Methods = new List<MethodSink> ();
        }

        /// <summary>
        ///     Name of Active Event.
        /// </summary>
        /// <value>The Active Event's name</value>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        ///     Encapsulates all methods tied to this specific Active Event.
        /// </summary>
        /// <value>The methods</value>
        public List<MethodSink> Methods
        {
            get;
            private set;
        }
    }
}
