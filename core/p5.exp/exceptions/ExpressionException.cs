/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;

/// <summary>
///     Main namespace for exceptions within the phosphorus.expressions project
/// </summary>
namespace p5.exp.exceptions
{
    /// <summary>
    ///     Exception thrown when expressions contains syntax errors
    /// </summary>
    public class ExpressionException : ArgumentException
    {
        private readonly string _expression;
        private readonly string _message;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionException" /> class
        /// </summary>
        /// <param name="expression">Expression that caused exception</param>
        public ExpressionException (string expression)
            : this (expression, null) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionException" /> class
        /// </summary>
        /// <param name="expression">Expression that caused exception</param>
        /// <param name="message">Message providing additional information</param>
        /// <param name="node">Node where expression was found</param>
        /// <param name="context">Application context Necessary to perform conversion from p5 lambda to Hyperlambda to show Hyperlambda StackTrace</param>
        public ExpressionException (string expression, string message)
        {
            _message = message;
            _expression = expression;
        }

        /*
         * Overriding Message to provide expression that malfunctioned as an additional piece of contextual information
         */
        public override string Message
        {
            get
            {
                var retVal = string.Format ("Expression '{0}' is not a valid expression", _expression);
                if (_message != null)
                    retVal = _message + "\r\n" + retVal;
                return retVal;
            }
        }
    }
}
