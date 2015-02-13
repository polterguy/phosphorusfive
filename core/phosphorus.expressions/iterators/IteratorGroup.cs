
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    /// special iterator for grouping iterators.  an IteratorGroup will either iterate on the result of its
    /// parent group, or a single node. by grouping iterators, you can have multiple iterators grouped together,
    /// working with the evaluatedd result of its parent group iterator
    /// </summary>
    public class IteratorGroup : Iterator
    {
        private List<Logical> _logicals = new List<Logical> ();
        private Iterator _groupRoot;
        private List<Node> _cachedEvaluatedNodes;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.iterators.IteratorGroup"/> class.
        /// this constructor is for creating the "outer most iterator" or "root iterator" of your hyperlisp expressions
        /// </summary>
        /// <param name="node">node to iterate upon</param>
        public IteratorGroup (Node node)
        {
            _groupRoot = new IteratorNode (node);
            AddLogical (new Logical (Logical.LogicalType.OR));
        }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.iterators.IteratorGroup"/> class.
        /// this constructor is for creating child groups
        /// </summary>
        /// <param name="parent">parent iterator group</param>
        public IteratorGroup (IteratorGroup parent)
        {
            _groupRoot = new IteratorLeftParent (parent.LastIterator);
            AddLogical (new Logical (Logical.LogicalType.OR));
            ParentGroup = parent;
            ParentGroup.AddIterator (this);
        }

        /// <summary>
        /// gets the parent group
        /// </summary>
        /// <value>the parent group</value>
        public IteratorGroup ParentGroup {
            get;
            private set;
        }

        /// <summary>
        /// gets the last iterator in the group
        /// </summary>
        /// <value>the last iterator</value>
        public Iterator LastIterator {
            get {
                return _logicals [_logicals.Count - 1].Iterator;
            }
        }

        // TODO: Rename to "IsReference"
        /// <summary>
        /// returns true if this is a "reference expression"
        /// </summary>
        /// <value>true if this is a reference expression</value>
        public override bool IsReference {
            get {
                return _groupRoot.IsReference;
            }
            set {
                _groupRoot.IsReference = value;
            }
        }

        /// <summary>
        /// adds a <see cref="phosphorus.execute.iterators.Logical"/> to the list of logicals in the group for performing
        /// boolean algebraic operations on node sets
        /// </summary>
        /// <param name="logical">logical</param>
        public void AddLogical (Logical logical)
        {
            _logicals.Add (logical);

            // making sure "group root iterator" is root iterator of all Logicals
            _logicals [_logicals.Count - 1].AddIterator (_groupRoot);
        }

        /// <summary>
        /// appends a new iterator to the last <see cref="phosphorus.execute.iterators.Logical"/> in the group
        /// </summary>
        /// <param name="iterator">Iterator.</param>
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
