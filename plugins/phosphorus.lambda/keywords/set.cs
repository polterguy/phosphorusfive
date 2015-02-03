
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
            bool relativeSource;
            object source = GetSource (e.Args, out relativeSource);

            if (relativeSource) {

                // source is relative to destination
                SetRelativeSource (e.Args, source as string);
            } else {

                // source is static, meaning either a constant, or an expression not relative to source
                SetStaticSource (e.Args, source);
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
        private static void SetRelativeSource (Node node, string sourceExpression)
        {
            // iterating through each destination
            XUtil.IterateNodes (node, 
            delegate (Node idxDestination, Match.MatchType destinationType) {

                // getting source relative to destination
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
         * returns source value from [set] statement back to caller
         */
        private static object GetSource (Node node, out bool isRelative)
        {
            isRelative = false;
            object retVal = null;
            var sourceNodes = new List<Node> (node.FindAll ("source"));
            if (sourceNodes.Count > 1)
                throw new ArgumentException ("[set] can only handle one [source]");

            // checking for static source, wwhich might be a constant or an expression
            if (sourceNodes.Count > 0) {
                if (sourceNodes [0] != node.LastChild)
                    throw new ArgumentException ("[source] must be last child of [set] statement");

                if (sourceNodes [0].Value != null) {

                    // source is an expression or a constant
                    retVal = XUtil.Single <object> (sourceNodes [0]);

                    // checking to see if source is "escaped"
                    if (retVal is string && ((string)retVal).StartsWith ("\\"))
                        retVal = ((string)retVal).Substring (1);
                } else if (sourceNodes [0].Count > 0) {

                    // source is wrapping another node
                    retVal = sourceNodes [0].FirstChild;
                }
            } else {

                // checking for "relative source"
                sourceNodes = new List<Node> (node.FindAll ("rel-source"));
                if (sourceNodes.Count > 1)
                    throw new ArgumentException ("[set] can only handle one [rel-source]");
                if (sourceNodes.Count > 0) {
                    if (sourceNodes [0] != node.LastChild)
                        throw new ArgumentException ("[rel-source] must be last child of [set] statement");

                    if (XUtil.IsExpression (sourceNodes [0].Value)) {

                        // relative source exists, and is an expression
                        retVal = XUtil.FormatNode (sourceNodes [0]);
                        isRelative = true;
                    } else {

                        // relative source can only be an expression
                        throw new ArgumentException ("[rel-source] in [set] can only be an expression");
                    }
                }
            }
            return retVal;
        }
    }
}
