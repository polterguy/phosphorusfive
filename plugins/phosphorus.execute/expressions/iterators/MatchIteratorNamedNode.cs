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
    public class MatchIteratorNamedNode : MatchIterator
    {
        private string _name;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.MatchIteratorRoot"/> class
        /// </summary>
        /// <param name="parent">parent match iterator</param>
        public MatchIteratorNamedNode (MatchIterator parent, string name)
            : base (parent)
        {
            if (name.StartsWith ("\\"))
                name = name.Substring (1);
            _name = name;
        }

        public override IEnumerable<Node> Nodes {
            get {
                foreach (Node idxCurrent in Parent.Nodes) {
                    foreach (Node idxChild in idxCurrent.Children) {
                        if (idxChild.Name == _name)
                            yield return idxChild;
                    }
                }
            }
        }
    }
}

