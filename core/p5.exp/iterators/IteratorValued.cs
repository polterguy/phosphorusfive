/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        readonly string _type;
        readonly string _value;
        readonly bool _like;

        public IteratorValued (string value, string type)
        {
            _type = type;
            if (value.StartsWithEx ("~")) {

                // "Like" equality
                _value = value.Substring (1);
                _like = true;
            } else if (value.StartsWithEx ("\\")) {

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

            // Checking if caller supplied a type conversion of comparison value, and if so, converting _value to type specified
            if (!string.IsNullOrEmpty (_type) && _type != "string") {

                // Verifying caller only supplies "like" to string types!
                if (_like)
                    throw new ExpressionException ("Cannot use 'like' addition to value iterator when value is not of type string");

                // converting given value to specified type
                value = context.RaiseEvent (".p5.hyperlambda.get-object-value." + _type, new Node ("", value)).Value;
            }

            // Filtering away all previous matches that does not match the specified value
            if (value is Regex) {

                // Special case for regular expressions
                return Left.Evaluate (context).Where (idxCurrent => (value as Regex).IsMatch (idxCurrent.Get (context, "")));
            }

            if (_like) {
                
                // Special case for empty value, making sure we return either empty or null values
                if (string.IsNullOrEmpty (_value)) {

                    // If type of equality is "like" and comparison value is "", we match for either null or "" string
                    return Left.Evaluate (context).Where (idxCurrent => string.IsNullOrEmpty (idxCurrent.Get (context, "")));
                }

                // Matching all value that contains the specified value
                return Left.Evaluate (context).Where (idxCurrent => idxCurrent.Get (context, "").Contains (_value));

            }

            // Exact match for value, that might be any type, meaning types must also match
            return Left.Evaluate (context).Where (idxCurrent => value.Equals (idxCurrent.Value));
        }
    }
}
