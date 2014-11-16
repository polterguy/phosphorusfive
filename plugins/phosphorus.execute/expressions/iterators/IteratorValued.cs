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
    /// returns all <see cref="phosphorus.core.Node"/>s from previous iterated result with a specified string value
    /// </summary>
    public class IteratorValued : Iterator
    {
        private string _value;

        /// <summary>
        /// gets or sets the value to search for
        /// </summary>
        /// <value>The value.</value>
        public string Value {
            get {
                return _value;
            }
            set {
                _value = value;
            }
        }

        public override IEnumerable<Node> Evaluate {
            get {
                foreach (Node idxCurrent in Left.Evaluate) {
                    if (idxCurrent.Get<string> () == _value)
                        yield return idxCurrent;
                }
            }
        }
    }
}

