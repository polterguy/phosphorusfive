/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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
        // Common helper used as callback for evaluating a lambda block.
        private delegate void ExecuteFunctor (
            ApplicationContext context,

            // Lambda block to evaluate.
            Node lambda,

            // Node that is currently being evaluated, might contain the lambda block as children, or have an expression leading to lambda block(s) we should evaluate.
            Node evalNode,

            // Arguments to lambda block, if any.
            IEnumerable<Node> args);

        /// <summary>
        ///     Evaluates a lambda.
        ///     Actually creates a copy of the specified lambda block, passsing in any arguments, and returning whatever it returns, if anything.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "eval")]
        public static void eval (ApplicationContext context, ActiveEventArgs e)
        {
            Executor (ExecuteBlockCopy, context, e.Args);
        }
        
        /// <summary>
        ///     Executes a specified lambda block as mutable, meaning it has access to entire tree.
        ///     Useful when creating keywords and such, but should very rarely be directly used from Hyperlambda!
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "eval-mutable")]
        public static void eval_mutable (ApplicationContext context, ActiveEventArgs e)
        {
            Executor (ExecuteBlockMutable, context, e.Args);
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

            // Setting whitelist for context, making sure we by default deny everything, unless an explicit [events] node is specified.
            context.Ticket.Whitelist = e.Args["events"]?.UnTie () ?? new Node ();

            // Making sure that whitelist is reset after evaluating its lambda.
            try {

                // Executing scope.
                Executor (ExecuteBlockCopy, context, e.Args);

            } finally {

                // Resetting whitelist.
                context.Ticket.Whitelist = null;
            }
        }

        /*
         * Worker method for [eval] and [eval-mutable].
         */
        private static void Executor (
            ExecuteFunctor functor, 
            ApplicationContext context, 
            Node args)
        {
            // Checking if we should foce execution of children nodes, and not evaluate expressions in main node
            if (args.Value == null || !args.Name.StartsWith ("eval")) {

                // Evaluating current scope
                functor (context, args, args, null);

            } else {

                // Evaluating a value object or an expression.
                foreach (var idxLambda in XUtil.Iterate<Node> (context, args)) {

                    // Evaluating currently iterated source
                    functor (context, idxLambda, args, args.Children);
                }
            }
        }

        /*
         * Executes a block of nodes by copying the nodes executed, and executing the copy, 
         * returning anything created inside of the block back to caller
         */
        private static void ExecuteBlockCopy (
            ApplicationContext context, 
            Node lambda, 
            Node evalNode, 
            IEnumerable<Node> args)
        {
            // Making sure lambda is executed on copy of execution nodes, without access to nodes outside of its own scope.
            Node lambdaClone = lambda.Clone ();

            // Passing in arguments, in order of appearance, if there are any arguments.
            if (args != null) {
                var index = evalNode["offset"] != null ? evalNode.IndexOf (evalNode["offset"]) : 0;
                foreach (var idx in args.Reverse ()) {
                    lambdaClone.Insert (index, idx.Clone ());
                }
            }

            // Storing the original nodes before execution, such that we can "diff" against nodes after execution,
            // to make it possible to return ONLY added nodes after execution
            List<Node> originalNodes = new List<Node> (lambdaClone.Children);

            // Actual execution of nodes
            ExecuteAll (lambdaClone, context);

            // Checking if we returned prematurely due to [return] invocation
            if (lambdaClone.FirstChild != null && lambdaClone.FirstChild.Name == "_return")
                lambdaClone.FirstChild.UnTie ();

            // Returning all nodes created inside of execution block, and ONLY these nodes, plus value of lambda block.
            // Notice, this means clearing the evalNode's children collection.
            evalNode.Clear ();
            evalNode.AddRange (lambdaClone.Children.Where (ix => originalNodes.IndexOf (ix) == -1));
            evalNode.Value = lambdaClone.Value;
        }
        
        /*
         * Executes a block of nodes in mutable state
         */
        private static void ExecuteBlockMutable (
            ApplicationContext context, 
            Node lambda, 
            Node evalNode, 
            IEnumerable<Node> args)
        {
            // Passing in arguments, in order of appearance, if there are any arguments.
            if (args != null) {
                var index = evalNode["offset"] != null ? evalNode.IndexOf (evalNode["offset"]) : 0;
                foreach (var idx in args.Reverse ()) {
                    lambda.Insert (index, idx.Clone ());
                }
            }

            // Actual execution of block
            ExecuteAll (lambda, context);

            // Checking if we returned prematurely due to [return] invocation
            if (lambda.FirstChild != null && lambda.FirstChild.Name == "_return")
                lambda.FirstChild.UnTie ();
        }

        /*
         * Executes one execution statement
         */
        private static void ExecuteAll (Node lambda, ApplicationContext context)
        {
            // Retrieving first node to be evaluated, if any.
            Node current = GetFirstExecutionNode (context, lambda);

            // Looping as long as we've got more nodes in scope.
            while (current != null) {

                // We don't execute nodes that start with an underscore "_" since these are considered "data segments", in addition
                // to nodes starting with ".", since these are considered lambda callbacks.
                // In addition, we don't execute nodes with no names ("", empty names), since these interfers with "null Active Event handlers".
                // Besides, they're also exlusively used as formatting nodes anyways.
                if (!current.Name.StartsWith ("_") && !current.Name.StartsWith (".") && current.Name != "") {

                    // Raising the given Active Event.
                    context.RaiseEvent (current.Name, current);

                    // Checking if we're supposed to return from evaluation.
                    var rootChildName = lambda.Root.FirstChild?.Name;
                    switch (rootChildName) {
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
        private static Node GetFirstExecutionNode (ApplicationContext context, Node lambda)
        {
            Node retVal = null;

            // Checking if we have an [offset]
            if (lambda["offset"] != null) {

                // Retrieving offset
                int offset = lambda["offset"].UnTie ().Get<int> (context);

                // Checking if execution block is "empty"
                if (offset == lambda.Children.Count)
                    return null;

                // Checking offset is not larger than number of children in current lambda
                if (offset > lambda.Children.Count)
                    throw new LambdaException ("[offset] was too large for lambda block, couldn't find that many children", lambda, context);

                // Setting first execution statement as the offset node
                retVal = lambda[offset];
            } else {

                // No [offset] given, executing everything
                retVal = lambda.FirstChild;
            }
            return retVal;
        }
    }
}
