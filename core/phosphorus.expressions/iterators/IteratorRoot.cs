/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using phosphorus.core;

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