
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
    /// returns all parent <see cref="phosphorus.core.Node"/>s of previous iterator result
    /// </summary>
    public class IteratorParent : Iterator
    {
        public override IEnumerable<Node> Evaluate {
            get {
                return from idxCurrent in Left.Evaluate where idxCurrent.Parent != null select idxCurrent.Parent;
            }
        }
    }
}
