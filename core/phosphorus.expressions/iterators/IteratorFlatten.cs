/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    public class IteratorFlatten : Iterator
    {
        public override IEnumerable<Node> Evaluate
        {
            get
            {
                foreach (var idxCurrent in Left.Evaluate) {
                    foreach (var idxChild in ReturnDescendants (idxCurrent)) {
                        yield return idxChild;
                    }
                }
            }
        }

        private static IEnumerable<Node> ReturnDescendants (Node idx)
        {
            foreach (var idxChild in idx.Children) {
                yield return idxChild;
                foreach (var idxGrandChild in ReturnDescendants (idxChild)) {
                    yield return idxGrandChild;
                }
            }
        }
    }
}