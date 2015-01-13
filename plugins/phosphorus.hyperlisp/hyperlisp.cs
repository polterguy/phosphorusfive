/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.lambda;

namespace phosphorus.hyperlisp
{
    /// <summary>
    /// class to help transform between hyperlisp and <see cref="phosphorus.core.Node"/> 
    /// </summary>
    public static class hyperlisp
    {
        /// <summary>
        /// helper to transform from hyperlisp code syntax to <see cref="phosphorus.core.Node"/> tree structure.
        /// will transform the given hyperlisp into a list of nodes and append them into the root node given
        /// through the active event args
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.hyperlisp-2-nodes")]
        private static void pf_hyperlisp_2_nodes (ApplicationContext context, ActiveEventArgs e)
        {
            string hyperlisp = e.Args.Get<string> ();
            if (Expression.IsExpression (hyperlisp)) {

                // retrieving hyperlisp as a function of the expression
                var match = Expression.Create (hyperlisp).Evaluate (e.Args);
                if (match.TypeOfMatch != Match.MatchType.Value)
                    throw new ArgumentException ("[pf.hyperlisp-2-nodes] can only take an expression of 'value' type");

                hyperlisp = string.Empty;
                foreach (Node idx in match.Matches) {
                    hyperlisp += "\r\n" + idx.Get<string> ();
                }
            }
            e.Args.AddRange (new NodeBuilder (context, hyperlisp).Nodes);
        }

        /// <summary>
        /// helper to transform from <see cref="phosphorus.core.Node"/> tree structure to hyperlisp code syntax.
        /// will transform the children nodes of the given active event args into hyperlisp and return as string
        /// value of the value of the given active event args
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.nodes-2-hyperlisp")]
        private static void pf_nodes_2_hyperlisp (ApplicationContext context, ActiveEventArgs e)
        {
            if (Expression.IsExpression (e.Args.Value)) {
                var match = Expression.Create (e.Args.Get<string> ()).Evaluate (e.Args);
                if (match.TypeOfMatch != Match.MatchType.Node)
                    throw new ArgumentException ("[pf.nodes-2-hyperlisp] can only take an expression of 'node' type");
                e.Args.Value = new HyperlispBuilder (context, match.Matches).Hyperlisp;
            } else {
                e.Args.Value = new HyperlispBuilder (context, e.Args.Children).Hyperlisp;
            }
        }
        
        /// <summary>
        /// helper to transform from <see cref="phosphorus.core.Node"/> tree structure to hyperlisp code syntax.
        /// will transform the given root node of the active event args into hyperlisp and return as string value
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.node-2-hyperlisp")]
        private static void pf_node_2_hyperlisp (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = new HyperlispBuilder (context, new Node[] { e.Args }).Hyperlisp;
        }
    }
}

