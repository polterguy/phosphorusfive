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
    public class IteratorModulo : Iterator
    {
        private readonly int _modulo;

        public IteratorModulo (int modulo)
        {
            _modulo = modulo;
        }

        public override IEnumerable<Node> Evaluate
        {
            get
            {
                var idxNo = 0;
                return Left.Evaluate.Where (ix => idxNo++%_modulo == 0);
            }
        }
    }
}