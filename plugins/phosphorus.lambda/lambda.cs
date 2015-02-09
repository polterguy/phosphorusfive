
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
        /*
         * type of lambda statement
         */
        private enum LambdaType
        {
            // execution of lambda has access to the entire tree
            Normal,

            // execution is done on a copy of the nodes executed, and not the original nodes, meaning the execution will
            // not have access to nodes outside of the [lambda] statement itself, unless passed in as reference nodes
            Copy,

            // execution is done immutable, such that the executed nodes will be set back to their original state after the
            // execution. execution still has access to the entire tree, but no changes done inside the [lambda] itself, will
            // exist after the execution of the lambda is done
            Immutable
        }

        /// <summary>
        /// main execution Active Event entry point for executing nodes as execution tree. [lambda] simply executes the given
        /// block or expression. [copy.lambda] creates a copy of the block or expression before executing the copy, which means
        /// it does not have access to any nodes outside of its own scope. [immutable.lambda] will allow the block or expression
        /// execute to have access to the entire tree, but will create a copy of the nodes executed, and after execution it will
        /// set the entire execution block or expression back to its original state
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "lambda")]
        [ActiveEvent (Name = "lambda.copy")]
        [ActiveEvent (Name = "lambda.immutable")]
        private static void lambda (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Name.StartsWith ("lambda") && e.Args.Value != null) {

                // executing a value object, converting to node before we pass into execution method
                foreach (var idxSource in XUtil.Iterate<Node> (e.Args, context)) {
                    ExecuteBlock (context, idxSource, e.Args.Children, GetLambdaType (e));
                }
            } else {

                // executing current scope
                ExecuteBlock (context, e.Args, new Node [] {}, GetLambdaType (e));
            }
        }

        /*
         * executes a block of nodes, this is where the actual execution happens
         * this is the "heart beat" method of the "pf.lambda" execution engine
         */
        private static void ExecuteBlock (ApplicationContext context, Node exe, IEnumerable<Node> args, LambdaType type)
        {
            // making sure lambda is executed on copy of execution nodes, if we should, without access to nodes outside of its own scope
            // (besides from parameters passed into it by reference though of course)
            exe = type == LambdaType.Copy ? exe.Clone () : exe;

            // storing "old nodes" to allow [lambda] to execute immutably, but only if type == Immutable
            List<Node> oldNodes = new List<Node> ();
            if (type == LambdaType.Immutable) {
                foreach (Node idx in exe.Children) {
                    oldNodes.Add (idx.Clone ());
                }
            }

            // passing in arguments, if there are any
            foreach (Node idx in args) {
                exe.Add (idx.Clone ());
            }

            // iterating through all nodes in execution scope, and raising these as Active Events
            Node idxExe = exe.FirstChild;
            while (idxExe != null) {

                // we don't execute nodes that start with an underscore "_" since these are considered "data segments"
                if (!idxExe.Name.StartsWith ("_") && idxExe.Name != string.Empty) {
                    context.Raise (idxExe.Name, idxExe);
                }
                idxExe = idxExe.NextSibling;
            }

            // making sure we reset original nodes, if execution type was "Immutable"
            if (type == LambdaType.Immutable) {
                exe.Clear ();
                exe.AddRange (oldNodes);
            }
        }

        /*
         * returns the type of [lambda] statement, Normal, Copy or Immutable
         */
        private static LambdaType GetLambdaType (ActiveEventArgs e)
        {
            switch (e.Name) {
            case "lambda":
                return LambdaType.Normal;
            case "lambda.copy":
                return LambdaType.Copy;
            case "lambda.immutable":
                return LambdaType.Immutable;
            }
            throw new ArgumentException ("unknown type of lambda execution; '" + e.Name + "'");
        }
    }
}
