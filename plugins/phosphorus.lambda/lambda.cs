/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.lambda
{
    /// <summary>
    /// class to help execute nodes
    /// </summary>
    public static class lambda
    {
        /// <summary>
        /// main execution Active Event entry point for executing nodes as execution tree
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.lambda")]
        [ActiveEvent (Name = "lambda")]
        private static void pf_lambda (ApplicationContext context, ActiveEventArgs e)
        {
            if ((e.Args.Name == "pf.lambda" || e.Args.Name == "lambda") && !string.IsNullOrEmpty (e.Args.Get<string> ())) {

                // executing expression
                ExecuteLambdaExpression (context, e.Args);
            } else {
                
                // executing current scope
                ExecuteBlock (context, e.Args);
            }
        }
        
        /*
         * executes a "lambda execution" block
         */
        private static void ExecuteLambdaExpression (ApplicationContext context, Node args)
        {
            string codeOrExpression = args.Get<string> ();
            if (Expression.IsExpression (codeOrExpression)) {

                // value of execution node is an expression, figuring out if our expression returns "node" 
                // or "something else" before we invoke further execution logic
                Match executionMatch = new Expression (args.Get<string> ()).Evaluate (args);
                if (executionMatch.TypeOfMatch == Match.MatchType.Node) {

                    // expression returned "node"(s)
                    foreach (Node current in executionMatch.Matches) {
                        ExecuteBlock (context, current, args.Children);
                    }
                } else {

                    // expression returned anything but "node", executing result as text, unless "value" is a reference node
                    for (int idxNo = 0; idxNo < executionMatch.Count; idxNo++) {
                        var idxRes = executionMatch.GetValue (idxNo);
                        if (idxRes is Node) {

                            // current match was a reference node, executing node
                            // notice since current executing node is a "root node", we'll need to create a new node being
                            // the actual node executed, containing the previous "root node" as its child
                            Node exeRootNode = idxRes as Node;
                            Node tmpExe = new Node ();
                            tmpExe.Add (exeRootNode);
                            ExecuteBlock (context, tmpExe, args.Children);
                            exeRootNode.Untie (); // cleaning up
                        } else {

                            // current match was a string or something that (hopefully) can be converted into a string
                            ExecuteLambdaText (context, (idxRes ?? "").ToString (), args.Children);
                        }
                    }
                }
            } else {

                // value of execution node is code in text format, making sure we escape it, in case it starts with "@"
                ExecuteLambdaText (context, codeOrExpression.TrimStart ('\\'), args.Children);
            }
        }

        /*
         * executes a piece of text
         */
        private static void ExecuteLambdaText (ApplicationContext context, string code, IEnumerable<Node> args)
        {
            if (string.IsNullOrEmpty (code))
                return; // nothing to execute here

            // first transforming code into nodes
            Node exe = new Node ("root", code);
            context.Raise ("pf.code-2-nodes", exe);

            // then executing nodes created from "code" parameter
            ExecuteBlock (context, exe, args);
        }

        /*
         * executes a block of nodes, this is where the actual execution happens
         * this is the "heart beat" method of the "pf.lambda" execution engine
         */
        private static void ExecuteBlock (ApplicationContext context, Node exe, IEnumerable<Node> args = null)
        {
            // making sure lambda is executed immutable, without access to parameters from outside of itself
            // (besides from parameters passed into it by reference though of course)
            exe = exe.Clone ();

            // passing in arguments
            if (args != null) {
                foreach (Node idx in args) {
                    exe.Add (idx.Clone ());
                }
            }

            // iterating through all nodes in execution scope
            Node idxExe = exe.FirstChild;
            while (idxExe != null) {

                // we don't execute nodes that start with an underscore "_" since these are considered "data segments"
                if (!idxExe.Name.StartsWith ("_")) {
                    string avName = idxExe.Name;

                    // making sure our active event is prefixed with a "pf." if it doesn't contain a period "." in its name anywhere
                    if (!avName.Contains ("."))
                        avName = "pf." + avName;
                    context.Raise (avName, idxExe);
                }
                idxExe = idxExe.NextSibling;
            }
        }
    }
}
