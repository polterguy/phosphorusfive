
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Linq;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    /// returns an ancestor node with the specified name
    /// </summary>
    public class IteratorNamedAncestor : Iterator
    {
        private readonly string _name;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorNamedAncestor"/> class
        /// </summary>
        /// <param name="name">name to look for</param>
        public IteratorNamedAncestor (string name)
        {
            _name = name;
        }

        public override IEnumerable<Node> Evaluate {
            get
            {
                foreach (var idxAncestor in Left.Evaluate.Select(idxCurrent => idxCurrent.Parent))
                {
                    var curAncestor = idxAncestor;
                    while (curAncestor != null) {
                        if (curAncestor.Name == _name) {
                            yield return curAncestor;
                            break;
                        }
                        curAncestor = curAncestor.Parent;
                    }
                }
            }
        }
    }
}
