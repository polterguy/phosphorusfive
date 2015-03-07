/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using phosphorus.core;

/// <summary>
///     Main namespace for all pf.lambda Expression Iterators.
/// 
///     A pf.lambda Expression Iterator is an sub-part of a pf.lambda expression, which informs the expression engine what types
///     of criteria the nodes from the previous iterator, if any, the nodes should match, in order to become a part of the result
///     for the next iterator.
/// 
///     An iterator starts with a slash (/), and can optionally end with a slash. The iterator's content, can optionally be put
///     inside of double-quotes ("), if you have complex characters, that are normally considered control characters in the expression
///     engine.
/// 
///     You can also compose multi-line string iterators, by putting your iterator's content inside of @"xxx" as a multi-line string literal.
/// </summary>
namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     Iterator class, wraps one iterator for hyperlisp expressions.
    /// 
    ///     Iterators are executed as a chain, encapsulated within a <see cref="phosphorus.expressions.Logical" /> object, which again 
    ///     is a child of <see cref="phosphorus.expressions.iterators.IteratorGroup" />, which again is an iterator.
    /// 
    ///     When iterators are evaluated, they're evaluated in a left-associative manner, where the last iterator is
    ///     evaluated last, and evaluated as a result of its previous iterator's Left property.
    /// 
    ///     The root iterator of all expressions, is normally an iterator containing a single <see cref="phosphorus.core.Node"/> instance,
    ///     from which iteration begins.
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
