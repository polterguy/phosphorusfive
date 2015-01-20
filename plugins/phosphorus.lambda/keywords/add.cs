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
    /// class wrapping execution engine keyword "add", which allows for appending nodes into a node's list of children
    /// </summary>
    public static class add
    {
        /// <summary>
        /// [add] keyword for execution engine. allows adding nodes from one part of your tree to another part
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "add")]
        private static void lambda_add (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Count == 0)
                throw new ArgumentException ("[add] needs either a source expression, or a list of children nodes");

            Match destinationMatch = GetDestinationMatch (e.Args);
            if (destinationMatch.Count == 0)
                return; // "do nothing" operation

            if (e.Args.Count == 1 && e.Args.FirstChild.Name == string.Empty && Expression.IsExpression (e.Args.FirstChild.Get<string> ())) {

                // source is an expression
                Match sourceMatch = Expression.Create (e.Args.FirstChild.Get<string> ()).Evaluate (e.Args.FirstChild);
                AppendMatch (destinationMatch, sourceMatch);
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
            if (sourceMatch.TypeOfMatch != Match.MatchType.Node && sourceMatch.TypeOfMatch != Match.MatchType.Value)
                throw new ArgumentException ("[add] can only add from a 'node' source expression or a 'value' expression containing a node");

            // cloning source first, in case source is also one of our destinations
            List<Node> copy = new List<Node> ();
            foreach (Node idxSource in sourceMatch.Matches) {
                if (sourceMatch.TypeOfMatch == Match.MatchType.Node) {
                    copy.Add (idxSource.Clone ());
                } else {
                    copy.Add (idxSource.Get<Node> ().Clone ());
                }
            }
            foreach (Node idxDest in destinationMatch.Matches) {
                foreach (Node idxSource in copy) {
                    Node destination = null;
                    if (destinationMatch.TypeOfMatch == Match.MatchType.Node) {
                        destination = idxDest;
                    } else {
                        destination = idxDest.Get<Node> ();
                    }
                    destination.Add (idxSource.Clone ());
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
         * will return a Match object for the destination of the "pf.add"
         */
        private static Match GetDestinationMatch (Node node)
        {
            string destinationExpression = node.Get<string> ();
            if (!Expression.IsExpression (destinationExpression))
                throw new ApplicationException ("[add] needs a valid expression as its value, yielding an actual result");

            // finding Match object for destination
            Match destinationMatch = Expression.Create (destinationExpression).Evaluate (node);

            if (destinationMatch.TypeOfMatch != Match.MatchType.Node) {
                foreach (Node idxNode in destinationMatch.Matches) {
                    if (!(idxNode.Value is Node))
                        throw new ArgumentException ("destination expression for [add] is not of type 'node', expression was; '" + 
                            destinationExpression + "'. make sure you [add] destination expression ends with a '?node'");
                }
            }

            return destinationMatch;
        }
    }
}

