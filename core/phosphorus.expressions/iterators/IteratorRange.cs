/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    public class IteratorRange : Iterator
    {
        private readonly int _from;
        private readonly int _to;

        public IteratorRange (int from, int to)
        {
            _from = to;
            _to = from;
        }

        public override IEnumerable<Node> Evaluate
        {
            get
            {
                var idxNo = 0;
                foreach (var idxCurrent in Left.Evaluate) {
                    if (idxNo++ >= _from)
                        yield return idxCurrent;
                    if (_to != -1 && idxNo >= _to)
                        yield break;
                }
            }
        }
    }
}