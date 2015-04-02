/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    public class IteratorNode : Iterator
    {
        private readonly Node _node;

        public IteratorNode (Node node)
        {
            _node = node;
        }

        public override IEnumerable<Node> Evaluate
        {
            get { yield return _node; }
        }
    }
}