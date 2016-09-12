/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Returns the "next node"
    /// </summary>
    [Serializable]
    public class IteratorShiftRight : Iterator
    {
        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            foreach (var idxCurrent in Left.Evaluate (context)) {
                var next = idxCurrent.NextNode;
                if (next != null) {
                    yield return next;
                } else {
                    yield return idxCurrent.Root;
                }
            }
        }
    }
}
