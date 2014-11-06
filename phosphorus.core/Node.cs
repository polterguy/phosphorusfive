/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;

namespace phosphorus.core
{
    /// <summary>
    /// arguments passed into and returned from Active Events
    /// </summary>
    [Serializable]
    public class Node
    {
        /// <summary>
        /// DNA code for Node
        /// </summary>
        public class DNA : IComparable
        {
            private List<int> _value;

            internal DNA (Node node)
            {
                _value = new List<int> ();
                Node idxNode = node;
                while (idxNode._parent != null) {
                    for (int idxNo = 0; idxNo < idxNode._parent._children.Count; idxNo ++) {
                        if (idxNode == idxNode._parent._children [idxNo]) {
                            _value.Add (idxNo);
                            break;
                        }
                    }
                    idxNode = idxNode._parent;
                }
            }

            public static bool operator == (DNA lhs, DNA rhs)
            {
                return lhs.CompareTo (rhs) == 0;
            }

            public static bool operator != (DNA lhs, DNA rhs)
            {
                return lhs.CompareTo (rhs) != 0;
            }

            public static bool operator > (DNA lhs, DNA rhs)
            {
                return lhs.CompareTo (rhs) == 1;
            }

            public static bool operator < (DNA lhs, DNA rhs)
            {
                return lhs.CompareTo (rhs) == -1;
            }

            public static bool operator >= (DNA lhs, DNA rhs)
            {
                return lhs.CompareTo (rhs) != -1;
            }

            public static bool operator <= (DNA lhs, DNA rhs)
            {
                return lhs.CompareTo (rhs) != 1;
            }

            public override bool Equals (object obj)
            {
                if (!(obj is DNA))
                    return false;
                return _value.Equals (((DNA)obj)._value);
            }

            public override int GetHashCode ()
            {
                return _value.GetHashCode ();
            }

            public override string ToString ()
            {
                string tmp = "";
                foreach (int idx in _value) {
                    tmp += "-" + idx;
                }
                tmp = tmp.Trim (new char[] { '-' });
                return string.Format ("[DNA: Value={0}]", tmp);
            }

            public int CompareTo (object obj)
            {
                if (obj == null || !(obj is DNA))
                    throw new ArgumentException ("cannot compare DNA to: " + (obj ?? "[null]").ToString ());
                DNA rhs = obj as DNA;

                for (int idxNo = 0; idxNo < _value.Count && idxNo < rhs._value.Count; idxNo ++) {
                    int cmpVals = _value [idxNo].CompareTo (rhs._value [idxNo]);
                    if (cmpVals != 0)
                        return cmpVals;
                }
                if (_value.Count > rhs._value.Count)
                    return 1;
                if (_value.Count < rhs._value.Count)
                    return -1;
                return 0;
            }
        }

        private List<Node> _children;
        private Node _parent;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.core.Node"/> class
        /// </summary>
        public Node ()
        {
            _children = new List<Node> ();
        }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.core.Node"/> class
        /// </summary>
        /// <param name="name">name of node</param>
        public Node (string name)
            : this ()
        {
            Name = name;
        }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.core.Node"/> class
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="value">value of node</param>
        public Node (string name, string value)
            : this (name)
        {
            Value = value;
        }

        /// <summary>
        /// gets or sets the name of the node
        /// </summary>
        /// <value>the name</value>
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// gets or sets the value of the node
        /// </summary>
        /// <value>the value</value>
        public object Value {
            get;
            set;
        }

        /// <summary>
        /// gets the children of this instance
        /// </summary>
        /// <value>its children</value>
        public IEnumerable<Node> Children {
            get {
                return _children;
            }
        }

        /// <summary>
        /// returns DNA code for Node
        /// </summary>
        /// <value>the DNA code, or position in Node tree</value>
        public DNA Position {
            get {
                return new DNA (this);
            }
        }

        /// <summary>
        /// adds a children node to its children collection
        /// </summary>
        /// <param name="node">node to add</param>
        public void Add (Node node)
        {
            node._parent = this;
            _children.Add (node);
        }

        /// <summary>
        /// gets or sets the <see cref="phosphorus.core.Node"/> with the specified index
        /// </summary>
        /// <param name="index">index of node to retrieve or set</param>
        public Node this [int index]
        {
            get {
                return _children [index];
            }
            set {
                _children [index]._parent = null;
                value._parent = this;
                _children [index] = value;
            }
        }
    }
}

