
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using phosphorus.core;

namespace phosphorus.lambda.iterators
{
    /// <summary>
    /// returns all nodes with the specified name of the previous iterator
    /// </summary>
    public class IteratorNamedRegex : Iterator
    {
        private string _regex;
        private string _options;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.iterators.IteratorNamed"/> class
        /// </summary>
        /// <param name="name">name to match</param>
        public IteratorNamedRegex (string regex)
        {
            _regex = regex.Substring (1, regex.LastIndexOf ("/") - 1);
            _options = regex.Substring (regex.LastIndexOf ("/") + 1);
        }

        public override IEnumerable<Node> Evaluate {
            get {
                bool distinct = _options.IndexOf ('d') > -1;
                RegexOptions options = RegexOptions.None;
                if (_options.IndexOf ('i') > -1)
                    options |= RegexOptions.IgnoreCase;
                if (_options.IndexOf ('g') > -1)
                    options |= RegexOptions.CultureInvariant;
                if (_options.IndexOf ('m') > -1)
                    options |= RegexOptions.Multiline;
                if (_options.IndexOf ('c') > -1)
                    options |= RegexOptions.Compiled;
                if (_options.IndexOf ('e') > -1)
                    options |= RegexOptions.ECMAScript;
                if (_options.IndexOf ('w') > -1)
                    options |= RegexOptions.IgnorePatternWhitespace;
                if (_options.IndexOf ('r') > -1)
                    options |= RegexOptions.RightToLeft;
                if (_options.IndexOf ('l') > -1)
                    options |= RegexOptions.RightToLeft;
                if (_options.IndexOf ('s') > -1)
                    options |= RegexOptions.Singleline;
                Regex regex = new Regex (_regex, options);
                if (distinct) {
                    Dictionary<string, bool> dict = new Dictionary<string, bool> ();
                    foreach (Node idxCurrent in Left.Evaluate) {
                        if (regex.IsMatch (idxCurrent.Name) && !dict.ContainsKey (idxCurrent.Name)) {
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
