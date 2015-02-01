
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
                throw new ArgumentException ("[add] needs a [source]");

            // fetching source nodes
            var source = new List<Node> (GetSource (e.Args));

            // looping through every destination node, adding a copy of every source node, to its children collection
            Expression.Iterate<Node> (e.Args, true, 
            delegate (Node idxDestination) {
                foreach (Node idxSource in source) {
                    idxDestination.Add (idxSource.Clone());
                }
            });
        }

        /*
         * returns source back to caller
         */
        private static IEnumerable<Node> GetSource (Node node)
        {
            var sourceNodes = new List<Node> (node.FindAll ("source"));

            // verifying syntax
            if (sourceNodes.Count > 1)
                throw new ArgumentException ("[add] can only handle one [source]");
            if (sourceNodes [0] != node.LastChild)
                throw new ArgumentException ("[source] must be the last child of [add] statement");

            if (Expression.IsExpression (sourceNodes [0].Value)) {

                // source is an expression
                List<Node> retVal = new List<Node> ();
                Expression.Iterate<Node> (sourceNodes [0], true, 
                delegate (Node idxDestination) {
                    retVal.Add (idxDestination.Clone ()); // cloning in case source and destination overlaps
                });
                return retVal;
            } else if (sourceNodes [0].Value != null) {

                // source node's value is not empty, still not an expression, which is a bug
                throw new ArgumentException ("[source] node contained a value which was not an expression");
            } else {

                // source is a bunch of static children
                return sourceNodes [0].Children;
            }
        }
    }
}
