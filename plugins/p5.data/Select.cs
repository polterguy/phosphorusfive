/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using p5.exp;
using p5.core;
using p5.data.helpers;
using p5.exp.exceptions;

namespace p5.data
{
    /// <summary>
    ///     Class wrapping [select-data]
    /// </summary>
    public static class Select
    {
        /// <summary>
        ///     Selects nodes from your database
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "select-data", Protection = EventProtection.Lambda)]
        private static void select_data (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving expression and doing basic syntax checking
            var ex = e.Args.Value as Expression;
            if (ex == null)
                throw new LambdaException ("[select-data] requires an expression to select items from database", e.Args, context);

            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Acquiring lock on database
                lock (Common.Lock) {

                    // Making sure database is initialized
                    Common.Initialize (context);

                    // Iterating through each result from database node tree
                    var match = ex.Evaluate (Common.Database, context, e.Args);
                    if (match.TypeOfMatch == Match.MatchType.count) {

                        // Returning number of items found as main value of node if expression was of type 'count'
                        e.Args.Value = match.Count;
                    } else {

                        // Looping through each match in expression result
                        foreach (var idxMatch in match) {

                            // Dependent upon type of expression, we either return a bunch of nodes, flat, with
                            // name being string.Empty, and value being matched value, or we append node itself back
                            // to caller. This allows us to select using expressions which are not of type 'node'
                            e.Args.Add (idxMatch.TypeOfMatch != Match.MatchType.node ? 
                                new Node (string.Empty, idxMatch.Value) : 
                                idxMatch.Node.Clone ());
                        }

                        // Removing argument
                        e.Args.Value = null;
                    }
                }
            }
        }
    }
}
