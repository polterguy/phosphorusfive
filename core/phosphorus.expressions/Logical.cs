/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System.Linq;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions.iterators;

namespace phosphorus.expressions
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

        public Logical (LogicalType type)
        {
            TypeOfLogical = type;
        }

        public Iterator Iterator { get; private set; }

        private LogicalType TypeOfLogical { get; set; }

        public void AddIterator (Iterator iterator)
        {
            iterator.Left = Iterator;
            Iterator = iterator;
        }

        internal List<Node> EvaluateNodes (List<Node> nodes)
        {
            var rhs = new List<Node> (Iterator.Evaluate);
            var retVal = new List<Node> ();
            switch (TypeOfLogical) {

                case LogicalType.OR:
                    retVal.AddRange (nodes);
                    foreach (var idx in rhs.Where (idx => !retVal.Contains (idx))) {
                        retVal.Add (idx);
                    }
                    break;

                case LogicalType.AND:
                    retVal.AddRange (nodes.FindAll (idx => rhs.Contains (idx)));
                    break;

                case LogicalType.XOR:
                    retVal.AddRange (nodes.FindAll (idx => !rhs.Contains (idx)));
                    retVal.AddRange (rhs.FindAll (idx => !nodes.Contains (idx)));
                    break;

                case LogicalType.NOT:
                    retVal.AddRange (nodes.FindAll (idx => !rhs.Contains (idx)));
                    break;
            }
            return retVal;
        }
    }
}
