
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
            // finding source
            object source = GetSource (e.Args);

            // iterating through each destination
            XUtil.IterateNodes (e.Args, 
            delegate (Node idxDestination, Match.MatchType destinationType) {
                switch (destinationType) {
                case Match.MatchType.Name:
                    idxDestination.Name = (source ?? "").ToString ();
                    break;
                case Match.MatchType.Value:
                    idxDestination.Value = source;
                    break;
                case Match.MatchType.Node:
                    if (source == null)
                        idxDestination.Untie ();
                    else
                        idxDestination.Replace ((source as Node).Clone ());
                    break;
                default:
                    throw new ArgumentException ("cannot set anything bu 'name', value' or 'node' in [set]");
                }
            });
        }

        /*
         * returns source value from [set] statement back to caller
         */
        private static object GetSource (Node node)
        {
            object retVal = null;
            var sourceNodes = new List<Node> (node.FindAll ("source"));
            if (sourceNodes.Count > 1)
                throw new ArgumentException ("[set] can only handle one [source]");

            if (sourceNodes.Count > 0) {
                if (sourceNodes [0] != node.LastChild)
                    throw new ArgumentException ("[source] must be last child of [set] statement");

                if (sourceNodes [0].Value != null) {

                    // source is an expression or a constant
                    retVal = XUtil.Single <object> (sourceNodes [0]);
                } else if (sourceNodes [0].Count > 0) {

                    // source is wrapping another node
                    retVal = sourceNodes [0].FirstChild;
                }
            }
            return retVal;
        }
    }
}
