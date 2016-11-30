/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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
