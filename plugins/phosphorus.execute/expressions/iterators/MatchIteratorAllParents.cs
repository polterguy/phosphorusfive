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
    /// <see cref="phosphorus.execute.MatchIterator"/> class for returning all parent <see cref="phosphorus.core.Node"/>s of tree hierarchy
    /// </summary>
    public class MatchIteratorAllParents : MatchIterator
    {
        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.MatchIteratorAllParents"/> class
        /// </summary>
        /// <param name="parent">parent match iterator</param>
        public MatchIteratorAllParents (MatchIterator parent)
            : base (parent)
        { }

        public override IEnumerable<Node> Nodes {
            get {
                foreach (Node idxCurrent in Parent.Nodes) {
                    yield return idxCurrent.Parent;
                }
            }
        }
    }
}

