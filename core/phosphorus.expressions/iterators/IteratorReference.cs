/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    public class IteratorReference : Iterator
    {
        public override IEnumerable<Node> Evaluate
        {
            get
            {
                foreach (var idxCurrent in Left.Evaluate) {
                    var idxNode = idxCurrent.Value as Node;
                    if (idxNode != null)
                        yield return idxNode;
                }
            }
        }
    }
}