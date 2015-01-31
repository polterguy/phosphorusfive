
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
    /// class wrapping execution engine keyword "add", which allows for appending nodes into a node's list of children
    /// </summary>
    public static class add
    {
        /// <summary>
        /// [add] keyword for execution engine. allows adding nodes from one part of your tree to another part
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "add")]
        private static void lambda_add (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Count == 0)
                throw new ArgumentException ("[add] needs either a source expression, or a list of children nodes");

            if (Expression.IsExpression (e.Args.LastChild.Value) && e.Args.Count == 1) {

                // source is an expression, cloning source first, in case source is overlapping destination
                List<Node> sourceNodes = new List<Node> (Expression.Clone (e.Args.LastChild, true));

                // looping through each destination, adding every cloned source node, into every destination node
                Expression.Iterate<Node> (e.Args, false, 
                delegate (Node idxDestination) {
                    foreach (Node idxSource in sourceNodes) {
                        idxDestination.Add (idxSource.Clone());
                    }
                });
            } else {

                // adding a bunch of children nodes into source
                Expression.Iterate<Node> (e.Args, false, 
                delegate (Node idxDestination) {
                    foreach (Node idxSource in e.Args.Children) {
                        idxDestination.Add (idxSource.Clone());
                    }
                });
            }
        }
    }
}
