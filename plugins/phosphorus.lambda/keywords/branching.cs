
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.lambda
{
    /// <summary>
    /// class wrapping execution engine keyword "pf.if", "pf.else-if" and "pf.else", which allows for branching or
    /// conditional execution of nodes
    /// </summary>
    public static class branching
    {
        /// <summary>
        /// [if] statement, allowing for evaluating condition, and executing lambda(s) if statement evaluates to true
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "if")]
        private static void lambda_if (ApplicationContext context, ActiveEventArgs e)
        {
            var condition = new Conditions (e.Args, context);
            if (condition.Evaluate ()) {

                // if you are only checking for a value's existence, you don't need to supply a [lambda] object
                // beneath [while]. if you do not, [while] will execute as [lambda.immutable]
                if (condition.IsSimpleExist) {
                    
                    // code tree does not contain any [lambda] objects beneath [if]
                    context.Raise ("lambda", e.Args);
                } else {
                    
                    // code tree contains [lambda.xxx] objects beneath [while]
                    foreach (Node idxExe in condition.ExecutionLambdas) {
                        context.Raise (idxExe.Name, idxExe);
                    }
                }

                // making sure we "signal" to any [else-if] and/or [else] that statement evaluated to true
                Node next = e.Args.NextSibling;
                if (next != null && (next.Name == "else-if" || next.Name == "else")) {
                    e.Args.Parent.Insert (0, new Node ("__pf_evaluated")); // "signal node" inserted
                }
            }
        }

        /// <summary>
        /// [else-if] statement, allowing for evaluating condition, and executing lambda(s) if statement evaluates to true, and
        /// no previous [if] or previous [else-if] has evaluated to true
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "else-if")]
        private static void lambda_else_if (ApplicationContext context, ActiveEventArgs e)
        {
            // syntax checking statement
            Node previous = e.Args.PreviousSibling;
            if (previous == null || (previous.Name != "if" && previous.Name != "else-if"))
                throw new ArgumentException ("you cannot have a [else-if] statement without a matching [if] or [else-if] as its previous sibling");

            // checking to see if previous "if" or "else-if" has already evaluated to true
            if (e.Args.Parent [0].Name == "__pf_evaluated") {

                // previous [if] or [else-if] in chain has already evaluated to true
                // hence, we don't even evaluate or execute this instance
                Node next = e.Args.NextSibling;
                if (next == null || (next.Name != "else-if" && next.Name != "else")) {
                    e.Args.Parent.RemoveAt (0);
                }
            } else {
                var condition = new Conditions (e.Args, context);
                if (condition.Evaluate ()) {

                    // if you are only checking for a value's existence, you don't need to supply a [lambda] object
                    // beneath [while]. if you do not, [while] will execute as [lambda.immutable]
                    if (condition.IsSimpleExist) {

                        // code tree does not contain any [lambda] objects beneath [if]
                        context.Raise ("lambda", e.Args);
                    } else {

                        // code tree contains [lambda.xxx] objects beneath [while]
                        foreach (Node idxExe in condition.ExecutionLambdas) {
                            context.Raise (idxExe.Name, idxExe);
                        }
                    }

                    // making sure we "signal" to any [else-if] and/or [else] that statement evaluated to true
                    Node next = e.Args.NextSibling;
                    if (next != null && (next.Name == "else-if" || next.Name == "else")) {
                        e.Args.Parent.Insert (0, new Node ("__pf_evaluated", true));
                    }
                }
            }
        }
        
        /// <summary>
        /// [else] statement, allowing for executing lambda(s) if no previous [if] or [else-if] has evaluated to true
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "else")]
        private static void lambda_else (ApplicationContext context, ActiveEventArgs e)
        {
            // syntax checking statement
            Node previous = e.Args.PreviousSibling;
            if (previous == null || (previous.Name != "if" && previous.Name != "else-if"))
                throw new ArgumentException ("you cannot have a [else] without a matching [if] or [else-if] as its previous sibling");

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
    }
}
