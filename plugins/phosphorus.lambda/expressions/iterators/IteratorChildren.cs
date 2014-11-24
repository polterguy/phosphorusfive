/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute.iterators
{
    /// <summary>
    /// returns all children of previous iterator
    /// </summary>
    public class IteratorChildren : Iterator
    {
        public override IEnumerable<Node> Evaluate {
            get {
                foreach (Node idxCurrent in Left.Evaluate) {
                    foreach (Node idxChild in idxCurrent.Children) {
                        yield return idxChild;
                    }
                }
            }
        }
    }
}

