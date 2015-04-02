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
    public class IteratorValued : Iterator
    {
        private readonly string _value;

        public IteratorValued (string value)
        {
            _value = value;
        }

        public override IEnumerable<Node> Evaluate
        {
            get
            {
                return Left.Evaluate.Where (ix => _value.Equals (ix.Value));
            }
        }
    }
}