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
    public class IteratorValuedRegex : IteratorRegex
    {
        private readonly string _value;

        public IteratorValuedRegex (string regex)
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
                    foreach (var idxCurrent in Left.Evaluate) {
                        var stringValue = idxCurrent.Value as string;
                        if (stringValue != null) {
                            if (!dict.ContainsKey (stringValue) && ex.IsMatch (stringValue)) {
                                dict [stringValue] = true;
                                yield return idxCurrent;
                            }
                        }
                    }
                } else {
                    foreach (var idxCurrent in Left.Evaluate) {
                        var stringValue = idxCurrent.Value as string;
                        if (stringValue != null) {
                            if (ex.IsMatch (stringValue)) {
                                yield return idxCurrent;
                            }
                        }
                    }
                }
            }
        }

        /*
         * creates options according to regex settings
         */

        private RegexOptions GetOptions (string optionsString)
        {
            // default options is invariant culture
            var options = RegexOptions.CultureInvariant;

            // looping through all options given
            foreach (var idx in optionsString) {
                switch (idx) {
                    case 'i':
                        options |= RegexOptions.IgnoreCase;
                        break;
                    case 'm':
                        options |= RegexOptions.Multiline;
                        break;
                    case 'c':
                        options |= RegexOptions.Compiled;
                        break;
                    case 'e':
                        options |= RegexOptions.ECMAScript;
                        break;
                    case 'w':
                        options |= RegexOptions.IgnorePatternWhitespace;
                        break;
                    case 'r':
                        options |= RegexOptions.RightToLeft;
                        break;
                    case 's':
                        options |= RegexOptions.Singleline;
                        break;
                    case 'd':
                        break; // handled outside of this method ... (distinct option)
                    default:
                        throw new ExpressionException (
                            _expression,
                            string.Format ("'{0}' is not a recognized option for regular expression iterator", idx),
                            _node,
                            _context);
                }
            }
            return options;
        }
    }
}