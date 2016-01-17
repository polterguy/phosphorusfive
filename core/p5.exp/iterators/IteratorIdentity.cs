/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     IdentityIterator, returning current <see cref="phosphorus.core.Node" />
    /// </summary>
    [Serializable]
    public class IteratorIdentity : Iterator
    {
        private Node _node;

        internal Node RootNode {
            get { return _node; }
            set { _node = value; }
        }

        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            yield return _node;
        }
    }
}
