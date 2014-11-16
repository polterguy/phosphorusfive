/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute.iterators
{
    public class IteratorLeftParent : Iterator
    {
        private Iterator _leftParent;

        public IteratorLeftParent (Iterator leftParent)
        {
            _leftParent = leftParent;
        }

        public override IEnumerable<Node> Evaluate {
            get {
                return _leftParent.Evaluate;
            }
        }
    }
}

