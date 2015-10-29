/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Linq;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Returns all nodes with the specified value.
    /// 
    ///     This Iterator can optionally be given a "type declaration", from which a type, represented by the string in
    ///     its value will be created from. But the type declaration is optional, and defaults to 'string'.
    /// 
    ///     Example that returns all nodes who's value equals the string 'foo';
    /// 
    ///     <pre>/=foo</pre>
    /// 
    ///     Example that returns all nodes who's value equals the integer value of '5';
    /// 
    ///     <pre>/=:int:5</pre>
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
                    value = _context.Raise ("p5.hyperlisp.get-object-value." + _type, new Node (string.Empty, value)).Value;
                }

                // filtering away all previous matches that does not match the specified value
                return Left.Evaluate.Where (idxCurrent => value.Equals (idxCurrent.Value));
            }
        }
    }
}