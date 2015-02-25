/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     returns all nodes found through value of previous node's matched converted to path or node
    /// </summary>
    public class IteratorReference : Iterator
    {
        public override IEnumerable<Node> Evaluate
        {
            get
            {
                foreach (var idxCurrent in Left.Evaluate) {
                    Node reference = null;
                    // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                    if (idxCurrent.Value is Node.Dna) {
                        reference = idxCurrent.Find ((Node.Dna) idxCurrent.Value);
                    } else {
                        var node = idxCurrent.Value as Node;
                        if (node != null) {
                            reference = node;
                        } else if (Node.Dna.IsPath (idxCurrent.Value as string)) {
                            var path = new Node.Dna ((string) idxCurrent.Value);
                            reference = idxCurrent.Find (path);
                        }
                    }
                    if (reference != null)
                        yield return reference;
                }
            }
        }
    }
}