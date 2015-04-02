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
    public class IteratorNamed : Iterator
    {
        private readonly string _name;

        public IteratorNamed (string name)
        {
            if (name.StartsWith ("\\"))
                name = name.Substring (1);
            _name = name;
        }

        public override IEnumerable<Node> Evaluate
        {
            get { return Left.Evaluate.Where (ix => ix.Name == _name); }
        }
    }
}