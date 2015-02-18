
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

                // static source, not a node, might be an expression
                SetStaticSource (e.Args, context);
            }
        }

        /*
         * sets all destinations to static value where value is string or expression
         */
        private static void SetStaticSource (Node node, ApplicationContext context)
        {
            // figuring out source
            object source = GetStaticSource (node, context);

            // making sure we support "escaped expressions"
            if (source is string && (source as string).StartsWith ("\\"))
                source = (source as string).Substring (1);

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
                idxDestination.Value = XUtil.Single<object> (node.LastChild, idxDestination.Node, context, null);
            }
        }

        /*
         * retrieves the source for a static ([source]) update
         */
        private static object GetStaticSource (Node node, ApplicationContext context)
        {
            object source = null;
            if (node.LastChild != null && node.LastChild.Name == "source") {

                // we have a [source] parameter here, figuring out what it points to
                if (node.LastChild.Value != null) {

                    // source is either constant value, or an expression
                    source = XUtil.Single<object> (node.LastChild, context, null);
                    if (source is Node) {

                        // source is a constant node, making sure we clone it, in case source and destination overlaps
                        source = (source as Node).Clone ();
                    }
                } else {

                    if (node.LastChild.Count == 1) {

                        // source is a constant node, making sure we clone it, in case source and destination overlaps
                        source = node.LastChild.FirstChild.Clone ();
                    } else {

                        // more than one source
                        throw new ArgumentException ("[set] requires that you give it one [source], or ommit the [source] node all together");
                    }
                }
            }
            return source;
        }
    }
}
