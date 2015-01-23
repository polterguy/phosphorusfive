
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
    /// returns all <see cref="phosphorus.core.Node"/>s from previous iterated result with a specified string value
    /// </summary>
    public class IteratorValued : Iterator
    {
        private string _value;
        private string _type;

        /// <summary>
        /// gets or sets the value to search for
        /// </summary>
        /// <value>the value</value>
        public string Value {
            get {
                return _value;
            }
            set {
                _value = value;
            }
        }

        /// <summary>
        /// gets or sets the type the value is
        /// </summary>
        /// <value>the type</value>
        public string Type {
            get {
                return _type;
            }
            set {
                _type = value;
            }
        }

        public override IEnumerable<Node> Evaluate {
            get {
                string value = _value;
                if (!string.IsNullOrEmpty (Type)) {
                    if (value.StartsWith ("\\"))
                        value = value.Substring (1);
                    ApplicationContext ctx = Loader.Instance.CreateApplicationContext ();
                    object objValue = ctx.Raise ("pf.hyperlist.get-object-value." + Type, new Node (string.Empty, value)).Value;
                    foreach (Node idxCurrent in Left.Evaluate) {
                        if (objValue.Equals (idxCurrent.Value))
                            yield return idxCurrent;
                    }
                } else if (value.StartsWith ("/")) {

                    // regular expression
                    if (value.LastIndexOf ("/") == 0)
                        throw new ArgumentException ("token; '" + value + "' is not a valid regular expression");
                    string _options = value.Substring (value.LastIndexOf ("/") + 1);
                    RegexOptions options = RegexOptions.CultureInvariant;
                    if (_options.IndexOf ('i') > -1)
                        options |= RegexOptions.IgnoreCase;
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
                    Regex regex = new Regex (value.Substring (1, value.LastIndexOf ("/") - 1), options);
                    foreach (Node idxCurrent in Left.Evaluate) {
                        if (idxCurrent.Value != null) {
                            if (regex.IsMatch (idxCurrent.Get<string> ()))
                                yield return idxCurrent;
                        }
                    }
                } else {
                   // to support iterators who's value starts with "/" or ".." to escape out of Regex and named ancestor iterators
                   if (value.StartsWith ("\\"))
                        value = value.Substring (1);
                    foreach (Node idxCurrent in Left.Evaluate) {
                        if (value.Equals (idxCurrent.Value))
                            yield return idxCurrent;
                    }
                }
            }
        }
    }
}
