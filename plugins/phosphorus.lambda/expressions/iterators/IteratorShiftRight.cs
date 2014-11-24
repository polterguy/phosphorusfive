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
    /// returns an offset sibling <see cref="phosphorus.core.Node"/>
    /// </summary>
    public class IteratorShiftRight : Iterator
    {
        public override IEnumerable<Node> Evaluate {
            get {
                foreach (Node idxCurrent in Left.Evaluate) {
                    Node next = idxCurrent.NextNode;
                    if (next != null)
                        yield return next;
                }
            }
        }
    }
}

