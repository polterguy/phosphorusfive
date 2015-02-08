
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions.iterators;

namespace phosphorus.expressions
{
    /// <summary>
    /// wrapper around a single Match item result
    /// </summary>
    public class MatchEntity
    {
        private Match _match;

        internal MatchEntity (Node node, Match match)
        {
            Node = node;
            _match = match;
        }

        /// <summary>
        /// node that was matched
        /// </summary>
        /// <value>node for match result</value>
        public Node Node {
            get;
            internal set;
        }

        /// <summary>
        /// type of match
        /// </summary>
        /// <value>type of match</value>
        public Match.MatchType TypeOfMatch {
            get {
                return _match.TypeOfMatch;
            }
        }
        
        /// <summary>
        /// retrieves or sets the value for the entity
        /// </summary>
        /// <returns>the value of the match</returns>
        public object Value {
            get {
                switch (TypeOfMatch) {
                case Match.MatchType.name:
                    return Node.Name;
                case Match.MatchType.value:
                    return Node.Value;
                case Match.MatchType.path:
                    return Node.Path;
                case Match.MatchType.node:
                    return Node;
                default:
                    throw new ArgumentException ("cannot get entity value from match of type 'count'");
                }
            }
            set {
                switch (TypeOfMatch) {
                case Match.MatchType.name:
                    Node.Name = Utilities.Convert<string> (value, _match.Context, string.Empty);
                    break;
                case Match.MatchType.value:
                    Node.Value = value; // ps, not cloned!
                    break;
                case Match.MatchType.node:
                    if (value == null)
                        Node.UnTie ();
                    else
                        Node.Replace (Utilities.Convert<Node> (value, _match.Context).Clone ()); // ps, cloned!
                    break;
                default:
                    throw new ArgumentException ("cannot get indexed value from match of type 'count'");
                }
            }
        }
    }
}
