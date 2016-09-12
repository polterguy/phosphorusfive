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
    ///     Returns all nodes with a distinct value
    /// </summary>
    [Serializable]
    public class IteratorDistinctValue : Iterator
    {
        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            var seen = new Dictionary<object, bool> ();
            foreach (var idxRetVal in Left.Evaluate (context)) {
                if (!seen.ContainsKey (idxRetVal.Value)) {
                    seen [idxRetVal.Value] = true;
                    yield return idxRetVal;
                }
            }
        }
    }
}
