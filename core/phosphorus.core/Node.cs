/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace phosphorus.core
{
    [Serializable]
    public class Node : IComparable
    {
        private string _name;

        public Node ()
        {
            Children = new List<Node> ();
            Name = string.Empty;
        }

        public Node (string name)
            : this ()
        {
            Name = name;
        }

        public Node (string name, object value)
            : this (name)
        {
            Value = value;
        }

        public Node (string name, object value, IEnumerable<Node> children)
            : this (name, value)
        {
            Children = new List<Node> (children);
            foreach (var idx in Children) {
                idx.Parent = this;
            }
        }

        public string Name {
            get {
                return _name;
            }
            set {
                if (value == null)
                    throw new ArgumentException ("you cannot set a node's name to 'null'");
                _name = value;
            }
        }

        public object Value {
            get;
            set;
        }

        public T Get<T> (T defaultValue = default (T))
        {
            if (Value == null)
                return defaultValue;
            return (T)Convert.ChangeType (Value, typeof(T));
        }

        public List<Node> Children {
            get;
            private set;
        }

        public Node Parent { get; private set; }

        public Node FirstChild {
            get {
                if (Children.Count > 0)
                    return Children [0];
                return null;
            }
        }

        public Node LastChild {
            get {
                if (Children.Count > 0)
                    return Children [Children.Count - 1];
                return null;
            }
        }

        public Node PreviousSibling {
            get {
                if (Parent == null)
                    return null;
                var idxNo = Parent.Children.IndexOf (this);
                idxNo -= 1;
                if (idxNo >= 0)
                    return Parent.Children [idxNo];
                return null;
            }
        }

        public Node NextSibling {
            get {
                if (Parent == null)
                    return null;
                var idxNo = Parent.Children.IndexOf (this);
                idxNo += 1;
                if (idxNo < Parent.Children.Count)
                    return Parent.Children [idxNo];
                return null;
            }
        }

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

        public Node Root {
            get {
                var idxNode = this;
                while (idxNode.Parent != null)
                    idxNode = idxNode.Parent;
                return idxNode;
            }
        }

        public Node UnTie ()
        {
            Parent.Children.Remove (this);
            Parent = null;
            return this;
        }

        public Node Replace (Node node)
        {
            if (node == null) {
                throw new ArgumentException ("cannot replace a node with null");
            }
            node.Parent = Parent;
            Parent.Children [Parent.Children.IndexOf (this)] = node;
            Parent = null;
            return node;
        }

        public Node Add (Node node)
        {
            if (node.Parent != null)
                node.Parent.Children.Remove (node);
            node.Parent = this;
            Children.Add (node);
            return this;
        }

        public Node Add (string name)
        {
            return Add (new Node (name));
        }

        public Node Add (string name, object value)
        {
            return Add (new Node (name, value));
        }

        public Node Add (string name, object value, IEnumerable<Node> nodes)
        {
            return Add (new Node (name, value, nodes));
        }

        public Node Insert (int index, Node node)
        {
            node.Parent = this;
            Children.Insert (index, node);
            return this;
        }

        public Node AddRange (IEnumerable<Node> nodes)
        {
            foreach (var idxNode in new List<Node> (nodes)) {
                Add (idxNode);
            }
            return this;
        }

        public Node Clone ()
        {
            var retVal = new Node (Name = Name, Value = Value);
            foreach (var idxChild in Children) {
                retVal.Add (idxChild.Clone ());
            }
            return retVal;
        }

        public int CompareTo (object rhs)
        {
            var rhsNode = rhs as Node;
            if (rhsNode == null)
                return 1;
            return CompareTo (rhsNode);
        }

        public int CompareTo (Node rhs)
        {
            var retVal = string.Compare (Name, rhs.Name, StringComparison.Ordinal);
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
            if (Children.Count < rhs.Children.Count) {
                return -1;
            } else if (Children.Count > rhs.Children.Count) {
                return 1;
            } else {
                for (var idxNo = 0; idxNo < Children.Count; idxNo ++) {
                    retVal = Children [idxNo].CompareTo (rhs.Children [idxNo]);
                    if (retVal != 0)
                        return retVal;
                }
            }
            return 0;
        }
        
        public Node this [int index]
        {
            get {
                return Children [index];
            }
            set {
                this [index].Replace (value);
            }
        }

        public Node this [string name]
        {
            get {
                return Children.Find (ix => ix.Name == name);
            }
            set {
                this [name].Replace (value);
            }
        }

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

        private int CompareValueObjects (object value, object rhsValue)
        {
            if (value == null && rhsValue == null)
                return 0;
            if (value.GetType () != rhsValue.GetType ()) {
                return string.Compare (value.GetType ().ToString (), rhsValue.GetType ().ToString (), StringComparison.Ordinal);
            }
            var thisValue = value as IComparable;
            if (thisValue == null)
                throw new ArgumentException ("cannot compare objects of type; '" + value.GetType () + "'");
            return thisValue.CompareTo (rhsValue);
        }
    }
}
