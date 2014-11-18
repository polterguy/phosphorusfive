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

            Match destinationMatch = GetDestinationMatch (e.Args);
            Node dp = new Node ("__pf_dp", null);
            e.Args.Add (dp);
            foreach (Node idxSource in destinationMatch.Matches) {
                dp.Value = idxSource;
                context.Raise ("pf.execute", e.Args);
            }
            dp.Untie ();
        }

        /*
         * will return a Match object for the destination of the "pf.add"
         */
        private static Match GetDestinationMatch (Node node)
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

