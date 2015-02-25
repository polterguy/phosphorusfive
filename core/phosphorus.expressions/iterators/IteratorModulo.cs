
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
    /// returns all nodes matching the given modulo
    /// </summary>
    public class IteratorModulo : Iterator
    {
        private readonly int _modulo;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorModulo"/> class
        /// </summary>
        /// <param name="modulo">modulo</param>
        public IteratorModulo (int modulo)
        {
            _modulo = modulo;
        }

        public override IEnumerable<Node> Evaluate {
            get
            {
                var idxNo = 0;
                return Left.Evaluate.Where(idxCurrent => idxNo++ % _modulo == 0);
            }
        }
    }
}
