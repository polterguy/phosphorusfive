/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     iterator class, wraps one iterator for hyperlisp expressions.  iterators are put in chain, encapsulated within a
    ///     <see cref="phosphorus.expressions.Logical" /> object, which again is a child of
    ///     <see cref="phosphorus.expressions.iterators.IteratorGroup" />,
    ///     which again is an iterator.  when iterators are evaluated, they're evaluated as last in, where the last iterates on
    ///     the evaluated
    ///     result of its previous or "Left" iterator, recursively, until root iterator is reached, which normally should
    ///     return a single
    ///     <see cref="phosphorus.core.Node" />, which at the end is the input to the iterators
    /// </summary>
    public abstract class Iterator
    {
        /// <summary>
        ///     gets or sets the left or "previous iterator"
        /// </summary>
        /// <value>its previous iterator in the chain of iterators</value>
        public Iterator Left { protected get; set; }

        /// <summary>
        ///     evaluates the iterator
        /// </summary>
        /// <value>the evaluated result returning a list of <see cref="phosphorus.core.Node" />s</value>
        public abstract IEnumerable<Node> Evaluate { get; }

        /// <summary>
        ///     returns true if this is a "reference expression"
        /// </summary>
        /// <value>true if this is a reference expression</value>
        public virtual bool IsReference { get; set; }
    }
}