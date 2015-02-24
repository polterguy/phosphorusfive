
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Linq;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    /// returns all children of previous iterator
    /// </summary>
    public class IteratorChildren : Iterator
    {
        public override IEnumerable<Node> Evaluate {
            get {
                return Left.Evaluate.SelectMany(idxCurrent => idxCurrent.Children);
            }
        }
    }
}
