/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using pf.core;

namespace phosphorus.expressions
{
    /// <summary>
    ///     Wraps a single match.
    /// 
    ///     When an Expression is evaluated, and returned as a Match object, then this class encapsulates one single
    ///     matched item (node, value, path, name or count) from your Expression.
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
        ///     Node that was matched.
        /// 
        ///     This is not necessarily the Value of your match entity object.
        /// </summary>
        /// <value>Node for match entity item.</value>
        public Node Node { get; private set; }

        /// <summary>
        ///     Type of match.
        /// 
        ///     Sometimes, especially when you're using reference expressions, then the type of match you end up with, is
        ///     a mixed type. Meaning, some of your match entity items will be of type 'node', while others might be of type 'value',
        ///     and so on. If so is the case, then this property will return the type declaration of your match entity item, instead
        ///     of the type declaration of your Expression as a whole.
        /// </summary>
        /// <value>Type of match for match entity item.</value>
        public Match.MatchType TypeOfMatch
        {
            get { return _type ?? _match.TypeOfMatch; }
        }

        /// <summary>
        ///     Returns the parent match instance.
        /// </summary>
        /// <value>Parent match.</value>
        public Match Match
        {
            get { return _match; }
        }

        /// <summary>
        ///     Retrieves or sets the value for the match entity item.
        /// 
        ///     Use this property to retrieve the actual value of your expression's matched entity items.
        ///     If you assign a <see cref="phosphorus.core.Node">Node</see> to another node, then the node will
        ///     be cloned before assignment, to avoid UnTying node from existing parent node's children collection.
        /// </summary>
        /// <returns>The value of the match entity item.</returns>
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
                                Node.Replace (tmp); // ps, NOT cloned!
                            } else {
                                Node.Replace (tmp.Clone ()); // ps, cloned!
                            }
                        }
                        break;
                    default:
                        throw new ApplicationException ("You cannot use neither 'count' nor 'path' as destinations in your expressions");
                }
            }
        }
    }
}
