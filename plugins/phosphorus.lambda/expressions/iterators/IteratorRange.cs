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
    /// returns all nodes within the specified range
    /// </summary>
    public class IteratorRange : Iterator
    {
        private int _start = 0;
        private int _end;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.iterators.IteratorRange"/> class
        /// </summary>
        /// <param name="start">start position</param>
        public IteratorRange (int start)
        {
            _start = start;
            _end = _start + 1;
        }

        /// <summary>
        /// gets or sets the start of the range to extract
        /// </summary>
        /// <value>start position</value>
        public int Start {
            get {
                return _start;
            }
            set {
                _start = value;
            }
        }

        /// <summary>
        /// gets or sets the end of the range to extract
        /// </summary>
        /// <value>end position</value>
        public int End {
            get {
                return _end;
            }
            set {
                if (value <= _start)
                    throw new ArgumentException ("end must be larger than start for range iterator to have a valid value");
                _end = value;
            }
        }

        public override IEnumerable<Node> Evaluate {
            get {
                int idxNo = 0;
                foreach (Node idxCurrent in Left.Evaluate) {
                    if (idxNo++ >= _start)
                        yield return idxCurrent;
                    if (idxNo >= _end)
                        yield break;
                }
            }
        }
    }
}

