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
    ///     Returns all nodes, and descendants, of previous iterator.
    /// 
    ///     This Iterator will return ALL nodes from its previous result, including all children, children's children, and 
    ///     so on, from its previous iterator.
    /// 
    ///     Warning! This might be a very, very large result set for large node trees!
    /// 
    ///     Example;
    ///     <pre>/**</pre>
    /// </summary>
    [Serializable]
    public class IteratorFlatten : Iterator
    {
        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            foreach (var idxCurrent in Left.Evaluate (context)) {
                var stop = idxCurrent.NextSibling;
                for (var idxNext = idxCurrent; idxNext != null && !idxNext.Equals (stop); idxNext = idxNext.NextNode) {
                    yield return idxNext;
                }
            }
        }
    }
}
