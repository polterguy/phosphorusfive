/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5.lambda.helpers;

namespace p5.lambda.keywords
{
    /// <summary>
    ///     Class wrapping the [while] keyword in p5.lambda.
    /// </summary>
    public static class While
    {
        /// <summary>
        ///     The [while] keyword allows you to loop for as long as some condition is true.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "while", Protection = EntranceProtection.Lambda)]
        private static void lambda_while (ApplicationContext context, ActiveEventArgs e)
        {
            // storing old while "body"
            Node oldWhile = e.Args.Clone ();

            // storing old while value, since Evaluate changes it to either true or false
            var oldWhileValue = e.Args.Value;

            // Trying to prevent infinite loops
            int iterations = 0;
            bool uncheck = e.Args.GetExChildValue ("_unchecked", context, false);

            // Actual [while] loop
            while (Conditions.Evaluate (context, e.Args)) {

                // changing value back to what it was, to support things like "while:int:5" and so on
                e.Args.Value = oldWhileValue;

                // executing current scope as long as while evaluates to true
                Conditions.ExecuteCurrentScope (context, e.Args);

                // making sure each iteration is immutable
                e.Args.Clear ();
                e.Args.AddRange (oldWhile.Clone ().Children);

                if (!uncheck && iterations++ > 10000)
                    throw new LambdaException ("Possible infinite loop encountered, more than 10.000 iterations of [while] loop. If this is not correct, then please make sure you invoke your [while] with [_unchecked] equals false", e.Args, context);
            }
        }
    }
}
