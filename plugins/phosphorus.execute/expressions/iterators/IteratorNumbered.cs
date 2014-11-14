/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute.iterators
{
    public class IteratorNumbered : Iterator
    {
        private int _number;

        public IteratorNumbered (int number)
        {
            _number = number;
        }

        public override IEnumerable<Node> Evaluate {
            get {
                foreach (Node idxCurrent in Left.Evaluate) {
                    if (idxCurrent.Count > _number) {
                        yield return idxCurrent [_number];
                    }
                }
            }
        }
    }
}

