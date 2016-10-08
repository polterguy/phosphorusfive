/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
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

using System.Collections;
using System.Collections.Generic;
using p5.core;
using p5.exp.matchentities;

namespace p5.exp
{
    /// <summary>
    ///     Expression result class
    /// </summary>
    public class Match : IEnumerable<MatchEntity>
    {
        /// <summary>
        ///     Type of match for your match object
        /// </summary>
        public enum MatchType
        {
            
            /// <summary>
            ///     Returns <see cref="phosphorus.core.Node">count</see> themselves
            /// </summary>
            node,

            /// <summary>
            ///     Returns <see cref="phosphorus.core.Node.Name">name</see> property of matched nodes
            /// </summary>
            name,

            /// <summary>
            ///     Returns <see cref="phosphorus.core.Node.Value">value</see> property of matched nodes
            /// </summary>
            value,

            /// <summary>
            ///     Returns <see cref="phosphorus.core.Node.Name">count</see> property of matched nodes
            /// </summary>
            count
        }

        /*
         * Kept around, to allow conversion of node values
         */
        private readonly ApplicationContext _context;

        /*
         * Contains all matched entities
         */
        private readonly List<MatchEntity> _matchEntities = new List<MatchEntity> ();

        /*
         * Internal ctor, to make sure only Expression class can instantiate instances of Match class
         */
        internal Match (IEnumerable<Node> nodes, MatchType type, ApplicationContext context, string convert, bool reference)
        {
            TypeOfMatch = type;
            _context = context;
            Convert = convert;
            foreach (var idx in nodes) {
                switch (type) {
                case MatchType.name:
                    _matchEntities.Add (new MatchNameEntity (idx, this));
                    break;
                case MatchType.value:
                    if (reference && idx.Value is Expression) {
                        var innerMatch = (idx.Value as Expression).Evaluate (context, idx, idx);
                        foreach (var idxInner in innerMatch) {
                            _matchEntities.Add (idxInner);
                        }
                    } else {
                        _matchEntities.Add (new MatchValueEntity (idx, this));
                    }
                    break;
                case MatchType.node:
                    _matchEntities.Add (new MatchNodeEntity (idx, this));
                    break;
                case MatchType.count:
                    _matchEntities.Add (new MatchCountEntity (idx, this));
                    break;
                }
            }
        }

        /// <summary>
        ///     Returns number of nodes in match
        /// </summary>
        /// <value>Number of nodes in match</value>
        public int Count
        {
            get { return _matchEntities.Count; }
        }

        /// <summary>
        ///     Gets the type of match
        /// </summary>
        /// <value>The type declaration of your Expression</value>
        public MatchType TypeOfMatch
        {
            get; 
            private set;
        }

        /// <summary>
        ///     Type to convert values retrieved from match to
        /// </summary>
        /// <value>Type to convert to, can be any of your Hyperlambda types, defined through your [p5.hyperlambda.get-type-name.xxx] 
        /// Active Events</value>
        public string Convert
        {
            get; 
            private set;
        }

        /// <summary>
        ///     Returns the MatchEntity at the index position
        /// </summary>
        /// <param name="index">Which position you wish to retrieve</param>
        public MatchEntity this [int index]
        {
            get { return _matchEntities [index]; }
        }

        /*
         * Used by MatchEntity class for converting values
         */
        internal ApplicationContext Context
        {
            get { return _context; }
        }

        /// <summary>
        ///     Gets the enumerator for MatchEntity objects
        /// </summary>
        /// <returns>the enumerator</returns>
        public IEnumerator<MatchEntity> GetEnumerator ()
        {
            return _matchEntities.GetEnumerator ();
        }

        /*
         * Private implementation of IEnumerable<MatchEntity>'s base interface to avoid confusion.
         */
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return _matchEntities.GetEnumerator ();
        }
    }
}
