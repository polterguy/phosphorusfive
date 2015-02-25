/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;

namespace phosphorus.expressions
{
    /// <summary>
    ///     wrapper around a single Match item result
    /// </summary>
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

        /// <summary>
        ///     node that was matched
        /// </summary>
        /// <value>node for match result</value>
        public Node Node { get; private set; }

        /// <summary>
        ///     type of match
        /// </summary>
        /// <value>type of match</value>
        public Match.MatchType TypeOfMatch
        {
            get { return _type ?? _match.TypeOfMatch; }
        }

        /// <summary>
        ///     returns match object
        /// </summary>
        /// <value>type of match</value>
        public Match Match
        {
            get { return _match; }
        }

        /// <summary>
        ///     retrieves or sets the value for the entity
        /// </summary>
        /// <returns>the value of the match</returns>
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
                    case Match.MatchType.path:
                        retVal = Node.Path;
                        break;
                    case Match.MatchType.node:
                        retVal = Node;
                        break;
                    default:
                        throw new ApplicationException ("cannot get entity value from match of type 'count'");
                }
                if (retVal != null && !string.IsNullOrEmpty (_match.Convert)) {
                    retVal = _match.Convert == "string" ?
                        Utilities.Convert<string> (retVal, _match.Context) :
                        _match.Context.Raise ("pf.hyperlisp.get-object-value." + _match.Convert, new Node (string.Empty, retVal)).Value;
                }
                return retVal;
            }
            set
            {
                switch (TypeOfMatch) {
                    case Match.MatchType.name:
                        Node.Name = Utilities.Convert (value, _match.Context, string.Empty);
                        break;
                    case Match.MatchType.value:
                        Node.Value = value; // ps, not cloned!
                        break;
                    case Match.MatchType.node:
                        if (value == null) {
                            Node.UnTie ();
                        } else {
                            var tmp = Utilities.Convert<Node> (value, _match.Context);
                            if (value is string) {
                                // Node was created from a conversion from string, making sure that we discard
                                // the automatically created "root node" in object
                                if (tmp.Count != 1)
                                    throw new ApplicationException ("tried to convert a string that would create multiple nodes to one node");
                                tmp = tmp [0];
                            }
                            Node.Replace (tmp.Clone ()); // ps, cloned!
                        }
                        break;
                    default:
                        throw new ApplicationException ("cannot set indexed value from match of type 'count' or 'path'");
                }
            }
        }
    }
}