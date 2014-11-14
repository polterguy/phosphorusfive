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

        public IteratorGroup (Node node, IteratorGroup parent)
        {
            AddLogical (node, new Logical (Logical.LogicalType.OR));
            ParentGroup = parent;
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

        public void AddLogical (Node node, Logical logical)
        {
            _logicals.Add (logical);
            AddIterator (new IteratorNode (node)); 
        }

        public void AddIterator (Iterator iterator)
        {
            _logicals [_logicals.Count - 1].AddIterator (iterator);
        }

        public override IEnumerable<Node> Evaluate {
            get {
                List<Node> retVal = new List<Node> ();
                foreach (Logical idxLogical in _logicals) {
                    switch (idxLogical.TypeOfLogical) {
                    case Logical.LogicalType.AND:
                        retVal = new List<Node> (new List<Node> (idxLogical.Evaluate).FindAll (
                            delegate(Node idxNode) {
                            return retVal.Contains (idxNode);
                        }));
                        break;
                    case Logical.LogicalType.OR:
                        retVal.AddRange (new List<Node> (idxLogical.Evaluate).FindAll (
                            delegate (Node idxNode) {
                            return !retVal.Contains (idxNode);
                        }));
                        break;
                    case Logical.LogicalType.XOR:
                        List<Node> tmpRhs = new List<Node> (idxLogical.Evaluate);
                        List<Node> tmpLhs = new List<Node> (retVal.FindAll (
                            delegate (Node idxNode) {
                            return !tmpRhs.Contains (idxNode);
                        }));
                        tmpLhs.AddRange (tmpRhs.FindAll (
                            delegate (Node idxNode) {
                            return !retVal.Contains (idxNode);
                        }));
                        retVal = tmpLhs;
                        break;
                    case Logical.LogicalType.NOT:
                        List<Node> tmpToRemove = new List<Node> (idxLogical.Evaluate);
                        retVal.RemoveAll (delegate (Node idxNode) {
                            return tmpToRemove.Contains (idxNode);
                        });
                        break;
                    }
                }
                return retVal;
            }
        }
    }
}

