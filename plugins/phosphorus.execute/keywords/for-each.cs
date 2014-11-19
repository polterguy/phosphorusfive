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
    /// class wrapping execution engine keyword "add", which allows for changing appending nodes into a node's list of children
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
                Node dp = new Node ("__dp", idxSource);
                e.Args.Insert (0, dp);
                context.Raise ("pf.lambda", e.Args);
                e.Args.RemoveAt (0);
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

