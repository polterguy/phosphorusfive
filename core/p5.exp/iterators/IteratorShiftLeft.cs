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
    ///     Returns the "previous node"
    /// </summary>
    [Serializable]
    public class IteratorShiftLeft : Iterator
    {
        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            foreach (var idxCurrent in Left.Evaluate (context)) {
                var previous = idxCurrent.PreviousNode;
                if (previous != null) {
                    yield return previous;
                } else {
                    var idxNode = idxCurrent.Root;
                    while (idxNode.Children.Count > 0) {
                        idxNode = idxNode.Children [idxNode.Children.Count - 1];
                    }
                    yield return idxNode;
                }
            }
        }
    }
}
