/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     returns an offset sibling <see cref="phosphorus.core.Node" />
    /// </summary>
    public class IteratorSibling : Iterator
    {
        private readonly int _offset;

        /// <summary>
        ///     initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorSibling" /> class
        /// </summary>
        /// <param name="offset">offset siblings from current nodes</param>
        public IteratorSibling (int offset) { _offset = offset; }

        public override IEnumerable<Node> Evaluate
        {
            get
            {
                foreach (var idxCurrent in Left.Evaluate) {
                    var offset = _offset;
                    var tmpIdx = idxCurrent;
                    while (offset != 0 && tmpIdx != null) {
                        if (offset < 0) {
                            offset += 1;
                            tmpIdx = tmpIdx.PreviousSibling;
                        } else {
                            offset -= 1;
                            tmpIdx = tmpIdx.NextSibling;
                        }
                    }
                    if (tmpIdx != null)
                        yield return tmpIdx;
                }
            }
        }
    }
}