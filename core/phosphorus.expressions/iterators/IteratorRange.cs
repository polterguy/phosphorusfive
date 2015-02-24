
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    /// returns all nodes within the specified range
    /// </summary>
    public class IteratorRange : Iterator
    {
        private readonly int _start = 0;
        private readonly int _end;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorRange"/> class
        /// </summary>
        /// <param name="start">start position</param>
        public IteratorRange (int start, int end)
        {
            _start = start;
            _end = end;
        }

        public override IEnumerable<Node> Evaluate {
            get {
                var idxNo = 0;
                foreach (var idxCurrent in Left.Evaluate) {
                    if (idxNo++ >= _start)
                        yield return idxCurrent;
                    if (_end != -1 && idxNo >= _end)
                        yield break;
                }
            }
        }
    }
}
