/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Linq;
using pf.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     Returns all parent <see cref="phosphorus.core.Node" />s of previous iterator result.
    /// 
    ///     Example;
    ///     <pre>/.</pre>
    /// </summary>
    public class IteratorParent : Iterator
    {
        public override IEnumerable<Node> Evaluate
        {
            get { return from idxCurrent in Left.Evaluate where idxCurrent.Parent != null select idxCurrent.Parent; }
        }
    }
}