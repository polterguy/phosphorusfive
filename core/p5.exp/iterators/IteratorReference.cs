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
    ///     Returns all nodes found through value of previous node's matched converted to node
    /// </summary>
    [Serializable]
    public class IteratorReference : Iterator
    {
        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            foreach (var idxCurrent in Left.Evaluate (context)) {
                var retVal = idxCurrent.Get<Node> (context, null);
                if (retVal != null)
                    yield return retVal;
            }
        }
    }
}
