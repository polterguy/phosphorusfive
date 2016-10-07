/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
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
using p5.core;

namespace p5.exp.exceptions
{
    /// <summary>
    ///     Exception thrown when p5 lambda security breach is encountered
    /// </summary>
    public class LambdaSecurityException : LambdaException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LambdaSecurityException" /> class
        /// </summary>
        /// <param name="message">Message for exception, describing what went wrong</param>
        /// <param name="node">Node where expression was found</param>
        /// <param name="context">Application context Necessary to perform conversion from p5 lambda to Hyperlambda to show Hyperlambda StackTrace</param>
        public LambdaSecurityException (string message, Node node, ApplicationContext context, Exception innerException = null)
            : base (message, node, context, innerException)
        { }
    }
}
