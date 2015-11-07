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
    /// 
    ///     The [while] keyword allows you to loop for as long as some condition is true.
    /// </summary>
    public static class While
    {
        /// <summary>
        ///     The [while] keyword allows you to loop for as long as some condition is true.
        /// 
        ///     The [while] keyword is an alternative to [for-each] to create loops. However, where the 
        ///     [for-each] loops for each item in a result-set, [while] loops as long as some 
        ///     condition is true.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "while")]
        private static void lambda_while (ApplicationContext context, ActiveEventArgs e)
        {
            // storing old while "body"
            Node oldWhile = e.Args.Clone ();

            // evaluating condition
            Conditions.LoopThrough (context, e.Args);

            while (true) {

                // executing current scope as long as while evaluates to true
                bool executed = Conditions.TryExecuteCurrentScope (context, e.Args);
                if (!executed)
                    break; // ending while

                // making sure each iteration is immutable
                // do NOT copy back old value unless it is an expression, to allow for things such as "while:foo-bar"
                if (oldWhile.Value is Expression)
                    e.Args.Value = oldWhile.Value;
                e.Args.Clear ();
                e.Args.AddRange (oldWhile.Clone ().Children);
                
                // evaluating condition, again
                Conditions.LoopThrough (context, e.Args);
            }
        }
    }
}
