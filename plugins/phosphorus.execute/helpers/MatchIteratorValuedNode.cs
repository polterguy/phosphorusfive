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
    /// <see cref="phosphorus.execute.MatchIterator"/> class for returning <see cref="phosphorus.core.Node"/>s of tree hierarchy
    /// with specified value
    /// </summary>
    public class MatchIteratorValuedNode : MatchIterator
    {
        private string _value;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.MatchIteratorValuedNode"/> class
        /// </summary>
        /// <param name="parent">parent match iterator</param>
        public MatchIteratorValuedNode (MatchIterator parent, string value)
            : base (parent)
        {
            _value = value;
        }

        public override IEnumerable<Node> Nodes {
            get {
                foreach (Node idxCurrent in Parent.Nodes) {
                    if (idxCurrent.Get<string> () == _value)
                        yield return idxCurrent;
                }
            }
        }
    }
}

