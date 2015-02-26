
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable PossibleNullReferenceException

namespace phosphorus.core
{
    /// <summary>
    /// arguments passed into and returned from Active Events
    /// </summary>
    [Serializable]
    public class Node : IComparable
    {
        /// <summary>
        /// DNA code for Node
        /// </summary>
        public class Dna : IComparable, IConvertible
        {
            internal readonly List<int> Value;

            private Dna ()
            {
                Value = new List<int> ();
            }

            public Dna (string value)
            {
                Value = new List<int> ();
                foreach (var idxInt in value.Split (new[] { '-' }, StringSplitOptions.RemoveEmptyEntries)) {
                    Value.Add (int.Parse (idxInt));
                }
            }

            internal Dna (Node node)
                : this ()
            {
                var idxNode = node;
                while (idxNode.Parent != null) {
                    for (var idxNo = 0; idxNo < idxNode.Parent._children.Count; idxNo ++) {
                        if (idxNode == idxNode.Parent._children [idxNo]) {
                            Value.Insert (0, idxNo);
                            break;
                        }
                    }
                    idxNode = idxNode.Parent;
                }
            }

            /// <summary>
            /// returns the depth of the DNA, meaning the number of ancestor nodes from root node the current node has
            /// </summary>
            /// <value>depth</value>
            public int Count {
                get {
                    return Value.Count;
                }
            }

            /// <summary>
            /// determines if is given value is a DNA path or not
            /// </summary>
            /// <returns><c>true</c> if is value is dna; otherwise, <c>false</c></returns>
            /// <param name="dna">value to check if is dna</param>
            public static bool IsPath (string dna)
            {
                return dna != null && dna.All (idx => "0123456789-".IndexOf (idx) != -1);
            }

            /// <summary>
            /// compares two nodes for equality
            /// </summary>
            /// <param name="lhs">left hand side node</param>
            /// <param name="rhs">right hand side node</param>
            public static bool operator == (Dna lhs, Dna rhs)
            {
                return lhs != null && lhs.CompareTo (rhs) == 0;
            }

            /// <summary>
            /// compares two nodes for not-equality
            /// </summary>
            /// <param name="lhs">left hand side node</param>
            /// <param name="rhs">right hand side node</param>
            public static bool operator != (Dna lhs, Dna rhs)
            {
                return lhs != null && lhs.CompareTo (rhs) != 0;
            }

            /// <summary>
            /// compares two nodes to see if left hand side is more than right hand side node
            /// </summary>
            /// <param name="lhs">left hand side node</param>
            /// <param name="rhs">right hand side node</param>
            public static bool operator > (Dna lhs, Dna rhs)
            {
                return lhs.CompareTo (rhs) == 1;
            }

            /// <summary>
            /// compares two nodes to see if left hand side is less than right hand side node
            /// </summary>
            /// <param name="lhs">left hand side node</param>
            /// <param name="rhs">right hand side node</param>
            public static bool operator < (Dna lhs, Dna rhs)
            {
                return lhs.CompareTo (rhs) == -1;
            }

            /// <summary>
            /// compares two nodes to see if left hand side is more than or equal to right hand side node
            /// </summary>
            /// <param name="lhs">left hand side node</param>
            /// <param name="rhs">right hand side node</param>
            public static bool operator >= (Dna lhs, Dna rhs)
            {
                return lhs.CompareTo (rhs) != -1;
            }

            /// <summary>
            /// compares two nodes to see if left hand side is less than or equal to right hand side node
            /// </summary>
            /// <param name="lhs">left hand side node</param>
            /// <param name="rhs">right hand side node</param>
            public static bool operator <= (Dna lhs, Dna rhs)
            {
                return lhs.CompareTo (rhs) != 1;
            }

            /// <summary>
            /// returns the logical AND of the given DNA codes.  basically finds the common ancestor's DNA code
            /// </summary>
            /// <param name="lhs">left hand side node</param>
            /// <param name="rhs">right hand side node</param>
            public static Dna operator & (Dna lhs, Dna rhs)
            {
                var retVal = new Dna ();
                for (var idxNo = 0; idxNo < lhs.Value.Count && idxNo < rhs.Value.Count; idxNo++) {
                    if (lhs.Value [idxNo].CompareTo (rhs.Value [idxNo]) == 0)
                        retVal.Value.Add (idxNo);
                    else
                        break;
                }
                return retVal;
            }

            public override bool Equals (object obj)
            {
                if (!(obj is Dna))
                    return false;
                var rhs = (Dna) obj;
                if (Count != rhs.Count)
                    return false;
                for (var idxNo = 0; idxNo < Count; idxNo++) {
                    if (Value [idxNo] != rhs.Value [idxNo])
                        return false;
                }
                return true;
            }

            public override int GetHashCode ()
            {
                return Value.GetHashCode ();
            }

            public override string ToString ()
            {
                var tmp = Value.Aggregate ("", (current, idx) => current + ("-" + idx));
                tmp = tmp.Trim ('-');
                return tmp;
            }

            public int CompareTo (object obj)
            {
                if (!(obj is Dna))
                    throw new ArgumentException ("cannot compare DNA to: " + (obj ?? "[null]"));
                var rhs = (Dna) obj;

                for (var idxNo = 0; idxNo < Value.Count && idxNo < rhs.Value.Count; idxNo ++) {
                    var cmpVals = Value [idxNo].CompareTo (rhs.Value [idxNo]);
                    if (cmpVals != 0)
                        return cmpVals;
                }
                if (Value.Count > rhs.Value.Count)
                    return 1;
                if (Value.Count < rhs.Value.Count)
                    return -1;
                return 0;
            }

            public TypeCode GetTypeCode ()
            {
                return TypeCode.Object;
            }

            public bool ToBoolean (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a DNA path to Boolean");
            }

            public byte ToByte (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a DNA path to byte");
            }

            public char ToChar (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a DNA path to char");
            }

            public DateTime ToDateTime (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a DNA path to DateTime");
            }

            public decimal ToDecimal (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a DNA path to decimal");
            }

            public double ToDouble (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a DNA path to double");
            }

            public short ToInt16 (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a DNA path to short");
            }

            public int ToInt32 (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a DNA path to int");
            }

            public long ToInt64 (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a DNA path to long");
            }

            public sbyte ToSByte (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a DNA path to sbyte");
            }

            public float ToSingle (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a DNA path to float");
            }

            public string ToString (IFormatProvider provider)
            {
                return ToString ();
            }

            public object ToType (Type conversionType, IFormatProvider provider)
            {
                if (conversionType != typeof(string))
                    throw new ApplicationException ("cannot convert a DNA path to; " + conversionType);
                return ToString ();
            }

            public ushort ToUInt16 (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a DNA path to ushort");
            }

            public uint ToUInt32 (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a DNA path to uint");
            }

            public ulong ToUInt64 (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a DNA path to ulong");
            }
        }

        private readonly List<Node> _children;
        private string _name;

        /// <summary>
        /// delegate for iterating children nodes
        /// </summary>
        public delegate T NodeIterator<out T> (Node node);

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.core.Node"/> class
        /// </summary>
        public Node ()
        {
            _children = new List<Node> ();
            Name = string.Empty;
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
        public Node (string name, object value)
            : this (name)
        {
            Value = value;
        }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.core.Node"/> class
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="value">value of node</param>
        /// <param name="children">initial children for node</param>
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
        /// gets or sets the name of the node
        /// </summary>
        /// <value>the name</value>
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

        /// <summary>
        /// gets or sets the value of the node
        /// </summary>
        /// <value>the value</value>
        public object Value {
            get;
            set;
        }

        /// <summary>
        /// returns the number of chilren nodes
        /// </summary>
        /// <value>number of children</value>
        public int Count {
            get {
                return _children.Count;
            }
        }

        /// <summary>
        /// returns the value of this instance as typeof(T). converts to T if necessary
        /// </summary>
        /// <typeparam name="T">type to return</typeparam>
        public T Get<T> (ApplicationContext context, T defaultValue = default (T))
        {
            return Utilities.Convert (Value, context, defaultValue);
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
        /// gets the children of this instance, while also untying them
        /// </summary>
        /// <value>its children after being untied</value>
        public IEnumerable<Node> UntieChildren ()
        {
            while (_children.Count > 0) {
                yield return _children [0].UnTie ();
            }
        }

        /// <summary>
        /// returns DNA code for Node
        /// </summary>
        /// <value>the DNA code, or position in Node tree</value>
        public Dna Path {
            get {
                return new Dna (this);
            }
        }

        /// <summary>
        /// returns the parent of node
        /// </summary>
        /// <value>parent node</value>
        public Node Parent { get; private set; }

        /// <summary>
        /// returns the first child of the node
        /// </summary>
        /// <value>the first child</value>
        public Node FirstChild {
            get {
                if (_children.Count > 0)
                    return _children [0];
                return null;
            }
        }

        /// <summary>
        /// returns the first child of the node not having the given name
        /// </summary>
        /// <value>the first child</value>
        public Node FirstChildNotOf (string name)
        { return _children.FirstOrDefault (idx => idx.Name != name); }

        /// <summary>
        /// returns the last child of the node
        /// </summary>
        /// <value>the last child</value>
        public Node LastChild {
            get {
                if (_children.Count > 0)
                    return _children [_children.Count - 1];
                return null;
            }
        }

        /// <summary>
        /// returns the previous sibling of the current node
        /// </summary>
        /// <value>the previous sibling</value>
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
        /// returns the next sibling of the current node
        /// </summary>
        /// <value>the next sibling</value>
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
        /// returns the previous node from the current node
        /// </summary>
        /// <value>the previous node</value>
        public Node PreviousNode {
            get {
                var idx = PreviousSibling;
                if (idx != null) {
                    while (idx.Count > 0) {
                        idx = idx [idx.Count - 1];
                    }
                    return idx;
                }
                return Parent;
            }
        }

        /// <summary>
        /// returns the next node from the current node
        /// </summary>
        /// <value>the next node</value>
        public Node NextNode {
            get {
                if (Count > 0)
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
        /// returns the root node of the tree
        /// </summary>
        /// <value>the root node</value>
        public Node Root {
            get {
                var idxNode = this;
                while (idxNode.Parent != null)
                    idxNode = idxNode.Parent;
                return idxNode;
            }
        }

        /// <summary>
        /// unties the node from its parent
        /// </summary>
        public Node UnTie ()
        {
            Parent._children.Remove (this);
            Parent = null;
            return this;
        }

        /// <summary>
        /// replace the specified node
        /// </summary>
        /// <param name="node">node to replace current node with</param>
        public Node Replace (Node node)
        {
            if (node == null) {
                throw new ArgumentException ("cannot replace a node with null");
            }
            node.Parent = Parent;
            Parent._children [Parent._children.IndexOf (this)] = node;
            Parent = null;
            return node;
        }

        /// <summary>
        /// finds a node according to the given dna
        /// </summary>
        /// <param name="dna">dna of node to find</param>
        public Node Find (Dna dna)
        {
            if (dna.Equals (null))
                return null;
            var idxNode = this;
            while (idxNode.Parent != null)
                idxNode = idxNode.Parent;
            foreach (var idxNo in dna.Value) {
                if (idxNo < 0 || idxNo >= idxNode._children.Count)
                    return null;
                idxNode = idxNode [idxNo];
            }
            return idxNode;
        }

        /// <summary>
        /// finds the node matching the given DNA
        /// </summary>
        /// <returns>the node, if found, otherwise null</returns>
        /// <param name="dna">the DNA or Path of the node to return</param>
        public Node FindDna (string dna)
        {
            if (string.IsNullOrEmpty (dna))
                return null;
            return Find (new Dna (dna));
        }

        /// <summary>
        /// finds the first matching node according to the given predicate
        /// </summary>
        /// <param name="functor">predicate that nodes must match</param>
        public Node Find (Predicate<Node> functor)
        {
            return _children.Find (functor);
        }

        /// <summary>
        /// finds all nodes according to the given predicate
        /// </summary>
        /// <param name="functor">functor nodes must match</param>
        public IEnumerable<Node> FindAll (Predicate<Node> functor)
        {
            return _children.FindAll (functor);
        }

        /// <summary>
        /// finds the first node having the given name
        /// </summary>
        /// <param name="name">name of node to return</param>
        public Node Find (string name)
        {
            return Find (idx => idx.Name == name);
        }

        /// <summary>
        /// finds all nodes having the given name
        /// </summary>
        /// <param name="name">name of node to return</param>
        public IEnumerable<Node> FindAll (string name)
        {
            return FindAll (idx => idx.Name == name);
        }

        /// <summary>
        /// returns all values of children nodes as type T
        /// </summary>
        /// <returns>the children values</returns>
        /// <typeparam name="T">the type you wish to convert values to</typeparam>
        public IEnumerable<T> GetChildrenValues<T> (
            ApplicationContext context, 
            Predicate<Node> functor = null)
        {
            if (functor == null) {
                foreach (var idx in _children) {
                    yield return idx.Get<T> (context);
                }
            } else {
                foreach (var idx in _children) {
                    if (functor (idx))
                        yield return idx.Get<T> (context);
                }
            }
        }

        /// <summary>
        /// iterates every single node, invoking the given functor, and if functor does not return
        /// default (T), it will yield that T value as a result back to caller
        /// </summary>
        /// <param name="functor">match delegate</param>
        /// <typeparam name="T">type of object you wish to construct from node iterator</typeparam>
        public IEnumerable<T> ConvertChildren<T> (NodeIterator<T> functor) {
            return _children.Select (idx => functor (idx)).Where (retVal => retVal != null && !retVal.Equals (default (T)));
        }

        /// <summary>
        /// finds the first node having the given name, if no node exists with given name,
        /// then a new node with the given name will be created and returned to caller
        /// </summary>
        /// <param name="name">name of node to return</param>
        public Node FindOrCreate (string name)
        {
            var retVal = _children.Find (idx => idx.Name == name);
            if (retVal != null)
                return retVal;
            return Add (new Node (name)).LastChild;
        }

        /// <summary>
        /// finds the first node having the given name and value, if no matching node exists,
        /// then a new node with the given name and value will be created and returned to caller
        /// </summary>
        /// <param name="name">name of node to return</param>
        /// <param name="value">value of node to return</param>
        public Node FindOrCreate (string name, object value)
        {
            var retVal = _children.Find (idx => idx.Name == name && ((value == null && idx.Value == null) || (value != null && value.Equals (idx.Value))));
            if (retVal != null)
                return retVal;
            return Add (new Node (name, value)).LastChild;
        }

        /// <summary>
        /// finds the first node having the given name
        /// </summary>
        /// <param name="name">name of node to return</param>
        /// <param name="context">application context</param>
        /// <param name="defaultValue">default value to use if no child with the given name is found</param>
        public T GetChildValue<T> (string name, ApplicationContext context, T defaultValue = default (T))
        {
            var child = _children.Find (idx => idx.Name == name);
            return child == null ? defaultValue : child.Get (context, defaultValue);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public Node Remove (Node node)
        {
            if (!_children.Remove (node))
                throw new ArgumentException ("node doesn't belong to collection");
            return this;
        }

        /// <summary>
        /// removes the node at the specified index
        /// </summary>
        /// <param name="index">where node to remove recides in the children collection</param>
        public Node RemoveAt (int index)
        {
            _children [index].UnTie ();
            return this;
        }

        /// <summary>
        /// removes all children nodes matching given predicate
        /// </summary>
        /// <param name="functor">predicate</param>
        public Node RemoveAll (Predicate<Node> functor)
        {
            _children.RemoveAll (functor);
            return this;
        }

        /// <summary>
        /// removes all nodes with given name
        /// </summary>
        /// <param name="name">name of nodes to remove</param>
        public Node RemoveAll (string name)
        {
            return RemoveAll (idx => idx.Name == name);
        }

        /// <summary>
        /// sorts the children of the node
        /// </summary>
        /// <param name="comparison">comparison delegate</param>
        public Node Sort (Comparison<Node> comparison)
        {
            _children.Sort (comparison);
            return this;
        }

        /// <summary>
        /// sorts the children of the node by name
        /// </summary>
        public Node Sort ()
        {
            _children.Sort ((lhs, rhs) => String.Compare (lhs.Name, rhs.Name, StringComparison.Ordinal));
            return this;
        }

        /// <summary>
        /// adds a child node to its children collection
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
        /// adds a child node to its children collection with given name
        /// </summary>
        /// <param name="name">name of node to add</param>
        public Node Add (string name)
        {
            return Add (new Node (name));
        }

        /// <summary>
        /// adds a child node to its children collection with given name and value
        /// </summary>
        /// <param name="name">name of node to add</param>
        /// <param name="value">value of node to add</param>
        public Node Add (string name, object value)
        {
            return Add (new Node (name, value));
        }

        /// <summary>
        /// adds a child node to its children collection with given name, value and children
        /// </summary>
        /// <param name="name">name of node to add</param>
        /// <param name="value">value of node to add</param>
        /// <param name="nodes">initial child collection of node</param>
        public Node Add (string name, object value, IEnumerable<Node> nodes)
        {
            return Add (new Node (name, value, nodes));
        }

        /// <summary>
        /// inserts a child node to its children collection at the specified index
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
        /// adds a range of nodes
        /// </summary>
        /// <param name="nodes">nodes to add</param>
        public Node AddRange (IEnumerable<Node> nodes)
        {
            foreach (var idxNode in new List<Node> (nodes)) {
                Add (idxNode);
            }
            return this;
        }

        /// <summary>
        /// clears the children collection
        /// </summary>
        public Node Clear ()
        {
            _children.Clear ();
            return this;
        }

        /// <summary>
        /// clones this instance
        /// </summary>
        public Node Clone ()
        {
            var retVal = new Node {Name = Name, Value = Value};
            foreach (var idxChild in _children) {
                retVal.Add (idxChild.Clone ());
            }
            return retVal;
        }

        /// <summary>
        /// IComparable implementation, compares node to another object
        /// </summary>
        /// <returns>-1 if this node is "less than", +1 if rhs is "less than", 0 if objects are equal</returns>
        /// <param name="rhs">the object to compare the node against</param>
        public int CompareTo (object rhs)
        {
            var rhsNode = rhs as Node;
            if (rhsNode == null)
                return 1;
            return CompareTo (rhsNode);
        }

        /// <summary>
        /// compares two nodes for equality and returns -1 if this is "less than" rhs, +1 if this is "more than" rhs, and 0
        /// if they are equal, meaning they contain similar nodes
        /// </summary>
        /// <returns>-1, 0 or 1 depending upon the equality of the this and rhs</returns>
        /// <param name="rhs">node to compare again the current instance for equality</param>
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
                for (var idxNo = 0; idxNo < Count; idxNo ++) {
                    retVal = _children [idxNo].CompareTo (rhs._children [idxNo]);
                    if (retVal != 0)
                        return retVal;
                }
            }
            return 0;
        }

        /*
         * does actual comparison of two non-null Node values
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
                throw new ArgumentException ("cannot compare objects of type; '" + value.GetType () + "'");
            return thisValue.CompareTo (rhsValue);
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
                this [index].Replace (value);
            }
        }

        /// <summary>
        /// gets or sets the first node in the children collection matching the given name
        /// </summary>
        /// <param name="name">name of node to retrieve or set</param>
        public Node this [string name]
        {
            get {
                return Find (name);
            }
            set {
                Find (name).Replace (value);
            }
        }

        /// <summary>
        /// returns a <see cref="System.String"/> that represents the current <see cref="phosphorus.core.Node"/>
        /// </summary>
        /// <returns>a <see cref="System.String"/> that represents the current <see cref="phosphorus.core.Node"/></returns>
        public override string ToString ()
        {
            var retVal = "";
            if (!string.IsNullOrEmpty (Name))
                retVal += "Name=" + Name;
            if (Value != null)
                retVal += ", Value=" + Value;
            if (Count > 0)
                retVal += ", Count=" + Count;
            if (Path.Count > 0)
                retVal += ", Path=" + Path;
            retVal = retVal.Trim (',', ' ');
            return retVal;
        }
    }
}
