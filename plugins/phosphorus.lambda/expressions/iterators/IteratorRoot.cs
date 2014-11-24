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
    /// returns the "root" <see cref="phosphorus.core.Node"/> of the entire tree which the iterator is iterating
    /// </summary>
    public class IteratorRoot : Iterator
    {
        public override IEnumerable<Node> Evaluate {
            get {
                IEnumerator<Node> enumerator = Left.Evaluate.GetEnumerator () as IEnumerator<Node>;
                enumerator.MoveNext ();
                yield return enumerator.Current.Root;
            }
        }
    }
}

