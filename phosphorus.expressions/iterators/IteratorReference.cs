
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    /// returns all nodes found through value of previous node's matched converted to path or node
    /// </summary>
    public class IteratorReference : Iterator
    {
        public override IEnumerable<Node> Evaluate {
            get {
                foreach (Node idxCurrent in Left.Evaluate) {
                    Node reference = null;
                    if (idxCurrent.Value is Node.DNA) {
                        reference = idxCurrent.Find (idxCurrent.Value as Node.DNA);
                    } else if (idxCurrent.Value is Node) {
                        reference = idxCurrent.Value as Node;
                    } else if (Node.DNA.IsPath (idxCurrent.Value as string)) {
                        Node.DNA dna = new Node.DNA (idxCurrent.Value as string);
                        reference = idxCurrent.Find (dna);
                    }
                    if (reference != null)
                        yield return reference;
                }
            }
        }
    }
}
