/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    public class IteratorGroup : Iterator
    {
        private readonly Iterator _groupRoot;
        private readonly List<Logical> _logicals = new List<Logical> ();
        private List<Node> _cachedEvaluatedNodes;

        internal IteratorGroup (Node node)
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

        public IteratorGroup ParentGroup { get; private set; }

        private Iterator LastIterator
        {
            get { return _logicals [_logicals.Count - 1].Iterator; }
        }

        public override bool IsReference
        {
            get { return _groupRoot.IsReference; }
            set { _groupRoot.IsReference = value; }
        }
        
        public void AddLogical (Logical logical)
        {
            _logicals.Add (logical);

            // making sure "group root iterator" is root iterator for all Logicals
            _logicals [_logicals.Count - 1].AddIterator (_groupRoot);
        }

        public void AddIterator (Iterator iterator)
        {
            _logicals [_logicals.Count - 1].AddIterator (iterator);
        }

        public override IEnumerable<Node> Evaluate
        {
            get
            {
                if (_cachedEvaluatedNodes != null)
                    return _cachedEvaluatedNodes;

                _cachedEvaluatedNodes = new List<Node> ();
                foreach (var idxLogical in _logicals) {
                    _cachedEvaluatedNodes = idxLogical.EvaluateNodes (_cachedEvaluatedNodes);
                }
                return _cachedEvaluatedNodes;
            }
        }
    }
}