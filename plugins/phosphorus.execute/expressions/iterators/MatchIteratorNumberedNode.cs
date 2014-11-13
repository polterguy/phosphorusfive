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
    /// <see cref="phosphorus.execute.MatchIterator"/> class for returning named <see cref="phosphorus.core.Node"/>s of tree hierarchy
    /// </summary>
    public class MatchIteratorNumberedNode : MatchIterator
    {
        private int _number;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.MatchIteratorRoot"/> class
        /// </summary>
        /// <param name="parent">parent match iterator</param>
        public MatchIteratorNumberedNode (MatchIterator parent, int number)
            : base (parent)
        {
            _number = number;
        }

        public override IEnumerable<Node> Nodes {
            get {
                foreach (Node idxCurrent in Parent.Nodes) {
                    if (idxCurrent.Count > _number) {
                        yield return idxCurrent [_number];
                    }
                }
            }
        }
    }
}

