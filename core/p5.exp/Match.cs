/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
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
