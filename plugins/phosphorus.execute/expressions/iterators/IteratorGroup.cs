/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute.iterators
{
    public class IteratorGroup : Iterator
    {
        private List<Logical> _logicals = new List<Logical> ();

        public IteratorGroup (Node node)
        {
            AddLogical (new Logical (Logical.LogicalType.OR));
            AddIterator (new IteratorNode (new Node[] { node }));
        }

        public IteratorGroup (IteratorGroup parent)
        {
            AddLogical (new Logical (Logical.LogicalType.OR));
            AddIterator (new IteratorLeftParent (parent.LastIterator));
            ParentGroup = parent;
            ParentGroup.AddIterator (this);
        }

        public IteratorGroup ParentGroup {
            get;
            private set;
        }

        public Iterator LastIterator {
            get {
                return _logicals [_logicals.Count - 1].Iterator;
            }
        }

        public void AddLogical (Logical logical)
        {
            _logicals.Add (logical);
            if (_logicals.Count > 1) {
                Iterator leftMost = _logicals [0].Iterator;
                while (leftMost.Left != null)
                    leftMost = leftMost.Left;
                _logicals [_logicals.Count - 1].AddIterator (leftMost);
            }
        }

        public void AddIterator (Iterator iterator)
        {
            _logicals [_logicals.Count - 1].AddIterator (iterator);
        }

        public override IEnumerable<Node> Evaluate {
            get {
                List<Node> retVal = new List<Node> ();
                foreach (Logical idxLogical in _logicals) {
                    retVal = idxLogical.EvaluateNodes (retVal);
                }
                return retVal;
            }
        }
    }
}

