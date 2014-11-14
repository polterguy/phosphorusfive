/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute.iterators
{
    public abstract class Iterator
    {
        private Iterator _left;

        protected Iterator ()
        { }

        public Iterator Left {
            get {
                return _left;
            }
            set {
                _left = value;
            }
        }

        public abstract IEnumerable<Node> Evaluate {
            get;
        }
    }
}

