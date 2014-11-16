/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute.iterators
{
    public class IteratorNode : Iterator
    {
        private Node _node;

        public IteratorNode (Node node)
        {
            _node = node;
        }

        public override IEnumerable<Node> Evaluate {
            get {
                yield return _node;
            }
        }
    }
}

