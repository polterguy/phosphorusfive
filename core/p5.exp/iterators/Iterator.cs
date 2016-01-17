/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using p5.core;

/// <summary>
///     Main namespace for all p5 lambda Expression Iterators
/// </summary>
namespace p5.exp.iterators
{
    /// <summary>
    ///     Iterator class, wrapping one iterator, for p5 lambda expressions
    /// </summary>
    [Serializable]
    public abstract class Iterator
    {
        /// <summary>
        ///     Gets or sets the left or "previous iterator"
        /// </summary>
        /// <value>Its previous iterator in its chain of iterators</value>
        public Iterator Left
        {
            get;
            set;
        }

        /// <summary>
        ///     Evaluates the iterator
        /// </summary>
        /// <value>The evaluated result, returning a list of <see cref="phosphorus.core.Node" />s</value>
        public abstract IEnumerable<Node> Evaluate (ApplicationContext context);
    }
}
