
/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace p5.core
{
    /// <summary>
    ///     Arguments passed into and returned from Active Events.
    /// 
    ///     All Active Events takes an instance of this class as its parameters. Since class is a tree structure, capable
    ///     of storing any objects as values of its nodes, you can effectively pass in any arguments you wish to your Active Events
    ///     using this class.
    /// 
    ///     Class is a "Genetic Tree" implementation, meaning it has a "materialized path" implementation, through its Path property.
    ///     The Path property is basically a list of integers, defining the exact position within the tree hierarchy, of your node.
    ///     Using this property, you can partition your tree, and determine if a specific node is "before" or "after" another node, 
    ///     what type of relationship two nodes from the same tree have, and what nodes are their "union nodes", etc.
    /// 
    ///     This class, together with the Active Event implementation in Phosphorus Five, found through Loader and ApplicationContext,
    ///     can be thought of as the "heart" of Phosphorus Five, and is the facilitator of everything that makes Active Event possible,
    ///     and Phosphorus Five beautiful, if such a thing can be said about software.
    /// 
    ///     If you understand the Loader, ApplicationContext and this class, then you understand what separates Phosphorus Five from
    ///     all other software frameworks, and makes it possible to do the things that Phosphorus Five does.
    /// </summary>
    [Serializable]
    public class Node : IComparable
    {
        /// <summary>
        ///     DNA code for Node, or its "Path" property.
        /// 
        ///     The Path for any given Node is basically a "materializedd path" implementation, allowing you to partition your tree-structure,
        ///     such that you can easily determine whether or not one specific node is "before" or "after" another node, and what types of
        ///     relationship two nodes from the same tree have.
        /// </summary>
        public class Dna : IComparable, IConvertible
        {
            internal readonly List<int> Value;

            private Dna ()
            {
                Value = new List<int> ();
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="phosphorus.core.Node+Dna"/> class.
            /// 
            ///     The value given, should be a list of integers, separated by hyphens (-)
            /// </summary>
            /// <param name="value">Value to create Dna object from.</param>
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
            ///     Returns the depth of the Dna.
            /// 
            ///     The depth of a Dna object is the number of ancestor nodes from its root node, to the current node.
            /// </summary>
            /// <value>Depth of Dna object.</value>
            public int Count {
                get {
                    return Value.Count;
                }
            }

            /// <summary>
            ///     Determines if given value is a Dna path or not.
            /// 
            ///     Will return true if the given value can be converted into a Dna object.
            /// </summary>
            /// <returns><c>true</c> if is value is convertible to a Dna object; otherwise, <c>false</c>.</returns>
            /// <param name="value">Value to check if is convertible to Dna.</param>
            public static bool IsPath (string value)
            {
                return value != null && value.All (idx => "0123456789-".IndexOf (idx) != -1);
            }

            /// <summary>
            ///     Compares two Dna objects for equality.
            /// </summary>
            /// <param name="lhs">Left hand side Dna.</param>
            /// <param name="rhs">Right hand side Dna.</param>
            public static bool operator == (Dna lhs, Dna rhs)
            {
                return lhs != null && lhs.CompareTo (rhs) == 0;
            }

            /// <summary>
            ///     Compares two Dna objects for not-equality.
            /// </summary>
            /// <param name="lhs">Left hand side Dna.</param>
            /// <param name="rhs">Right hand side Dna.</param>
            public static bool operator != (Dna lhs, Dna rhs)
            {
                return lhs != null && lhs.CompareTo (rhs) != 0;
            }

            /// <summary>
            ///     Compares two Dna objects to see if left hand side is "more" than right hand side Dna objects.
            /// 
            ///     If this method returns true, then the left-hand-side Dna object is "further out" in the tree, than
            ///     the right-hand-side instance. Meaning, "after" the rhs.
            /// </summary>
            /// <param name="lhs">Left hand side Dna object.</param>
            /// <param name="rhs">Right hand side Dna object.</param>
            public static bool operator > (Dna lhs, Dna rhs)
            {
                return lhs.CompareTo (rhs) == 1;
            }

            /// <summary>
            ///     Compares two Dna objects to see if left hand side is "less" than right hand side Dna objects.
            /// 
            ///     If this method returns true, then the right-hand-side Dna object is "further out" in the tree, than
            ///     the left-hand-side instance. Meaning, "after" the lhs.
            /// </summary>
            /// <param name="lhs">Left hand side Dna object.</param>
            /// <param name="rhs">Right hand side Dna object.</param>
            public static bool operator < (Dna lhs, Dna rhs)
            {
                return lhs.CompareTo (rhs) == -1;
            }

            /// <summary>
            ///     Compares two Dna objects to see if left hand side is "more than or equal" to right hand side Dna objects.
            /// 
            ///     If this method returns true, then the left-hand-side Dna object is "further out or same" in the tree, than
            ///     the right-hand-side instance. Meaning, "after or equals" the rhs.
            /// </summary>
            /// <param name="lhs">Left hand side Dna object.</param>
            /// <param name="rhs">Right hand side Dna object.</param>
            public static bool operator >= (Dna lhs, Dna rhs)
            {
                return lhs.CompareTo (rhs) != -1;
            }

            /// <summary>
            ///     Compares two Dna objects to see if left hand side is "less than or equal to" the right hand side Dna objects.
            /// 
            ///     If this method returns true, then the left-hand-side Dna object is "earlier or equal" to  the right-hand-side instance.
            ///     Meaning, "before or equals" the rhs.
            /// </summary>
            /// <param name="lhs">Left hand side Dna object.</param>
            /// <param name="rhs">Right hand side Dna object.</param>
            public static bool operator <= (Dna lhs, Dna rhs)
            {
                return lhs.CompareTo (rhs) != 1;
            }

            /// <summary>
            ///     Returns the logical AND of the given Dna codes.
            /// 
            ///     Basically finds the common ancestor's Dna code. Using this, you can find the "union" of two nodes from the same tree,
            ///     or their "common ancestor".
            /// </summary>
            /// <param name="lhs">Left hand side Dna.</param>
            /// <param name="rhs">Right hand side Dna.</param>
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

            /// <summary>
            ///     Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="phosphorus.core.Node+Dna"/>.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="phosphorus.core.Node+Dna"/>.</param>
            /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
            /// <see cref="phosphorus.core.Node+Dna"/>; otherwise, <c>false</c>.</returns>
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

            /// <summary>
            ///     Serves as a hash function for a <see cref="phosphorus.core.Node+Dna"/> object.
            /// </summary>
            /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as
            /// a hash table.</returns>
            public override int GetHashCode ()
            {
                return Value.GetHashCode ();
            }

            /// <summary>
            ///     Returns a <see cref="System.String"/> that represents the current <see cref="phosphorus.core.Node+Dna"/>.
            /// </summary>
            /// <returns>A <see cref="System.String"/> that represents the current <see cref="phosphorus.core.Node+Dna"/>.</returns>
            public override string ToString ()
            {
                var tmp = Value.Aggregate ("", (current, idx) => current + ("-" + idx));
                tmp = tmp.Trim ('-');
                return tmp;
            }

            /// <summary>
            ///     Compares the given object with the Dna instance and returns -1 if this is "before" obj, 0 if they equal, and +1 if
            ///     obj is "more than" instance object.
            /// 
            ///     The given obj parameter, must be a Dna object, otherwise method will throw an exception.
            /// </summary>
            /// <returns>-1, 0 or +1, depending upon which of the two objects are "before" the other.</returns>
            /// <param name="obj">The object to compare against this instance.</param>
            public int CompareTo (object obj)
            {
                if (!(obj is Dna))
                    throw new ArgumentException ("cannot compare Dna to: " + (obj ?? "[null]"));
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

            // \todo Do we really need the IConvertible implementation in Dna?
            /*
             * IConvertible implementation, no need to document these parts ...
             */
            public TypeCode GetTypeCode ()
            {
                return TypeCode.Object;
            }

            public bool ToBoolean (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a Dna path to Boolean");
            }

            public byte ToByte (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a Dna path to byte");
            }

            public char ToChar (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a Dna path to char");
            }

            public DateTime ToDateTime (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a Dna path to DateTime");
            }

            public decimal ToDecimal (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a Dna path to decimal");
            }

            public double ToDouble (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a Dna path to double");
            }

            public short ToInt16 (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a Dna path to short");
            }

            public int ToInt32 (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a Dna path to int");
            }

            public long ToInt64 (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a Dna path to long");
            }

            public sbyte ToSByte (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a Dna path to sbyte");
            }

            public float ToSingle (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a Dna path to float");
            }

            public string ToString (IFormatProvider provider)
            {
                return ToString ();
            }

            public object ToType (Type conversionType, IFormatProvider provider)
            {
                if (conversionType != typeof(string))
                    throw new ApplicationException ("cannot convert a Dna path to; " + conversionType);
                return ToString ();
            }

            public ushort ToUInt16 (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a Dna path to ushort");
            }

            public uint ToUInt32 (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a Dna path to uint");
            }

            public ulong ToUInt64 (IFormatProvider provider)
            {
                throw new ApplicationException ("cannot convert a Dna path to ulong");
            }
        }

        private readonly List<Node> _children;
        private string _name;

        /// <summary>
        ///     Delegate for iterating children nodes.
        /// </summary>
        public delegate T NodeIterator<out T> (Node node);

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.core.Node"/> class.
        /// </summary>
        public Node ()
        {
            _children = new List<Node> ();
            Name = string.Empty;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.core.Node"/> class.
        /// </summary>
        /// <param name="name">Name of node. Cannot be null.</param>
        public Node (string name)
            : this ()
        {
            Name = name;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.core.Node"/> class.
        /// </summary>
        /// <param name="name">Name of node. Cannot be null.</param>
        /// <param name="value">Value of node. Can be any object, including null.</param>
        public Node (string name, object value)
            : this (name)
        {
            Value = value;
        }

        /// <summary>
        ///     initializes a new instance of the <see cref="phosphorus.core.Node"/> class.
        /// </summary>
        /// <param name="name">Name of node. Cannot be null.</param>
        /// <param name="value">Value of node. Can be any object, including null.</param>
        /// <param name="children">Initial children collection for node. Notice that if children given already
        /// belongs to another Node's children collection, then they will be UnTied from the other node, and ReTied 
        /// into currently created node.</param>
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
        ///     Gets or sets the name of the node.
        /// </summary>
        /// <value>The node's new name.</value>
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
        ///     Gets or sets the value of the node.
        /// </summary>
        /// <value>The node's new value.</value>
        public object Value {
            get;
            set;
        }

        /// <summary>
        ///     Returns the number of children nodes the node has.
        /// </summary>
        /// <value>Number of children.</value>
        public int Count {
            get {
                return _children.Count;
            }
        }

        /// <summary>
        ///     Returns the index of the given node in children collection.
        /// 
        ///     Returns -1 if node is not in collection
        /// </summary>
        /// <returns>The of.</returns>
        /// <param name="node">Node.</param>
        public int IndexOf (Node node)
        {
            return _children.IndexOf (node);
        }

        /// <summary>
        ///     Returns the number of children nodes matching the given predicate.
        /// </summary>
        /// <param name="functor">Predicate nodes must match to be counted.</param>
        public int CountWhere (Predicate<Node> functor)
        {
            int retVal = 0;
            foreach (var idxNode in Children) {
                if (functor (idxNode))
                    retVal += 1;
            }
            return retVal;
        }

        /// <summary>
        ///     Returns the value of this instance as typeof(T).
        ///
        ///    Returns Value, converted to T, if necessary. If a conversion is not possible, then an exception might occur.
        /// </summary>
        /// <typeparam name="T">Type to convert value to.</typeparam>
        public T Get<T> (ApplicationContext context, T defaultValue = default (T))
        {
            return Utilities.Convert (Value, context, defaultValue);
        }

        /// <summary>
        ///     Returns the string value of the value of the node.
        /// 
        ///     Will convert the node's value if necessary to an encoded string, and return that string. For a byte[] value,
        ///     this means base64 encode its value.
        /// </summary>
        /// <returns>The string value of the node's value.</returns>
        /// <param name="context">Application context.</param>
        /// <param name="encode">If set to <c>true</c> then string will be base64 encoded if necessary.</param>
        public string StringEncodeValue (ApplicationContext context, bool encode)
        {
            return Utilities.Convert<string> (Value, context, null, encode);
        }

        /// <summary>
        ///     Returns the children of this instance.
        /// </summary>
        /// <value>Its children nodes.</value>
        public IEnumerable<Node> Children {
            get {
                return _children;
            }
        }

        /// <summary>
        ///     Returns the children of this instance, while also untying them.
        ///
        ///     Untiyng a node's children, means that the nodes will no longer belong to the Children collection of the node
        ///     they originally belonged to.
        /// </summary>
        /// <value>Its children after being untied.</value>
        public IEnumerable<Node> UnTieChildren ()
        {
            while (_children.Count > 0) {
                yield return _children [0].UnTie ();
            }
        }

        /// <summary>
        ///     Returns Dna code for Node.
        /// </summary>
        /// <value>The Dna code, or position in Node tree.</value>
        public Dna Path {
            get {
                return new Dna (this);
            }
        }

        /// <summary>
        ///     Returns the parent of current node.
        /// 
        ///     The parent, is the node which the current node belongs to, through its parent's Children collection.
        ///     The Node class, is a dually linked Tree Structure, where any node can have a list of children nodes, through
        ///     its Children collection, where each child will also know its parent node, through this property.
        /// </summary>
        /// <value>The parent node of this instance.</value>
        public Node Parent { get; private set; }

        /// <summary>
        ///     Returns the first child of the node.
        /// </summary>
        /// <value>The current node's first child node.</value>
        public Node FirstChild {
            get {
                if (_children.Count > 0)
                    return _children [0];
                return null;
            }
        }

        /// <summary>
        ///     Returns the first child node of the curent node, not having the given name.
        /// 
        ///     Will return the first child node of the Children collection of the durrent node, who's Name property is
        ///     NOT the given name.
        /// </summary>
        /// <value>the first child</value>
        public Node FirstChildNotOf (string name)
        {
            return _children.FirstOrDefault (idx => idx.Name != name);
        }

        /// <summary>
        ///     Returns the last child of the node.
        /// </summary>
        /// <value>The last child node.</value>
        public Node LastChild {
            get {
                if (_children.Count > 0)
                    return _children [_children.Count - 1];
                return null;
            }
        }

        /// <summary>
        ///     Returns the previous sibling of the current node.
        /// 
        ///     The previous sibling node, is defined as the node in the Parent Children collection of nodes, who's index is "one less", than
        ///     the index of the current node. Meaning, if current node is the 3rd instance in the Children collection of its Parent node, then
        ///     this property will return the node that is the 2nd node in its Parent collection.
        /// </summary>
        /// <value>The current node's previous sibling.</value>
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
        ///     Returns the next sibling of the current node.
        /// 
        ///     The "opposite" of PreviousSibling. See PreviousSibling to understand what this property returns.
        /// </summary>
        /// <value>The current node's next sibling.</value>
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
        ///     Returns the previous node from the current node.
        /// 
        ///     Please notice, that this is not the same as the PreviousSibling property, since this will traverse the tree-hierarchy, and
        ///     might return nodes from a completely different level, since it basically looks at the tree-structure of nodes, as a "sequential
        ///     list of nodes", where the Dna code basically defines the "sequential positioning" of the nodes as a "list".
        /// 
        ///     If you perceive the Node hierarchy as Hyperlisp, where each node is defined as one line of code, then this property will 
        ///     basically return the "previous line's node".
        /// </summary>
        /// <value>The current node's previous node.</value>
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
        ///     Returns the next node from the current node.
        /// 
        ///     This property basically does the "opposite" of PreviousNode. To understand what this property does, please see the documentation
        ///     for PreviousNode.
        /// </summary>
        /// <value>The current node's next node.</value>
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
        ///     Returns the root node of the tree.
        /// 
        ///     The root node is found by retrieving the Parent node's Parent, and so on, until we come to a node that does not 
        ///     have a Parent node. That node is the "root node" of the current node. Meaning the root node of your entire tree.
        /// </summary>
        /// <value>The root node of the current node.</value>
        public Node Root {
            get {
                var idxNode = this;
                while (idxNode.Parent != null)
                    idxNode = idxNode.Parent;
                return idxNode;
            }
        }

        /// <summary>
        ///     Unties the node from its Parent Children collection.
        /// 
        ///     Meaning, the node will effectively become its own "root node", and no longer belong to its Parent's Children collection.
        /// </summary>
        public Node UnTie ()
        {
            Parent._children.Remove (this);
            Parent = null;
            return this;
        }

        /// <summary>
        ///     Replaces the current node with another node.
        /// 
        ///     Will replace the current node in its Parent Children collection with the specified node given as node.
        /// </summary>
        /// <param name="node">Node to replace the current node with.</param>
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
        ///     Finds a node according to the given dna.
        /// 
        ///     Will return the node who's Dna is the given dna from the tree. Regardless of which node you invoke this method from,
        ///     the method will recursively lookup the node who's Dna matches the given Dna from the Root node of the current node.
        /// </summary>
        /// <param name="dna">Dna of node to find.</param>
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
        ///     Finds the node matching the given Dna.
        /// 
        ///     See the overload of Find taking a Dna as its parameter to understand what this method does.
        /// </summary>
        /// <returns>The node matching the given Dna string, if found, otherwise null.</returns>
        /// <param name="dna">The Dna, or Path, of the node to return.</param>
        public Node FindDna (string dna)
        {
            if (string.IsNullOrEmpty (dna))
                return null;
            return Find (new Dna (dna));
        }

        /// <summary>
        ///     Finds the first matching node, according to the given predicate.
        /// 
        ///     Will iterate the Children collection of the current node, and return the first Node matching the given functor.
        /// </summary>
        /// <param name="functor">Predicate that nodes must match in order to be returned.</param>
        public Node Find (Predicate<Node> functor)
        {
            return _children.Find (functor);
        }

        /// <summary>
        ///     Finds all nodes according to the given predicate.
        /// 
        ///     Will return all nodes matching the given functor from the current node's Children collection.
        /// </summary>
        /// <param name="functor">Predicate that nodes must match in order to be returned.</param>
        public IEnumerable<Node> FindAll (Predicate<Node> functor)
        {
            return _children.FindAll (functor);
        }

        /// <summary>
        ///     Finds the first node having the given name.
        /// 
        ///     Will return the first node from the current node's Children collection matching the given name. Search is case-sensitive.
        /// </summary>
        /// <param name="name">Name of node to return.</param>
        public Node Find (string name)
        {
            return Find (idx => idx.Name == name);
        }

        /// <summary>
        ///     Finds all nodes having the given name.
        /// 
        ///     Will return all nodes from the current node's Children collection matching the given name. Search is case-sensitive.
        /// </summary>
        /// <param name="name">Name of nodes to return.</param>
        public IEnumerable<Node> FindAll (string name)
        {
            return FindAll (idx => idx.Name == name);
        }

        /// <summary>
        ///     Returns all Value of children nodes as type T.
        /// 
        ///     Will return all node's Value properties converted to type T, if necessary.
        /// </summary>
        /// <returns>The children values.</returns>
        /// <typeparam name="T">The type you wish to convert the Children values to.</typeparam>
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
        ///     Finds, or creates, the first node having the given name.
        /// 
        ///     If no node exists with given name, then a new node with the given name will be created, and returned to caller.
        /// </summary>
        /// <param name="name">Name of node to return or create.</param>
        public Node FindOrCreate (string name)
        {
            var retVal = _children.Find (idx => idx.Name == name);
            if (retVal != null)
                return retVal;
            return Add (new Node (name)).LastChild;
        }

        /// <summary>
        ///     Finds, or creates, the first node having the given name and value
        /// 
        ///     If no matching node exists, then a new node with the given name and value will be created, and returned to caller.
        /// </summary>
        /// <param name="name">Name of node to return.</param>
        /// <param name="value">Value of node to return.</param>
        public Node FindOrCreate (string name, object value)
        {
            var retVal = _children.Find (idx => idx.Name == name && ((value == null && idx.Value == null) || (value != null && value.Equals (idx.Value))));
            if (retVal != null)
                return retVal;
            return Add (new Node (name, value)).LastChild;
        }

        /// <summary>
        ///     Returns the Value of the first Children node, matching the given name, as type T.
        /// 
        ///     Will traverse the Children collection, looking for a node who's Name is name, and if found, return
        ///     the Value of that node as type T.
        /// </summary>
        /// <param name="name">Name of node to return.</param>
        /// <param name="context">Application context.</param>
        /// <param name="defaultValue">Default value to use, if no child with the given name is found.</param>
        public T GetChildValue<T> (string name, ApplicationContext context, T defaultValue = default (T))
        {
            var child = _children.Find (idx => idx.Name == name);
            return child == null ? defaultValue : child.Get (context, defaultValue);
        }

        /// <summary>
        ///     Removes the specified node.
        /// 
        ///     Removes the specified node, if it exists. If node doesn't exist in Children collection of current node, then an exception
        ///     will be thrown. Returns the current node after removal, to make it easy to chain methods.
        /// </summary>
        /// <param name="node">Node.</param>
        public Node Remove (Node node)
        {
            if (!_children.Remove (node))
                throw new ArgumentException ("node doesn't belong to collection");
            return this;
        }

        /// <summary>
        ///     Removes the node at the specified index.
        /// 
        ///     Removes the node at the specified index. If index is larger than the Children collection Count, then an exception
        ///     will be thrown. Returns the current node after removal, to make it easy to chain methods.
        /// </summary>
        /// <param name="index">where node to remove recides in the children collection</param>
        public Node RemoveAt (int index)
        {
            _children [index].UnTie ();
            return this;
        }

        /// <summary>
        ///     Removes all children nodes matching given predicate.
        /// 
        ///     Removes all nodes from the current node's Children collection matching the given functor Predicate. Returns current node
        ///     afterwards, to make it easy to chain methods.
        /// </summary>
        /// <param name="functor">predicate</param>
        public Node RemoveAll (Predicate<Node> functor)
        {
            _children.RemoveAll (functor);
            return this;
        }

        /// <summary>
        ///     Removes all nodes matching the given name.
        /// 
        ///     Removes all nodes from the current node's Children collection matching the given name. Returns the current node afterwards,
        ///     to make it easy to chain methods.
        /// </summary>
        /// <param name="name">name of nodes to remove</param>
        public Node RemoveAll (string name)
        {
            return RemoveAll (idx => idx.Name == name);
        }

        /// <summary>
        ///     Sorts the children of the current node.
        /// 
        ///     Sorts the Children collection of the current node, according to the Comparison functor given. Returns the current node
        ///     afterwards, to make chaining of methods easy.
        /// </summary>
        /// <param name="functor">Comparison delegate.</param>
        public Node Sort (Comparison<Node> functor)
        {
            _children.Sort (functor);
            return this;
        }

        /// <summary>
        ///     Sorts the children of the current node by their names.
        /// 
        ///     Sorts the Children collection of the current node, according to their Name properties. Returns the current node
        ///     afterwards, to make chaining of methods easy.
        /// </summary>
        public Node Sort ()
        {
            _children.Sort ((lhs, rhs) => String.Compare (lhs.Name, rhs.Name, StringComparison.Ordinal));
            return this;
        }

        /// <summary>
        ///     Adds a child node to the current node's children collection.
        /// 
        ///     Adds a child node to the current node's Children collection. Returns the current node afterwards, to make chaining of methods easy.
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
        ///     Adds a child node to the current node's children collection.
        /// 
        ///     Adds a child node to the current node's Children collection, with the specified name.
        ///     Returns the current node afterwards, to make chaining of methods easy.
        /// </summary>
        /// <param name="name">name of node to add</param>
        public Node Add (string name)
        {
            return Add (new Node (name));
        }

        /// <summary>
        ///     Adds a child node to the current node's children collection.
        /// 
        ///     Adds a child node to the current node's Children collection, with the specified name, and the specified value.
        ///     Returns the current node afterwards, to make chaining of methods easy.
        /// </summary>
        /// <param name="name">name of node to add</param>
        /// <param name="value">value of node to add</param>
        public Node Add (string name, object value)
        {
            return Add (new Node (name, value));
        }

        /// <summary>
        ///     Adds a child node to the current node's children collection.
        /// 
        ///     Adds a child node to the current node's Children collection, with the specified name, the specified value, and the specified
        ///     initial children collection. Returns the current node afterwards, to make chaining of methods easy.
        /// </summary>
        /// <param name="name">name of node to add</param>
        /// <param name="value">value of node to add</param>
        /// <param name="nodes">initial child collection of node</param>
        public Node Add (string name, object value, IEnumerable<Node> nodes)
        {
            return Add (new Node (name, value, nodes));
        }

        /// <summary>
        ///     Inserts a child node to the current node's children collection.
        /// 
        ///     Inserts a child node to the current node's Children collection, at the specified index.
        ///     Returns the current node afterwards, to make chaining of methods easy.
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
        ///     Adds a range of nodes.
        /// 
        ///     Adds the specified nodes collection to the current node's Children collection.
        ///     Returns the current node afterwards, to make chaining of methods easy.
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
        ///     Clears the children collection.
        /// 
        ///     Completely empties the Children property of the current node.
        ///     Returns the current node afterwards, to make chaining of methods easy.
        /// </summary>
        public Node Clear ()
        {
            _children.Clear ();
            return this;
        }

        /// <summary>
        ///     Clones the current node.
        /// 
        ///     Creates a "deep copy" of the current node, returning that newly created node to caller.
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
        ///     IComparable implementation, compares current node to another object.
        /// 
        ///     Will compare the current node to the specified value, and if equal, returns 0. If not equal, it will either return
        ///     +1 or -1, depending upon which node is "before" the other. If rhs is not a Node, this method will return +1.
        /// </summary>
        /// <returns>-1 if this node is "less than", +1 if rhs is "less than", 0 if objects are equal.</returns>
        /// <param name="rhs">The object to compare the node against.</param>
        public int CompareTo (object rhs)
        {
            var rhsNode = rhs as Node;
            if (rhsNode == null)
                return 1;
            return CompareTo (rhsNode);
        }

        /// <summary>
        ///     Compares current node to another node.
        /// 
        ///     Will compare the current node to the specified node, and if equal, returns 0. If not equal, it will either return
        ///     +1 or -1, depending upon which node is "before" the other.
        /// </summary>
        /// <returns>-1 if this node is "less than", +1 if rhs is "less than", 0 if objects are equal.</returns>
        /// <param name="rhs">The object to compare the node against.</param>
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
        
        /// <summary>
        ///     Gets or sets the node at the specified index.
        /// 
        ///     Will replace or retrieve the node in the Children collection at the specified index.
        /// </summary>
        /// <param name="index">Index of node to retrieve or set.</param>
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
        ///     Gets or sets the first node in the children collection matching the given name.
        /// 
        ///     Returns or replaces the first node matching the given name.
        /// </summary>
        /// <param name="name">Name of node to retrieve or set.</param>
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
        ///     Returns a <see cref="System.String"/> that represents the current node.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current node.</returns>
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
        
        /// <summary>
        ///     Iterates every single node, invoking the given functor, and if functor does not return
        ///     default (T), it will yield that T value, as a result back to caller.
        /// </summary>
        /// <param name="functor">Match delegate</param>
        /// <typeparam name="T">The type of object you wish to construct from node iterator.</typeparam>
        public IEnumerable<T> ConvertChildren<T> (NodeIterator<T> functor) {
            return _children.Select (idx => functor (idx)).Where (retVal => retVal != null && !retVal.Equals (default (T)));
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
    }
}
