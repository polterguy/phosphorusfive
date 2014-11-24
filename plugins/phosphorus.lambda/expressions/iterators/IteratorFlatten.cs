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
    /// return all nodes and descendants of previous iterator, flattening hierarchy
    /// </summary>
    public class IteratorFlatten : Iterator
    {
        public override IEnumerable<Node> Evaluate {
            get {
                List<Node> retVal = new List<Node> ();
                foreach (Node idxCurrent in Left.Evaluate) {
                    retVal.Add (idxCurrent);
                    ReturnChildren (idxCurrent, retVal);
                }
                foreach (Node idx in retVal) {
                    yield return idx;
                }
            }
        }

        /*
         * recursively invoked for all descendant nodes
         */
        private void ReturnChildren (Node idx, List<Node> retVal)
        {
            foreach (Node idxChild in idx.Children) {
                retVal.Add (idxChild);
                ReturnChildren (idxChild, retVal);
            }
        }
    }
}

