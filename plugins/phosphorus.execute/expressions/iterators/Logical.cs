/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute.iterators
{
    public class Logical
    {
        public enum LogicalType
        {
            OR,
            AND,
            XOR,
            NOT
        }

        private LogicalType _type;
        private Iterator _iterator;

        public Logical (LogicalType type)
        {
            _type = type;
        }

        public void AddIterator (Iterator iterator)
        {
            iterator.Left = _iterator;
            _iterator = iterator;
        }

        public Iterator Iterator {
            get {
                return _iterator;
            }
        }

        public LogicalType TypeOfLogical {
            get {
                return _type;
            }
        }

        public List<Node> EvaluateNodes (List<Node> nodes)
        {
            List<Node> rhs = new List<Node> (_iterator.Evaluate);
            List<Node> retVal = new List<Node> ();
            switch (_type) {
            case LogicalType.OR:
                retVal.AddRange (nodes);
                foreach (Node idx in rhs) {
                    if (!retVal.Contains (idx))
                        retVal.Add (idx);
                }
                break;
            case LogicalType.AND:
                retVal.AddRange (nodes.FindAll (
                    delegate (Node idx) {
                    return rhs.Contains (idx);
                }));
                break;
            case LogicalType.XOR:
                retVal.AddRange (nodes.FindAll (
                    delegate (Node idx) {
                    return !rhs.Contains (idx);
                }));
                retVal.AddRange (rhs.FindAll (
                    delegate (Node idx) {
                    return !nodes.Contains (idx);
                }));
                break;
            case LogicalType.NOT:
                retVal.AddRange (nodes.FindAll (
                    delegate (Node idx) {
                    return !rhs.Contains (idx);
                }));
                break;
            }
            return retVal;
        }
    }
}

