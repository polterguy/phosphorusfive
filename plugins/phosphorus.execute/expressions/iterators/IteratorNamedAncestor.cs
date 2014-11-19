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
    /// returns all parent <see cref="phosphorus.core.Node"/>s of previous iterator result
    /// </summary>
    public class IteratorNamedAncestor : Iterator
    {
        private string _name;

        public IteratorNamedAncestor (string name)
        {
            _name = name;
        }

        public override IEnumerable<Node> Evaluate {
            get {
                foreach (Node idxCurrent in Left.Evaluate) {
                    Node idxAncestor = idxCurrent.Parent;
                    while (true) {
                        if (idxAncestor.Name == _name) {
                            yield return idxAncestor;
                            break;
                        }
                        idxAncestor = idxAncestor.Parent;
                        if (idxAncestor == null)
                            break;
                    }
                }
            }
        }
    }
}

