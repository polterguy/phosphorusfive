
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Linq;
using phosphorus.core;
using phosphorus.expressions.iterators;

namespace phosphorus.expressions
{
    /// <summary>
    /// logical class, for performing boolean algebra on <see cref="phosphorus.expressions.iterators.Iterator"/>, making it
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
            Or,

            /// <summary>
            /// AND operator, for ANDing results together
            /// </summary>
            And,

            /// <summary>
            /// XOR operator, for eXclusively ORing results together
            /// </summary>
            Xor,

            /// <summary>
            /// NOT operator, for return all from previous results, except those in next result
            /// </summary>
            Not
        }

        private readonly LogicalType _type;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.expressions.Logical"/> class
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
            iterator.Left = Iterator;
            Iterator = iterator;
        }

        /// <summary>
        /// returns the last <see cref="phosphorus.expressions.iterators.Iterator"/> in the list of iterators belonging to this logical
        /// </summary>
        /// <value>the last iterator in the stack of iterators</value>
        public Iterator Iterator {
            get;
            private set;
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
            var rhs = new List<Node> (Iterator.Evaluate);
            var retVal = new List<Node> ();
            switch (_type) {
            case LogicalType.Or:
                retVal.AddRange (nodes);
                foreach (var idx in rhs.Where (idx => !retVal.Contains (idx))) {
                    retVal.Add (idx);
                }
                break;
            case LogicalType.And:
                retVal.AddRange (nodes.FindAll (
                    idx => rhs.Contains(idx)));
                break;
            case LogicalType.Xor:
                retVal.AddRange (nodes.FindAll (
                    idx => !rhs.Contains(idx)));
                retVal.AddRange (rhs.FindAll (
                    idx => !nodes.Contains(idx)));
                break;
            case LogicalType.Not:
                retVal.AddRange (nodes.FindAll (
                    idx => !rhs.Contains(idx)));
                break;
            }
            return retVal;
        }
    }
}
