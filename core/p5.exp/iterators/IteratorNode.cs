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
    ///     <see cref="phosphorus.core.Node" /> Iterator.
    /// 
    ///     This Iterator is never used directly by your code, but implicitly given through your expressions, and normally
    ///     points to the "identity Node", which is the <see cref="phosphorus.core.Node">Node</see> where your Expression is
    ///     declared.
    /// </summary>
    [Serializable]
    public class IteratorNode : Iterator
    {
        private Node _node;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorNode" /> class.
        /// </summary>
        /// <param name="node">the node to start iterating upon</param>
        public IteratorNode (Node node)
        {
            _node = node;
        }

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
