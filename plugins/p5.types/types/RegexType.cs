/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.Text.RegularExpressions;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.types.types
{
    /// <summary>
    ///     Class helps converts from Regex to string, and vice versa
    /// </summary>
    public static class RegexType
    {
        /// <summary>
        ///     Creates a long from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-object-value.regex")]
        static void p5_hyperlisp_get_object_value_regex (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value is Regex) {
                return;
            }
            var strValue = XUtil.FormatNode (context, e.Args).ToString ();

            // Sanity check
            if (!strValue.StartsWithEx ("/"))
                throw new LambdaException ("Syntax error in regular expression, missing initial /", e.Args, context);

            // Converting string to regular expression
            var regexString = strValue.Substring (1); // Removing initial "/"

            // More sanity check
            if (!regexString.Contains ("/"))
                throw new LambdaException ("Syntax error in regular expression, missing trailing /", e.Args, context);

            // Retrieving options
            var options = regexString.Substring (regexString.LastIndexOfEx ("/") + 1);

            // Removing options from actual regex string
            regexString = regexString.Substring (0, regexString.LastIndexOfEx ("/"));

            e.Args.Value = new Regex (regexString, GetOptions (context, e.Args, options));
        }

        /// <summary>
        ///     Creates a string from a regular expression
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-string-value.System.Text.RegularExpressions.Regex")]
        static void p5_hyperlisp_get_string_value_System_DateTime (ApplicationContext context, ActiveEventArgs e)
        {
            var value = e.Args.Get<Regex> (context);
            var retVal = "/" + value + "/";
            retVal += GetOptionString (value);
            e.Args.Value = retVal;
        }

        /// <summary>
        ///     Returns the Hyperlambda type-name for the regex type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-type-name.System.Text.RegularExpressions.Regex")]
        static void p5_hyperlisp_get_type_name_System_Text_RegularExpressions_Regex (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "regex";
        }

        static string GetOptionString (Regex regex)
        {
            var retVal = "";
            if ((regex.Options & RegexOptions.Compiled) == RegexOptions.Compiled)
                retVal += "o";
            if ((regex.Options & RegexOptions.CultureInvariant) == RegexOptions.CultureInvariant)
                retVal += "c";
            if ((regex.Options & RegexOptions.ECMAScript) == RegexOptions.ECMAScript)
                retVal += "e";
            if ((regex.Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase)
                retVal += "i";
            if ((regex.Options & RegexOptions.Multiline) == RegexOptions.Multiline)
                retVal += "m";
            if ((regex.Options & RegexOptions.RightToLeft) == RegexOptions.RightToLeft)
                retVal += "r";
            if ((regex.Options & RegexOptions.Singleline) == RegexOptions.Singleline)
                retVal += "s";
            if ((regex.Options & RegexOptions.IgnorePatternWhitespace) == RegexOptions.IgnorePatternWhitespace)
                retVal += "w";
            if ((regex.Options & RegexOptions.ExplicitCapture) == RegexOptions.ExplicitCapture)
                retVal += "x";
            return retVal;
            
        }

        /*
         * Parses supplied options and returns as Option object to caller
         */
        static RegexOptions GetOptions (
            ApplicationContext context,
            Node args,
            string optStr)
        {
            // Creating return value
            var retVal = RegexOptions.None;

            // Looping through each character in option string, converting to RegexOption enum, and appending to return value
            foreach (var idxChar in optStr) {
                switch (idxChar) {
                case 'o':
                    retVal |= RegexOptions.Compiled;
                    break;
                case 'c':
                    retVal |= RegexOptions.CultureInvariant;
                    break;
                case 'e':
                    retVal |= RegexOptions.ECMAScript;
                    break;
                case 'i':
                    retVal |= RegexOptions.IgnoreCase;
                    break;
                case 'm':
                    retVal |= RegexOptions.Multiline;
                    break;
                case 'r':
                    retVal |= RegexOptions.RightToLeft;
                    break;
                case 's':
                    retVal |= RegexOptions.Singleline;
                    break;
                case 'w':
                    retVal |= RegexOptions.IgnorePatternWhitespace;
                    break;
                case 'x':
                    retVal |= RegexOptions.ExplicitCapture;
                    break;
                default:
                    throw new LambdaException ("Unknown option supplied for regex type '" + idxChar + "'. Legal values are 'oceimrswx'", args, context);
                }
            }

            // Returning parsed options to callr
            return retVal;
        }
    }
}
