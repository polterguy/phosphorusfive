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

using System;
using System.Collections.Generic;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Returns all nodes, and descendants, of previous iterator
    /// </summary>
    [Serializable]
    public class IteratorDescendants : Iterator
    {
        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            foreach (var idxCurrent in Left.Evaluate (context)) {

                // If next sibling is not null, then that is our "stop node"
                var stop = idxCurrent.NextSibling;

                // Looping until stop not equals null, or we have no more nodes to iterate, because we're at root
                var curIdx = idxCurrent.Parent;
                while (stop == null && curIdx != null) {
                    stop = curIdx.NextSibling;
                    curIdx = curIdx.Parent;
                }

                // At this point, stop is either node where to stop iterator, or null, at which point we iterate until null is reached
                for (var idxNext = idxCurrent.NextNode; idxNext != null && idxNext != stop; idxNext = idxNext.NextNode) {
                    yield return idxNext;
                }
            }
        }
    }
}
