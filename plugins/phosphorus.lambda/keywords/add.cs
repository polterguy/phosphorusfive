
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
                throw new ArgumentException ("[add] needs a valid [source] or [rel-source]");

            if (e.Args.LastChild.Name == "source") {

                // static source
                AddStaticSource (e.Args, context);
            } else if (e.Args.LastChild.Name == "rel-source" && XUtil.IsExpression (e.Args.LastChild.Value)) {

                // relative source
                AddRelativeSource (e.Args, context);
            } else {
            
                // syntax error
                throw new ArgumentException ("[add] needs a valid [source] or [rel-source]");
            }
        }

        /*
         * source is static
         */
        private static void AddStaticSource (Node node, ApplicationContext context)
        {
            // retrieving source before we start iterating destination,
            // in case destination and source overlaps
            List<Node> sourceNodes = new List<Node> ();
            foreach (var idx in XUtil.Content<Node> (node.LastChild, context)) {
                sourceNodes.Add (idx.Clone ());
            }

            // looping through every destination node
            bool isFirst = true; // since source is already cloned, we avoid cloning the first run
            XUtil.Iterate<Node> (node, context,
            delegate (Node idxDestination) {
                if (isFirst) {
                    idxDestination.AddRange (sourceNodes);
                    isFirst = false;
                } else {
                    foreach (Node idxSource in sourceNodes) {
                        idxDestination.Add (idxSource.Clone ());
                    }
                }
            });
        }

        /*
         * relative source
         */
        private static void AddRelativeSource (Node node, ApplicationContext context)
        {
            XUtil.Iterate<Node> (node, context, 
            delegate (Node idxDestination) {
                foreach (var idxSource in XUtil.Content<Node> (node.LastChild, idxDestination, context)) {
                    idxDestination.Add (idxSource.Clone ());
                }
            });
        }
    }
}
