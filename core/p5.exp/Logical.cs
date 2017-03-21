/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.Linq;
using System.Collections.Generic;
using p5.core;
using p5.exp.iterators;

namespace p5.exp
{
    /// <summary>
    ///     Class encapsulating Boolean algebraic operations for the Expression class
    /// </summary>
    [Serializable]
    public class Logical
    {
        /// <summary>
        ///     Type of boolean operator
        /// </summary>
        public enum LogicalType
        {
            /// <summary>
            ///     OR operator, for ORing results together
            /// </summary>
            Or,

            /// <summary>
            ///     AND operator, for ANDing results together
            /// </summary>
            And,

            /// <summary>
            ///     XOR operator, for XORing results together
            /// </summary>
            Xor,

            /// <summary>
            ///     NOT operator, for NOTing results together
            /// </summary>
            Not
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Logical" /> class
        /// </summary>
        /// <param name="type">Type of logical, Or, And, Xor or Not</param>
        public Logical (LogicalType type)
        {
            TypeOfLogical = type;
        }

        /// <summary>
        ///     Returns the last <see cref="Iterator" /> in the list of iterators belonging to this logical
        /// </summary>
        /// <value>The last iterator in the chain of iterators</value>
        public Iterator Iterator
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the type of logical
        /// </summary>
        /// <value>The type of logical</value>
        LogicalType TypeOfLogical
        {
            get;
            set;
        }

        /// <summary>
        ///     Adds an iterator to the current logical group
        /// </summary>
        /// <param name="iterator">Tterator to append to chain of iterators</param>
        public void AddIterator (Iterator iterator)
        {
            iterator.Left = Iterator;
            Iterator = iterator;
        }

        internal List<Node> EvaluateNodes (List<Node> nodes, ApplicationContext context)
        {
            var rhs = new List<Node> (Iterator.Evaluate (context));
            var retVal = new List<Node> ();
            switch (TypeOfLogical) {
                case LogicalType.Or:
                    retVal.AddRange (nodes);
                    foreach (var idx in rhs.Where (idx => !retVal.Contains (idx))) {
                        retVal.Add (idx);
                    }
                    break;
                case LogicalType.And:
                    retVal.AddRange (nodes.FindAll (
                        idx => rhs.Contains (idx)));
                    break;
                case LogicalType.Xor:
                    retVal.AddRange (nodes.FindAll (
                        idx => !rhs.Contains (idx)));
                    retVal.AddRange (rhs.FindAll (
                        idx => !nodes.Contains (idx)));
                    break;
                case LogicalType.Not:
                    retVal.AddRange (nodes.FindAll (
                        idx => !rhs.Contains (idx)));
                    break;
            }
            return retVal;
        }
    }
}
