/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Collections.Generic;
using p5.core;
using p5.exp.iterators;

namespace p5.exp
{
    /// <summary>
    ///     Class encapsulating Boolean algebraic operations for the Expression class.
    ///
    ///     Logicals are what allows you to perform Boolean Algebra on your expressions, and are normally defined in your 
    ///     expressions using either of these characters; &, |, ^ or !
    ///
    ///     ! is your NOT operator, and means that it should NOT return whatever is returned as a result of your right-hand-side sub-expression.
    ///
    ///     | is your OR operator, and signifies that your expression should add together the results of your right-hand-side expression, 
    ///     with whatever is at your left-hand-side, and create a combined result.
    ///
    ///     & is your AND operator, and basically means that in order to be a part of the combined results, the nodes must match both the 
    ///     left-hand-side, and the right-hand-side sub-expression.
    ///
    ///     ^ is your XOR operator, and basically means that it should combine together the results of both your left-hand-side, with your
    ///     right-hand-side, but only if it matches only ONE of your sides, and not if it matches neither sides, nor if it matches both sides.
    /// 
    ///     Below is an example of an Expression returning all children nodes of the root node, having a value of "foo", but excluding the ones
    ///     who's name is "bar";
    /// 
    ///     <pre>@/../"*"(/=foo!/bar)?node</pre>
    /// 
    ///     When you use the Boolean algebraic properties of p5.lambda expressions, you often will want to group your logicals together, inside 
    ///     a group, such that they react upon the same node-set, and such that you can shorten your expressions. The below two expressions, will
    ///     for instance extract the same result-set, and logically, they're synonyms to each other. One of them though, is significantly shorter 
    ///     than the other, since it uses a "group", signified by the parenthesis;
    /// 
    ///     <pre>  @/../"*"/_data/"*"/foo|/../"*"/_data/"*"/bar?value
    ///   @/../"*"/_data/"*"(/foo|/bar)?value</pre>
    /// 
    ///     When you create a group, by adding parenthesis to your expressions, then each sub-expression, inside your grouping parenthesis,
    ///     is evaluated on its parent group's result-set. This allows you to avoid repetition when combining logicals together, to perform
    ///     Boolean algebra, to create your own dimensional extraction from your p5.lambda node-set.
    /// </summary>
    [Serializable]
    public class Logical
    {
        /// <summary>
        ///     Type of boolean operator.
        /// 
        ///     This is the type declaration of your Logical, and can be either !, &, | or ^, declaring which Boolean operator
        ///     and operation you wish to perform on your expressions and sub-expressions.
        /// </summary>
        public enum LogicalType
        {
            /// <summary>
            ///     OR operator, for ORing results together.
            /// 
            ///     Defined through the pipe character (|).
            /// </summary>
            Or,

            /// <summary>
            ///     AND operator, for ANDing results together.
            /// 
            ///     Defined through the ampersand character (&).
            /// </summary>
            And,

            /// <summary>
            ///     XOR operator, for XORing results together.
            /// 
            ///     Defined through the hat character (^).
            /// </summary>
            Xor,

            /// <summary>
            ///     NOT operator, for NOTing results together.
            /// 
            ///     Defined through the exclamation mark character (!).
            /// </summary>
            Not
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.Logical" /> class.
        /// </summary>
        /// <param name="type">Type of logical, Or, And, Xor or Not.</param>
        public Logical (LogicalType type)
        {
            TypeOfLogical = type;
        }

        /// <summary>
        ///     Returns the last <see cref="phosphorus.expressions.iterators.Iterator" /> in the list of iterators belonging to
        ///     this logical.
        /// </summary>
        /// <value>The last iterator in the chain of iterators.</value>
        public Iterator Iterator { get; private set; }

        /// <summary>
        ///     Gets the type of logical.
        /// 
        ///     Can be either Or, And, Xor or Not.
        /// </summary>
        /// <value>The type of logical.</value>
        private LogicalType TypeOfLogical { get; set; }

        /// <summary>
        ///     Adds an iterator to the current logical group.
        /// </summary>
        /// <param name="iterator">Tterator to append to chain of iterators.</param>
        public void AddIterator (Iterator iterator)
        {
            iterator.Left = Iterator;
            Iterator = iterator;
        }

        internal List<Node> EvaluateNodes (List<Node> nodes, ApplicationContext context)
        {
            var rhs = new List<Node> (Iterator.Evaluate (context));
            var retVal = new List<Node> ();
            switch (TypeOfLogical) {
                case LogicalType.Or:
                    retVal.AddRange (nodes);
                    foreach (var idx in rhs.Where (idx => !retVal.Contains (idx))) {
                        retVal.Add (idx);
                    }
                    break;
                case LogicalType.And:
                    retVal.AddRange (nodes.FindAll (
                        idx => rhs.Contains (idx)));
                    break;
                case LogicalType.Xor:
                    retVal.AddRange (nodes.FindAll (
                        idx => !rhs.Contains (idx)));
                    retVal.AddRange (rhs.FindAll (
                        idx => !nodes.Contains (idx)));
                    break;
                case LogicalType.Not:
                    retVal.AddRange (nodes.FindAll (
                        idx => !rhs.Contains (idx)));
                    break;
            }
            return retVal;
        }
    }
}
