
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

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
                foreach (var idxCurrent in Left.Evaluate) {
                    Node reference = null;
                    if (idxCurrent.Value is Node.DNA) {
                        reference = idxCurrent.Find ((Node.DNA) idxCurrent.Value);
                    } else if (idxCurrent.Value is Node) {
                        reference = (Node) idxCurrent.Value;
                    } else if (Node.DNA.IsPath (idxCurrent.Value as string)) {
                        var dna = new Node.DNA ((string) idxCurrent.Value);
                        reference = idxCurrent.Find (dna);
                    }
                    if (reference != null)
                        yield return reference;
                }
            }
        }
    }
}
