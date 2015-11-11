/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

/// <summary>
///     Main namespace for p5.lambda keywords.
/// </summary>
namespace p5.lambda
{
    /// <summary>
    ///     Class wrapping all [lambda.xxx] keywords in p5.lambda.
    /// 
    ///     The [lambda.xxx] keywords allows you to execute some specific piece of p5.lambda code.
    /// </summary>
    public static class Lambda
    {
        private delegate void ExecuteFunctor (ApplicationContext context, Node exe, IEnumerable<Node> args);

        private static void Executor (ExecuteFunctor functor, ApplicationContext context, Node args, bool forceChildren)
        {
            if (forceChildren || args.Value != null) {
                
                // executing current scope
                functor (context, args, new Node[] {});
            } else {
                
                // executing a value object or an expression
                foreach (var idxSource in XUtil.Iterate<Node> (args, context)) {
                    functor (context, idxSource, args.Children);
                }
            }
        }

        /// <summary>
        ///     Executes a specified piece of p5.lambda block.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "eval")]
        private static void eval (ApplicationContext context, ActiveEventArgs e)
        {
            Executor (ExecuteBlockCopy, context, e.Args, false);
        }
        
        /// <summary>
        ///     Executes a specified piece of p5.lambda block.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "eval-mutable")]
        private static void eval_mutable (ApplicationContext context, ActiveEventArgs e)
        {
            Executor (ExecuteBlockMutable, context, e.Args, true);
        }

        /*
         * executes a block of nodes by copying the nodes executed, and executing the copy, 
         * returning anything created inside of the block back to caller
         */
        private static void ExecuteBlockCopy (ApplicationContext context, Node exe, IEnumerable<Node> args)
        {
            // making sure lambda is executed on copy of execution nodes,
            // without access to nodes outside of its own scope
            Node exeCopy = exe.Clone ();

            // passing in arguments, if there are any
            foreach (var idx in args) {
                exeCopy.Add (idx.Clone ());
            }

            // storing the original nodes before execution, such that we can "diff" against nodes after execution,
            // to make it possible to return ONLY added nodes after execution
            List<Node> originalNodes = new List<Node> (exeCopy.Children);

            // actual execution of nodes
            ExecuteAll (exeCopy, context);

            // returning all nodes created inside of execution block, and ONLY these nodes, plus value of lambda block
            exe.Clear ().AddRange (exeCopy.Children.Where (ix => originalNodes.IndexOf (ix) == -1));
            exe.Value = exeCopy.Value;
        }
        
        /*
         * executes a block of nodes in mutable state
         */
        private static void ExecuteBlockMutable (ApplicationContext context, Node exe, IEnumerable<Node> args)
        {
            // passing in arguments, if there are any
            foreach (var idx in args) {
                exe.Add (idx.Clone ());
            }

            // actual execution of block
            ExecuteAll (exe, context);
        }

        /*
         * executes one execution statement
         */
        private static void ExecuteAll (Node exe, ApplicationContext context)
        {
            // iterating through all nodes in execution scope, and raising these as Active Events
            var idxExe = exe.FirstChild;

            // looping as long as we've got more nodes in scope
            while (idxExe != null) {

                // storing "next execution node" as fallback, to support "delete this node" logic
                var nextFallback = exe.NextSibling;

                // we don't execute nodes that start with an underscore "_" since these are considered "data segments"
                // also we don't execute nodes with no name, since these interfers with "null Active Event handlers"
                if (!exe.Name.StartsWith ("_") && exe.Name != string.Empty) {

                    // raising the given Active Event normally, taking inheritance chain into account
                    context.Raise (exe.Name, exe);
                }

                // prioritizing "NextSibling", in case this node created new nodes, while having
                // nextFallback as "fallback node", in case current execution node removed current execution node,
                // but in case nextFallback alaso was removed, we set idxExe to null, breaking the while loop
                idxExe = exe.NextSibling ?? (nextFallback.Parent != null ? nextFallback : null);
            }
        }
    }
}
