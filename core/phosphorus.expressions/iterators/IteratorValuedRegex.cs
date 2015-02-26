/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using phosphorus.core;
using phosphorus.expressions.exceptions;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     returns all <see cref="phosphorus.core.Node" />s from previous iterated result with a specified string value
    /// </summary>
    public class IteratorValuedRegex : IteratorRegex
    {
        private readonly ApplicationContext _context;
        // kept around to be able to create sane exceptions
        private readonly string _expression;
        private readonly Node _node;
        private readonly string _value;

        public IteratorValuedRegex (string value, string expression, Node node, ApplicationContext context)
        {
            _value = value;
            _expression = expression;
            _node = node;
            _context = context;
        }

        public override IEnumerable<Node> Evaluate
        {
            get
            {
                if (_value.LastIndexOf ("/", StringComparison.Ordinal) == 0)
                    throw new ExpressionException (
                        _expression,
                        "token; '" + _value + "' is not a valid regular expression, missing back slash at end of regex.",
                        _node,
                        _context);
                var optionsString = _value.Substring (_value.LastIndexOf ("/", StringComparison.Ordinal) + 1);
                var distinct = optionsString.IndexOf ('d') > -1;
                var regex = new Regex (
                    _value.Substring (1, _value.LastIndexOf ("/", StringComparison.Ordinal) - 1),
                    GetOptions (optionsString));
                if (distinct) {
                    // requesting "distinct" (unique) values
                    var dict = new Dictionary<string, bool> ();
                    foreach (var idxCurrent in Left.Evaluate) {
                        if (idxCurrent.Value == null)
                            continue;

                        var valueOfNode = idxCurrent.Get<string> (_context);
                        if (dict.ContainsKey (valueOfNode) || !regex.IsMatch (valueOfNode))
                            continue;

                        dict [valueOfNode] = true;
                        yield return idxCurrent;
                    }
                } else {
                    // requesting all node values that matches regular expression normally
                    foreach (var idxCurrent in 
                        Left.Evaluate.Where (
                            idxCurrent => idxCurrent.Value != null).Where (idxCurrent => regex.IsMatch (idxCurrent.Get<string> (_context)))) {
                        yield return idxCurrent;
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