/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System.Text.RegularExpressions;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    public abstract class IteratorRegex : Iterator
    {
        protected RegexOptions GetOptions (string optionsString)
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
                        throw new ExpressionException (string.Format ("'{0}' is not a recognized option for a regular expression iterator.", idx));
                }
            }
            return options;
        }
    }
}