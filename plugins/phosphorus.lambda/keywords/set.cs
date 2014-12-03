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
            if (e.Args.Count > 1)
                throw new ArgumentException ("[set] was given multiple sources, which is illegal. make sure [set] has 0 or one children nodes");

            // getting destination match nodes
            Match destinationMatch = GetDestinationMatch (e.Args);
            if (destinationMatch.Count == 0)
                return; // no destination, no reasons to go further

            // checking type of assignment
            if (e.Args.Count == 1 && e.Args.FirstChild.Name == string.Empty && Expression.IsExpression (e.Args.FirstChild.Get<string> ())) {

                // assigning the result of an expression here
                string expression = Expression.FormatNode (e.Args.FirstChild);
                Match sourceMatch = Expression.Create (expression).Evaluate (e.Args.FirstChild);
                if (sourceMatch.Count == 0) {

                    // source expression returned nothing
                    AssignNull (destinationMatch);
                } else if (sourceMatch.Count == 1 || sourceMatch.TypeOfMatch == Match.MatchType.Count) {

                    // destination is either an expression of type 'count' or has only one result
                    AssignMatch (context, destinationMatch, sourceMatch);
                } else {
                    throw new ArgumentException ("[set] requires a source expression yielding no more than 1 match as its result, " + 
                        "unless the source expression is of type 'count'. expression; '" + 
                        expression + "' returned multiple results and was not a 'count' expression, and hence operation is a logical error");
                }
            } else if (e.Args.Count == 1 && e.Args.FirstChild.Name == string.Empty && !Expression.IsExpression (e.Args.FirstChild.Value)) {

                // assigning a "constant" to destination
                AssignValue (destinationMatch, e.Args.FirstChild);
            } else if (e.Args.Count == 0) {

                // assigning "null" to destination
                AssignNull (destinationMatch);
            } else if (e.Args.Count == 1 && destinationMatch.TypeOfMatch == Match.MatchType.Node) {

                // replacing all match nodes with first child node
                AssignNode (context, destinationMatch, e.Args.FirstChild);
            } else {
                throw new ArgumentException ("[set] cannot handle multiple sources, neither as result of expressions, nor as constant nodes beneath itself");
            }
        }

        /*
         * assigns a value to match
         */
        private static void AssignValue (Match destinationMatch, Node valueNode)
        {
            object value = valueNode.Value;
            if (valueNode.Count > 0) {

                // this is a formatting expression, where the source is the result of a formatting operation
                value = Expression.FormatNode (valueNode);
            }
            foreach (Node idxDest in destinationMatch.Matches) {
                switch (destinationMatch.TypeOfMatch) {
                case Match.MatchType.Name:
                    idxDest.Name = (value ?? "").ToString (); // name cannot be null
                    break;
                case Match.MatchType.Value:
                    idxDest.Value = value;
                    break;
                case Match.MatchType.Node:
                    throw new ArgumentException ("you cannot assign a value to a destination expression of type 'node'");
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
                }
            }
        }

        /*
         * assigns a match to another match
         */
        private static void AssignMatch (ApplicationContext context, Match destinationMatch, Match sourceMatch)
        {
            // cloning our source if both destination and source is node, in case source is also one of our destinations
            Node copy = null;
            if (sourceMatch.TypeOfMatch == Match.MatchType.Node && 
                destinationMatch.TypeOfMatch == Match.MatchType.Node)
                copy = sourceMatch [0].Clone ();

            foreach (Node idxDest in destinationMatch.Matches) {
                switch (destinationMatch.TypeOfMatch) {
                case Match.MatchType.Name:
                    idxDest.Name = ConvertObjectToString (context, GetObjectFromMatch (sourceMatch));
                    break;
                case Match.MatchType.Value:
                    idxDest.Value = GetObjectFromMatch (sourceMatch);
                    break;
                case Match.MatchType.Node:
                    if (sourceMatch.TypeOfMatch != Match.MatchType.Node)
                        throw new ArgumentException ("tried to assign a non-node match to a node match with [set], you can only assign a node match to another node match");
                    idxDest.Replace (copy.Clone ());
                    break;
                }
            }
        }

        /*
         * converts an object to string
         */
        private static string ConvertObjectToString (ApplicationContext context, object obj)
        {
            if (obj is Node) {
                Node tmp = obj as Node;
                Node convert = new Node (string.Empty);
                convert.Add (tmp.Clone ());
                context.Raise ("pf.nodes-2-code", convert);
                return convert.Get<string> ();
            } else {
                return obj.ToString ();
            }
        }

        /*
         * returns an object from match
         */
        private static object GetObjectFromMatch (Match match)
        {
            if (match.TypeOfMatch == Match.MatchType.Count)
                return match.Count;
            else if (match.Count == 1) {

                // single match
                return match.GetValue (0);
            }
            throw new ArgumentException ("[set] cannot assign the result of a source expression yielding multiple results");
        }

        /*
         * replaces all match nodes with given "node"
         */
        private static void AssignNode (ApplicationContext context, Match destination, Node node)
        {
            foreach (Node idxDest in destination.Matches) {
                idxDest.Replace (node.Clone ());
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
            Match destinationMatch = Expression.Create (destinationExpression).Evaluate (node);

            if (!destinationMatch.IsAssignable)
                throw new ArgumentException ("destination expression for [pf.set] is not assignable, expression was; '" + destinationExpression + "'");

            return destinationMatch;
        }
    }
}

