/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using phosphorus.core;
using phosphorus.lambda.keywords.helpers;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.lambda.keywords
{
    /// <summary>
    ///     class wrapping execution engine keyword "pf.while" for looping while condition is true
    ///     conditional execution of nodes
    /// </summary>
    public static class While
    {
        /// <summary>
        ///     "while" statement, allowing for evaluating condition, and executing lambda(s) as long as statement evaluates to
        ///     true
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "while")]
        private static void lambda_while (ApplicationContext context, ActiveEventArgs e)
        {
            var condition = new Conditions (e.Args, context);
            while (condition.Evaluate ()) {
                // if you are only checking for a value's existence, you don't need to supply a [lambda] object
                // beneath [while]. if you do not, [while] will execute as [lambda.immutable]
                if (condition.IsSimpleExist) {
                    // code tree does not contain any [lambda] objects beneath [while]
                    context.Raise ("lambda.immutable", e.Args);
                } else {
                    // code tree contains [lambda.xxx] objects beneath [while]
                    foreach (var idxExe in condition.ExecutionLambdas) {
                        context.Raise (idxExe.Name, idxExe);
                    }
                }
            }
        }
    }
}