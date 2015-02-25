/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using phosphorus.core;
using phosphorus.expressions.exceptions;
using phosphorus.lambda.keywords.helpers;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.lambda.keywords
{
    /// <summary>
    ///     class wrapping execution engine keyword "pf.if", "pf.else-if" and "pf.else", which allows for branching or
    ///     conditional execution of nodes
    /// </summary>
    public static class Branching
    {
        /// <summary>
        ///     [if] statement, allowing for evaluating condition, and executing lambda(s) if statement evaluates to true
        /// </summary>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext" /> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "if")]
        private static void lambda_if (ApplicationContext context, ActiveEventArgs e)
        {
            var condition = new Conditions (e.Args, context);
            if (condition.Evaluate ()) {
                // executing block within [if]
                ExecuteIfOrElseIf (condition, e.Args, context);

                // making sure we "signal" to related [else-if] and/or [else] statements
                // that statement chain has already evaluated to true
                CreateEvaluatedSignal (e.Args);
            }
        }

        /// <summary>
        ///     [else-if] statement, allowing for evaluating condition, and executing lambda(s) if statement evaluates to true, and
        ///     no previous [if] or previous [else-if] has evaluated to true
        /// </summary>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext" /> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "else-if")]
        private static void lambda_else_if (ApplicationContext context, ActiveEventArgs e)
        {
            // syntax checking statement
            VerifyElseSyntax (e.Args, context, "else-if");

            // checking to see if previous "if" or "else-if" has already evaluated to true
            if (e.Args.Parent [0].Name == "__pf_evaluated") {
                // previous [if] or [else-if] in chain has already evaluated to true
                // hence, we don't even evaluate this statement, or execute this instance, 
                // but we might have to remove signal node
                TryRemoveSignal (e.Args);
            } else {
                var condition = new Conditions (e.Args, context);
                if (condition.Evaluate ()) {
                    // executing block within [else-if]
                    ExecuteIfOrElseIf (condition, e.Args, context);

                    // making sure we "signal" to any [else-if] and/or [else] that statement evaluated to true
                    CreateEvaluatedSignal (e.Args);
                }
            }
        }

        /// <summary>
        ///     [else] statement, allowing for executing lambda(s) if no previous [if] or [else-if] has evaluated to true
        /// </summary>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext" /> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "else")]
        private static void lambda_else (ApplicationContext context, ActiveEventArgs e)
        {
            // syntax checking statement
            VerifyElseSyntax (e.Args, context, "else");

            // checking to see if previous "if" or "else-if" has already evaluated to true
            if (e.Args.Parent [0].Name == "__pf_evaluated") {
                // previous [if] or [else-if] in chain has already evaluated to true
                // hence, we don't execute this instance, however since this is [else], being
                // last statement in branching chain, we'll have to remove "signal node", if it exists
                e.Args.Parent.RemoveAt (0);
            } else {
                // since [else] does not have any conditions, we simply execute its children, unless previous
                // branching statements have evaluated to true
                context.Raise ("lambda", e.Args);
            }
        }

        /*
         * creates a "signal node" for next statements in if/else-if/else chain signaling that
         * current statement evaluated to "true"
         */

        private static void CreateEvaluatedSignal (Node node)
        {
            var next = node.NextSibling;
            if (next != null && (next.Name == "else-if" || next.Name == "else")) {
                node.Parent.Insert (0, new Node ("__pf_evaluated")); // "signal node" inserted
            }
        }

        /*
         * verifies that an [else] or [else-if] has a previous [if]
         */

        private static void VerifyElseSyntax (Node node, ApplicationContext context, string statement)
        {
            var previous = node.PreviousSibling;
            if (previous == null || (previous.Name != "if" && previous.Name != "else-if"))
                throw new LambdaException ("you cannot have an [" + statement + "] without a matching [if] or [else-if] as its previous sibling", node, context);
        }

        /*
         * removes signal node if next execution statement is not part of current [if]/[else-if]/[else] chain
         */

        private static void TryRemoveSignal (Node node)
        {
            var next = node.NextSibling;
            if (next == null || (next.Name != "else-if" && next.Name != "else")) {
                node.Parent.RemoveAt (0);
            }
        }

        /*
         * executes an [if] or an [else-if] block
         */

        private static void ExecuteIfOrElseIf (Conditions condition, Node node, ApplicationContext context)
        {
            if (condition.IsSimpleExist) {
                // code tree does not contain any [lambda] objects beneath [if]
                context.Raise ("lambda", node);
            } else {
                // code tree contains [lambda.xxx] objects beneath [while]
                foreach (var idxExe in condition.ExecutionLambdas) {
                    context.Raise (idxExe.Name, idxExe);
                }
            }
        }
    }
}