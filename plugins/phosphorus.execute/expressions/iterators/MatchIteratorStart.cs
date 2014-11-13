/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute
{
    /// <summary>
    /// <see cref="phosphorus.execute.MatchIterator"/> class for returning start <see cref="phosphorus.core.Node"/> of tree hierarchy iteration
    /// </summary>
    public class MatchIteratorStart : MatchIterator
    {
        private Node _node;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.MatchIteratorStart"/> class
        /// </summary>
        /// <param name="parent">parent match iterator</param>
        public MatchIteratorStart (Node node)
        {
            _node = node;
        }

        /// <summary>
        /// returns the Node for this instance
        /// </summary>
        /// <value>the node where the iterator started</value>
        public Node Node {
            get {
                return _node;
            }
        }

        public override IEnumerable<Node> Nodes {
            get {
                yield return _node;
            }
        }
    }
}

