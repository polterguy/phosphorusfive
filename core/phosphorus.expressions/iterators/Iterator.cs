/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    public abstract class Iterator
    {
        public Iterator Left { get; set; }

        public abstract IEnumerable<Node> Evaluate { get; }

        public virtual bool IsReference { get; set; }
    }
}
