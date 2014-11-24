/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.lambda.iterators;

namespace phosphorus.lambda
{
    /// <summary>
    /// logical class, for performing boolean algebra on <see cref="phosphorus.execute.iterators.Iterator"/>, making it
    /// possible to use all four main boolean operators on <see cref="phosphorus.core.Node"/> sets
    /// </summary>
    public class Logical
    {
        /// <summary>
        /// type of boolean operator
        /// </summary>
        public enum LogicalType
        {
            /// <summary>
            /// OR operator, for ORing results together
            /// </summary>
            OR,

            /// <summary>
            /// AND operator, for ANDing results together
            /// </summary>
            AND,

            /// <summary>
            /// XOR operator, for eXclusively ORing results together
            /// </summary>
            XOR,

            /// <summary>
            /// NOT operator, for return all from previous results, except those in next result
            /// </summary>
            NOT
        }

        private LogicalType _type;
        private Iterator _iterator;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.iterators.Logical"/> class
        /// </summary>
        /// <param name="type">type of logical, OR, AND, XOR or NOT</param>
        public Logical (LogicalType type)
        {
            _type = type;
        }

        /// <summary>
        /// adds an iterator to the logical boolean operator
        /// </summary>
        /// <param name="iterator">iterator to append</param>
        public void AddIterator (Iterator iterator)
        {
            iterator.Left = _iterator;
            _iterator = iterator;
        }

        /// <summary>
        /// returns the last <see cref="phosphorus.execute.iterators.Iterator"/> in the list of iterators belonging to this logical
        /// </summary>
        /// <value>the last iterator in the stack of iterators</value>
        public Iterator Iterator {
            get {
                return _iterator;
            }
        }

        /// <summary>
        /// gets the type of logical
        /// </summary>
        /// <value>the type of logical</value>
        public LogicalType TypeOfLogical {
            get {
                return _type;
            }
        }

        internal List<Node> EvaluateNodes (List<Node> nodes)
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

