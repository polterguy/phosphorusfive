/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System.Linq;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    public class IteratorShiftRight : Iterator
    {
        public override IEnumerable<Node> Evaluate
        {
            get { return Left.Evaluate.Select (ix => ix.NextNode).Where (ix => ix != null); }
        }
    }
}