
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
        /// [for-each] keyword for execution engine, allowing for iterating over an expression returning a list of nodes
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "for-each")]
        private static void lambda_for_each (ApplicationContext context, ActiveEventArgs e)
        {
            Match dataSource = GetDataSource (e.Args);
            if (e.Args.Count == 0)
                return; // "do nothing" operation
            foreach (Node idxSource in dataSource.Matches) {
                Node dp = new Node ("__dp", idxSource);
                e.Args.Insert (0, dp);
                context.Raise ("lambda.immutable", e.Args);
                e.Args [0].Untie ();
            }
        }

        /*
         * will return a Match object for the destination of the "pf.add"
         */
        private static Match GetDataSource (Node node)
        {
            string dataSourceExpression = node.Get<string> ();
            if (!Expression.IsExpression (dataSourceExpression))
                throw new ApplicationException ("[for-each] needs a valid source expression yielding an actual result as its value");

            // finding Match object for destination
            Match dataSourceMatch = Expression.Create (dataSourceExpression).Evaluate (node);

            if (dataSourceMatch.TypeOfMatch != Match.MatchType.Node)
                throw new ArgumentException ("source expression for [for-each] is not of type 'node', expression was; '" + 
                    dataSourceExpression + "'. make sure you end your [for-each] expression with '?node'");

            return dataSourceMatch;
        }
    }
}
