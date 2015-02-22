
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
            if (e.Args.Count > 0 && e.Args.LastChild.Name == "rel-source") {

                // relative source, source must be an expression
                SetRelativeSource (e.Args, context);
            } else {

                // static source, source might be anything
                SetStaticSource (e.Args, context);
            }
        }

        /*
         * sets all destinations to static value where value is string or expression
         */
        private static void SetStaticSource (Node node, ApplicationContext context)
        {
            // figuring out source
            object source = XUtil.SourceSingle<object> (node, context, "set");

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
            // iterating through all destinations, figuring out source relative to each destinations
            foreach (var idxDestination in XUtil.Iterate (node, context)) {

                // source is relative to destination
                object source = XUtil.Single<object> (node.LastChild, idxDestination.Node, context, null);
                if (source is Node) {

                    // source is a constant node, making sure we clone it, in case source and destination overlaps
                    source = (source as Node).Clone ();
                }

                idxDestination.Value = source;
            }
        }
    }
}
