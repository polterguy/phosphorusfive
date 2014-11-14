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

        public IEnumerable<Node> Evaluate {
            get {
                return _iterator.Evaluate;
            }
        }
    }
}

