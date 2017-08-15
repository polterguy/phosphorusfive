/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda
{
    /// <summary>
    ///     Class wrapping all [eval] type of Active Events.
    /// </summary>
    public static class Eval
    {
        /// <summary>
        ///     Evaluates a lambda.
        ///     Creates a copy of the specified lambda block, passsing in any arguments, and returning whatever it returns, if anything.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "eval")]
        public static void eval (ApplicationContext context, ActiveEventArgs e)
        {
            EvalCopy (context, e.Args);
        }

        /// <summary>
        ///     Executes a specified lambda block with the specified whitelist.
        ///     Besides from creating a whitelist, it evaluates the exact same way as [eval].
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "eval-whitelist")]
        public static void eval_whitelist (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure no existing whitelist has been applied earlier.
            if (context.Ticket.Whitelist != null)
                throw new LambdaSecurityException ("Whitelist was previously applied.", e.Args, context);

            // Setting whitelist for context, making sure we deny everything by default, unless an explicit [events] node is specified.
            context.Ticket.Whitelist = e.Args ["events"]?.UnTie () ?? new Node ();

            // Making sure that whitelist is reset after evaluating its lambda.
            try {

                // Actual evaluation, which equals [eval] implementation.
                EvalCopy (context, e.Args);

            } finally {

                // Resetting whitelist.
                context.Ticket.Whitelist = null;
            }
        }

        /// <summary>
        ///     Executes a specified lambda block mutably, meaning it has access to entire tree.
        /// 
        ///     Useful when creating keywords and such, but should very rarely be directly used from Hyperlambda.
        ///     Notice, you cannot pass in or return arguments from this Active Event.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "eval-mutable")]
        public static void eval_mutable (ApplicationContext context, ActiveEventArgs e)
        {
            // Checking if we should foce execution of children nodes, and not evaluate expression/object in main node.
            if (e.Args.Value == null || e.Args.Name != e.Name) {

                // Evaluating current scope.
                ExecuteAll (e.Args, context);

            } else {

                // Evaluating a value object or an expression.
                foreach (var idxLambda in XUtil.Iterate<Node> (context, e.Args)) {

                    // Evaluating currently iterated lambda.
                    ExecuteAll (idxLambda, context);
                }
            }
        }

        /*
         * Helper for above two mutably evaluation events.
         */
        static void EvalCopy (ApplicationContext context, Node args)
        {
            // Checking if we should force evaluation of children nodes, and not evaluate expression/object in main node.
            if (args.Value == null || !args.Name.StartsWithEx ("eval")) {

                // Evaluating current scope.
                var clone = args.Clone ();
                args.Clear ().AddRange (ExecuteBlockCopy (context, clone)).Value = clone.Value;

            } else {

                // Evaluating a value object or an expression, making sure we are able to return everything resulting from evaluation of all lambdas.
                var retVal = new Node ();
                foreach (var idxLambda in XUtil.Iterate<Node> (context, args)) {

                    // Evaluating currently iterated lambda.
                    var clone = idxLambda.Clone ();

                    // Passing in arguments, in order of appearance, if there are any arguments, making sure we also insert our offset.
                    clone.Insert (0, new Node ("offset", args.Count));
                    clone.InsertRange (1, args.Clone ().Children);

                    // Evaluating cloned lambda, and making sure we save any return value for after iteration.
                    retVal.AddRange (ExecuteBlockCopy (context, clone)).Value = clone.Value ?? retVal.Value;
                }

                // Returning results from all above evaluations.
                args.Clear ().AddRange (retVal.Children).Value = retVal.Value;
            }
        }

        /*
         * Executes a block of nodes by copying the nodes executed, and executing the copy, 
         * returning anything returned from the evaluation of lambda back to caller.
         */
        static IEnumerable<Node> ExecuteBlockCopy (ApplicationContext context, Node clone)
        {
            // Storing the original nodes before execution, such that we can "diff" against nodes after execution,
            // to make it possible to return ONLY added nodes after execution
            var originalNodes = clone.Children.ToList ();

            // Actual execution of nodes
            ExecuteAll (clone, context);

            // Checking if we returned prematurely due to [return] invocation
            if (clone.FirstChild?.Name == "_return")
                clone.FirstChild.UnTie ();

            // Returning all nodes created inside of execution block, and ONLY these nodes, plus value of lambda block.
            // Notice, this means clearing the evalNode's children collection.
            return clone.Children.Where (ix => !originalNodes.Contains (ix));
        }
        
        /*
         * Executes one execution statement
         */
        static void ExecuteAll (Node lambdaObj, ApplicationContext context)
        {
            // Retrieving first node to be evaluated, if any.
            Node current = GetFirstExecutionNode (context, lambdaObj);

            // Looping as long as we've got more nodes in scope.
            while (current != null) {

                // We don't execute nodes that start with an underscore "_" since these are considered "data segments", in addition
                // to nodes starting with ".", since these are considered lambda callbacks.
                // In addition, we don't execute nodes with no names ("", empty names), since these interfers with "null Active Event handlers".
                // Besides, they're also exlusively used as formatting nodes anyways.
                if (!current.Name.StartsWithEx ("_") && !current.Name.StartsWithEx (".") && current.Name != "") {

                    // Raising the given Active Event.
                    context.RaiseEvent (current.Name, current);

                    // Checking if we're supposed to return from evaluation.
                    switch (lambdaObj.Root.FirstChild?.Name) {
                        case "_return":
                        case "_break":
                        case "_continue":
                            return;
                    }
                }

                // Finding our next execution node.
                // Notice, by postponing this until after execution of "current", we support the execution of "current" injecting a new node after itself, 
                // and have that newly injected node executed as well, immediately after the execution of "current".
                current = current.NextSibling;
            }
        }

        /*
         * Retrieves the first lambda node that should be evaluates, calculating any [offset} arguments, if there are any.
         */
        static Node GetFirstExecutionNode (ApplicationContext context, Node lambdaObj)
        {
            Node retVal = null;

            // Checking if we have an [offset]
            if (lambdaObj ["offset"] != null) {

                // Retrieving offset
                int offset = lambdaObj ["offset"].UnTie ().Get<int> (context);

                // Checking if execution block is "empty"
                if (offset == lambdaObj.Count)
                    return null;

                // Checking offset is not larger than number of children in current lambda
                if (offset > lambdaObj.Count)
                    throw new LambdaException ("[offset] was too large for lambda block, couldn't find that many children", lambdaObj, context);

                // Setting first execution statement as the offset node
                retVal = lambdaObj [offset];
            } else {

                // No [offset] given, executing everything
                retVal = lambdaObj.FirstChild;
            }
            return retVal;
        }
    }
}
