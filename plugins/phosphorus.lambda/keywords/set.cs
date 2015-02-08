
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
    /// class wrapping execution engine keyword "put", which allows for changing the value and name of nodes, or the node itself
    /// </summary>
    public static class set
    {
        /// <summary>
        /// [set] keyword for execution engine. allows changing the node tree. legal sources and destinations are 'name', 'value' or 'node'
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "set")]
        private static void lambda_set (ApplicationContext context, ActiveEventArgs e)
        {
            // figuring out source, and executing the corresponding logic
            if (e.Args.Count > 0 && e.Args.LastChild.Name == "source") {

                // static source, not a node, might be an expression
                SetStaticSource (e.Args, context);
            } else if (e.Args.Count > 0 && e.Args.LastChild.Name == "rel-source") {

                // relative source, source must be an expression
                SetRelativeSource (e.Args, context);
            } else {

                // no source, setting all destinations to null
                SetNull (e.Args, context);
            }
        }

        /*
         * sets all destinations to static value where value is string or expression
         */
        private static void SetStaticSource (Node node, ApplicationContext context)
        {
            // figuring out source
            object source = null;
            if (node.LastChild.Value != null) {

                // source is either constant value or an expression
                source = XUtil.Single<object> (node.LastChild, context, null);
            } else {

                if (node.LastChild.Count == 1) {

                    // source is a node
                    source = node.LastChild.FirstChild;
                } else if (node.LastChild.Count == 0) {

                    // source is null
                    source = null;
                } else {

                    // more than one source
                    throw new ArgumentException ("[set] requires that you give it only one source");
                }
            }

            // iterating through all destinations, updating with source
            foreach (var idxDestination in XUtil.Iterate (node, context)) {
                idxDestination.Value = source;
            }
        }

        /*
         * sets all destination nodes relative to themselves
         */
        private static void SetRelativeSource (Node node, ApplicationContext context)
        {
            if (!XUtil.IsExpression (node.LastChild.Value))
                throw new ArgumentException ("a [rel-source] must be an expression");

            // iterating through all destinations, figuring out source relative to each destinations
            foreach (var idxDestination in XUtil.Iterate (node, context)) {

                // source is relative to destination
                idxDestination.Value = XUtil.Single<object> (node.LastChild, idxDestination.Node, context, null);
            }
        }

        /*
         * sets all destinations to null
         */
        private static void SetNull (Node node, ApplicationContext context)
        {
            // iterating through all destinations, setting them to null
            foreach (var idxDestination in XUtil.Iterate (node, context)) {
                idxDestination.Value = null;
            }
        }
    }
}
