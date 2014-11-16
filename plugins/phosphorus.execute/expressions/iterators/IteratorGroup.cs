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
        private Iterator _groupRoot;
        private List<Node> _cachedEvaluatedNodes;

        public IteratorGroup (Node node)
        {
            _groupRoot = new IteratorNode (node);
            AddLogical (new Logical (Logical.LogicalType.OR));
        }

        public IteratorGroup (IteratorGroup parent)
        {
            _groupRoot = new IteratorLeftParent (parent.LastIterator);
            AddLogical (new Logical (Logical.LogicalType.OR));
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

            // making sure "group root iterator" is root iterator of all Logicals
            _logicals [_logicals.Count - 1].AddIterator (_groupRoot);
        }

        public void AddIterator (Iterator iterator)
        {
            _logicals [_logicals.Count - 1].AddIterator (iterator);
        }

        public override IEnumerable<Node> Evaluate {
            get {
                if (_cachedEvaluatedNodes == null) {
                    _cachedEvaluatedNodes = new List<Node> ();
                    foreach (Logical idxLogical in _logicals) {
                        _cachedEvaluatedNodes = idxLogical.EvaluateNodes (_cachedEvaluatedNodes);
                    }
                }
                return _cachedEvaluatedNodes;
            }
        }
    }
}

