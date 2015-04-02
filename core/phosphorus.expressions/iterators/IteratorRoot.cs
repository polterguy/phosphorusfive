/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    public class IteratorRoot : Iterator
    {
        public override IEnumerable<Node> Evaluate
        {
            get
            {
                var enumerator = Left.Evaluate.GetEnumerator ();
                if (enumerator.MoveNext ())
                    yield return enumerator.Current.Root;
            }
        }
    }
}