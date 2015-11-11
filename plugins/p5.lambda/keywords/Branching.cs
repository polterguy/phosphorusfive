/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using p5.core;
using p5.exp;
using p5.exp.exceptions;
using p5.lambda.helpers;

namespace p5.lambda.keywords
{
    /// <summary>
    ///     Class wrapping conditional p5.lambda keywords, such as [if], [else-if] and [else].
    /// 
    ///     Class wrapping p5.lambda keywords [if], [else-if] and [else], which allows for branching, or conditional execution of p5.lambda nodes.
    /// </summary>
    public static class Branching
    {
        /// <summary>
        ///     The [if] keyword allows for conditional execution of p5.lambda nodes.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "if")]
        private static void lambda_if (ApplicationContext context, ActiveEventArgs e)
        {
            // evaluating condition
            if (Conditions.Evaluate (context, e.Args)) {

                // executing current scope
                Conditions.ExecuteCurrentScope (context, e.Args);
            }
        }

        /// <summary>
        ///     The [else-if] keyword allows for conditional execution of p5.lambda nodes.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "else-if")]
        private static void lambda_else_if (ApplicationContext context, ActiveEventArgs e)
        {
            // syntax checking statement
            VerifyElseSyntax (e.Args, context, "else-if");

            // checking if previous [if] or [else-if] returned true, and if not, evaluating current node
            if (PreviousConditionEvaluatedTrue (e.Args, context)) {
                e.Args.Clear ();
                e.Args.Value = false;
                return;
            }

            // evaluating current scope
            if (Conditions.Evaluate (context, e.Args)) {

                // executing current scope (maybe)
                Conditions.ExecuteCurrentScope (context, e.Args);
            }
        }

        /// <summary>
        ///     The [else] keyword allows for conditional execution of p5.lambda nodes.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "else")]
        private static void lambda_else (ApplicationContext context, ActiveEventArgs e)
        {
            // syntax checking statement
            VerifyElseSyntax (e.Args, context, "else");
            
            // checking if previous [if] or [else-if] returned true, and if not, evaluating current node
            if (PreviousConditionEvaluatedTrue (e.Args, context)) {
                e.Args.Clear ();
                e.Args.Value = false;
                return;
            }

            // since no previous conditions evaluated to true, we simply execute this scope, 
            // without checking any conditions, since there are none!
            e.Args.Value = true;
            Conditions.ExecuteCurrentScope (context, e.Args);
        }

        /*
         * checks if previous conditional statement ([if] or [else-if]) evaluated to true, and if so
         * return true, else returns false to caller
         */
        private static bool PreviousConditionEvaluatedTrue (Node args, ApplicationContext context)
        {
            Node curIdx = args.PreviousSibling;
            while (true) {

                if (curIdx.Name == "if" || curIdx.Name == "else-if") {
                    var val = curIdx.Get<bool> (context);
                    if (val)
                        return true;
                } else {

                    // no previous condition has evaluated to true
                    return false;
                }
                curIdx = curIdx.PreviousSibling;
            }
        }

        /*
         * verifies that an [else] or [else-if] has a previous [if]
         */
        private static void VerifyElseSyntax (Node node, ApplicationContext context, string statement)
        {
            var previous = node.PreviousSibling;
            while (true) {

                if (previous == null || (previous.Name != "if" && previous.Name != "else-if"))
                    throw new LambdaException ("you cannot have an [" + statement + "] without a matching [if] and/or [else-if] as its previous sibling", node, context);
                if (previous.Name == "if")
                    break;
            }
        }
    }
}
