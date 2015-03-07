/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Text.RegularExpressions;
using phosphorus.core;
using phosphorus.expressions.exceptions;

namespace phosphorus.expressions.iterators
{
    /// <summary>
    ///     base class for all regular expression iterators, providing helper methods
    /// </summary>
    public abstract class IteratorRegex : Iterator
    {
        /// <summary>
        ///     returns regular expression engine options for regex iterator
        /// </summary>
        /// <returns>regex options</returns>
        /// <param name="optionsString">string containing textual representation of all options</param>
        /// <param name="expression">necessary in case exception needs to be raised, to provide contextual information</param>
        /// <param name="node">same as expression</param>
        /// <param name="context">application context</param>
        protected RegexOptions GetOptions (string optionsString, string expression, Node node, ApplicationContext context)
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
                            expression,
                            string.Format ("'{0}' is not a recognized option for regular expression iterator", idx),
                            node,
                            context);
                }
            }
            return options;
        }
    }
}