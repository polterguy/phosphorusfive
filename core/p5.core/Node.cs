
/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Collections.Generic;

namespace p5.core
{
    /// <summary>
    ///     Arguments passed into and returned from Active Events
    /// </summary>
    [Serializable]
    public class Node : IComparable
    {
        private readonly List<Node> _children;
        private string _name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.core.Node"/> class
        /// </summary>
        public Node ()
        {
            _children = new List<Node> ();
            Name = "";
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.core.Node"/> class
        /// </summary>
        /// <param name="name">Name of node. Cannot be null</param>
        public Node (string name)
            : this ()
        {
            Name = name;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.core.Node"/> class
        /// </summary>
        /// <param name="name">Name of node. Cannot be null</param>
        /// <param name="value">Value of node. Can be any object, including null</param>
        public Node (string name, object value)
            : this (name)
        {
            Value = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.core.Node"/> class
        /// </summary>
        /// <param name="name">Name of node. Cannot be null</param>
        /// <param name="value">Value of node. Can be any object, including null</param>
        /// <param name="children">Initial children collection for node. Notice that if children given already
        /// belongs to another Node's children collection, then they will be UnTied from the other node, and ReTied 
        /// into currently created node</param>
        public Node (string name, object value, IEnumerable<Node> children)
            : this (name, value)
        {
            var collection = children as Node[] ?? children.ToArray ();
            _children = new List<Node> (collection);
            foreach (var idx in collection) {
                idx.Parent = this;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the node
        /// </summary>
        /// <value>The node's new name</value>
        public string Name {
            get {
                return _name;
            }
            set {
                if (value == null)
                    throw new ArgumentException ("You cannot set a node's name to 'null'");
                _name = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value of the node
        /// </summary>
        /// <value>The node's new value</value>
        public object Value {
            get;
            set;
        }

        /// <summary>
        ///     Returns the index of the given node in children collection
        /// </summary>
        /// <returns>The of</returns>
        /// <param name="node">Node</param>
        public int IndexOf (Node node)
        {
            return _children.IndexOf (node);
        }

        /// <summary>
        ///     Returns the value of this instance as typeof(T)
        /// </summary>
        /// <typeparam name="T">Type to convert value to</typeparam>
        public T Get<T> (ApplicationContext context, T defaultValue = default (T))
        {
            return Utilities.Convert (context, Value, defaultValue);
        }

        /// <summary>
        ///     Returns the children of this instance
        /// </summary>
        /// <value>Its children nodes</value>
        public List<Node> Children {
            get {
                return _children;
            }
        }

        /// <summary>
        ///     Returns the parent of current node
        /// </summary>
        /// <value>The parent node of this instance</value>
        public Node Parent { get; private set; }

        /// <summary>
        ///     Returns the first child of the node
        /// </summary>
        /// <value>The current node's first child node</value>
        public Node FirstChild {
            get {
                if (_children.Count > 0)
                    return _children [0];
                return null;
            }
        }

        /// <summary>
        ///     Returns the last child of the node
        /// </summary>
        /// <value>The last child node</value>
        public Node LastChild {
            get {
                if (_children.Count > 0)
                    return _children [_children.Count - 1];
                return null;
            }
        }

        /// <summary>
        ///     Returns the previous sibling of the current node
        /// </summary>
        /// <value>The current node's previous sibling</value>
        public Node PreviousSibling {
            get {
                if (Parent == null)
                    return null;
                var idxNo = Parent._children.TakeWhile (idxNode => idxNode != this).Count ();
                idxNo -= 1;
                if (idxNo >= 0)
                    return Parent._children [idxNo];
                return null;
            }
        }

        /// <summary>
        ///     Returns the next sibling of the current node
        /// </summary>
        /// <value>The current node's next sibling</value>
        public Node NextSibling {
            get {
                if (Parent == null)
                    return null;
                var idxNo = Parent._children.IndexOf (this);
                idxNo += 1;
                if (idxNo < Parent._children.Count)
                    return Parent._children [idxNo];
                return null;
            }
        }

        /// <summary>
        ///     Returns the previous node from the current node
        /// </summary>
        /// <value>The current node's previous node</value>
        public Node PreviousNode {
            get {
                var idx = PreviousSibling;
                if (idx != null) {
                    while (idx.Children.Count > 0) {
                        idx = idx [idx.Children.Count - 1];
                    }
                    return idx;
                }
                return Parent;
            }
        }

        /// <summary>
        ///     Returns the next node from the current node
        /// </summary>
        /// <value>The current node's next node</value>
        public Node NextNode {
            get {
                if (Children.Count > 0)
                    return FirstChild;
                var nextSibling = NextSibling;
                if (nextSibling != null)
                    return nextSibling;
                var idxParent = Parent;
                while (idxParent != null) {
                    var next = idxParent.NextSibling;
                    if (next != null)
                        return next;
                    idxParent = idxParent.Parent;
                }
                return null;
            }
        }

        /// <summary>
        ///     Returns the root node of the tree
        /// </summary>
        /// <value>The root node of the current node</value>
        public Node Root {
            get {
                var idxNode = this;
                while (idxNode.Parent != null)
                    idxNode = idxNode.Parent;
                return idxNode;
            }
        }

        /// <summary>
        ///     Gets the number of ancestors from this node to root
        /// </summary>
        /// <value>The offset to root</value>
        public int OffsetToRoot
        {
            get {
                int idxNo = 0;
                var tmpIdx = this;
                var root = Root;
                while (tmpIdx != root) {
                    tmpIdx = tmpIdx.Parent;
                    idxNo += 1;
                }
                return idxNo;
            }
        }

        /// <summary>
        ///     Unties the node from its Parent Children collection
        /// </summary>
        public Node UnTie ()
        {
            Parent._children.Remove (this);
            Parent = null;
            return this;
        }

        /// <summary>
        ///     Replaces the current node with another node
        /// </summary>
        /// <param name="node">Node to replace the current node with</param>
        public Node Replace (Node node)
        {
            if (node == null) {
                throw new ArgumentException ("Cannot replace a node with null");
            }
            node.Parent = Parent;
            Parent._children [Parent._children.IndexOf (this)] = node;
            Parent = null;
            return node;
        }

        /// <summary>
        ///     Finds, or creates, the first node having the given name
        /// </summary>
        /// <param name="name">Name of node to return or create</param>
        public Node FindOrCreate (string name)
        {
            var retVal = _children.Find (idx => idx.Name == name);
            if (retVal != null)
                return retVal;
            return Add (new Node (name)).LastChild;
        }

        /// <summary>
        ///     Returns the Value of the first Children node, matching the given name, as type T
        /// </summary>
        /// <param name="name">Name of node to return</param>
        /// <param name="context">Application Context</param>
        /// <param name="defaultValue">Default value to use, if no child with the given name is found</param>
        public T GetChildValue<T> (string name, ApplicationContext context, T defaultValue = default (T))
        {
            var child = _children.Find (idx => idx.Name == name);
            return child == null ? defaultValue : child.Get (context, defaultValue);
        }

        /// <summary>
        ///     Removes the specified node
        /// </summary>
        /// <param name="node">Node</param>
        public Node Remove (Node node)
        {
            if (!_children.Remove (node))
                throw new ArgumentException ("Node doesn't belong to collection");
            return this;
        }

        /// <summary>
        ///     Sorts the children of the current node
        /// </summary>
        /// <param name="functor">Comparison delegate</param>
        public Node Sort (Comparison<Node> functor)
        {
            _children.Sort (functor);
            return this;
        }

        /// <summary>
        ///     Adds a child node to the current node's children collection
        /// </summary>
        /// <param name="node">node to add</param>
        public Node Add (Node node)
        {
            if (node.Parent != null)
                node.Parent.Remove (node);
            node.Parent = this;
            _children.Add (node);
            return this;
        }

        /// <summary>
        ///     Adds a child node to the current node's children collection
        /// </summary>
        /// <param name="name">name of node to add</param>
        public Node Add (string name)
        {
            return Add (new Node (name));
        }

        /// <summary>
        ///     Adds a child node to the current node's children collection
        /// </summary>
        /// <param name="name">name of node to add</param>
        /// <param name="value">value of node to add</param>
        public Node Add (string name, object value)
        {
            return Add (new Node (name, value));
        }

        /// <summary>
        ///     Adds a child node to the current node's children collection
        /// </summary>
        /// <param name="name">name of node to add</param>
        /// <param name="value">value of node to add</param>
        /// <param name="nodes">initial child collection of node</param>
        public Node Add (string name, object value, IEnumerable<Node> nodes)
        {
            return Add (new Node (name, value, nodes));
        }

        /// <summary>
        ///     Inserts a child node to the current node's children collection
        /// </summary>
        /// <param name="node">node to add</param>
        /// <param name="index">where to add</param>
        public Node Insert (int index, Node node)
        {
            node.Parent = this;
            _children.Insert (index, node);
            return this;
        }

        /// <summary>
        ///     Adds a range of nodes
        /// </summary>
        /// <param name="nodes">nodes to add</param>
        public Node AddRange (IEnumerable<Node> nodes)
        {
            foreach (var idxNode in nodes.ToList ()) {
                Add (idxNode);
            }
            return this;
        }

        /// <summary>
        ///     Clears the children collection
        /// </summary>
        public Node Clear ()
        {
            _children.Clear ();
            return this;
        }

        /// <summary>
        ///     Clones the current node
        /// </summary>
        public Node Clone ()
        {
            var retVal = new Node (Name, Value);
            foreach (var idxChild in _children) {
                retVal.Add (idxChild.Clone ());
            }
            return retVal;
        }

        /// <summary>
        ///     IComparable implementation, compares current node to another object
        /// </summary>
        /// <returns>-1 if this node is "less than", +1 if rhs is "less than", 0 if objects are equal</returns>
        /// <param name="rhs">The object to compare the node against</param>
        public int CompareTo (object rhs)
        {
            var rhsNode = rhs as Node;
            if (rhsNode == null)
                return 1;
            return CompareTo (rhsNode);
        }

        /// <summary>
        ///     Compares current node to another node
        /// </summary>
        /// <returns>-1 if this node is "less than", +1 if rhs is "less than", 0 if objects are equal</returns>
        /// <param name="rhs">The object to compare the node against</param>
        public int CompareTo (Node rhs)
        {
            var retVal = String.Compare(Name, rhs.Name, StringComparison.Ordinal);
            if (retVal != 0)
                return retVal;
            if (Value == null) {
                if (rhs.Value != null)
                    return -1;
            } else if (rhs.Value == null) {
                return 1;
            }
            retVal = CompareValueObjects (Value, rhs.Value);
            if (retVal != 0)
                return retVal;
            if (_children.Count < rhs._children.Count) {
                return -1;
            } else if (rhs._children.Count < _children.Count) {
                return 1;
            } else {
                for (var idxNo = 0; idxNo < Children.Count; idxNo ++) {
                    retVal = _children [idxNo].CompareTo (rhs._children [idxNo]);
                    if (retVal != 0)
                        return retVal;
                }
            }
            return 0;
        }
        
        /// <summary>
        ///     Gets or sets the node at the specified index
        /// </summary>
        /// <param name="index">Index of node to retrieve or set</param>
        public Node this [int index]
        {
            get {
                return _children [index];
            }
            set {
                this [index].Replace (value);
            }
        }

        /// <summary>
        ///     Gets or sets the first node in the children collection matching the given name
        /// </summary>
        /// <param name="name">Name of node to retrieve or set</param>
        public Node this [string name]
        {
            get {
                return Children.FirstOrDefault (ix => ix.Name == name);
            }
            set {
                Children.First (ix => ix.Name == Name).Replace (value);
            }
        }

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents the current node
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current node</returns>
        public override string ToString ()
        {
            var retVal = "";
            if (!string.IsNullOrEmpty (Name))
                retVal += "Name=" + Name;
            if (Value != null)
                retVal += ", Value=" + Value;
            if (Children.Count > 0)
                retVal += ", Count=" + Children.Count;
            retVal = retVal.Trim (',', ' ');
            return retVal;
        }
        
        /*
         * Does actual comparison of two non-null Node values
         */
        private int CompareValueObjects (object value, object rhsValue)
        {
            if (value == null && rhsValue == null)
                return 0;
            if (value.GetType () != rhsValue.GetType ()) {
                return String.Compare(value.GetType ().ToString (), rhsValue.GetType ().ToString (), StringComparison.Ordinal);
            }
            var thisValue = value as IComparable;
            if (thisValue == null)
                throw new ArgumentException ("Cannot compare objects of type; '" + value.GetType () + "'");
            return thisValue.CompareTo (rhsValue);
        }
    }
}
