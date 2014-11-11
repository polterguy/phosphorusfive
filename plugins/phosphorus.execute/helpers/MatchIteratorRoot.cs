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
    /// <see cref="phosphorus.execute.MatchIterator"/> class for returning root <see cref="phosphorus.core.Node"/> of tree hierarchy
    /// </summary>
    public class MatchIteratorRoot : MatchIterator
    {
        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.MatchIteratorRoot"/> class
        /// </summary>
        /// <param name="parent">parent match iterator</param>
        public MatchIteratorRoot (MatchIterator parent)
            : base (parent)
        { }

        public override IEnumerable<Node> Nodes {
            get {
                IEnumerator<Node> enumerator = Parent.Nodes.GetEnumerator () as IEnumerator<Node>;
                enumerator.MoveNext ();
                yield return enumerator.Current.Root;
            }
        }
    }
}

