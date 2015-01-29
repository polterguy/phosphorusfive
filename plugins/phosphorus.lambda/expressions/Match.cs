
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

        private MatchType _type;
        private List<Node> _nodes;

        internal Match (IteratorGroup group, string type)
        {
            _nodes = new List<Node> (group.Evaluate);
            switch (type) {
            case "name":
                _type = MatchType.Name;
                break;
            case "value":
                _type = MatchType.Value;
                break;
            case "path":
                _type = MatchType.Path;
                break;
            case "node":
                _type = MatchType.Node;
                break;
            case "count":
                _type = MatchType.Count;
                break;
            default:
                throw new ArgumentException ("don't know how to construct a match type out of; " + type);
            }
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
            get {
                return _type;
            }
        }

        /// <summary>
        /// returns true if the <see cref="phosphorus.execute.Match"/> contains only one match, and the match will return a single
        /// object literal and not a <see cref="phosphorus.core.Node"/> 
        /// </summary>
        /// <value><c>true</c> if this instance is a single literal; otherwise, <c>false</c></value>
        public bool IsSingleLiteral {
            get {
                return _nodes.Count == 1 && _type != MatchType.Node;
            }
        }

        /// <summary>
        /// returns true if <see cref="phosphorus.execute.Match"/> is assignable
        /// </summary>
        /// <value><c>true</c> if this instance is assignable; otherwise, <c>false</c></value>
        public bool IsAssignable {
            get {
                return _type == MatchType.Name || _type == MatchType.Value || _type == MatchType.Node;
            }
        }

        /// <summary>
        /// gets the <see cref="phosphorus.core.Node"/> at the specified index
        /// </summary>
        /// <param name="index">index</param>
        public Node this [int index] {
            get {
                return _nodes [index];
            }
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
            if (index >= _nodes.Count)
                return defaultValue;
            object retVal = GetValue (index);
            if (retVal != null) {
                if (retVal is IConvertible)
                    return (T)Convert.ChangeType (retVal, typeof(T));
                else
                    return (T)(object)retVal.ToString ();
            }
            return defaultValue;
        }

        /// <summary>
        /// returns the value for the given index
        /// </summary>
        /// <returns>the value of the match</returns>
        /// <param name="index">index</param>
        public object GetValue (int index)
        {
            Node retVal = _nodes [index];
            switch (_type) {
            case MatchType.Name:
                return retVal.Name;
            case MatchType.Value:
                return retVal.Value;
            case MatchType.Path:
                return retVal.Path;
            case MatchType.Node:
                return retVal;
            default:
                throw new ArgumentException ("cannot get indexed value from match");
            }
        }
    }
}
