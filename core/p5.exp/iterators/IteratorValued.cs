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
    [Serializable]
    public class IteratorValued : Iterator
    {
        private readonly string _type;
        private readonly string _value;
        private bool _like;

        public IteratorValued (string value, string type)
        {
            _value = value;
            _type = type;
            if (_value.StartsWith ("~")) {

                // "like" equality
                _value = _value.Substring (1);
                _like = true;
            } else if (_value.StartsWith ("\\~")) {

                // escaped "like operator"
                _value = _value.Substring (1);
            }
        }

        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            object value = _value;
            if (!string.IsNullOrEmpty (_type) && _type != "string") {

                // verifying caller only supplies "lie" to string types!
                if (_like)
                    throw new ExpressionException ("Cannot use 'like' addition to value iterator when value is not of type string");

                // converting given value to specified type
                value = context.Raise ("p5.hyperlisp.get-object-value." + _type, new Node (string.Empty, value)).Value;
            }

            // filtering away all previous matches that does not match the specified value
            if (_like)
                return Left.Evaluate (context).Where (idxCurrent => idxCurrent.Get<string> (context).Contains (_value));
            else
                return Left.Evaluate (context).Where (idxCurrent => value.Equals (idxCurrent.Value));
        }
    }
}
