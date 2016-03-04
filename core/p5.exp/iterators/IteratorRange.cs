/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Collections.Generic;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Returns all nodes within the specified range
    /// </summary>
    [Serializable]
    public class IteratorRange : Iterator
    {
        private readonly int _from;
        private readonly int _to;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorRange" /> class
        /// </summary>
        /// <param name="from">start position, from</param>
        /// <param name="to">end position, to</param>
        public IteratorRange (int from, int to)
        {
            _from = from;
            _to = to;
        }

        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            if (_to == -1) {
                foreach (var idxCurrent in Left.Evaluate (context).Skip (_from)) {
                    yield return idxCurrent;
                }
            } else {
                foreach (var idxCurrent in Left.Evaluate (context).Skip (_from).Take (_to - _from)) {
                    yield return idxCurrent;
                }
            }
        }
    }
}
