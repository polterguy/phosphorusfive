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
    ///     returns all <see cref="phosphorus.core.Node" />s from previous iterated result,
    ///     which matches the specified value, with the (optional) type declaration. if no type is
    ///     given, type defaults to "string"
    /// </summary>
    public class IteratorValued : Iterator
    {
        private readonly ApplicationContext _context;
        private readonly string _type;
        private readonly string _value;

        public IteratorValued (string value, string type, ApplicationContext context)
        {
            _value = value;
            _type = type;
            _context = context;
        }

        public override IEnumerable<Node> Evaluate
        {
            get
            {
                object value = _value;
                if (!string.IsNullOrEmpty (_type)) {
                    // converting given value to specified type
                    value = _context.Raise ("pf.hyperlisp.get-object-value." + _type, new Node (string.Empty, value)).Value;
                }

                // filtering away all previous matches that does not match the specified value
                return Left.Evaluate.Where (idxCurrent => value.Equals (idxCurrent.Value));
            }
        }
    }
}