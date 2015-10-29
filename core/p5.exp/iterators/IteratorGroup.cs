/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Special Iterator for grouping iterators.
    /// 
    ///     An IteratorGroup will either iterate on the result of its parent group, or a single node. By grouping iterators, 
    ///     you can have multiple iterators grouped together, working with the evaluated results, of its parent group iterator.
    /// 
    ///     Normally a "group" is declared by having a sub-expression declared inside of parenthesis "()", where the sub-expression(s)
    ///     will be evaluated using the outer Expression as its source nodes.
    /// 
    ///     Example;
    ///     <pre>(/some-iterator)</pre>
    /// </summary>
    public class IteratorGroup : Iterator
    {
        private readonly Iterator _groupRoot;
        private readonly List<Logical> _logicals = new List<Logical> ();

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorGroup" /> class.
        ///
        ///     This constructor is for creating the "outer most iterator", or "root iterator", of your hyperlisp expressions, 
        ///     and is normally exclusively used as the main root iterator for your entire Expression.
        /// </summary>
        internal IteratorGroup ()
        {
            _groupRoot = new IteratorNode (null);
            AddLogical (new Logical (Logical.LogicalType.Or));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorGroup" /> class.
        /// 
        ///     This constructor is for creating sub-expressions, where each sub-expression is being evaluated from its parent
        ///     group's Expression.
        /// </summary>
        /// <param name="parent">Parent iterator group.</param>
        public IteratorGroup (IteratorGroup parent)
        {
            _groupRoot = new IteratorLeftParent (parent.LastIterator);
            AddLogical (new Logical (Logical.LogicalType.Or));
            ParentGroup = parent;
            ParentGroup.AddIterator (this);
        }

        /// <summary>
        ///     Returns the parent group.
        /// 
        ///     The parent group, is the "outer-expression" of your group, if any. The root group, has no parent group.
        /// </summary>
        /// <value>the parent group</value>
        public IteratorGroup ParentGroup { get; private set; }
        
        internal Node GroupRootNode {
            get { return ((IteratorNode)_groupRoot).RootNode; }
            set { ((IteratorNode)_groupRoot).RootNode = value; }
        }

        /// <summary>
        ///     Gets the last iterator in the group.
        /// 
        ///     This is where you append a new Iterator, when constructing your chain of iterators.
        /// </summary>
        /// <value>the last iterator</value>
        private Iterator LastIterator
        {
            get { return _logicals [_logicals.Count - 1].Iterator; }
        }

        /// <summary>
        ///     Returns true if this is a "reference expression".
        /// 
        ///     A reference Expression is normally delcared by starting your expression with two "@@" characters. This
        ///     signals to the expression engine, that instead of directly evaluating that expression, evaluate what the expression
        ///     refers to. This allows you to pass around expressions as arguments to Active Events, where your Active Events evaluates
        ///     the expressions you pass into them, instead of evaluating them directly.
        /// 
        ///     Think of a reference expression as a "pointer" from C/C++, or a "reference object" in C#.
        /// </summary>
        /// <value>true if this is a reference expression, otherwise false</value>
        public override bool IsReference
        {
            get { return _groupRoot.IsReference; }
            set { _groupRoot.IsReference = value; }
        }
        
        /// <summary>
        ///     Adds a Logical to the list of logicals in the group.
        ///
        ///     Logicals are what allows you to perform Boolean Algebra on your expressions, and are normally defined in your 
        ///     expressions using either of these characters; &, |, ^ or !
        ///
        ///     ! is your NOT operator, and means that it should NOT return whatever is returned as a result of your right-hand-side sub-expression.
        ///
        ///     | is your OR operator, and signifies that your expression should add together the results of your right-hand-side expression, 
        ///     with whatever is at your left-hand-side as a combined result.
        ///
        ///     & is your AND operator, and basically means that in order to be a part of the combined results, the nodes must match both the 
        ///     left-hand-side, and the right-hand-side sub-expression.
        ///
        ///     ^ is your XOR operator, and basically means that it should combine together the results of both your left-hand-side, with your
        ///     right-hand-side, but only if it matches only ONE of your sides, and not if it matches neither sides or both sides.
        /// </summary>
        /// <param name="logical">logical</param>
        public void AddLogical (Logical logical)
        {
            _logicals.Add (logical);

            // making sure "group root iterator" is root iterator for all Logicals
            _logicals [_logicals.Count - 1].AddIterator (_groupRoot);
        }

        /// <summary>
        ///     Appends a new iterator to the last <see cref="phosphorus.expressions.Logical" /> in the group.
        ///
        ///     Appends a new Iterator at the end of your current iterator chain for evaluation of your Expression.
        /// </summary>
        /// <param name="iterator">Iterator to append.</param>
        public void AddIterator (Iterator iterator)
        {
            _logicals [_logicals.Count - 1].AddIterator (iterator);
        }

        public override IEnumerable<Node> Evaluate
        {
            get
            {
                var nodes = new List<Node> ();
                foreach (var idxLogical in _logicals) {
                    nodes = idxLogical.EvaluateNodes (nodes);
                }
                return nodes;
            }
        }
    }
}
