/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.execute.iterators;

namespace phosphorus.execute
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
                case @"node":
                    _type = MatchType.Node;
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
        /// gets the <see cref="phosphorus.execute.Match"/> at the specified index
        /// </summary>
        /// <param name="index">index</param>
        public Node this [int index] {
            get {
                return _nodes [index];
            }
        }

        /// <summary>
        /// returns value of index match as typeof(T)
        /// </summary>
        /// <returns>the value converted to T if possible</returns>
        /// <param name="index">index of which match to retrieve</param>
        /// <param name="defaultValue">default value to return if there is no match at index position, or match yields null</param>
        /// <typeparam name="T">type to convert match into</typeparam>
        public T GetValue<T> (int index, T defaultValue)
        {
            if (index >= _nodes.Count)
                return defaultValue;
            object retVal = GetValue (index);
            if (retVal != null)
                return (T)Convert.ChangeType (retVal, typeof(T));
            return defaultValue;
        }

        /// <summary>
        /// returns the value for the given index
        /// </summary>
        /// <returns>the value of the match</returns>
        /// <param name="index">index</param>
        public object GetValue (int index)
        {
            switch (_type) {
                case MatchType.Name:
                    return _nodes [index].Name;
                case MatchType.Value:
                    return _nodes [index].Value;
                case MatchType.Path:
                    return _nodes [index].Path;
                case MatchType.Node:
                    return _nodes [index];
                default:
                    throw new ArgumentException ("unknown type of match");
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
        /// changes the nodes in this <see cref="phosphorus.execute.Expression.Match"/> to the given value 
        /// </summary>
        /// <param name="value">new value</param>
        public void AssignValue (object value)
        {
            Node node = new Node ("", value);
            Match match = new Match (new IteratorGroup (node, null), "value");
            AssignMatch (match);
        }
        
        /// <summary>
        /// changes the nodes in this <see cref="phosphorus.execute.Expression.Match"/> to the given rhs match
        /// </summary>
        /// <param name="rhs">new value(s) for nodes in match</param>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext"/> currently executing within</param>
        public void AssignMatch (Match rhs)
        {
            switch (_type) {
                case MatchType.Name:
                    AssignName (rhs);
                    break;
                case MatchType.Value:
                    AssignValue (rhs);
                    break;
                case MatchType.Path:
                    throw new ArgumentException ("cannot assign path of node, path is read only");
                case MatchType.Node:
                    AssignNode (rhs);
                    break;
            }
        }

        /*
         * assigns name of result nodes
         */
        private void AssignName (Match rhs)
        {
            if (rhs == null) {
                foreach (Node idxDest in _nodes) {
                    idxDest.Name = string.Empty; // name should and cannot be null, hence empty string
                }
            } else {
                if (rhs._nodes.Count > 1)
                    throw new ArgumentException ("source match cannot have multiple values when assigning name of match");
                string sourceValue = null;
                switch (rhs._type) {
                    case MatchType.Name:
                        sourceValue = rhs._nodes [0].Name;
                        break;
                    case MatchType.Value:
                        sourceValue = rhs._nodes [0].Get<string> ();
                        break;
                    case MatchType.Path:
                        sourceValue = rhs._nodes [0].Path.ToString ();
                        break;
                    default:
                        throw new ArgumentException ("cannot assign name anything but another name, value or path");
                }
                sourceValue = sourceValue ?? "";
                if (sourceValue.IndexOfAny (new char[] { ' ', '"', '\n', '\r', '\t', ':' }) != -1)
                    throw new ArgumentException (string.Format ("'{0}' was illegal value for name of node", sourceValue));
                foreach (Node idxDest in _nodes) {
                    idxDest.Name = sourceValue;
                }
            }
        }

        /*
         * assigns value of result nodes
         */
        private void AssignValue (Match rhs)
        {
            if (rhs == null) {
                foreach (Node idxDest in _nodes) {
                    idxDest.Value = null;
                }
            } else {
                if (rhs._nodes.Count > 1)
                    throw new ArgumentException ("source match cannot have multiple values when assigning value");
                string sourceValue = null;
                switch (rhs._type) {
                    case MatchType.Name:
                        sourceValue = rhs._nodes [0].Name;
                        break;
                    case MatchType.Value:
                        sourceValue = rhs._nodes [0].Get<string> ();
                        break;
                    case MatchType.Path:
                        sourceValue = rhs._nodes [0].Path.ToString ();
                        break;
                    default:
                        throw new ArgumentException ("cannot assign value anything but another value, name or path");
                }
                foreach (Node idxDest in _nodes) {
                    idxDest.Value = sourceValue;
                }
            }
        }
        
        /*
         * assigns node of resulting nodes
         */
        private void AssignNode (Match rhs)
        {
            if (rhs == null) {
                foreach (Node idxDest in _nodes) {
                    idxDest.Untie ();
                }
            } else {
                if (rhs._type != MatchType.Node || rhs._nodes.Count != 1)
                    throw new ArgumentException ("you can only assign a node to one other node");
                foreach (Node idxDest in _nodes) {
                    idxDest.Replace (rhs._nodes [0].Clone ());
                }
            }
        }
    }
}

