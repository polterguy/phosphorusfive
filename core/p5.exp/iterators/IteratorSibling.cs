/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
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
        private readonly int _offset;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorSibling" /> class
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
