/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Linq;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     Returns the first ancestor node matching the specified name.
    /// 
    ///     Will traverse the node hierarchy upwards from its current results, and return the first ancestor node
    ///     who's name matches the value after the ".." parts of the Iterator.
    /// 
    ///     Example;
    ///     <pre>/..foo</pre>
    /// </summary>
    public class IteratorNamedAncestor : Iterator
    {
        private readonly string _name;

        /// <summary>
        ///     initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorNamedAncestor" /> class
        /// </summary>
        /// <param name="name">name to look for</param>
        public IteratorNamedAncestor (string name) { _name = name; }

        public override IEnumerable<Node> Evaluate
        {
            get
            {
                foreach (var idxAncestor in Left.Evaluate.Select (idxCurrent => idxCurrent.Parent)) {
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