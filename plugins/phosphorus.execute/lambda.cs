/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute
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
                            ExecuteBlock (context, idxRes as Node, args.Children);
                        } else {
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
                return;

            Node exe = new Node ("root", code);
            context.Raise ("pf.code-2-nodes", exe);
            ExecuteBlock (context, exe, args);
        }

        /*
         * executes a block of nodes
         */
        private static void ExecuteBlock (ApplicationContext context, Node exe, IEnumerable<Node> args = null)
        {
            // storing "old nodes" to make sure execution block stays "immutable"
            List<Node> oldNodes = new List<Node> ();
            foreach (Node idx in exe.Children) {
                oldNodes.Add (idx.Clone ());
            }
            if (args != null) {
                foreach (Node idx in args) {
                    exe.Add (new Node (idx.Name, idx));
                }
            }
            Node current = exe.FirstChild;
            while (current != null) {
                if (!current.Name.StartsWith ("_")) {
                    string avName = current.Name;
                    if (!avName.Contains ("."))
                        avName = "pf." + avName;
                    context.Raise (avName, current);
                }
                current = current.NextSibling;
            }
            exe.Clear ();
            exe.AddRange (oldNodes);
        }
    }
}

