/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Returns the "root" <see cref="phosphorus.core.Node" /> of your tree-structure.
    /// 
    ///     Will return the root node of your tree.
    /// 
    ///     Example;
    ///     <pre>/..</pre>
    /// </summary>
    [Serializable]
    public class IteratorRoot : Iterator
    {
        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            var enumerator = Left.Evaluate (context).GetEnumerator ();
            if (enumerator.MoveNext ())
                yield return enumerator.Current.Root;
        }
    }
}
