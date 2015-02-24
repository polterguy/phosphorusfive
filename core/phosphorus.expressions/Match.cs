
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections;
using System.Collections.Generic;
using phosphorus.core;
// ReSharper disable InconsistentNaming

namespace phosphorus.expressions
{
    /// <summary>
    /// expression result class, contains evaluated result of a <see cref="phosphorus.expressions.Expression"/>.
    /// only to be used indirectly through Expression class
    /// </summary>
    public class Match : IEnumerable<MatchEntity>
    {
        /// <summary>
        /// type of match for <see cref="phosphorus.expressions.Match"/> object
        /// </summary>
        public enum MatchType
        {
            /// <summary>
            /// matches name of node(s)
            /// </summary>
            name,

            /// <summary>
            /// matches value of node(s)
            /// </summary>
            value,
            
            /// <summary>
            /// matches number of nodes in <see cref="phosphorus.expressions.Match"/> 
            /// </summary>
            count,

            /// <summary>
            /// matches path of node(s)
            /// </summary>
            path,

            /// <summary>
            /// matches node itself of node(s)
            /// </summary>
            node
        }

        /*
         * contains all matched entities
         */
        private readonly List<MatchEntity> _matchEntities;

        /*
         * kept around, to allow conversion of node values
         */
        private readonly ApplicationContext _context;

        private readonly string _convert;

        /*
         * internal ctor, to make sure only Expression class can instantiate instances of Match class
         */
        internal Match (IEnumerable<Node> nodes, MatchType type, ApplicationContext context, string convert)
        {
            TypeOfMatch = type;
            _matchEntities = new List<MatchEntity> ();
            _context = context;
            _convert = convert;
            foreach (var idx in nodes) {
                _matchEntities.Add (new MatchEntity (idx, this));
            }
        }

        /*
         * internal ctor, to make sure only Expression class can instantiate instances of Match class
         */
        internal Match (MatchType type, ApplicationContext context, string convert)
        {
            _matchEntities = new List<MatchEntity> ();
            _convert = convert;
            TypeOfMatch = type;
            _context = context;
        }

        /// <summary>
        /// return number of nodes in match
        /// </summary>
        /// <value>number of nodes</value>
        public int Count {
            get { return _matchEntities.Count; }
        }

        /// <summary>
        /// gets the type of match
        /// </summary>
        /// <value>the type of match</value>
        public MatchType TypeOfMatch {
            get;
            private set;
        }

        /// <summary>
        /// type to convert values retrieved from match
        /// </summary>
        /// <value>The convert.</value>
        public string Convert {
            get { return _convert; }
        }

        /// <summary>
        /// returns the MatchEntity at the index position
        /// </summary>
        /// <param name="index">Index.</param>
        public MatchEntity this [int index] {
            get { return _matchEntities [index]; }
        }

        /// <summary>
        /// gets the enumerator for MatchEntity objects
        /// </summary>
        /// <returns>the enumerator</returns>
        public IEnumerator<MatchEntity> GetEnumerator ()
        {
            return _matchEntities.GetEnumerator ();
        }

        /*
         * private implementation of IEnumerable<MatchEntity>'s base interface for clarity
         */
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return _matchEntities.GetEnumerator ();
        }

        /*
         * used by MatchEntity class for converting values
         */
        internal ApplicationContext Context {
            get { return _context; }
        }

        /*
         * used by expressions to build Match object
         */
        internal List<MatchEntity> Entities {
            get { return _matchEntities; }
        }
    }
}
