
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
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
    /// returns all nodes who's names matches the specified regular expression
    /// </summary>
    public class IteratorNamedRegex : IteratorRegex
    {
        private readonly string _regex;
        private readonly string _options;
        
        // kept around to be able to create sane exceptions
        private readonly string _expression;
        private readonly Node _node;
        private readonly ApplicationContext _context;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorNamed"/> class
        /// </summary>
        /// <param name="regex">regular expression</param>
        /// <param name="expression">pf.lambda expression</param>
        /// <param name="node">node</param>
        /// <param name="context">application context</param>
        public IteratorNamedRegex (string regex, string expression, Node node, ApplicationContext context)
        {
            _regex = regex.Substring (1, regex.LastIndexOf("/", StringComparison.InvariantCulture) - 1);
            _options = regex.Substring (regex.LastIndexOf("/", StringComparison.InvariantCulture) + 1);
            _expression = expression;
            _node = node;
            _context = context;
        }

        public override IEnumerable<Node> Evaluate {
            get {
                var distinct = _options.IndexOf ('d') > -1;
                var regex = new Regex (_regex, GetOptions (_options, _expression, _node, _context));
                if (distinct) {
                    var dict = new Dictionary<string, bool> ();
                    foreach (var idxCurrent in Left.Evaluate.Where(idxCurrent => !dict.ContainsKey(idxCurrent.Name) && regex.IsMatch(idxCurrent.Name))) {
                        dict [idxCurrent.Name] = true;
                        yield return idxCurrent;
                    }
                }
                else {
                    foreach (var idxCurrent in Left.Evaluate.Where(idxCurrent => regex.IsMatch (idxCurrent.Name))) {
                        yield return idxCurrent;
                    }
                }
            }
        }
    }
}
