
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    /// "stop iterator", which is used as "root iterators" for nested
    /// <see cref="phosphorus.expressions.iterators.IteratorGroup"/> iterators
    /// iterators
    /// </summary>
    public class IteratorLeftParent : Iterator
    {
        private readonly Iterator _leftParent;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorLeftParent"/> class.
        /// </summary>
        /// <param name="leftParent">the last iterator of the parent group iterator</param>
        public IteratorLeftParent (Iterator leftParent)
        {
            _leftParent = leftParent;
        }

        public override IEnumerable<Node> Evaluate {
            get {
                return _leftParent.Evaluate;
            }
        }
    }
}
