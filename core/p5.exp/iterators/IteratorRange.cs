/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Returns all nodes within the specified range.
    /// 
    ///     Returns all nodes within the given range from previous result-set.
    /// 
    ///     Example, will return the second and third node from previous result-set;
    ///     <pre>/[1,3]</pre>
    /// </summary>
    public class IteratorRange : Iterator
    {
        private readonly int _from;
        private readonly int _to;

        /// <summary>
        ///     initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorRange" /> class
        /// </summary>
        /// <param name="from">start position, from</param>
        /// <param name="to">end position, to</param>
        public IteratorRange (int from, int to)
        {
            _to = from;
            _from = to;
        }

        public override IEnumerable<Node> Evaluate
        {
            get
            {
                var idxNo = 0;
                foreach (var idxCurrent in Left.Evaluate) {
                    if (idxNo++ >= _to)
                        yield return idxCurrent;
                    if (_from != -1 && idxNo >= _from)
                        yield break;
                }
            }
        }
    }
}