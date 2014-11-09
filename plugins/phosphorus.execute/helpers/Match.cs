/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

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
            /// matches children of node(s)
            /// </summary>
            Children,

            /// <summary>
            /// matches node itself of node(s)
            /// </summary>
            Node,

            /// <summary>
            /// matches path of node(s)
            /// </summary>
            Path,
        }

        private List<Node> _nodes;
        private MatchType _type;

        internal Match (List<Node> nodes, MatchType type)
        {
            _nodes = nodes;
            _type = type;
        }

        /// <summary>
        /// changes the nodes in this <see cref="phosphorus.execute.Expression.Match"/> to null
        /// </summary>
        public void Assign ()
        {
            Assign (null as Match);
        }

        /// <summary>
        /// changes the nodes in this <see cref="phosphorus.execute.Expression.Match"/> to the given value 
        /// </summary>
        /// <param name="value">new value</param>
        public void Assign (string value)
        {
            Node node = new Node ("", value);
            List<Node> list = new List<Node> ();
            list.Add (node);
            Match match = new Match (list, MatchType.Value);
            Assign (match);
        }
        
        /// <summary>
        /// changes the nodes in this <see cref="phosphorus.execute.Expression.Match"/> to the given rhs
        /// </summary>
        /// <param name="rhs">new value(s) for nodes in match</param>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext"/> currently executing within</param>
        public void Assign (Match rhs)
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
                case MatchType.Children:
                    AssignChildren (rhs);
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
                    case MatchType.Children:
                        throw new ArgumentException ("cannot assign node list to node name");
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
                    case MatchType.Children:
                        throw new ArgumentException ("cannot assign node list to node value");
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

        /*
         * assigns children of resulting nodes
         */
        private void AssignChildren (Match rhs)
        {
            if (rhs == null) {
                foreach (Node idxDest in _nodes) {
                    idxDest.Clear ();
                }
            } else {
                if (rhs._type != MatchType.Children)
                    throw new ArgumentException ("you can only assign a node list to another node list");
                List<Node> sourceNodes = new List<Node> ();
                foreach (Node idxSource in rhs._nodes) {
                    foreach (Node idxSourceChild in idxSource.Children) {
                        sourceNodes.Add (idxSourceChild.Clone ());
                    }
                }
                foreach (Node idxDest in _nodes) {
                    idxDest.Clear ();
                    foreach (Node idxSource in sourceNodes) {
                        idxDest.Add (idxSource.Clone ());
                    }
                }
            }
        }
    }
}

