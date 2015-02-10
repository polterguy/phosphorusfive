
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.lambda
{
    /// <summary>
    /// class to help execute nodes
    /// </summary>
    public static class pfLambda
    {
        /// <summary>
        /// executes given block, or result of expression, as a block of statements to be executed
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "lambda")]
        private static void lambda (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Name == "lambda" && e.Args.Value != null) {

                // executing a value object, converting to node, before we pass into execution method,
                // making sure we pass in children of [lambda] as "arguments" or "parameters" to [lambda] statement
                foreach (var idxSource in XUtil.Iterate<Node> (e.Args, context)) {
                    ExecuteBlockNormal (context, idxSource, e.Args.Children);
                }
            } else {

                // executing current scope
                ExecuteBlockNormal (context, e.Args, new Node [] { });
            }
        }

        /// <summary>
        /// executes given block, or result of expression, as a block of statements to be executed, setting
        /// the the execution block node's back to what they were originally afterwards
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "lambda.immutable")]
        private static void lambda_immutable (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Name == "lambda.immutable" && e.Args.Value != null) {

                // executing a value object, converting to node, before we pass into execution method,
                // making sure we pass in children of [lambda] as "arguments" or "parameters" to [lambda] statement
                foreach (var idxSource in XUtil.Iterate<Node> (e.Args, context)) {
                    ExecuteBlockImmutable (context, idxSource, e.Args.Children);
                }
            } else {

                // executing current scope
                ExecuteBlockImmutable (context, e.Args, new Node [] { });
            }
        }
        
        /// <summary>
        /// executes given block, or result of expression, as a block of statements to be executed, such
        /// that the block executed is a deep copy of the execution block given to execute, such that
        /// execution does not in any ways have access to nodes from the tree outside of itself, unless nodes are
        /// passed in by reference
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "lambda.copy")]
        private static void lambda_copy (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Name == "lambda.copy" && e.Args.Value != null) {

                // executing a value object, converting to node, before we pass into execution method,
                // making sure we pass in children of [lambda] as "arguments" or "parameters" to [lambda] statement
                foreach (var idxSource in XUtil.Iterate<Node> (e.Args, context)) {
                    ExecuteBlockCopy (context, idxSource, e.Args.Children);
                }
            } else {

                // executing current scope
                ExecuteBlockCopy (context, e.Args, new Node [] { });
            }
        }
        
        /// <summary>
        /// executes a single pf.lambda statement
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "lambda.single")]
        private static void lambda_single (ApplicationContext context, ActiveEventArgs e)
        {
            if (string.IsNullOrEmpty (e.Args.Get<string> (context)))
                throw new ArgumentException ("nothing was given to [lambda.single] for execution");

            // executing a value object, converting to node, before we pass into execution method,
            // making sure we pass in children of [lambda] as "arguments" or "parameters" to [lambda] statement
            foreach (var idxSource in XUtil.Iterate<Node> (e.Args, context)) {
                ExecuteStatement (idxSource, context, true);
            }
        }

        /*
         * executes a block of nodes, this is where the actual execution happens
         * this is the "heart beat" method of the "pf.lambda" execution engine
         */
        private static void ExecuteBlockNormal (ApplicationContext context, Node exe, IEnumerable<Node> args)
        {
            // passing in arguments, if there are any
            foreach (Node idx in args) {
                exe.Add (idx.Clone ());
            }

            // iterating through all nodes in execution scope, and raising these as Active Events
            Node idxExe = exe.FirstChild;
            while (idxExe != null) {

                // executing current statement and retrieving next execution statement
                idxExe = ExecuteStatement (idxExe, context);
            }
        }
        
        /*
         * executes a block of nodes, this is where the actual execution happens
         * this is the "heart beat" method of the "pf.lambda" execution engine
         */
        private static void ExecuteBlockImmutable (ApplicationContext context, Node exe, IEnumerable<Node> args)
        {
            // storing original execution nodes, such that we can set back execution
            // block to what it originally was
            List<Node> oldNodes = new List<Node> ();
            foreach (Node idx in exe.Children) {
                oldNodes.Add (idx.Clone ());
            }

            // passing in arguments, if there are any
            foreach (Node idx in args) {
                exe.Add (idx.Clone ());
            }

            // iterating through all nodes in execution scope, and raising these as Active Events
            Node idxExe = exe.FirstChild;
            while (idxExe != null) {

                // executing current statement and retrieving next execution statement
                idxExe = ExecuteStatement (idxExe, context);
            }

            // making sure we set back execution block to original nodes
            exe.Clear ();
            exe.AddRange (oldNodes);
        }
        
        /*
         * executes a block of nodes, this is where the actual execution happens
         * this is the "heart beat" method of the "pf.lambda" execution engine
         */
        private static void ExecuteBlockCopy (ApplicationContext context, Node exe, IEnumerable<Node> args)
        {
            // making sure lambda is executed on copy of execution nodes, if we should,
            // without access to nodes outside of its own scope
            exe = exe.Clone ();

            // passing in arguments, if there are any
            foreach (Node idx in args) {
                exe.Add (idx.Clone ());
            }

            // iterating through all nodes in execution scope, and raising these as Active Events
            Node idxExe = exe.FirstChild;
            while (idxExe != null) {

                // executing current statement and retrieving next execution statement
                idxExe = ExecuteStatement (idxExe, context);
            }
        }

        /*
         * executes one execution statement
         */
        private static Node ExecuteStatement (Node exe, ApplicationContext context, bool force = false)
        {
            // storing "next execution node" as fallback, to support "delete this node" pattern
            Node nextFallback = exe.NextSibling;

            // we don't execute nodes that start with an underscore "_" since these are considered "data segments"
            // also we don't execute nodes with no name, since these interfers with "null Active Event handlers"
            if (force || (!exe.Name.StartsWith ("_") && exe.Name != string.Empty)) {
                context.Raise (exe.Name, exe);
            }

            // prioritizing "NextSibling", in case this node created new nodes, while having
            // nextFallback as "fallback node", in case current execution node removed current execution node,
            // but if "current execution node" untied nextFallback, in addition to "NextSibling",
            // we return null back to caller
            return exe.NextSibling ?? (nextFallback != null && nextFallback.Parent != null && 
                                       nextFallback.Parent == exe.Parent ? nextFallback : null);
        }
    }
}
