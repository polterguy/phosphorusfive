
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
            if (e.Args.Count > 0 && e.Args.LastChild.Name == "source") {
                object source = GetStaticSource (e.Args);
                SetStaticSource (e.Args, source);
            } else if (e.Args.Count > 0 && e.Args.LastChild.Name == "rel-source") {
                SetRelativeSource (e.Args);
            } else {
                SetStaticSource (e.Args, null);
            }
        }

        /*
         * source is static
         */
        private static void SetStaticSource (Node node, object source)
        {
            // iterating through each destination
            XUtil.IterateNodes (node, 
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
                    throw new ArgumentException ("cannot set anything but 'name', value' or 'node' in [set]");
                }
            });
        }
        
        /*
         * source is relative
         */
        private static void SetRelativeSource (Node node)
        {
            // iterating through each destination
            XUtil.IterateNodes (node, 
            delegate (Node idxDestination, Match.MatchType destinationType) {

                // getting source relative to destination, fetching relative source
                string sourceExpression = XUtil.FormatNode (node.LastChild, idxDestination) as string;
                object source = XUtil.Single <object> (idxDestination, sourceExpression);

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
                    throw new ArgumentException ("cannot set anything but 'name', value' or 'node' in [set]");
                }
            });
        }

        /*
         * returns static [source] value from [set] statement back to caller
         */
        private static object GetStaticSource (Node node)
        {
            object retVal = null;
            var sourceNodes = new List<Node> (node.FindAll ("source"));
            if (sourceNodes.Count > 1)
                throw new ArgumentException ("[set] can only handle one [source]");

            // checking for static source, which might be a constant, or an expression
            if (sourceNodes.Count == 1) {

                if (sourceNodes [0].Value != null) {

                    // source is an expression or a constant
                    retVal = XUtil.Single <object> (sourceNodes [0]);

                    // checking to see if source is "escaped"
                    string strRetVal = retVal as string;
                    if (strRetVal != null && strRetVal.StartsWith ("\\"))
                        retVal = strRetVal.Substring (1);
                } else if (sourceNodes [0].Count > 0) {

                    // source is wrapping another node
                    retVal = sourceNodes [0].FirstChild;
                }
            }
            return retVal;
        }
    }
}
