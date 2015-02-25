/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Linq;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     returns an offset sibling <see cref="phosphorus.core.Node" />
    /// </summary>
    public class IteratorShiftLeft : Iterator
    {
        public override IEnumerable<Node> Evaluate
        {
            get { return Left.Evaluate.Select (idxCurrent => idxCurrent.PreviousNode).Where (previous => previous != null); }
        }
    }
}