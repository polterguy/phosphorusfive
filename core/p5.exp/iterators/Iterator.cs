/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using p5.core;

/// <summary>
///     Main namespace for all p5.lambda Expression Iterators.
/// 
///     A p5.lambda Expression Iterator is an sub-part of a p5.lambda expression, which informs the expression engine what types
///     of criteria the nodes from the previous iterator, if any, the nodes should match, in order to become a part of the result
///     for the next iterator.
/// 
///     An iterator starts with a slash (/), and can optionally end with a slash. The iterator's content, can optionally be put
///     inside of double-quotes ("), if you have complex characters, that are normally considered control characters in the expression
///     engine.
/// 
///     You can also compose multi-line string iterators, by putting your iterator's content inside of @"xxx" as a multi-line string literal.
/// </summary>
namespace p5.exp.iterators
{
    /// <summary>
    ///     Iterator class, wrapping one iterator, for p5.lambda expressions.
    /// 
    ///     Iterators is at the heart of <see cref="phosphorus.expressions.Expression">Expressions</see>, and are executed as a chain, 
    ///     encapsulated within a <see cref="phosphorus.expressions.Logical">Logical</see> object, which again  is a child of an 
    ///     <see cref="phosphorus.expressions.iterators.IteratorGroup">IteratorGroup</see>. When iterators are evaluated, they are 
    ///     evaluated in a left-associative manner, where the last iterator is evaluated last, and evaluated as a result of its previous 
    ///     iterator's Left property.
    /// 
    ///     The root iterator of all expressions, is normally an iterator, containing a single <see cref="phosphorus.core.Node">Node</see>
    ///     instance, from which iteration begins. Below is an example of an Expression containing two iterators;
    /// 
    ///     <pre>@/../*?node</pre>
    /// 
    ///     The above Expression has one IteratorRoot iterator, and one IteratorChildren iterator, and has the 'value' type declaration,
    ///     which means it will extract the 'value' of its resulting nodes. Effectively extracting all value properties, from all children nodes,
    ///     of the root node of your expression's execution tree.
    /// </summary>
    public abstract class Iterator
    {
        /// <summary>
        ///     Gets or sets the left or "previous iterator".
        /// 
        ///     The left iterator is the "previous iterator" in the chain of iterators for your expressions.
        /// </summary>
        /// <value>Its previous iterator in its chain of iterators.</value>
        public Iterator Left { get; set; }

        /// <summary>
        ///     Evaluates the iterator.
        /// 
        ///     Will evaluate your expression, and return a list of nodes, matching your Expression.
        /// </summary>
        /// <value>The evaluated result, returning a list of <see cref="phosphorus.core.Node" />s.</value>
        public abstract IEnumerable<Node> Evaluate { get; }

        /// <summary>
        ///     Returns true if this is a "reference expression".
        /// 
        ///     A reference expression is normally defined by having two consecutive @ characters in its beginning. This
        ///     signals to the Expression engine that the expression is not necessarily to be directly evaluated, but that it
        ///     might point to another expression, and if it does, then that Expression will be evaluated, and the results of
        ///     that evaluation process returned back to caller.
        /// </summary>
        /// <value>true if this is a reference expression, otherwise false.</value>
        public virtual bool IsReference { get; set; }
    }
}
