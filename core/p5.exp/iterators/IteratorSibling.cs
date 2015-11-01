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
    ///     Returns an offset sibling node from previous result-set.
    /// 
    ///     Can start with either "+" or "-", depending upon whether or not you'd like to retrieve a "younger sibling"
    ///     or an "older sibling" from the previous result-set. Next comes an integer value, defining how many siblings
    ///     you wish to "jump".
    /// 
    ///     Example, that returns the sibling which is "two generations older" than your current result-set node's;
    ///     <pre>/+2</pre>
    /// </summary>
    [Serializable]
    public class IteratorSibling : Iterator
    {
        private readonly int _offset;

        /// <summary>
        ///     initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorSibling" /> class
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
