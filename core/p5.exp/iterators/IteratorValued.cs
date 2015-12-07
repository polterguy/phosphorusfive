/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Collections.Generic;
using p5.core;
using p5.exp.exceptions;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Returns all nodes with the specified value
    /// </summary>
    [Serializable]
    public class IteratorValued : Iterator
    {
        private readonly string _type;
        private readonly string _value;
        private bool _like;

        public IteratorValued (string value, string type)
        {
            _type = type;
            if (value.StartsWith ("~")) {

                // "Like" equality
                _value = value.Substring (1);
                _like = true;
            } else if (value.StartsWith ("\\")) {

                // Escaped "like operator"
                _value = value.Substring (1);
            } else {

                // "Plain" and simple
                _value = value;
            }
        }

        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            object value = _value;
            if (!string.IsNullOrEmpty (_type) && _type != "string") {

                // Verifying caller only supplies "lie" to string types!
                if (_like)
                    throw new ExpressionException ("Cannot use 'like' addition to value iterator when value is not of type string");

                // converting given value to specified type
                value = context.RaiseNative ("p5.hyperlisp.get-object-value." + _type, new Node ("", value)).Value;
            }

            // Filtering away all previous matches that does not match the specified value
            if (_like)
                return Left.Evaluate (context).Where (idxCurrent => idxCurrent.Get (context, "").Contains (_value));
            else
                return Left.Evaluate (context).Where (idxCurrent => value.Equals (idxCurrent.Value));
        }
    }
}
