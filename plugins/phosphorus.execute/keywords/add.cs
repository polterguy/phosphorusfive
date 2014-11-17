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
    /// class wrapping execution engine keyword "add", which allows for changing values of nodes
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
            Match destinationMatch = GetDestinationMatch (e.Args);
            if (e.Args.Count == 1 && 
                e.Args.FirstChild.Name == string.Empty && 
                Expression.IsExpression (e.Args.FirstChild.Get<string> ())) {

                // source is an expression
                Match sourceMatch = new Expression (e.Args.FirstChild.Get<string> ()).Evaluate (e.Args.FirstChild);
                AppendMatch (destinationMatch, sourceMatch);
            } else if (e.Args.Count == 0) {

                // tried to add "null", which is illegal
                throw new ArgumentException ("[pf.add] was given nothing to add into destination");
            } else {

                // source is a node list
                AppendNodes (destinationMatch, e.Args.Children);
            }
        }

        /*
         * appends from a match object
         */
        private static void AppendMatch (Match destinationMatch, Match sourceMatch)
        {
            foreach (Node idxDest in destinationMatch.Matches) {
                if (sourceMatch.TypeOfMatch == Match.MatchType.Count) {
                    idxDest.Add (new Node (string.Empty, sourceMatch.Count));
                } else {
                    foreach (Node idxSource in sourceMatch.Matches) {
                        switch (sourceMatch.TypeOfMatch) {
                        case Match.MatchType.Name:
                            idxDest.Add (new Node (string.Empty, idxSource.Name));
                            break;
                        case Match.MatchType.Value:
                            idxDest.Add (new Node (string.Empty, idxSource.Value));
                            break;
                        case Match.MatchType.Path:
                            idxDest.Add (new Node (string.Empty, idxSource.Path));
                            break;
                        case Match.MatchType.Node:
                            idxDest.Add (idxSource.Clone ());
                            break;
                        }
                    }
                }
            }
        }

        /*
         * appends from a list of nodes
         */
        private static void AppendNodes (Match destinationMatch, IEnumerable<Node> sourceNodes)
        {
            foreach (Node idxDest in destinationMatch.Matches) {
                foreach (Node idxSource in sourceNodes) {
                    idxDest.Add (idxSource.Clone ());
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
                throw new ApplicationException ("[pf.add] needs a valid expression yielding an actual result as its value");

            // finding Match object for destination
            Match destinationMatch = new Expression (destinationExpression).Evaluate (node);
            if (destinationMatch.Count == 0)
                throw new ArgumentException ("destination expression for [pf.add] yielded no result, expression was; '" + destinationExpression + "'");

            if (destinationMatch.TypeOfMatch != Match.MatchType.Node)
                throw new ArgumentException ("destination expression for [pf.add] is not of type 'node', expression was; '" + destinationExpression + "'");

            return destinationMatch;
        }
    }
}

