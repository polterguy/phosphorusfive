/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using p5.core;

namespace p5.exp.iterators
{
    // \todo Do we really need this class at all ...?
    /// <summary>
    ///     Root iterators for nested IteratorGroup iterators.
    ///
    ///     Helper Iterator to declare a new IteratorGroup. Internally used when a new group, or sub-expression is declared, 
    ///     using parenthesis.
    /// 
    ///     This iterator is never directly consumed in your code, but automatically created for you when you create an IteratorGroup
    ///     Iterator.
    /// </summary>
    public class IteratorLeftParent : Iterator
    {
        private readonly Iterator _leftParent;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorLeftParent" /> class.
        /// </summary>
        /// <param name="leftParent">The last iterator of the parent group iterator.</param>
        public IteratorLeftParent (Iterator leftParent)
        {
            _leftParent = leftParent;
        }

        public override IEnumerable<Node> Evaluate
        {
            get { return _leftParent.Evaluate; }
        }
    }
}