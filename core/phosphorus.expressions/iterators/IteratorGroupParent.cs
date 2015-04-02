/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    public class IteratorGroupParent : Iterator
    {
        private readonly Iterator _leftParent;

        public IteratorGroupParent (Iterator leftParent)
        {
            _leftParent = leftParent;
        }

        public override IEnumerable<Node> Evaluate
        {
            get { return _leftParent.Evaluate; }
        }
    }
}