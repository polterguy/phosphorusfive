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
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.Runtime.ExceptionServices;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping exception support in p5 lambda.
    /// </summary>
    public static class Exceptions
    {
        /// <summary>
        ///     The [try] keyword allows you to execute code, while trapping any potential exceptions occurring
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "try")]
        public static void lambda_try (ApplicationContext context, ActiveEventArgs e)
        {
            try {
                // Evaluating [try] lambda block
                context.Raise ("eval-mutable", e.Args);
            } catch (Exception err) {

                // Storing exception message as [_exception] in parent of e.Args, 
                // to let [catch] have access to it, but only if next node is [catch]
                if (e.Args.NextSibling.Name == "catch") {

                    // Storing exception for [catch] block
                    e.Args.Parent.Insert (0, new Node ("_exception", err));
                } else if (e.Args.NextSibling.Name == "finally") {

                    // Evaluating [finally] block before rethrowing exception
                    context.Raise ("eval-mutable", e.Args.NextNode);
                    ExceptionDispatchInfo.Capture(err.InnerException).Throw();
                }
            }
        }

        /// <summary>
        ///     The [catch] keyword allows you to execute code, only if an exceptions has occurred
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "catch")]
        public static void lambda_catch (ApplicationContext context, ActiveEventArgs e)
        {
            // Checking if an exception did indeed occur
            if (e.Args.Parent [0].Name == "_exception" && e.Args.Parent [0].Value is Exception) {

                // Making sure we delete exception, and replace with exception text
                e.Args.Insert (0, new Node ("message", (e.Args.Parent [0].Value as Exception).Message));
                e.Args.Insert (1, new Node ("stack-trace", (e.Args.Parent [0].Value as Exception).StackTrace));
                e.Args.Insert (2, new Node ("type", (e.Args.Parent [0].Value as Exception).GetType ().FullName));
                e.Args.Insert (3, new Node ("offset", 4));
                e.Args.Parent ["_exception"].UnTie ();

                // Evaluating [catch] lambda block
                context.Raise ("eval-mutable", e.Args);
            }
        }

        /// <summary>
        ///     The [throw] keyword allows you to throw an exception of type LambdaException with a message
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "throw")]
        public static void lambda_throw (ApplicationContext context, ActiveEventArgs e)
        {
            throw new LambdaException (e.Args.GetExValue (context, "no message"), e.Args, context);
        }
    }
}
