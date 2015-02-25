/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     return all nodes and descendants of previous iterator, flattening hierarchy
    /// </summary>
    public class IteratorFlatten : Iterator
    {
        public override IEnumerable<Node> Evaluate
        {
            get
            {
                var retVal = new List<Node> ();
                foreach (var idxCurrent in Left.Evaluate) {
                    retVal.Add (idxCurrent);
                    ReturnChildren (idxCurrent, retVal);
                }
                return retVal;
            }
        }

        /*
         * recursively invoked for all descendant nodes
         */

        private static void ReturnChildren (Node idx, List<Node> retVal)
        {
            foreach (var idxChild in idx.Children) {
                retVal.Add (idxChild);
                ReturnChildren (idxChild, retVal);
            }
        }
    }
}