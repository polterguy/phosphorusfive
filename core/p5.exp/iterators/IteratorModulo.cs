/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Collections.Generic;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Returns all nodes matching the given modulo
    /// </summary>
    [Serializable]
    public class IteratorModulo : Iterator
    {
        private readonly int _modulo;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorModulo" /> class
        /// </summary>
        /// <param name="modulo">modulo</param>
        public IteratorModulo (int modulo)
        {
            _modulo = modulo;
        }

        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            var idxNo = 0;
            return Left.Evaluate (context).Where (idxCurrent => (++idxNo)%_modulo == 0);
        }
    }
}
