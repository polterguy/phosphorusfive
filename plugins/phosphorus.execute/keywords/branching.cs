/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute
{
    /// <summary>
    /// class wrapping execution engine keyword "pf.if", "pf.else-if" and "pf.else", which allows for branching or
    /// conditional execution of nodes
    /// </summary>
    public static class branching
    {
        /// <summary>
        /// "pf.if" statement, allowing for evaluating condition, and executing lambda(s) if statement evaluates to true
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.if")]
        private static void pf_if (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Count == 0)
                throw new ArgumentException ("syntax error in [pf.if], no children makes for invalid statement");

            var condition = new Condition (e.Args);
            if (condition.Evaluate ()) {
                foreach (Node idxExe in condition.ExecutionLambdas) {
                    context.Raise (idxExe.Name, idxExe);
                }
                Node next = e.Args.NextSibling;
                if (next != null && (next.Name == "pf.else-if" || next.Name == "else-if" || next.Name == "pf.else" || next.Name == "else")) {
                    e.Args.Parent.Insert (0, new Node ("__pf_evaluated"));
                }
            }
        }

        /// <summary>
        /// "pf.else-if" statement, allowing for evaluating condition, and executing lambda(s) if statement evaluates to true, and
        /// no previous "if" or "else-if" has evaluated to true
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.else-if")]
        private static void pf_else_if (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Count == 0)
                throw new ArgumentException ("syntax error in [pf.else-if], no children makes for invalid statement");

            Node previous = e.Args.PreviousSibling;
            if (previous == null || (previous.Name != "pf.if" && previous.Name != "if" && previous.Name != "pf.else-if" && previous.Name != "else-if"))
                throw new ArgumentException ("you cannot have a [pf.else-if] without a matching [pf.if] or [pf.else-if] before it");

            // checking to see if previous "if" or "else-if" has already evaluated to true
            if (e.Args.Parent [0].Name == "__pf_evaluated") {
                Node next = e.Args.NextSibling;
                if (next == null || (next.Name != "pf.else-if" && next.Name != "else-if" && next.Name != "pf.else" && next.Name != "else")) {
                    e.Args.Parent.RemoveAt (0);
                }
            } else {
                var condition = new Condition (e.Args);
                if (condition.Evaluate ()) {
                    foreach (Node idxExe in condition.ExecutionLambdas) {
                        context.Raise (idxExe.Name, idxExe);
                    }
                    Node next = e.Args.NextSibling;
                    if (next != null && (next.Name == "pf.else-if" || next.Name == "else-if" || next.Name == "pf.else" || next.Name == "else")) {
                        e.Args.Parent.Insert (0, new Node ("__pf_evaluated", true));
                    }
                }
            }
        }
        
        /// <summary>
        /// "pf.else" statement, allowing for executing lambda(s) if no previous "if" or "else-if" has evaluated to true
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.else")]
        private static void pf_else (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Count == 0)
                throw new ArgumentException ("syntax error in [pf.else-if], no children makes for invalid statement");

            Node previous = e.Args.PreviousSibling;
            if (previous == null || (previous.Name != "pf.if" && previous.Name != "if" && previous.Name != "pf.else-if" && previous.Name != "else-if"))
                throw new ArgumentException ("you cannot have a [pf.else] without a matching [pf.if] or [pf.else-if] before it");

            // checking to see if previous "if" or "else-if" has already evaluated to true
            if (e.Args.Parent [0].Name == "__pf_evaluated") {
                Node next = e.Args.NextSibling;
                if (next == null || (next.Name != "pf.else-if" && next.Name != "else-if" && next.Name != "pf.else" && next.Name != "else")) {
                    e.Args.Parent.RemoveAt (0);
                }
            } else {
                var condition = new Condition (e.Args); // easy way to access all lambda objects beneath "else"
                foreach (Node idxExe in condition.ExecutionLambdas) {
                    context.Raise (idxExe.Name, idxExe);
                }
            }
        }
    }
}

