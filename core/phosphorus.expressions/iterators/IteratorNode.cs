/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     <see cref="phosphorus.core.Node" /> Iterator.
    /// 
    ///     This Iterator is never used directly by your code, but implicitly given through your expressions, and normally
    ///     points to the "identity Node", which is the <see cref="phosphorus.core.Node">Node</see> where your Expression is
    ///     declared.
    /// </summary>
    public class IteratorNode : Iterator
    {
        private readonly Node _node;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorNode" /> class.
        /// </summary>
        /// <param name="node">the node to start iterating upon</param>
        public IteratorNode (Node node) { _node = node; }

        public override IEnumerable<Node> Evaluate
        {
            get { yield return _node; }
        }
    }
}