/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;

namespace p5.exp.matchentities
{
    /// <summary>
    ///     Represents a single item in a Match created from an expression
    /// </summary>
    public abstract class MatchEntity
    {
        protected readonly Match _match;

        internal MatchEntity (Node node, Match match)
        {
            Node = node;
            _match = match;
        }

        /// <summary>
        ///     Returns the node encapsulated by this entity
        /// </summary>
        /// <value>The node.</value>
        public Node Node { get; private set; }

        /// <summary>
        ///     Returns the type of match for this entity
        /// </summary>
        /// <value>The type of match</value>
        public abstract Match.MatchType TypeOfMatch {
            get;
        }

        /// <summary>
        ///     Returns the match for this entity
        /// </summary>
        /// <value>The match.</value>
        public Match Match {
            get { return _match; }
        }

        /// <summary>
        ///     Gets or sets the value for this entity
        /// </summary>
        /// <value>The value.</value>
        public abstract object Value {
            get;
            set;
        }
    }
}
