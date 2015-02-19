
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
    /// returns all <see cref="phosphorus.core.Node"/>s from previous iterated result with a specified string value
    /// </summary>
    public class IteratorValuedRegex : IteratorRegex
    {
        private string _value;

        // kept around to be able to create sane exceptions
        private string _expression;
        private Node _node;
        private ApplicationContext _context;

        public IteratorValuedRegex (string value, string expression, Node node, ApplicationContext context)
        {
            _value = value;
            _expression = expression;
            _node = node;
            _context = context;
        }

        public override IEnumerable<Node> Evaluate {
            get {
                if (_value.LastIndexOf ("/") == 0)
                    throw new ExpressionException (
                        _expression, 
                        "token; '" + _value + "' is not a valid regular expression, missing back slash at end of regex.",
                        _node,
                        _context);
                string optionsString = _value.Substring (_value.LastIndexOf ("/") + 1);
                bool distinct = optionsString.IndexOf ('d') > -1;
                Regex regex = new Regex (
                    _value.Substring (1, _value.LastIndexOf ("/") - 1), 
                    GetOptions (optionsString));
                if (distinct) {

                    // requesting "distinct" (unique) values
                    Dictionary<string, bool> dict = new Dictionary<string, bool> ();
                    foreach (Node idxCurrent in Left.Evaluate) {
                        if (idxCurrent.Value != null) {
                            string valueOfNode = idxCurrent.Get<string> (_context);
                            if (!dict.ContainsKey (valueOfNode) && regex.IsMatch (valueOfNode)) {
                                dict [valueOfNode] = true;
                                yield return idxCurrent;
                            }
                        }
                    }
                } else {

                    // requesting all node values that matches regular expression normally
                    foreach (Node idxCurrent in Left.Evaluate) {
                        if (idxCurrent.Value != null) {
                            if (regex.IsMatch (idxCurrent.Get<string> (_context)))
                                yield return idxCurrent;
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
            RegexOptions options = RegexOptions.CultureInvariant;

            // looping through all options given
            foreach (char idx in optionsString) {
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
                        string.Format ("'{0}' is not a recognized option for regular expression iterator"),
                        _node,
                        _context);
                }
            }
            return options;
        }
    }
}
