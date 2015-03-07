/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
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
    ///     Returns all nodes having a value matching the given regular expression.
    /// 
    ///     Will return all nodes who's value matches the given regular expression. Notice that when you use this Iterator, 
    ///     the entire iterator's value must be put into either double quote ("), to signify a single line string literal, 
    ///     or created using multi-line strings, since the iterator is having "slash" (/) as part of its control mechanisms.
    /// 
    ///     Example; <pre>/"=/foo/"</pre>
    /// 
    ///     Will match all nodes who's value contains the text "foo".
    /// 
    ///     You can optionally pass in arguments to the regular expression engine, after the last slash (/) of your iterator's content.
    ///     These parameters can be either of;
    /// 
    ///     <strong>d</strong> - Only return distinct names. If two nodes have the same names, only return the first node matching.
    /// 
    ///     <strong>i</strong> - Ignore case when doing the comparison.
    /// 
    ///     <strong>m</strong> - Multiline regex comparison.
    /// 
    ///     <strong>c</strong> - Compile the regular expression to optimize execution for later usage of the same regular expression.
    /// 
    ///     <strong>e</strong> - Use ECMA script regular expression type.
    /// 
    ///     <strong>w</strong> - Ignore white space patterns.
    /// 
    ///     <strong>r</strong> - Execute the regular expression from right to left.
    /// 
    ///     <strong>s</strong> - Single line regular expression mode.
    /// 
    ///     Here's an example of how to use ECMA script type of regular expressions, compile the regular expression, while only 
    ///     returning distinct values, to return any node's who's values contains the text "bar";
    /// 
    ///     <pre>/"=/bar/edc"</pre>
    /// </summary>
    public class IteratorValuedRegex : IteratorRegex
    {
        private readonly ApplicationContext _context;
        // kept around to be able to create sane exceptions
        private readonly string _expression;
        private readonly Node _node;
        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorValuedRegex"/> class.
        /// </summary>
        /// <param name="regex">Regular expression to match.</param>
        /// <param name="expression">Expression containing the Iterator. Necessary to provide contextual information in case an
        /// exception occurs.</param>
        /// <param name="node">The node containing the Expression. Also necessary to provide contextual information in case an exception
        /// occurs.</param>
        /// <param name="context">Application context.</param>
        public IteratorValuedRegex (string regex, string expression, Node node, ApplicationContext context)
        {
            _value = regex;
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