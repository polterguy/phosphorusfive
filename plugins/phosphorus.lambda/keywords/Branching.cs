/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
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
    ///     Class wrapping conditional pf.lambda keywords, such as [if], [else-if] and [else].
    /// 
    ///     Class wrapping pf.lambda keywords [if], [else-if] and [else], which allows for branching, or conditional execution of pf.lambda nodes.
    /// </summary>
    public static class Branching
    {
        /// <summary>
        ///     The [if] keyword allows for conditional execution of pf.lambda nodes.
        /// 
        ///     Will evaluate some <see cref="phosphorus.lambda.keywords.helpers.Conditions">Condition</see>, and if the condition evaluates 
        ///     to true, it will execute the blocks of pf.lambda code inside of itself.
        /// 
        ///     Example;
        /// 
        ///     <pre>_foo:bar
        /// if:@/-?value
        ///   =:bar
        ///   lambda
        ///     set:@/././-?value
        ///       source:matched!</pre>
        /// 
        ///     The [if] statement, can also be coupled with [else-if] and [else] statements. The [else-if] statement allows you to check another
        ///     condition if your first [if] condition yields false. While the [else] statement, is the default pf.lambda nodes to execute, if
        ///     none of your [if] or [else-if] statements conditions yields true. Consider the following code;
        /// 
        ///     <pre>_foo:bar
        /// if:@/-?value
        ///   =:bar
        ///   lambda
        ///     set:@/././-?value
        ///       source:matched with if!
        /// else-if:@/-2?value
        ///   =:bar2
        ///   lambda
        ///     set:@/././-2?value
        ///       source:matched with else-if!
        /// else
        ///   set:@/./-3?value
        ///     source:matched with else!</pre>
        /// 
        ///     Try to change the above [_foo] node's value, first change it to "bar2", then change it to anything else, and see how that
        ///     affects your end result when executing the code.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
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
        ///     The [else-if] keyword allows for conditional execution of pf.lambda nodes.
        /// 
        ///     The [else-if] statement must be coupled with a preceeding [if] statement. To understand how the [else-if] keywords
        ///     works, see the documentation for <see cref="phosphorus.lambda.keywords.Branching.lambda_if">[if]</see>.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
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
        ///     The [else] keyword allows for conditional execution of pf.lambda nodes.
        /// 
        ///     The [else] statement must be coupled with a preceeding [if] statement. To understand how the [else] keywords
        ///     works, see the documentation for <see cref="phosphorus.lambda.keywords.Branching.lambda_if">[if]</see>.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
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