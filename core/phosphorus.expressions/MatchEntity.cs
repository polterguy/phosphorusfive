/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System;
using phosphorus.core;

namespace phosphorus.expressions
{
    public class MatchEntity
    {
        private readonly Match _match;
        private readonly Match.MatchType? _type;

        internal MatchEntity (Node node, Match match, Match.MatchType? type)
        {
            Node = node;
            _match = match;
            _type = type;
        }

        public Node Node { get; private set; }

        public Match.MatchType TypeOfMatch
        {
            get { return _type ?? _match.TypeOfMatch; }
        }

        public Match Match
        {
            get { return _match; }
        }

        public object Value
        {
            get
            {
                object retVal;
                switch (TypeOfMatch) {
                    case Match.MatchType.name:
                        retVal = Node.Name;
                        break;
                    case Match.MatchType.value:
                        retVal = Node.Value;
                        break;
                    case Match.MatchType.node:
                        retVal = Node;
                        break;
                    default:
                        throw new ExpressionException ("Cannot enumerate 'count' type of expression.");
                }
                return retVal;
            }
            set
            {
                switch (TypeOfMatch) {
                    case Match.MatchType.name:
                        Node.Name = (string)value;
                        break;
                    case Match.MatchType.value:
                        Node.Value = value; // ps, not cloned!
                        break;
                    case Match.MatchType.node:
                        if (value == null) {
                            Node.UnTie ();
                        } else {
                            var tmp = (Node)value;
                            Node.Replace (tmp.Clone ()); // ps, cloned!
                        }
                        break;
                    default:
                        throw new CoreException ("You cannot use 'count' as a destination for your expressions.");
                }
            }
        }
    }
}
