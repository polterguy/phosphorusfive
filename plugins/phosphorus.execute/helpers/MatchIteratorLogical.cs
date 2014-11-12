/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute
{
    /// <summary>
    /// <see cref="phosphorus.execute.MatchIterator"/> class for returning all children <see cref="phosphorus.core.Node"/>s of tree hierarchy
    /// </summary>
    public class MatchIteratorLogical : MatchIteratorStart
    {
        /// <summary>
        /// type of boolean logical operator
        /// </summary>
        public enum LogicalType
        {
            OR,
            AND,
            XOR,
            NOT
        }

        private LogicalType _type;
        MatchIterator _lhs;
        MatchIterator _rhs;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.MatchIteratorLogical"/> class
        /// </summary>
        /// <param name="parent">parent match iterator</param>
        public MatchIteratorLogical (Node node, MatchIterator lhs, MatchIterator rhs, LogicalType type)
            : base (node)
        {
            _type = type;
            _lhs = lhs;
            _rhs = rhs;
        }

        public override IEnumerable<Node> Nodes {
            get {
                List<Node> lhs = new List<Node> (_lhs.Nodes);
                List<Node> rhs = new List<Node> (_rhs.Nodes);
                switch (_type) {
                    case LogicalType.AND:
                        foreach (Node idx in lhs) {
                            if (rhs.Contains (idx))
                                yield return idx;
                        }
                        break;
                    case LogicalType.OR:
                        foreach (Node idx in lhs) {
                            yield return idx;
                        }
                        foreach (Node idx in rhs) {
                            if (!lhs.Contains (idx))
                                yield return idx;
                        }
                        break;
                    case LogicalType.NOT:
                        foreach (Node idx in lhs) {
                            if (!rhs.Contains (idx))
                                yield return idx;
                        }
                        break;
                    case LogicalType.XOR:
                        foreach (Node idx in lhs) {
                            if (!rhs.Contains (idx))
                                yield return idx;
                        }
                        foreach (Node idx in rhs) {
                            if (!lhs.Contains (idx))
                                yield return idx;
                        }
                        break;
                }
            }
        }
    }
}

