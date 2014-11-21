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
    /// class wrapping execution engine keyword "move", which allows for appending nodes into a node's list of children, while
    /// removing them from their original location
    /// </summary>
    public static class move
    {
        /// <summary>
        /// move keyword for execution engine
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.move")]
        private static void pf_move (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Count == 0)
                return; // "do nothing" operation

            Match destinationMatch = GetDestinationMatch (e.Args);
            if (destinationMatch.Count == 0)
                return; // "do nothing" operation

            if (e.Args.Count == 1 && e.Args.FirstChild.Name == string.Empty && Expression.IsExpression (e.Args.FirstChild.Get<string> ())) {

                // source is an expression
                Match sourceMatch = new Expression (e.Args.FirstChild.Get<string> ()).Evaluate (e.Args.FirstChild);
                if (sourceMatch.TypeOfMatch != Match.MatchType.Node)
                    throw new ArgumentException ("source match of [pf.move] must be of type 'node'");
                AppendMatch (destinationMatch, sourceMatch);
            } else {
                throw new ArgumentException ("you need a source location to move nodes from");
            }
        }

        /*
         * appends from a match object
         */
        private static void AppendMatch (Match destinationMatch, Match sourceMatch)
        {
            // untying nodes from previous parent, and storing as list
            List<Node> sourceNodes = new List<Node> (sourceMatch.Matches);
            foreach (Node idxSource in sourceNodes) {
                idxSource.Untie ();
            }

            // appending copy of nodes into destination
            foreach (Node idxDest in destinationMatch.Matches) {
                foreach (Node idxSource in sourceNodes) {
                    idxDest.Add (idxSource.Clone ());
                }
            }
        }

        /*
         * will return a Match object for the destination of the "pf.move"
         */
        private static Match GetDestinationMatch (Node node)
        {
            string destinationExpression = node.Get<string> ();
            if (!Expression.IsExpression (destinationExpression))
                throw new ApplicationException ("[pf.move] needs a valid expression yielding an actual result as its value");

            // finding Match object for destination
            Match destinationMatch = new Expression (destinationExpression).Evaluate (node);

            if (destinationMatch.TypeOfMatch != Match.MatchType.Node)
                throw new ArgumentException ("destination expression for [pf.move] is not of type 'node', expression was; '" + destinationExpression + "'");

            return destinationMatch;
        }
    }
}

