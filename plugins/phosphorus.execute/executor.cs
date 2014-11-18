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
            if (e.Args.Count == 0 || !Expression.IsExpression (e.Args.FirstChild.Get<string> ())) {

                // executing current scope
                Node current = e.Args.FirstChild;
                while (current != null) {
                    if (!current.Name.StartsWith ("_")) {
                        ip.Value = current.Path;
                        context.Raise (current.Name, current);
                    }
                    current = current.NextSibling;
                }
            } else {

                // executing expression
                Match executionMatch = new Expression (e.Args.FirstChild.Get<string> ()).Evaluate (e.Args.FirstChild);
                foreach (Node current in executionMatch.Matches) {
                    ip.Value = current.Path;
                    context.Raise (current.Name, current);
                }
            }
            ip.Untie ();
        }
    }
}

