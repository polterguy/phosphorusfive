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
    public abstract class MatchIterator
    {
        private MatchIterator _parent;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.MatchIterator"/> class
        /// </summary>
        public MatchIterator ()
        { }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.MatchIterator"/> class
        /// </summary>
        /// <param name="parent">parent match iterator object</param>
        protected MatchIterator (MatchIterator parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// returns nodes matching criteria
        /// </summary>
        /// <value>the nodes matching the criteria</value>
        public abstract IEnumerable<Node> Nodes {
            get;
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
        public MatchIterator Parent {
            get {
                return _parent;
            }
        }
    }
}

