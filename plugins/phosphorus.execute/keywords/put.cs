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
    /// class wrapping execution engine keyword "put", which allows for changing values of nodes
    /// </summary>
    public static class put
    {
        /// <summary>
        /// put keyword for execution engine
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.put")]
        private static void pf_put (ApplicationContext context, ActiveEventArgs e)
        {
            Match destinationMatch = GetDestinationMatch (e.Args);
            if (e.Args.Count == 1 && Expression.IsExpression (e.Args.FirstChild.Get<string> ())) {

                // we're assigning an expression here
                Match sourceMatch = new Expression (e.Args.FirstChild.Get<string> ()).Evaluate (e.Args.FirstChild);
                destinationMatch.AssignMatch (sourceMatch);
            } else {

                // source is not an expression, either it's a "null assignment" or we're putting a bunch of nodes into another node
                if (e.Args.Count == 0) {

                    // "null assignment"
                    destinationMatch.AssignMatch (null);
                } else {

                    // assigning a bunch of nodes to destination
                    if (destinationMatch.TypeOfMatch != Match.MatchType.Node)
                        throw new ArgumentException ("cannot assign a list of nodes to expression; '" + e.Args.Get<string> () + "'");
                    foreach (Node idxSource in e.Args.Children) {
                        foreach (Node idxDestination in destinationMatch.Matches) {
                            idxDestination.Add (idxSource.Clone ());
                        }
                    }
                }
            }
        }

        /*
         * will return a Match object for the destination of the "pf.put"
         */
        private static Match GetDestinationMatch (Node node)
        {
            string destinationExpression = node.Get<string> ();
            if (!Expression.IsExpression (destinationExpression))
                throw new ApplicationException ("[pf.put] needs an expression as its value");

            // finding Match object for destination
            Match destinationMatch = new Expression (destinationExpression).Evaluate (node);
            if (destinationMatch.Count == 0)
                throw new ArgumentException ("destination expression for [pf.put] yielded no result, expression was; '" + destinationExpression + "'");

            if (!destinationMatch.IsAssignable)
                throw new ArgumentException ("destination expression for [pf.put] is not assignable, expression was; '" + destinationExpression + "'");

            return destinationMatch;
        }
    }
}

