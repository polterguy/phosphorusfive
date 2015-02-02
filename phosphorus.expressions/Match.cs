
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.lambda.iterators;

namespace phosphorus.lambda
{
    /// <summary>
    /// expression result class, contains evaluated result of an <see cref="phosphorus.execute.Expression"/> 
    /// </summary>
    public class Match
    {
        /// <summary>
        /// type of match for <see cref="phosphorus.execute.Match"/> object
        /// </summary>
        public enum MatchType
        {
            /// <summary>
            /// matches name of node(s)
            /// </summary>
            Name,

            /// <summary>
            /// matches value of node(s)
            /// </summary>
            Value,
            
            /// <summary>
            /// matches number of nodes in <see cref="phosphorus.execute.Match"/> 
            /// </summary>
            Count,

            /// <summary>
            /// matches path of node(s)
            /// </summary>
            Path,

            /// <summary>
            /// matches node itself of node(s)
            /// </summary>
            Node
        }

        private List<Node> _nodes;

        internal Match (IteratorGroup group, string type)
        {
            _nodes = new List<Node> (group.Evaluate);
            TypeOfMatch = (MatchType)Enum.Parse (typeof(MatchType), type.Substring (0,1).ToUpper () + type.Substring (1));
        }

        /// <summary>
        /// return number of nodes in match
        /// </summary>
        /// <value>number of nodes</value>
        public int Count {
            get {
                return _nodes.Count;
            }
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
        /// return all nodes in match
        /// </summary>
        /// <value>the matches</value>
        public IEnumerable<Node> Matches {
            get {
                return _nodes;
            }
        }

        /// <summary>
        /// returns value of index match as typeof(T)
        /// </summary>
        /// <returns>the value converted to T if possible</returns>
        /// <param name="index">index of which match to retrieve</param>
        /// <param name="defaultValue">default value to return if there is no match at index position, or match yields null</param>
        /// <typeparam name="T">type to convert match into</typeparam>
        public T GetValue<T> (int index, T defaultValue = default (T))
        {
            return Utilities.Convert<T> (GetValue (index), defaultValue);
        }

        /// <summary>
        /// returns the value for the given index
        /// </summary>
        /// <returns>the value of the match</returns>
        /// <param name="index">index</param>
        public object GetValue (int index)
        {
            if (index >= _nodes.Count || index < 0)
                throw new ArgumentException ("index not within range of match");

            switch (TypeOfMatch) {
            case MatchType.Name:
                return _nodes [index].Name;
            case MatchType.Value:
                return _nodes [index].Value;
            case MatchType.Path:
                return _nodes [index].Path;
            case MatchType.Node:
                return _nodes [index];
            default:
                throw new ArgumentException ("cannot get indexed value from match of type 'count'");
            }
        }

        /// <summary>
        /// returns the node at the index position within the match
        /// </summary>
        /// <returns>the node</returns>
        /// <param name="index">index</param>
        public Node GetNode (int index)
        {
            if (index >= _nodes.Count || index < 0)
                throw new ArgumentException ("index not within range of match");
            return _nodes [index];
        }
    }
}
