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
    public static class set
    {
        /// <summary>
        /// set keyword for execution engine
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.set")]
        private static void pf_set (ApplicationContext context, ActiveEventArgs e)
        {
            // getting destination match nodes
            Match destinationMatch = GetDestinationMatch (e.Args);

            if (e.Args.Count == 1 && Expression.IsExpression (e.Args.FirstChild.Get<string> ()) && e.Args.FirstChild.Name == string.Empty) {

                // we're assigning an expression here
                Match sourceMatch = new Expression (e.Args.FirstChild.Get<string> ()).Evaluate (e.Args.FirstChild);
                if (sourceMatch.Count == 0) {
                    AssignNull (destinationMatch);
                } else {
                    AssignMatch (context, destinationMatch, sourceMatch);
                }
            } else {

                // source is not an expression, either it's a "null assignment" or we're putting a bunch of nodes into another node
                if (e.Args.Count == 0) {

                    // "null assignment"
                    AssignNull (destinationMatch);
                } else {

                    // replacing all match nodes with child node
                    if (e.Args.Count > 1)
                        throw new ArgumentException ("you can only replace a node with a single node, source contained multiple nodes");
                    AssignNode (context, destinationMatch, e.Args.FirstChild);
                }
            }
        }

        /*
         * assigns a match to another match
         */
        private static void AssignMatch (ApplicationContext context, Match destinationMatch, Match sourceMatch)
        {
            foreach (Node idxDest in destinationMatch.Matches) {
                switch (destinationMatch.TypeOfMatch) {
                case Match.MatchType.Name:
                    idxDest.Name = (GetObjectFromMatch (context, sourceMatch) ?? "").ToString ();
                    break;
                case Match.MatchType.Value:
                    idxDest.Value = GetObjectFromMatch (context, sourceMatch);
                    break;
                case Match.MatchType.Node:
                    if (sourceMatch.TypeOfMatch != Match.MatchType.Node)
                        throw new ArgumentException ("tried to assign a non-node match to a node match, you can only assign a node match to another node match");
                    if (sourceMatch.Count > 1)
                        throw new ArgumentException ("tried to assign multiple nodes to a node match, you can only replace one node with one other node");
                    idxDest.Replace (sourceMatch [0].Clone ());
                    break;
                }
            }
        }

        /*
         * returns an object from match
         */
        private static object GetObjectFromMatch (ApplicationContext context, Match match)
        {
            if (match.TypeOfMatch == Match.MatchType.Count)
                return match.Count;
            if (match.Count == 1) {

                // single match
                switch (match.TypeOfMatch) {
                case Match.MatchType.Name:
                    return match [0].Name;
                case Match.MatchType.Value:
                    return match [0].Value;
                case Match.MatchType.Path:
                    return match [0].Path;
                case Match.MatchType.Node:
                    Node tmpCode = new Node ("root");
                    tmpCode.Add (match [0].Clone ());
                    context.Raise ("pf.node-2-hyperlisp", tmpCode);
                    return tmpCode.Value;
                }
            } else {

                // multiple matches
                Node tmpCode = new Node ("root");
                foreach (Node idx in match.Matches) {
                    switch (match.TypeOfMatch) {
                    case Match.MatchType.Name:
                        tmpCode.Add (new Node (string.Empty, idx.Name));
                        break;
                    case Match.MatchType.Value:
                        tmpCode.Add (new Node (string.Empty, idx.Value));
                        break;
                    case Match.MatchType.Path:
                        tmpCode.Add (new Node (string.Empty, idx.Path));
                        break;
                    case Match.MatchType.Node:
                        tmpCode.Add (idx.Clone ());
                        break;
                    }
                }
                context.Raise ("pf.node-2-hyperlisp", tmpCode);
                return tmpCode.Value;
            }
            return null; // we should really never get here, but to make sure compiler doesn't "hickup", we'll need this line of code regardless
        }

        /*
         * replaces all match nodes with given "node"
         */
        private static void AssignNode (ApplicationContext context, Match destination, Node node)
        {
            foreach (Node idxDest in destination.Matches) {
                switch (destination.TypeOfMatch) {
                case Match.MatchType.Node:
                    idxDest.Replace (node.Clone ());
                    break;
                case Match.MatchType.Name:
                case Match.MatchType.Value:
                    Node tmpCode = new Node ("root");
                    tmpCode.Add (node.Clone ());
                    context.Raise ("pf.node-2-hyperlisp", tmpCode);
                    if (destination.TypeOfMatch == Match.MatchType.Name)
                        idxDest.Name = (tmpCode.Value ?? "").ToString ();
                    else
                        idxDest.Value = tmpCode.Value;
                    break;
                }
            }
        }

        /*
         * assigns null to the given match object
         */
        private static void AssignNull (Match destination)
        {
            foreach (Node idxDest in destination.Matches) {
                switch (destination.TypeOfMatch) {
                case Match.MatchType.Name:
                    idxDest.Name = "";
                    break;
                case Match.MatchType.Value:
                    idxDest.Value = null;
                    break;
                case Match.MatchType.Node:
                    idxDest.Untie ();
                    break;
                default:
                    throw new ArgumentException ("you can only assign to a 'name', 'value' or 'node' expression");
                }
            }
        }

        /*
         * will return a Match object for the destination of the "pf.set"
         */
        private static Match GetDestinationMatch (Node node)
        {
            string destinationExpression = node.Get<string> ();
            if (!Expression.IsExpression (destinationExpression))
                throw new ApplicationException ("[pf.set] needs a valid expression yielding an actual result as its value");

            // finding Match object for destination
            Match destinationMatch = new Expression (destinationExpression).Evaluate (node);
            if (destinationMatch.Count == 0)
                throw new ArgumentException ("destination expression for [pf.set] yielded no result, expression was; '" + destinationExpression + "'");

            if (!destinationMatch.IsAssignable)
                throw new ArgumentException ("destination expression for [pf.set] is not assignable, expression was; '" + destinationExpression + "'");

            return destinationMatch;
        }
    }
}

