/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed license.txt file for details
 */

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace phosphorus.types
{
    /// <summary>
    /// type for creating general purpose graph objects
    /// </summary>
    public class Node : IEnumerable
    {
        private string _name;
        private object _value;
        private Node _parent;

        /// <summary>
        /// empty ctor
        /// </summary>
        public Node ()
        { }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.types.Node"/> class and sets its name
        /// </summary>
        /// <param name="name">Name.</param>
        public Node (string name)
            : this()
        {
            _name = name;
        }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.types.Node"/> class and set its name and value
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public Node (string name, object value)
            : this(name)
        {
            _value = value;
        }

        /// <summary>
        /// gets or sets the name
        /// </summary>
        /// <value>the name</value>
        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// gets or sets the value
        /// </summary>
        /// <value>value</value>
        public object Value {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// returns the parent node of this instance
        /// </summary>
        /// <value>the parent node</value>
        public Node Parent {
            get { return _parent; }
        }

        /// <summary>
        /// returns the number of children nodes in this instance, if any
        /// </summary>
        /// <value>number of nodes</value>
        public int Count {
            get {
                List<Node> lst = _value as List<Node>;
                if (lst != null)
                    return lst.Count;
                return 0;
            }
        }

        /// <summary>
        /// clears value of node
        /// </summary>
        public void Clear ()
        {
            _value = null;
        }

        /// <summary>
        /// gets or sets the <see cref="phosphorus.types.Node"/> with the specified name
        /// </summary>
        /// <param name="i">the name of the node to set</param>
        public Node this [string i] {
            get {
                List<Node> lst = _value as List<Node>;
                if (lst == null) {
                    lst = new List<Node> ();
                    _value = lst;
                }
                Node tmp = lst.Find(
                    delegate (Node idx) {
                        return idx.Name == i;
                    });
                if (tmp == null) {
                    tmp = new Node (i);
                    tmp._parent = this;
                    lst.Add (tmp);
                }
                return tmp;
            }
            set {
                List<Node> lst = _value as List<Node>;
                if (lst == null) {
                    lst = new List<Node> ();
                    _value = lst;
                }
                Node tmp = lst.Find(
                    delegate (Node idx) {
                        return idx.Name == i;
                    });
                if (tmp != null) {
                    lst.Remove (tmp);
                }
                lst.Add (value);
                value._parent = this;
            }
        }

        /// <summary>
        /// returns the value as typeof (T)
        /// </summary>
        /// <typeparam name="T">the type you wish to return the value as</typeparam>
        public T Get<T>()
        {
            if (_value == null)
                return default (T);
            return (T)_value;
        }
        
        /// <summary>
        /// IEnumerable support for enumerating nodes in value
        /// </summary>
        /// <returns>the enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator ()
        {
            List<Node> tmp = _value as List<Node>;
            if (tmp == null)
                return null;
            return tmp.GetEnumerator();
        }
    }
}

