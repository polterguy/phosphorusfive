/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.Linq;
using System.Collections.Generic;

namespace p5.core
{
    /// <summary>
    ///     Class wrapping arguments passed into and returned from Active Events.
    /// </summary>
    [Serializable]
    public class Node : IComparable
    {
        private readonly List<Node> _children;
        private string _name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        public Node ()
            : this ("")
        { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="name">Name of node, cannot be null</param>
        public Node (string name)
            : this (name, null)
        { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="name">Name of node, cannot be null</param>
        /// <param name="value">Value of node, can be any object, including null</param>
        public Node (string name, object value)
            : this (name, value, null)
        { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="name">Name of node, cannot be null</param>
        /// <param name="value">Value of node, can be any object, including null</param>
        /// <param name="children">Initial children collection for node</param>
        public Node (string name, object value, IEnumerable<Node> children)
        {
            Name = name;
            Value = value;
            _children = new List<Node> ();
            AddRange (children);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.core.Node"/> class.
        /// </summary>
        /// <param name="name">Name of node, cannot be null</param>
        /// <param name="value">Value of node, can be any object, including null</param>
        /// <param name="children">Initial children collection for node</param>
        public Node (string name, object value, params Node[] children)
        {
            Name = name;
            Value = value;
            _children = new List<Node> ();
            AddRange (children);
        }

        /// <summary>
        ///     Gets or sets the name of the Node.
        /// </summary>
        /// <value>The Node's new name</value>
        public string Name
        {
            get {
                return _name;
            }
            set {
                if (value == null)
                    throw new ArgumentException ("You cannot set a node's name to 'null'", nameof (value));
                _name = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value of the node.
        /// </summary>
        /// <value>The Node's new value</value>
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        ///     Returns the children of current Node.
        /// </summary>
        /// <value>Its children nodes</value>
        public IEnumerable<Node> Children
        {
            get {
                return _children;
            }
        }

        /// <summary>
        ///     Returns the parent of current Node.
        /// </summary>
        /// <value>The parent node of this instance</value>
        public Node Parent { get; private set; }

        /// <summary>
        ///     Returns the number of children node this instance has.
        /// </summary>
        /// <value>Number of children</value>
        public int Count
        {
            get { return _children.Count; }
        }

        /// <summary>
        ///     Gets or sets the child node at the specified index.
        /// 
        ///     Notice, if you try to get a node, outside of the boundaries of children, this method will return null to caller, and not throw.
        /// </summary>
        /// <param name="index">Index of node to retrieve or set</param>
        public Node this [int index]
        {
            get {
                return _children.Count > index && index >= 0 ? _children [index] : null;
            }
            set {
                this [index].Replace (value);
            }
        }

        /// <summary>
        ///     Gets or sets the first node in the children collection matching the given name.
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
        ///     Returns the first child of the node, if there are any children.
        /// </summary>
        /// <value>The current node's first child node, or null of node has no children</value>
        public Node FirstChild
        {
            get {
                return Children.FirstOrDefault ();
            }
        }

        /// <summary>
        ///     Returns the last child of the node, of there are any children.
        /// </summary>
        /// <value>The current node's last child node, or null of node has no children</value>
        public Node LastChild
        {
            get {
                return Children.LastOrDefault ();
            }
        }

        /// <summary>
        ///     Returns the previous sibling of the current node, if there are any.
        /// </summary>
        /// <value>The current node's previous sibling node, or null of node has no younger siblings</value>
        public Node PreviousSibling
        {
            get {
                return Parent? [Parent.IndexOf (this) - 1] ?? null;
            }
        }

        /// <summary>
        ///     Returns the next sibling of the current node, if there are any.
        /// </summary>
        /// <value>The current node's next sibling node, or null of node has no elder siblings</value>
        public Node NextSibling
        {
            get {
                return Parent? [Parent.IndexOf (this) + 1] ?? null;
            }
        }

        /// <summary>
        ///     Returns the previous node from the current node.
        /// 
        ///     The previous node, is the previous node in the tree as a graph object, using breadth first search.
        /// </summary>
        /// <value>The current node's previous node</value>
        public Node PreviousNode
        {
            get {
                var idx = PreviousSibling;
                for (; idx != null && idx.Count > 0; idx = idx.LastChild) { }
                return idx ?? Parent;
            }
        }

        /// <summary>
        ///     Returns the next node from the current node.
        /// 
        ///     The next node, is the next node in the tree as a graph object, using breadth first search.
        /// </summary>
        /// <value>The current node's next node</value>
        public Node NextNode
        {
            get {

                // Simple cases first.
                if (Count > 0)
                    return FirstChild;
                if (NextSibling != null)
                    return NextSibling;

                // Hard case, need to find NextSibling for first ancestor node that has a NextSibling.
                for (var idx = Parent; idx != null; idx = idx.Parent) {
                    if (idx.NextSibling != null)
                        return idx.NextSibling;
                }
                return null;
            }
        }

        /// <summary>
        ///     Returns the root node of the tree.
        /// </summary>
        /// <value>The root node of the tree the current node belongs to</value>
        public Node Root
        {
            get {
                var idxNode = this;
                for (; idxNode.Parent != null; idxNode = idxNode.Parent) { }
                return idxNode;
            }
        }

        /// <summary>
        ///     Gets the number of ancestors from this node to root.
        /// </summary>
        /// <value>The offset to root</value>
        public int OffsetToRoot
        {
            get {
                var idxNo = 0;
                for (var idxNode = this; idxNode.Parent != null; idxNo += 1, idxNode = idxNode.Parent) { }
                return idxNo;
            }
        }

        /// <summary>
        ///     Returns the index of the given node in children collection.
        /// </summary>
        /// <returns>The index of the specified node</returns>
        /// <param name="node">Node to return the index of</param>
        public int IndexOf (Node node)
        {
            return _children.IndexOf (node);
        }

        /// <summary>
        ///     Returns the value of this instance as typeof(T).
        /// </summary>
        /// <typeparam name="T">Type to convert value to</typeparam>
        public T Get<T> (ApplicationContext context, T defaultValue = default (T))
        {
            return Utilities.Convert (context, Value, defaultValue);
        }

        /// <summary>
        ///     Unties the node from its parent children collection.
        /// </summary>
        /// <returns>Self</returns>
        public Node UnTie ()
        {
            Parent?.Remove (this);
            Parent = null;
            return this;
        }

        /// <summary>
        ///     Replaces the current node with another node.
        /// </summary>
        /// <param name="node">Node to replace the current node with</param>
        /// <returns>The newly inserted node</returns>
        public Node Replace (Node node)
        {
            // Sanity check.
            if (node == null)
                throw new ArgumentException ("Cannot replace a node with null");

            // To make sure we detach it from its previous parent, if any.
            node.UnTie ();
            node.Parent = Parent;
            Parent._children [Parent._children.IndexOf (this)] = node;

            // Making sure we remove current node's Parent.
            Parent = null;

            // Notice, returning NEW node, and not "this".
            return node;
        }

        /// <summary>
        ///     Returns the first node matching the given predicate, or null if none.
        /// </summary>
        /// <param name="functor">Predicate to use for find</param>
        /// <returns>The first node matching the given predicate</returns>
        public Node Find (Predicate<Node> functor)
        {
            return _children.Find (functor);
        }

        /// <summary>
        ///     Finds, or inserts, the first node having the given name, and returns that node to caller.
        /// </summary>
        /// <param name="name">Name of node to return or create</param>
        /// <returns>The node that was found, or inserted</returns>
        public Node FindOrInsert (string name, int index = -1)
        {
            var retVal = Find (ix => ix.Name == name);
            if (retVal != null)
                return retVal;
            if (index == -1)
                return Add (new Node (name)).LastChild;
            else
                return Insert (index, new Node (name))[index];
        }

        /// <summary>
        ///     Removes the specified child node.
        /// 
        ///     Throws exception if node does not belong to children collection.
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Self</returns>
        public Node Remove (Node node)
        {
            if (!_children.Remove (node))
                throw new ArgumentException ("Node doesn't belong to collection");
            node.Parent = null;
            return this;
        }

        /// <summary>
        ///     Removes all nodes matching the given predicate.
        /// </summary>
        /// <param name="functor">Predicate for removal process</param>
        /// <returns>Self</returns>
        public Node RemoveAll (Predicate<Node> functor)
        {
            _children.RemoveAll (functor);
            return this;
        }

        /// <summary>
        ///     Adds a child node to the current node's children collection.
        /// </summary>
        /// <param name="name">Name of node to add</param>
        /// <returns>Self</returns>
        public Node Add (string name)
        {
            return Add (new Node (name));
        }

        /// <summary>
        ///     Adds a child node to the current node's children collection, with the specified value.
        /// </summary>
        /// <param name="name">Name of node to add</param>
        /// <param name="value">value of node to add</param>
        /// <returns>Self</returns>
        public Node Add (string name, object value)
        {
            return Add (new Node (name, value));
        }

        /// <summary>
        ///     Adds a child node to the current node's children collection, with the specified name, value and children collection.
        /// </summary>
        /// <param name="name">Name of node to add</param>
        /// <param name="value">Value of node to add</param>
        /// <param name="nodes">Initial child collection of node</param>
        /// <returns>Self</returns>
        public Node Add (string name, object value, IEnumerable<Node> nodes)
        {
            return Add (new Node (name, value, nodes));
        }

        /// <summary>
        ///     Adds a child node to the current node's children collection, with the specified name, value and children collection.
        /// </summary>
        /// <param name="name">Name of node to add</param>
        /// <param name="value">Value of node to add</param>
        /// <param name="nodes">Initial child collection of node</param>
        /// <returns>Self</returns>
        public Node Add (string name, object value, params Node[] nodes)
        {
            return Add (new Node (name, value, nodes));
        }

        /// <summary>
        ///     Adds a child node to the current node's children collection.
        /// </summary>
        /// <param name="node">Node to add</param>
        /// <returns>Self</returns>
        public Node Add (Node node)
        {
            if (node == null)
                throw new ArgumentException ("Tried to add a null Node to another Node", nameof (node));
            node.UnTie ();
            node.Parent = this;
            _children.Add (node);
            return this;
        }

        /// <summary>
        ///     Adds a range of nodes.
        /// </summary>
        /// <param name="nodes">Nodes to add</param>
        /// <returns>Self</returns>
        public Node AddRange (IEnumerable<Node> nodes)
        {
            return AddRange (nodes.ToArray ());
        }

        /// <summary>
        ///     Adds a range of nodes
        /// </summary>
        /// <param name="nodes">Nodes to add</param>
        /// <returns>Self</returns>
        public Node AddRange (params Node[] nodes)
        {
            // Sanity check.
            if (nodes == null)
                return this;

            // Making sure we untie all nodes from any previous parent nodes, and set their new parent to "this".
            foreach (var idx in nodes) {
                idx.UnTie ();
                idx.Parent = this;
            }
            _children.AddRange (nodes);
            return this;
        }

        /// <summary>
        ///     Inserts a child node to the current node's children collection, at the specified index.
        /// </summary>
        /// <param name="index">Where to insert node</param>
        /// <param name="node">Node to add</param>
        /// <returns>Self</returns>
        /// <returns>Self</returns>
        public Node Insert (int index, Node node)
        {
            node.UnTie ();
            node.Parent = this;
            _children.Insert (index, node);
            return this;
        }

        /// <summary>
        ///     Inserts a range of children nodes to the current node's children collection, at the specified index.
        /// </summary>
        /// <param name="index">Where to insert nodes</param>
        /// <param name="nodes">Nodes to add</param>
        /// <returns>Self</returns>
        public Node InsertRange (int index, IEnumerable<Node> nodes)
        {
            return InsertRange (index, nodes.ToArray ());
        }

        /// <summary>
        ///     Inserts a range of children nodes to the current node's children collection, at the specified index.
        /// </summary>
        /// <param name="index">Where to insert</param>
        /// <param name="nodes">Nodes to add</param>
        /// <returns>Self</returns>
        public Node InsertRange (int index, params Node[] nodes)
        {
            foreach (var idx in nodes) {
                idx.UnTie ();
                idx.Parent = this;
            }
            _children.InsertRange (index, nodes);
            return this;
        }

        /// <summary>
        ///     Clears the children collection.
        /// </summary>
        /// <returns>Self</returns>
        public Node Clear ()
        {
            foreach (var idx in _children) {
                idx.Parent = null;
            }
            _children.Clear ();
            return this;
        }

        /// <summary>
        ///     Returns the Value of the first Children node, matching the given name, converting value to type T.
        /// </summary>
        /// <param name="name">Name of node to return</param>
        /// <param name="context">Application Context</param>
        /// <param name="defaultValue">Default value to use, if no child with the given name is found</param>
        public T GetChildValue<T> (string name, ApplicationContext context, T defaultValue = default (T))
        {
            var retVal = Find (ix => ix.Name == name);
            return retVal == null ? defaultValue : retVal.Get (context, defaultValue);
        }

        /// <summary>
        ///     Sorts the children of the current node.
        /// </summary>
        /// <param name="functor">Comparison delegate to use for sorting operation</param>
        /// <returns>Self</returns>
        public Node Sort (Comparison<Node> functor)
        {
            _children.Sort (functor);
            return this;
        }

        /// <summary>
        ///     Clones the current node.
        /// </summary>
        /// <returns>Cloned node</returns>
        public Node Clone ()
        {
            var retVal = new Node (Name, Value);
            retVal.AddRange (_children.Select (ix => ix.Clone ()));
            return retVal;
        }

        /// <summary>
        ///     IComparable implementation, compares current node to another object.
        /// </summary>
        /// <param name="rhs">The object to compare this Node against</param>
        /// <returns>-1 if this node is "less than" rhs, +1 if this is "more than" rhs, 0 if objects are equal</returns>
        public int CompareTo (object rhs)
        {
            var rhsNode = rhs as Node;
            if (rhsNode == null)
                return 1;
            return CompareTo (rhsNode);
        }

        /// <summary>
        ///     Compares current node to another node.
        /// </summary>
        /// <param name="rhs">The object to compare the node against</param>
        /// <returns>-1 if this node is "less than" rhs, +1 if this node is "more than" rhs, 0 if objects are equal</returns>
        public int CompareTo (Node rhs)
        {
            // Simple case first.
            var retVal = string.Compare (Name, rhs.Name);
            if (retVal != 0)
                return retVal;

            // Comparing values of objects, and returning results, if inequality was found.
            retVal = CompareValueObjects (Value, rhs.Value);
            if (retVal != 0)
                return retVal;

            // Doing a "deep comparison", first counting children, checking for inequality, before iterating each node in both 
            // children collections, comparing them to each other.
            if (Count < rhs.Count) {
                return -1;
            } else if (rhs.Count < Count) {
                return 1;
            } else {

                // Both nodes have similar values, names and number of children.
                // Hence, we need to compare one child against the other, looking for inequalities.
                for (var idxNo = 0; idxNo < _children.Count; idxNo ++) {

                    // Doing comparison on currently iterated nodes.
                    retVal = this [idxNo].CompareTo (rhs [idxNo]);

                    // If we found an inequality, we return it, otherwise we continue iteration.
                    if (retVal != 0)
                        return retVal;
                }
            }

            // Nodes are equal.
            return 0;
        }

        /*
         * Helper for above, does a comparison of two values.
         */
        private int CompareValueObjects (object value, object rhsValue)
        {
            // First the simple case, checking objects for "null".
            if (value == null)
                return rhsValue == null ? 0 : -1;
            else if (rhsValue == null)
                return 1;

            // Second comparison, making sure Types are equal.
            if (value.GetType () != rhsValue.GetType ())
                return string.Compare (value.GetType ().ToString (), rhsValue.GetType ().ToString (), false);

            // Resorting to IComparable, if none of the above yielded any differences, assuming value argument implements IComparable.
            return (value as IComparable).CompareTo (rhsValue);
        }
    }
}
