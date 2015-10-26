/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using pf.core;

// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull

namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     Returns all nodes found through value of previous node's matched converted to path or node.
    /// 
    ///     Example;
    ///     <pre>/#</pre>
    /// </summary>
    public class IteratorReference : Iterator
    {
        private ApplicationContext _context;

        public IteratorReference (ApplicationContext context)
        {
            _context = context;
        }

        public override IEnumerable<Node> Evaluate
        {
            get
            {
                foreach (var idxCurrent in Left.Evaluate) {
                    if (idxCurrent.Value is Node)
                        yield return idxCurrent.Get<Node> (_context);
                }
            }
        }
    }
}