/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.lambda.iterators
{
    /// <summary>
    /// <see cref="phosphorus.core.Node"/> iterator, which is useful for using as the outer most iterator for the hyperlisp expression,
    /// taking a node as its input
    /// </summary>
    public class IteratorNode : Iterator
    {
        private Node _node;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.iterators.IteratorNode"/> class
        /// </summary>
        /// <param name="node">the node to start iterating upon</param>
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

