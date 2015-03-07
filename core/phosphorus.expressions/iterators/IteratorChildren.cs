/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Linq;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     Returns all children of previous iterator.
    /// 
    ///     Will return all Children nodes of the results of the previous Iterator.
    /// </summary>
    public class IteratorChildren : Iterator
    {
        public override IEnumerable<Node> Evaluate
        {
            get { return Left.Evaluate.SelectMany (idxCurrent => idxCurrent.Children); }
        }
    }
}