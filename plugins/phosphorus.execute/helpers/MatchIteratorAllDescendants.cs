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
    /// <see cref="phosphorus.execute.MatchIterator"/> class for returning all descendant <see cref="phosphorus.core.Node"/>s of tree hierarchy
    /// </summary>
    public class MatchIteratorAllDescendants : MatchIterator
    {
        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.MatchIteratorRoot"/> class
        /// </summary>
        /// <param name="parent">parent match iterator</param>
        public MatchIteratorAllDescendants (MatchIterator parent)
            : base (parent)
        { }

        public override IEnumerable<Node> Nodes {
            get {
                foreach (Node idxCurrent in Parent.Nodes) {
                    foreach (Node idxChild in ReturnChildren (idxCurrent)) {
                        yield return idxChild;
                    }
                }
            }
        }

        private IEnumerable<Node> ReturnChildren (Node idx)
        {
            foreach (Node idxChild in idx.Children) {
                yield return idxChild;
                foreach (Node idxChildChild in ReturnChildren (idxChild)) {
                    yield return idxChildChild;
                }
            }
        }
    }
}

