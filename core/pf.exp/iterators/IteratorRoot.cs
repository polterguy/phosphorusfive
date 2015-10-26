/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using pf.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     Returns the "root" <see cref="phosphorus.core.Node" /> of your tree-structure.
    /// 
    ///     Will return the root node of your tree.
    /// 
    ///     Example;
    ///     <pre>/..</pre>
    /// </summary>
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