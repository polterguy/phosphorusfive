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
    public static class executor
    {
        /// <summary>
        /// main execution Active Event entry point for executing nodes as execution tree
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.execute")]
        private static void pf_execute (ApplicationContext context, ActiveEventArgs e)
        {
            Node ip = new Node ("__pf_ip", null);
            e.Args.Root.Add (ip);
            if (e.Args.Name == "pf.execute" && Expression.IsExpression (e.Args.Get<string> ())) {
                
                // executing expression
                ExecuteLambda (context, e.Args, ip);
            } else {
                
                // executing current scope
                ExecuteBlock (context, e.Args, ip);
            }
            ip.Untie ();
        }

        /*
         * executes a block of nodes
         */
        private static void ExecuteBlock (ApplicationContext context, Node exe, Node ip)
        {
            Node current = exe.FirstChild;
            while (current != null) {
                if (!current.Name.StartsWith ("_")) {
                    ip.Value = current;
                    context.Raise (current.Name, current);
                }
                current = current.NextSibling;
            }
        }

        /*
         * executes a "lambda execution" block
         */
        private static void ExecuteLambda (ApplicationContext context, Node exe, Node ip)
        {
            Match executionMatch = new Expression (exe.Get<string> ()).Evaluate (exe);
            if (executionMatch.TypeOfMatch != Match.MatchType.Node)
                throw new ArgumentException ("you can only execute a 'node' expression, [pf.execute] was given illegal expression; '" + exe.Get<string> () + "'");
            foreach (Node current in executionMatch.Matches) {
                List<Node> oldCurrentChildren = new List<Node> ();
                if (exe.Count > 0) {

                    // arguments are being passed into "lambda" execution block, making sure we store old children list such that execution 
                    // becomes "immutable"
                    foreach (Node idx in current.Children) {
                        oldCurrentChildren.Add (idx.Clone ());
                    }
                    foreach (Node idx in exe.Children) {
                        current.Add (idx.Clone ());
                    }
                }
                ip.Value = current;
                context.Raise (current.Name, current); // we're also raising active events starting with "_" here
                if (exe.Count > 0) {
                    current.Clear ();
                    foreach (Node idx in oldCurrentChildren) {
                        current.Add (idx);
                    }
                }
            }
        }
    }
}

