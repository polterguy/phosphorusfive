/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;
using p5.exp;
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
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "while")]
        private static void lambda_while (ApplicationContext context, ActiveEventArgs e)
        {
            // storing old while "body"
            Node oldWhile = e.Args.Clone ();

            // storing old while value, since Evaluate changes it to either true or false
            var oldWhileValue = e.Args.Value;

            while (Conditions.Evaluate (context, e.Args)) {

                // changing value back to what it was, to support things like "while:int:5" and so on
                e.Args.Value = oldWhileValue;

                // executing current scope as long as while evaluates to true
                Conditions.ExecuteCurrentScope (context, e.Args);

                // making sure each iteration is immutable
                e.Args.Clear ();
                e.Args.AddRange (oldWhile.Clone ().Children);
            }
        }
    }
}
