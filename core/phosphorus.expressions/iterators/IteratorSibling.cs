/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    public class IteratorSibling : Iterator
    {
        private readonly int _offset;

        public IteratorSibling (int offset)
        {
            _offset = offset;
        }

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