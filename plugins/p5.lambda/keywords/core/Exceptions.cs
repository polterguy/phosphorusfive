/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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

using System;
using System.Runtime.ExceptionServices;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping exception Active Events, and lambdas, such as [try], [catch], [throw] and [finally].
    /// </summary>
    public static class Exceptions
    {
        /// <summary>
        ///     The [try] event allows you to execute a piece of lambda, trapping any potential exceptions occurring.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "try")]
        public static void lambda_try (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity checking [try]/[catch]/[finally] chain.
            SanityCheckTry (context, e);

            // Executing [try] lambda, such that we can trap any exceptions occurring during execution.
            try {

                // Evaluating [try] lambda block.
                context.Raise ("eval-mutable", e.Args);

            } catch (Exception err) {

                // Storing exception message as [_exception] in parent of e.Args, to let [catch] have access to it, 
                // but only if our next node is a [catch] lambda.
                if (e.Args.NextSibling.Name == "catch") {

                    // Storing exception for [catch] block.
                    e.Args.Parent.Insert (0, new Node ("_exception", err));

                } else if (e.Args.NextSibling.Name == "finally") {

                    // Evaluating [finally] block before rethrowing exception.
                    // Notice, we do this by invoking [finally], even though we could have simply evaluated finally block directly.
                    // This is due to things such as for instance [eval-whitelist], and other injection patterns, needing to know and/or control which
                    // exact Active Events are being raised in the system.
                    context.Raise ("finally", e.Args.NextSibling);

                    // Rethrowing after having evaluated [finally].
                    ExceptionDispatchInfo.Capture (err.InnerException).Throw ();
                }
            }
        }

        /// <summary>
        ///     The [catch] event allows you to force the execution of a piece of lambda, even if an exception occurs, 
        ///     stopping the exception from further bubbling up your stack.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "catch")]
        public static void lambda_catch (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check.
            if ((e.Args.PreviousSibling?.Name ?? "") != "try")
                throw new LambdaException ("You cannot have a [catch] lambda without a preceeding [try]", e.Args, context);

            // Checking if an exception did indeed occur
            if (e.Args.Parent [0].Name == "_exception" && e.Args.Parent [0].Value is Exception) {

                // Making sure we delete exception, and replace with exception text
                e.Args.Insert (0, new Node ("message", (e.Args.Parent [0].Value as Exception).Message));
                e.Args.Insert (1, new Node ("stack-trace", (e.Args.Parent [0].Value as Exception).StackTrace));
                e.Args.Insert (2, new Node ("type", (e.Args.Parent [0].Value as Exception).GetType ().FullName));
                e.Args.Insert (3, new Node ("offset", 3));
                e.Args.Parent ["_exception"].UnTie ();

                // Evaluating [catch] lambda block
                context.Raise ("eval-mutable", e.Args);

                /*
                 * Notice, we do not directly raise [finally] here, since unless another exception occurs inside of [catch] block, it will
                 * be the next executed Active Event in the [try]/[catch]/[finally] chain, if there is a [finally] block though ...
                 * And if an exception occurs inside of a [catch], there's no ways we could recover anyways, and the system would be in an
                 * "undefined" and instable state anyway ...!! :P
                 */
            }
        }

        /// <summary>
        ///     The [finally] event allows you to make sure a piece of lambda is executed, even if an exception occurs.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "finally")]
        public static void lambda_finally (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check.
            var previousNodeName = (e.Args.PreviousSibling?.Name ?? "");
            if (previousNodeName != "try" && previousNodeName != "catch")
                throw new LambdaException ("You cannot have a [finally] lambda without a preceeding [try]", e.Args, context);

            // Evaluating [finally] lambda block.
            context.Raise ("eval-mutable", e.Args);
        }

        /// <summary>
        ///     The [throw] event allows you to throw an exception with an optional message attached.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "throw")]
        public static void lambda_throw (ApplicationContext context, ActiveEventArgs e)
        {
            throw new LambdaException (e.Args.GetExValue (context, "No message provided when exception was raised."), e.Args, context);
        }

        /*
         * Sanity checks [try]/[catch]/[finally], making sure at least one of [catch] or [finally] occurs, and that their occur in the right order.
         */
        private static void SanityCheckTry (ApplicationContext context, ActiveEventArgs e)
        {
            // Used to track if at least one of [catch] or [finally] have been seen.
            bool seenCatchFinally = false;

            // Looping to next sibling as long as it is either [catch] or [finally].
            var idxNode = e.Args.NextSibling;
            while (idxNode != null && (idxNode.Name == "catch" || idxNode.Name == "finally")) {

                // Verifying that [finally] is always after any [catch] lambdas.
                if (idxNode.PreviousSibling.Name == "finally")
                    throw new LambdaException ("[finally] must be the last lambda in your [try]/[catch]/[finally] chain.", e.Args, context);

                // Verifying that there is only one [catch] lambda.
                if (idxNode.Name == "catch" && idxNode.PreviousSibling.Name == "catch")
                    throw new LambdaException ("You can only have one [catch] lambda block in your [try]/[catch]/[finally] chain.", e.Args, context);

                seenCatchFinally = true;
                idxNode = idxNode.NextSibling;
            }

            // Making sure at least one of [catch] or [finally] appeared in chain.
            if (!seenCatchFinally)
                throw new LambdaException ("A [try] lambda must be followed by either a [catch], a [finally], or both in the mentioned order.", e.Args, context);
        }
    }
}
