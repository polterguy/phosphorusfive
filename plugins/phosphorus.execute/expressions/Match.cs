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
        /// gets the <see cref="phosphorus.core.Node"/> at the specified index
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
                throw new ArgumentException ("cannot get indexed value from match");
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
        /// returns true if the <see cref="phosphorus.execute.Match"/> contains only one match, and the match will return a single
        /// object literal
        /// </summary>
        /// <value><c>true</c> if this instance is a single literal; otherwise, <c>false</c></value>
        public bool IsSingleLiteral {
            get {
                return _nodes.Count == 1 && 
                    (_type == MatchType.Name || 
                     _type == MatchType.Value || 
                     _type == MatchType.Count || 
                     _type == MatchType.Path);
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
        /// changes the nodes in this <see cref="phosphorus.execute.Expression.Match"/> to the given value 
        /// </summary>
        /// <param name="value">new value</param>
        public void AssignValue (object value)
        {
            Node node = new Node ("", value);
            IteratorGroup iGroup = new IteratorGroup (node);
            Match match = new Match (iGroup, "value");
            AssignMatch (match, null);
        }
        
        /// <summary>
        /// changes the nodes in this <see cref="phosphorus.execute.Expression.Match"/> to the given rhs match
        /// </summary>
        /// <param name="rhs">new value(s) for nodes in match</param>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext"/> currently executing within</param>
        public void AssignMatch (Match rhs, ApplicationContext context)
        {
            switch (_type) {
            case MatchType.Name:
                AssignName (rhs, context);
                break;
            case MatchType.Value:
                AssignValue (rhs, context);
                break;
            case MatchType.Node:
                AssignNodes (rhs);
                break;
            default:
                throw new ArgumentException ("match is unassignable and read only, only 'name', 'value' and 'node' can be assigned");
            }
        }

        /*
         * returns all values from match object
         */
        private object GetValue (ApplicationContext context)
        {
            if (_type == MatchType.Count) {
                return Count;
            } else if (Count == 0) {
                return null;
            } else if (_type == MatchType.Node) {
                Node tmpCode = new Node ("root");
                foreach (Node idx in Matches) {
                    tmpCode.Add (idx.Clone ());
                }
                context.Raise ("pf.node-2-hyperlisp", tmpCode);
                return tmpCode.Value;
            } else if (Count == 1) {
                return GetValue (0);
            } else {
                string retVal = "";
                for (int idxNo = 0; idxNo < Count; idxNo++) {
                    object tmpVal = GetValue (idxNo);
                    if (tmpVal == null) {
                        retVal += ",";
                    } else {
                        string tmpStringVal = tmpVal.ToString ();
                        if (tmpStringVal.Contains ("\r") || 
                            tmpStringVal.Contains ("\n") || 
                            tmpStringVal.Contains (@"""") || 
                            tmpStringVal.Contains (",") || 
                            tmpStringVal.Trim () != tmpStringVal)
                            retVal += string.Format (@",@""{0}""", tmpStringVal.Replace (@"""", @""""""));
                        else
                            retVal += string.Format (@",{0}", tmpStringVal);
                    }
                }
                return string.Format (@"[{0}]", retVal.Trim (new char[] { ',' }));
            }
        }

        /*
         * assigns value of result nodes
         */
        private void AssignValue (Match rhs, ApplicationContext context)
        {
            object sourceValue = rhs.GetValue (context);
            foreach (Node idxDest in _nodes) {
                idxDest.Value = sourceValue;
            }
        }
        
        /*
         * assigns name of result nodes
         */
        private void AssignName (Match rhs, ApplicationContext context)
        {
            object sourceValue = rhs.GetValue (context);
            foreach (Node idxDest in _nodes) {
                idxDest.Name = (sourceValue ?? "").ToString ();
            }
        }

        /*
         * assigns node of resulting nodes
         */
        private void AssignNodes (Match rhs)
        {
            if (rhs._type == MatchType.Node) {
                foreach (Node idxDest in _nodes) {
                    foreach (Node idxSource in rhs.Matches) {
                        idxDest.Add (idxSource.Clone ());
                    }
                }
            } else if (rhs._type == MatchType.Name) {
                foreach (Node idxDest in _nodes) {
                    foreach (Node idxSource in rhs.Matches) {
                        idxDest.Add (new Node ("", idxSource.Name));
                    }
                }
            } else if (rhs._type == MatchType.Value) {
                foreach (Node idxDest in _nodes) {
                    foreach (Node idxSource in rhs.Matches) {
                        idxDest.Add (new Node ("", idxSource.Value));
                    }
                }
            } else if (rhs._type == MatchType.Count) {
                foreach (Node idxDest in _nodes) {
                    idxDest.Add (new Node ("", rhs.Count));
                }
            } else if (rhs._type == MatchType.Path) {
                foreach (Node idxDest in _nodes) {
                    foreach (Node idxSource in rhs.Matches) {
                        idxDest.Add (new Node ("", idxSource.Path));
                    }
                }
            }
        }
    }
}

