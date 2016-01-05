/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Collections.Generic;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Iterator for returning the n'th children of previous iterator result
    /// </summary>
    [Serializable]
    public class IteratorNumberedChild : Iterator
    {
        private readonly int _number;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorNumbered" /> class
        /// </summary>
        /// <param name="number">The n'th child to return, if it exists, from previous result-set</param>
        public IteratorNumberedChild (int number)
        {
            _number = number;
        }

        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            return from idxCurrent in Left.Evaluate (context) where idxCurrent.Children.Count > _number select idxCurrent [_number];
        }
    }
}
