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
    ///     Base class for all regular expression iterators.
    /// 
    ///     Not to be consumed directly by your code, but indirectly accessible through IteratorNamedRegex and IteratorValuedRegex.
    ///     Contains helper methods for previously mentioned iterators, such as extracting regex options, etc.
    /// </summary>
    public abstract class IteratorRegex : Iterator
    {
        /// <summary>
        ///     Returns regular expression engine options for regex iterator.
        /// </summary>
        /// <returns>Regex options.</returns>
        /// <param name="optionsString">String containing textual representation of all options.</param>
        /// <param name="expression">Necessary in case an exception needs to be raised, to provide contextual information.</param>
        /// <param name="node">Necessary to provide contextual information in case an exception occurs.</param>
        /// <param name="context">Application context</param>
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