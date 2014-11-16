/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute
{
    /// <summary>
    /// class wrapping execution engine keyword "add", which allows for adding children nodes to nodes
    /// </summary>
    public static class add
    {
        /// <summary>
        /// add keyword for execution engine
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.add")]
        private static void pf_add (ApplicationContext context, ActiveEventArgs e)
        {
            if (!Expression.IsExpression (e.Args.Get<string> ()))
                throw new ApplicationException ("[pf.add] needs a destination expression as its value");
            if (e.Args.Count == 0)
                throw new ArgumentException ("[pf.add] needs a source value or expression");

            // finding Match object for destination
            Match destinationMatch = new Expression (e.Args.Get<string> ()).Evaluate (e.Args);
            if (destinationMatch.Count == 0)
                throw new ArgumentException ("destination in [pf.add] returned no match");
            if (destinationMatch.TypeOfMatch != Match.MatchType.Node)
                throw new ArgumentException ("destination of [pf.add] must be a node");

            string source = Expression.FormatNode (e.Args.FirstChild);
            if (Expression.IsExpression (source)) {
                Match sourceMatch = new Expression (source).Evaluate (e.Args.FirstChild);
                foreach (Node idxDest in destinationMatch.Matches) {
                    foreach (Node idxSource in sourceMatch.Matches) {
                        idxDest.Add (idxSource.Clone ());
                    }
                }
            } else {
                foreach (Node idxDest in destinationMatch.Matches) {
                    foreach (Node idxSource in e.Args.Children) {
                        idxDest.Add (idxSource.Clone ());
                    }
                }
            }
        }
    }
}

