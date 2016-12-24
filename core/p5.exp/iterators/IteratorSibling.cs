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
using System.Collections.Generic;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Returns an offset sibling node from previous result-set
    /// </summary>
    [Serializable]
    public class IteratorSibling : Iterator
    {
        readonly int _offset;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IteratorSibling" /> class
        /// </summary>
        /// <param name="offset">offset siblings from current nodes</param>
        public IteratorSibling (int offset)
        {
            _offset = offset;
        }

        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            foreach (var idxCurrent in Left.Evaluate (context)) {

                var offset = _offset;
                var tmpIdx = idxCurrent;

                while (offset != 0 && tmpIdx != null) {
                    if (offset < 0) {
                        offset += 1;
                        if (tmpIdx.PreviousSibling == null)
                            tmpIdx = tmpIdx.Parent.LastChild;
                        else
                            tmpIdx = tmpIdx.PreviousSibling;
                    } else {
                        offset -= 1;
                        if (tmpIdx.NextSibling == null)
                            tmpIdx = tmpIdx.Parent.FirstChild;
                        else
                            tmpIdx = tmpIdx.NextSibling;
                    }
                }
                if (tmpIdx != null)
                    yield return tmpIdx;
            }
        }
    }
}
