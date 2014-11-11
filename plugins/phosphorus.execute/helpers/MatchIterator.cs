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
    /// iterator base class for traversing matches in <see cref="phosphorus.core.Node"/> 
    /// </summary>
    public class MatchIterator
    {
        private Node _node;
        private MatchIterator _parent;

        private MatchIterator (Node node)
        {
            _node = node;
            _parent = null;
        }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.MatchIterator"/> class
        /// </summary>
        /// <param name="parent">parent match iterator object</param>
        protected MatchIterator (MatchIterator parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// static constructor creating a root match iterator object
        /// </summary>
        /// <param name="node">root node for match iterator hierarchy</param>
        public static MatchIterator Create (Node node)
        {
            return new MatchIterator (node);
        }

        /// <summary>
        /// returns nodes matching criteria
        /// </summary>
        /// <value>the nodes matching the criteria</value>
        public virtual IEnumerable<Node> Nodes {
            get {
                yield return _node;
            }
        }

        /// <summary>
        /// gets a value indicating whether this instance has a match or not
        /// </summary>
        /// <value><c>true</c> if this instance has match; otherwise, <c>false</c></value>
        public bool HasMatch {
            get {
                return Nodes.GetEnumerator ().MoveNext ();
            }
        }

        /// <summary>
        /// returns parent match iterator object
        /// </summary>
        /// <value>the parent match iterator of the current instance</value>
        protected MatchIterator Parent {
            get {
                return _parent;
            }
        }
    }
}

