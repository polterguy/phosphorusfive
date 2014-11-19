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
            if (destinationMatch.Count == 0)
                return; // "do nothing" operation

            // checking type of assignment
            if (e.Args.Count == 1 && e.Args.FirstChild.Name == string.Empty && Expression.IsExpression (e.Args.FirstChild.Get<string> ())) {

                // we're assigning an expression here
                Match sourceMatch = new Expression (e.Args.FirstChild.Get<string> ()).Evaluate (e.Args.FirstChild);
                if (sourceMatch.Count == 0) {
                    AssignNull (destinationMatch);
                } else {
                    AssignMatch (context, destinationMatch, sourceMatch);
                }
            } else if (e.Args.Count == 1 && e.Args.FirstChild.Name == string.Empty && !Expression.IsExpression (e.Args.FirstChild.Get<string> ())) {
                AssignValue (destinationMatch, e.Args.FirstChild.Value);
            } else if (e.Args.Count == 0) {

                // "null assignment"
                AssignNull (destinationMatch);
            } else {

                // replacing all match nodes with first child node
                AssignNode (context, destinationMatch, e.Args.FirstChild);
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
         * assigns a match to another match
         */
        private static void AssignMatch (ApplicationContext context, Match destinationMatch, Match sourceMatch)
        {
            // cloning our source is both destination and source is node, in case source is also one of our destinations
            Node copy = null;
            if (sourceMatch.TypeOfMatch == Match.MatchType.Node && 
                destinationMatch.TypeOfMatch == Match.MatchType.Node)
                copy = sourceMatch [0].Clone ();

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
                    idxDest.Replace (copy.Clone ());
                    break;
                }
            }
        }

        /*
         * assigns a value to match
         */
        private static void AssignValue (Match destinationMatch, object value)
        {
            foreach (Node idxDest in destinationMatch.Matches) {
                switch (destinationMatch.TypeOfMatch) {
                case Match.MatchType.Name:
                    idxDest.Name = (value ?? "").ToString ();
                    break;
                case Match.MatchType.Value:
                    idxDest.Value = value;
                    break;
                case Match.MatchType.Node:
                    throw new ArgumentException ("you cannot assign a value to a node match");
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
                return GetSingleObjectFromMatch (context, match);
            } else {

                // multiple matches
                return GetMultipleObjectFromMatch (context, match);
            }
        }
        
        /*
         * returns a single object from match
         */
        private static object GetSingleObjectFromMatch (ApplicationContext context, Match match)
        {
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
                context.Raise ("pf.nodes-2-code", tmpCode);
                return tmpCode.Value;
            }
            return null; // we should never get here, but compiler creates hickup unless we return something at end of method here
        }
        
        /*
         * returns multiple objects from match
         */
        private static object GetMultipleObjectFromMatch (ApplicationContext context, Match match)
        {
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
            context.Raise ("pf.nodes-2-code", tmpCode);
            return tmpCode.Value;
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
                    context.Raise ("pf.nodes-2-code", tmpCode);
                    if (destination.TypeOfMatch == Match.MatchType.Name)
                        idxDest.Name = (tmpCode.Value ?? "").ToString ();
                    else
                        idxDest.Value = tmpCode.Value;
                    break;
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

            if (!destinationMatch.IsAssignable)
                throw new ArgumentException ("destination expression for [pf.set] is not assignable, expression was; '" + destinationExpression + "'");

            return destinationMatch;
        }
    }
}

