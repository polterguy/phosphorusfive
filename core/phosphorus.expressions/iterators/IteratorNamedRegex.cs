/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using phosphorus.core;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     Returns all nodes who's names matches the specified regular expression.
    /// 
    ///     Will return all nodes who's name matches the given regular expression. Notice that when you use this Iterator, 
    ///     the entire iterator's value must be put into either double quote ("), to signify a single line string literal, 
    ///     or created using multi-line strings, since the iterator is recognized as starting and ending with a "slash".
    /// 
    ///     Example; <pre>/"/foo/"</pre>
    /// 
    ///     Will match all nodes who's name contains the text "foo".
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
    ///     returning distinct names, to return any node's who's names are "bar";
    /// 
    ///     <pre>/"/bar/edc"</pre>
    /// </summary>
    public class IteratorNamedRegex : IteratorRegex
    {
        private readonly ApplicationContext _context;
        // kept around to be able to create sane exceptions
        private readonly string _expression;
        private readonly Node _node;
        private readonly string _options;
        private readonly string _regex;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorNamed" /> class.
        /// </summary>
        /// <param name="regex">regular expression</param>
        /// <param name="expression">pf.lambda expression</param>
        /// <param name="node">node</param>
        /// <param name="context">application context</param>
        public IteratorNamedRegex (string regex, string expression, Node node, ApplicationContext context)
        {
            _regex = regex.Substring (1, regex.LastIndexOf ("/", StringComparison.Ordinal) - 1);
            _options = regex.Substring (regex.LastIndexOf ("/", StringComparison.Ordinal) + 1);
            _expression = expression;
            _node = node;
            _context = context;
        }

        public override IEnumerable<Node> Evaluate
        {
            get
            {
                var distinct = _options.IndexOf ('d') > -1;
                var regex = new Regex (_regex, GetOptions (_options, _expression, _node, _context));
                if (distinct) {
                    var dict = new Dictionary<string, bool> ();
                    foreach (var idxCurrent in Left.Evaluate.Where (idxCurrent => !dict.ContainsKey (idxCurrent.Name) && regex.IsMatch (idxCurrent.Name))) {
                        dict [idxCurrent.Name] = true;
                        yield return idxCurrent;
                    }
                } else {
                    foreach (var idxCurrent in Left.Evaluate.Where (idxCurrent => regex.IsMatch (idxCurrent.Name))) {
                        yield return idxCurrent;
                    }
                }
            }
        }
    }
}
