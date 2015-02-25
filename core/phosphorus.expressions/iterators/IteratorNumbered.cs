/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Linq;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     iterator for returning all n'th children of previous iterator result
    /// </summary>
    public class IteratorNumbered : Iterator
    {
        private readonly int _number;

        /// <summary>
        ///     initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorNumbered" /> class
        /// </summary>
        /// <param name="number">n'th child to return if it exists in previous result</param>
        public IteratorNumbered (int number) { _number = number; }

        public override IEnumerable<Node> Evaluate
        {
            get { return from idxCurrent in Left.Evaluate where idxCurrent.Count > _number select idxCurrent [_number]; }
        }
    }
}