
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
    /// class wrapping execution engine keyword "append",
    /// which allows for appending nodes into a node's list of children
    /// </summary>
    public static class append
    {
        /// <summary>
        /// [add] keyword for execution engine. allows adding nodes from one part of your tree to another part
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "append")]
        private static void lambda_append (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Count == 0)
                throw new ArgumentException ("[append] needs a valid [source] or [rel-source]");

            if (e.Args.LastChild.Name == "source") {

                // static source
                AppendStaticSource (e.Args, context);
            } else if (e.Args.LastChild.Name == "rel-source" && XUtil.IsExpression (e.Args.LastChild.Value)) {

                // relative source
                AppendRelativeSource (e.Args, context);
            } else {
            
                // syntax error
                throw new ArgumentException ("[append] needs a valid [source] or [rel-source]");
            }
        }

        /*
         * retrieves source nodes for "static" update
         */
        private static List<Node> GetSourceNodes (Node node, ApplicationContext context)
        {
            List<Node> sourceNodes = new List<Node> ();
            if (XUtil.IsExpression (node.LastChild.Value)) {
                foreach (var idx in XUtil.Iterate (node.LastChild, context)) {
                    if (idx.TypeOfMatch != Match.MatchType.node && !(idx.Value is Node)) {

                        // [source] is an expression leading to something that's not a node, this
                        // will trigger conversion from string to node, adding a "root node" during
                        // conversion. making sure we remove this node, when appending source nodes
                        foreach (var idxInner in Utilities.Convert<Node> (idx.Value, context).Children) {
                            sourceNodes.Add (idxInner.Clone ());
                        }
                    } else {

                        // [source] is an expression, leading to something that's already a node somehow
                        sourceNodes.Add (Utilities.Convert<Node> (idx.Value, context).Clone ());
                    }
                }
            } else if (node.LastChild.Value is Node) {

                // value of [source] is a node, adding this node
                foreach (var idx in XUtil.Iterate<Node> (node.LastChild, context)) {
                    sourceNodes.Add (idx.Clone ());
                }
            } else if (node.LastChild.Value == null) {

                // [source] has no value, neither static string values, nor expressions
                // adding all children of [source]
                foreach (var idx in node.LastChild.Children) {
                    sourceNodes.Add (idx.Clone ());
                }
            } else {

                // [source] is not an expression, but has a string value. this will trigger a conversion
                // from string to node, creating a "root node" during conversion. skipping this "root" node,
                // and only adding children of that auto-generated root node
                foreach (var idx in XUtil.Iterate<Node> (node.LastChild, context)) {
                    foreach (var idxInner in idx.Children) {
                        sourceNodes.Add (idxInner.Clone ());
                    }
                }
            }
            return sourceNodes;
        }

        /*
         * source is static
         */
        private static void AppendStaticSource (Node node, ApplicationContext context)
        {
            // retrieving source before we start iterating destination,
            // in case destination and source overlaps
            List<Node> sourceNodes = GetSourceNodes (node, context);

            // looping through every destination node
            bool isFirst = true; // since source is already cloned, we avoid cloning the first run
            foreach (var idxDestination in XUtil.Iterate (node, context)) {

                // verifying destination actually is a node
                Node curDest = idxDestination.Value as Node;
                if (curDest == null)
                    throw new ArgumentException ("cannot [append] into something that's not a node");

                // minor optimization trick, since source already is cloned upon first run
                if (isFirst) {
                    curDest.AddRange (sourceNodes);
                    isFirst = false;
                } else {
                    foreach (Node idxSource in sourceNodes) {
                        curDest.Add (idxSource.Clone ());
                    }
                }
            }
        }

        /*
         * relative source
         */
        private static void AppendRelativeSource (Node node, ApplicationContext context)
        {
            foreach (var idxDestination in XUtil.Iterate (node, context)) {
                
                // verifying destination actually is a node
                Node curDest = idxDestination.Value as Node;
                if (curDest == null)
                    throw new ArgumentException ("cannot [append] into something that's not a node");

                foreach (var idxSource in XUtil.Iterate<Node> (node.LastChild, curDest, context)) {
                    curDest.Add (idxSource.Clone ());
                }
            }
        }
    }
}
