
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
    /// returns all nodes who's names matches the specified regular expression
    /// </summary>
    public class IteratorNamedRegex : IteratorRegex
    {
        private string _regex;
        private string _options;
        
        // kept around to be able to create sane exceptions
        private string _expression;
        private Node _node;
        private ApplicationContext _context;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.iterators.IteratorNamed"/> class
        /// </summary>
        /// <param name="regex">regular expression</param>
        public IteratorNamedRegex (string regex, string expression, Node node, ApplicationContext context)
        {
            _regex = regex.Substring (1, regex.LastIndexOf ("/") - 1);
            _options = regex.Substring (regex.LastIndexOf ("/") + 1);
            _expression = expression;
            _node = node;
            _context = context;
        }

        public override IEnumerable<Node> Evaluate {
            get {
                bool distinct = _options.IndexOf ('d') > -1;
                Regex regex = new Regex (_regex, GetOptions (_options, _expression, _node, _context));
                if (distinct) {
                    Dictionary<string, bool> dict = new Dictionary<string, bool> ();
                    foreach (Node idxCurrent in Left.Evaluate) {
                        if (!dict.ContainsKey (idxCurrent.Name) && regex.IsMatch (idxCurrent.Name)) {
                            dict [idxCurrent.Name] = true;
                            yield return idxCurrent;
                        }
                    }
                } else {
                    foreach (Node idxCurrent in Left.Evaluate) {
                        if (regex.IsMatch (idxCurrent.Name))
                            yield return idxCurrent;
                    }
                }
            }
        }
    }
}
