
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.lambda.iterators
{
    /// <summary>
    /// returns all nodes matching the given modulo
    /// </summary>
    public class IteratorModulo : Iterator
    {
        private int _modulo = 0;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.iterators.IteratorModulo"/> class
        /// </summary>
        /// <param name="modulo">modulo</param>
        public IteratorModulo (int modulo)
        {
            _modulo = modulo;
        }

        /// <summary>
        /// gets or sets the modulo to extract
        /// </summary>
        /// <value>modulo</value>
        public int Modulo {
            get {
                return _modulo;
            }
            set {
                _modulo = value;
            }
        }

        public override IEnumerable<Node> Evaluate {
            get {
                int idxNo = 0;
                foreach (Node idxCurrent in Left.Evaluate) {
                    if (idxNo++ % _modulo == 0)
                        yield return idxCurrent;
                }
            }
        }
    }
}
