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
using System.Collections.Generic;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Iterator class, wrapping one iterator, for p5 lambda expressions
    /// </summary>
    [Serializable]
    public abstract class Iterator
    {
        /// <summary>
        ///     Gets or sets the left or "previous iterator"
        /// </summary>
        /// <value>Its previous iterator in its chain of iterators</value>
        public Iterator Left
        {
            get;
            set;
        }

        /// <summary>
        ///     Evaluates the iterator
        /// </summary>
        /// <value>The evaluated result, returning a list of <see cref="Node" />s</value>
        public abstract IEnumerable<Node> Evaluate (ApplicationContext context);
    }
}
