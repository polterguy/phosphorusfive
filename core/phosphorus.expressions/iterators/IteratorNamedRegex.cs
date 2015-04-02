/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    public class IteratorNamedRegex : IteratorRegex
    {
        private readonly string _value;

        public IteratorNamedRegex (string regex)
        {
            _value = regex;
        }

        public override IEnumerable<Node> Evaluate
        {
            get
            {
                if (_value.LastIndexOf ("/", StringComparison.Ordinal) == 0)
                    throw new ExpressionException (string.Format ("Syntax Error, regular expression iterator missed '/' at end close to '{0}'.", _value));

                var regex = _value.Substring (1, _value.LastIndexOf ("/", StringComparison.Ordinal) - 1);
                var options = _value.Substring (_value.LastIndexOf ("/", StringComparison.Ordinal) + 1);
                var distinct = options.IndexOf ('d') > -1;
                var ex = new Regex (regex, GetOptions (options));
                if (distinct) {
                    var dict = new Dictionary<string, bool> ();
                    foreach (var idxCurrent in Left.Evaluate.Where (ix => !dict.ContainsKey (ix.Name) && regex.IsMatch (ix.Name))) {
                        dict [idxCurrent.Name] = true;
                        yield return idxCurrent;
                    }
                } else {
                    foreach (var idxCurrent in Left.Evaluate.Where (ix => regex.IsMatch (ix.Name))) {
                        yield return idxCurrent;
                    }
                }
            }
        }
    }
}
