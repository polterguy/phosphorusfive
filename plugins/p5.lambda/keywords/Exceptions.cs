/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.lambda.keywords
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
        [ActiveEvent (Name = "try", Protection = EventProtection.LambdaClosed)]
        public static void lambda_try (ApplicationContext context, ActiveEventArgs e)
        {
            try {
                // Evaluating [try] lambda block
                context.RaiseLambda ("eval-mutable", e.Args);
            } catch (Exception err) {

                // Storing exception message as [_exception] in parent of e.Args, 
                // to let [catch] have access to it, but only if next node is [catch]
                if (e.Args.NextSibling.Name == "catch") {

                    // Storing exception for [catch] block
                    e.Args.Parent.Insert (0, new Node ("_exception", err));
                } else if (e.Args.NextSibling.Name == "finally") {

                    // Evaluating [finally] block before rethrowing exception
                    context.RaiseLambda ("eval-mutable", e.Args.NextNode);
                    ExceptionDispatchInfo.Capture(err.InnerException).Throw();
                }
            }
        }

        /// <summary>
        ///     The [catch] keyword allows you to execute code, only if an exceptions has occurred
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "catch", Protection = EventProtection.LambdaClosed)]
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
                context.RaiseLambda ("eval-mutable", e.Args);
            }
        }

        /// <summary>
        ///     The [throw] keyword allows you to throw an exception of type LambdaException with a message
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "throw", Protection = EventProtection.LambdaClosed)]
        public static void lambda_throw (ApplicationContext context, ActiveEventArgs e)
        {
            throw new LambdaException (e.Args.GetExValue (context, "no message"), e.Args, context);
        }
    }
}
