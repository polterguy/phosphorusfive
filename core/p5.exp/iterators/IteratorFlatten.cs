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
    ///     Returns all nodes, and descendants, of previous iterator
    /// </summary>
    [Serializable]
    public class IteratorFlatten : Iterator
    {
        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            foreach (var idxCurrent in Left.Evaluate (context)) {

                // If next sibling is not null, then that is our "stop node"
                var stop = idxCurrent.NextSibling;

                // Looping until stop not equals null, or we have no more nodes to iterate, because we're at root
                var curIdx = idxCurrent.Parent;
                while (stop == null && curIdx != null) {
                    stop = curIdx.NextSibling;
                    curIdx = curIdx.Parent;
                }

                // At this point, stop is either node where to stop iterator, or null, at which point we iterate until null is reached
                for (var idxNext = idxCurrent.NextNode; idxNext != null && !idxNext.Equals (stop); idxNext = idxNext.NextNode) {
                    yield return idxNext;
                }
            }
        }
    }
}
