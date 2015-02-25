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
    ///     returns all nodes with the specified name from the previous iterator
    /// </summary>
    public class IteratorNamed : Iterator
    {
        private readonly string _name;

        /// <summary>
        ///     initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorNamed" /> class
        /// </summary>
        /// <param name="name">name to match</param>
        public IteratorNamed (string name) { _name = name; }

        public override IEnumerable<Node> Evaluate
        {
            get { return Left.Evaluate.Where (idxCurrent => idxCurrent.Name == _name); }
        }
    }
}