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
            if (e.Args.Count == 0)
                throw new ArgumentException ("syntax error in [if]. [if] needs at the very least a [lambda] child to execute if statement yields true");

            var condition = new Conditions (e.Args);
            if (condition.Evaluate ()) {
                foreach (Node idxExe in condition.ExecutionLambdas) {
                    context.Raise (idxExe.Name, idxExe);
                }
                Node next = e.Args.NextSibling;
                if (next != null && (next.Name == "else-if" || next.Name == "else")) {
                    e.Args.Parent.Insert (0, new Node ("__pf_evaluated"));
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
            if (e.Args.Count == 0)
                throw new ArgumentException ("syntax error in [else-if]. [else-if] needs at the very least a [lambda] child to execute if statement yields true");

            Node previous = e.Args.PreviousSibling;
            if (previous == null || (previous.Name != "if" && previous.Name != "else-if"))
                throw new ArgumentException ("you cannot have a [else-if] statement without a matching [if] or [else-if] as its previous sibling");

            // checking to see if previous "if" or "else-if" has already evaluated to true
            if (e.Args.Parent [0].Name == "__pf_evaluated") {
                Node next = e.Args.NextSibling;
                if (next == null || (next.Name != "else-if" && next.Name != "else")) {
                    e.Args.Parent.RemoveAt (0);
                }
            } else {
                var condition = new Conditions (e.Args);
                if (condition.Evaluate ()) {
                    foreach (Node idxExe in condition.ExecutionLambdas) {
                        context.Raise (idxExe.Name, idxExe);
                    }
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
            if (e.Args.Count == 0)
                throw new ArgumentException ("syntax error in [else]. [else] needs at the very least a [lambda] child to execute if statement yields true");

            Node previous = e.Args.PreviousSibling;
            if (previous == null || (previous.Name != "pf.if" && previous.Name != "if" && previous.Name != "pf.else-if" && previous.Name != "else-if"))
                throw new ArgumentException ("you cannot have a [else] without a matching [if] or [else-if] as its previous sibling");

            // checking to see if previous "if" or "else-if" has already evaluated to true
            if (e.Args.Parent [0].Name == "__pf_evaluated") {
                Node next = e.Args.NextSibling;
                if (next == null || (next.Name != "else-if" && next.Name != "else")) {
                    e.Args.Parent.RemoveAt (0);
                }
            } else {
                var condition = new Conditions (e.Args); // easy way to access all lambda objects beneath "else"
                foreach (Node idxExe in condition.ExecutionLambdas) {
                    context.Raise (idxExe.Name, idxExe);
                }
            }
        }
    }
}

