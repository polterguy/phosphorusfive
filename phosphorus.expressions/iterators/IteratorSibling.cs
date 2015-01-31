
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
    /// returns an offset sibling <see cref="phosphorus.core.Node"/>
    /// </summary>
    public class IteratorSibling : Iterator
    {
        private int _offset;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.iterators.IteratorSibling"/> class
        /// </summary>
        /// <param name="offset">offset siblings from current nodes</param>
        public IteratorSibling (int offset)
        {
            _offset = offset;
        }

        /// <summary>
        /// gets or sets the offset value
        /// </summary>
        /// <value>the offset</value>
        public int Offset {
            get {
                return _offset;
            }
            set {
                if (value == 0)
                    throw new ArgumentException ("zero is not a valid offset value for sibling iterator");
                _offset = value;
            }
        }

        public override IEnumerable<Node> Evaluate {
            get {
                foreach (Node idxCurrent in Left.Evaluate) {
                    int offset = _offset;
                    Node tmpIdx = idxCurrent;
                    while (offset != 0 && tmpIdx != null) {
                        if (offset < 0) {
                            offset += 1;
                            tmpIdx = tmpIdx.PreviousSibling;
                        } else {
                            offset -= 1;
                            tmpIdx = tmpIdx.NextSibling;
                        }
                    }
                    if (tmpIdx != null)
                        yield return tmpIdx;
                }
            }
        }
    }
}
