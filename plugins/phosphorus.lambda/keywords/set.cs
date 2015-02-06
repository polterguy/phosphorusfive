
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
            // figuring out destination expression
            string destinationExpression = null;
            if (XUtil.IsFormatted (e.Args))
                destinationExpression = XUtil.FormatNode (e.Args, context);
            else
                destinationExpression = e.Args.Get<string> (context);

            // figuring out source, and executing the corresponding logic
            if (e.Args.LastChild.Name == "source") {

                // static source, not a node, might be an expression
                SetStaticSource (destinationExpression, e.Args, context);
            } else if (e.Args.LastChild.Name == "rel-source") {

                // relative source, source must be an expression
                SetRelativeSource (destinationExpression, e.Args, context);
            } else {

                // no source, setting all destinations to null
                SetNull (destinationExpression, e.Args, context);
            }
        }

        /*
         * sets all destinations to static value where value is string or expression
         */
        private static void SetStaticSource (string destinationExpression, Node node, ApplicationContext context)
        {
            // figuring out source
            object source = null;
            if (node.LastChild.Value != null)
                XUtil.Single<object> (node.LastChild, context);
            else
                source = node.LastChild.FirstChild;

            // iterating through all destinations
            XUtil.Iterate (destinationExpression, node, context, 
            delegate (MatchEntity idxDestination) {
                idxDestination.Value = source;
            });
        }

        /*
         * sets all destination nodes relative to themselves
         */
        private static void SetRelativeSource (string destinationExpression, Node node, ApplicationContext context)
        {
            // finding source expression before iteration start, in case iteration changes source node
            var sourceExpression = node.LastChild.Get<string> (context);

            // iterating through all destinations, figuring out source relative to each destinations
            XUtil.Iterate (destinationExpression, node, context, 
            delegate (MatchEntity idxDestination) {

                // figuring out source relative to destination, for then to update destination
                var source = XUtil.Single<object> (sourceExpression, idxDestination.Node, context);
                idxDestination.Value = source;
            });
        }

        /*
         * sets all destinations to null
         */
        private static void SetNull (string destinationExpression, Node node, ApplicationContext context)
        {
            XUtil.Iterate (destinationExpression, node, context, 
            delegate (Match.MatchEntity idxDestination) {
                idxDestination.Value = null;
            });
        }
    }
}
