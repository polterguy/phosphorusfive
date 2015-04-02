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
    public class IteratorNamedAncestor : Iterator
    {
        private readonly string _name;

        public IteratorNamedAncestor (string name)
        {
            _name = name;
        }

        public override IEnumerable<Node> Evaluate
        {
            get
            {
                foreach (var idxAncestor in Left.Evaluate.Select (ix => ix.Parent)) {
                    var curAncestor = idxAncestor;
                    while (curAncestor != null) {
                        if (curAncestor.Name == _name) {
                            yield return curAncestor;
                            break;
                        }
                        curAncestor = curAncestor.Parent;
                    }
                }
            }
        }
    }
}