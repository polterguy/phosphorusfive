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
    ///     Returns all children of previous iterator.
    /// 
    ///     Will return all Children nodes of the results of the previous Iterator.
    /// 
    ///     Example;
    ///     <pre>/*</pre>
    /// </summary>
    public class IteratorChildren : Iterator
    {
        public override IEnumerable<Node> Evaluate
        {
            get { return Left.Evaluate.SelectMany (idxCurrent => idxCurrent.Children); }
        }
    }
}