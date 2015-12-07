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
    ///     Class wrapping all [eval] keywords in p5.lambda.
    /// </summary>
    public static class Eval
    {
        // Used to extract commonalities for eval Active Events
        private delegate void ExecuteFunctor (
            ApplicationContext context, 
            Node exe, 
            Node evalNode, 
            IEnumerable<Node> args,
            bool isFirst);

                /// <summary>
        ///     Executes a specified piece of p5.lambda block as a copied lambda object
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "eval", Protection = EventProtection.LambdaClosed)]
        private static void eval (ApplicationContext context, ActiveEventArgs e)
        {
            Executor (ExecuteBlockCopy, context, e.Args, e.Args.Name != "eval");
        }
        
        /// <summary>
        ///     Executes a specified piece of p5.lambda block mutably
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "eval-mutable", Protection = EventProtection.LambdaClosed)]
        private static void eval_mutable (ApplicationContext context, ActiveEventArgs e)
        {
            Executor (ExecuteBlockMutable, context, e.Args, e.Args.Name != "eval-mutable");
        }

        /*
         * Worker method for [eval]
         */
        private static void Executor (ExecuteFunctor functor, ApplicationContext context, Node args, bool forceChildren)
        {
            if (forceChildren || args.Value == null) {

                // executing current scope
                functor (context, args, args, new Node[] {}, true);
            } else {

                // executing a value object or an expression, making sure we let functor know which
                // was the first invocation
                bool isFirst = true;
                foreach (var idxSource in XUtil.Iterate<Node> (context, args)) {

                    functor (context, idxSource, args, args.Children, isFirst);
                    isFirst = false;
                }
            }
        }

        /*
         * Executes a block of nodes by copying the nodes executed, and executing the copy, 
         * returning anything created inside of the block back to caller
         */
        private static void ExecuteBlockCopy (
            ApplicationContext context, 
            Node exe, 
            Node evalNode, 
            IEnumerable<Node> args,
            bool isFirst)
        {
            // Making sure lambda is executed on copy of execution nodes,
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
            // notice, this means clearing the evalNode's children collection ONLY the first time we execute it
            if (isFirst)
                evalNode.Clear ();
            evalNode.AddRange (exeCopy.Children.Where (ix => originalNodes.IndexOf (ix) == -1));
            evalNode.Value = exeCopy.Value;
        }
        
        /*
         * executes a block of nodes in mutable state
         */
        private static void ExecuteBlockMutable (
            ApplicationContext context, 
            Node exe, 
            Node evalNode, 
            IEnumerable<Node> args,
            bool isFirst)
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
            // Iterating through all nodes in execution scope, unless [offset] is given, and raising these as Active Events
            Node idxExe = null;

            // Checking if we have an [offset]
            if (exe ["offset"] != null) {

                // Retrieving offset
                int offset = exe ["offset"].Get<int> (context);

                // Checking offset is not larger than number of children in current lambda
                if (offset >= exe.Count)
                    throw new LambdaException ("[offset] was too large for lambda block, couldn't find that many children", exe, context);

                // Setting first execution statement as the offset node
                idxExe = exe [offset];
            } else {

                // No [offset] given, executing everything
                idxExe = exe.FirstChild;
            }

            // looping as long as we've got more nodes in scope
            while (idxExe != null) {

                // storing "next execution node" as fallback, to support "delete this node" logic
                var nextFallback = idxExe.NextSibling;

                // we don't execute nodes that start with an underscore "_" since these are considered "data segments"
                // also we don't execute nodes with no name, since these interfers with "null Active Event handlers"
                if (!idxExe.Name.StartsWith ("_") && idxExe.Name != "") {

                    // raising the given Active Event normally, taking inheritance chain into account
                    context.RaiseLambda (idxExe.Name, idxExe);
                }

                // prioritizing "NextSibling", in case this node created new nodes, while having
                // nextFallback as "fallback node", in case current execution node removed current execution node,
                // but in case nextFallback alaso was removed, we set idxExe to null, breaking the while loop
                idxExe = idxExe.NextSibling ?? (nextFallback != null && nextFallback.Parent != null ? nextFallback : null);
            }
        }
    }
}
