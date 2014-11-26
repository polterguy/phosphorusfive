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
    /// class wrapping execution engine keyword "for-each", which allows for iteratively executing code for every instance in an expression
    /// </summary>
    public static class forEach
    {
        /// <summary>
        /// for-each keyword for execution engine
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.for-each")]
        private static void pf_for_each (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Count == 0)
                return; // "do nothing" operation

            Match dataSource = GetDataSource (e.Args);
            foreach (Node idxSource in dataSource.Matches) {
                // storing "old nodes"
                List<Node> oldNodes = new List<Node> ();
                foreach (Node idx in e.Args.Children) {
                    oldNodes.Add (idx.Clone ());
                }
                Node dp = new Node ("__dp", idxSource);
                e.Args.Insert (0, dp);
                Node idxExe = e.Args.FirstChild;
                while (idxExe != null) {

                    // we don't execute nodes that start with an underscore "_" since these are considered "data segments"
                    if (!idxExe.Name.StartsWith ("_")) {
                        string avName = idxExe.Name;

                        // making sure our active event is prefixed with a "pf." if it doesn't contain a period "." in its name anywhere
                        if (!avName.Contains ("."))
                            avName = "pf." + avName;
                        context.Raise (avName, idxExe);
                    }
                    idxExe = idxExe.NextSibling;
                }
                e.Args.Clear ();
                e.Args.AddRange (oldNodes);
            }
        }

        /*
         * will return a Match object for the destination of the "pf.add"
         */
        private static Match GetDataSource (Node node)
        {
            string destinationExpression = node.Get<string> ();
            if (!Expression.IsExpression (destinationExpression))
                throw new ApplicationException ("[pf.for-each] needs a valid expression yielding an actual result as its value");

            // finding Match object for destination
            Match destinationMatch = new Expression (destinationExpression).Evaluate (node);

            if (destinationMatch.TypeOfMatch != Match.MatchType.Node)
                throw new ArgumentException ("destination expression for [pf.for-each] is not of type 'node', expression was; '" + destinationExpression + "'");

            return destinationMatch;
        }
    }
}

