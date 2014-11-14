/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute.iterators
{
    public class IteratorNamed : Iterator
    {
        private string _name;

        public IteratorNamed (string name)
        {
            if (name.StartsWith ("\\"))
                name = name.Substring (1);
            _name = name;
        }

        public override IEnumerable<Node> Evaluate {
            get {
                foreach (Node idxCurrent in Left.Evaluate) {
                    if (idxCurrent.Name == _name)
                        yield return idxCurrent;
                }
            }
        }
    }
}

