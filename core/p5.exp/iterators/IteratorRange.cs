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
using System.Linq;
using System.Collections.Generic;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Returns all nodes within the specified range
    /// </summary>
    [Serializable]
    public class IteratorRange : Iterator
    {
        private readonly int _from;
        private readonly int _to;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorRange" /> class
        /// </summary>
        /// <param name="from">start position, from</param>
        /// <param name="to">end position, to</param>
        public IteratorRange (int from, int to)
        {
            _from = from;
            _to = to;
        }

        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            if (_to == -1) {
                foreach (var idxCurrent in Left.Evaluate (context).Skip (_from)) {
                    yield return idxCurrent;
                }
            } else {
                foreach (var idxCurrent in Left.Evaluate (context).Skip (_from).Take (_to - _from)) {
                    yield return idxCurrent;
                }
            }
        }
    }
}
