/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute.iterators
{
    /// <summary>
    /// returns all nodes with the specified name of the previous iterator
    /// </summary>
    public class IteratorNamed : Iterator
    {
        private string _name;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.iterators.IteratorNamed"/> class
        /// </summary>
        /// <param name="name">name to match</param>
        public IteratorNamed (string name)
        {
            // to support iterators who's value starts with "/" or ".." to escape out of Regex and named ancestor iterators
            if (name.StartsWith ("\\"))
                name = name.Substring (1);
            _name = name;
        }

        public override IEnumerable<Node> Evaluate {
            get {
                foreach (Node idxCurrent in Left.Evaluate) {
                    if (idxCurrent.Name == _name)
                        yield return idxCurrent;
                }
            }
        }
    }
}

