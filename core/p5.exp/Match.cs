/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections;
using System.Collections.Generic;
using p5.core;
using p5.exp.matchentities;

namespace p5.exp
{
    /// <summary>
    ///     Expression result class.
    /// 
    ///     When you evaluate a p5.lambda expression, then you end upo with a Match object.
    /// </summary>
    public class Match : IEnumerable<MatchEntity>
    {
        /// <summary>
        ///     Type of match for your match object.
        /// 
        ///     Can be either 'name', 'value', 'count', 'path' or 'node'. This is the part that you define when you create 
        ///     your type declaration within your p5.lambda Expression. For instance '?name', creates a 'name' type of Expression.
        ///     If no type is explicitly specified, then 'node' will be assumed.
        /// </summary>
        public enum MatchType
        {
            
            /// <summary>
            ///     Returns <see cref="phosphorus.core.Node">count</see> themselves.
            /// 
            ///     Declared through the type declaration of '?node'.
            /// </summary>
            node,

            /// <summary>
            ///     Returns <see cref="phosphorus.core.Node.Name">name</see> property of matched nodes.
            /// 
            ///     Declared through the type declaration of '?name'.
            /// </summary>
            name,

            /// <summary>
            ///     Returns <see cref="phosphorus.core.Node.Value">value</see> property of matched nodes.
            /// 
            ///     Declared through the type declaration of '?value'.
            /// </summary>
            value,

            /// <summary>
            ///     Returns <see cref="phosphorus.core.Node.Name">count</see> property of matched nodes.
            /// 
            ///     Declared through the type declaration of '?count'.
            /// </summary>
            count,

            /// <summary>
            ///     Returns <see cref="phosphorus.core.Node.Path">count</see> property of matched nodes.
            /// 
            ///     Declared through the type declaration of '?path'.
            /// </summary>
            path
        }

        /*
         * kept around, to allow conversion of node values
         */
        private readonly ApplicationContext _context;

        /*
         * contains all matched entities
         */
        private readonly List<MatchEntity> _matchEntities = new List<MatchEntity> ();

        /*
         * internal ctor, to make sure only Expression class can instantiate instances of Match class
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
                        var innerMatch = (idx.Value as Expression).Evaluate (idx, context, idx);
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
                case MatchType.path:
                    _matchEntities.Add (new MatchPathEntity (idx, this));
                    break;
                case MatchType.count:
                    _matchEntities.Add (new MatchCountEntity (idx, this));
                    break;
                }
            }
        }

        /// <summary>
        ///     Returns number of nodes in match.
        /// 
        ///     This value is the number of nodes you have in your result-set, after evaluating your Expression.
        /// </summary>
        /// <value>Number of nodes in match</value>
        public int Count
        {
            get { return _matchEntities.Count; }
        }

        /// <summary>
        ///     Gets the type of match.
        /// 
        ///     The type declaration of your match.
        /// </summary>
        /// <value>The type declaration of your Expression</value>
        public MatchType TypeOfMatch
        {
            get; 
            private set;
        }

        /// <summary>
        ///     Type to convert values retrieved from match to.
        /// 
        ///     Optionally, you can create a type-conversion of your result-set, such that the returned result-set values
        ///     in your Expression, after evaluation, becomes converted to any of the types your project supports through your
        ///     existing [p5.hyperlisp.get-object-value.xxx] Active Events.
        /// 
        ///     Unless you explicitly tells the expression engine that you wish to convert your Expression's result-set, then
        ///     no conversion will occur, and the values of your evaluated expression will be returned "as is".
        /// </summary>
        /// <value>Type to convert to, can be any of your Hyperlisp types, defined through your [p5.hyperlisp.get-type-name.xxx] 
        /// Active Events</value>
        public string Convert
        {
            get; 
            private set;
        }

        /// <summary>
        ///     Returns the MatchEntity at the index position.
        /// 
        ///     Returns the n'th matched entity.
        /// 
        ///     PS!<br/>
        ///     The match class implements IEnumerable, which allows you to iterate over all results, which means you'll very rarely,
        ///     if ever, need to fiddle with this method yourself.
        /// </summary>
        /// <param name="index">Which position you wish to retrieve</param>
        public MatchEntity this [int index]
        {
            get { return _matchEntities [index]; }
        }

        /*
         * used by MatchEntity class for converting values
         */
        internal ApplicationContext Context
        {
            get { return _context; }
        }

        /// <summary>
        ///     Gets the enumerator for MatchEntity objects.
        /// 
        ///     Returns the IEnumerator for all MatchEntity objects within your match.
        /// </summary>
        /// <returns>the enumerator</returns>
        public IEnumerator<MatchEntity> GetEnumerator ()
        {
            return _matchEntities.GetEnumerator ();
        }

        /*
         * private implementation of IEnumerable<MatchEntity>'s base interface to avoid confusion.
         */
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return _matchEntities.GetEnumerator ();
        }
    }
}
