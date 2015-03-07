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
    ///     Returns the "next node".
    /// 
    ///     To understand how this method works, see the documentation for <see cref="phosphorus.core.NextNode"/>, since
    ///     it basically is an implementation of an Iterator doing exactly what that method does.
    /// 
    ///     Example;
    ///     <pre>/&gt;</pre>
    /// </summary>
    public class IteratorShiftRight : Iterator
    {
        public override IEnumerable<Node> Evaluate
        {
            get { return Left.Evaluate.Select (idxCurrent => idxCurrent.NextNode).Where (next => next != null); }
        }
    }
}