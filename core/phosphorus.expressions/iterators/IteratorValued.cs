
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    /// returns all <see cref="phosphorus.core.Node"/>s from previous iterated result,
    /// which matches the specified value, with the (optional) type declaration. if no type is 
    /// given, type defaults to "string"
    /// </summary>
    public class IteratorValued : Iterator
    {
        private string _value;
        private string _type;
        private ApplicationContext _context;

        public IteratorValued (string value, string type, ApplicationContext context)
        {
            _value = value;
            _type = type;
            _context = context;
        }

        public override IEnumerable<Node> Evaluate {
            get {
                object value = _value;
                if (!string.IsNullOrEmpty (_type)) {

                    // converting given value to specified type
                    value = _context.Raise ("pf.hyperlisp.get-object-value." + _type, new Node (string.Empty, value)).Value;
                }

                // filtering away all previous matches that does not match the specified value
                foreach (Node idxCurrent in Left.Evaluate) {
                    if (value.Equals (idxCurrent.Value))
                        yield return idxCurrent;
                }
            }
        }
    }
}
