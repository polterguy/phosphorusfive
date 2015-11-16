/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using p5.core;
using p5.exp;
using p5.data.helpers;

namespace p5.data
{
    /// <summary>
    ///     Class wrapping [select-data].
    /// </summary>
    public static class Select
    {
        /// <summary>
        ///     Selects nodes from your database.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "select-data")]
        private static void select_data (ApplicationContext context, ActiveEventArgs e)
        {
            var ex = e.Args.Value as Expression;
            if (ex == null)
                throw new ArgumentException ("[select-data] requires an expression to select items from database");

            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // acquiring lock on database
                lock (Common.Lock) {

                    // making sure database is initialized
                    Common.Initialize (context);

                    // iterating through each result from database node tree
                    bool foundResults = false;
                    foreach (var idxMatch in ex.Evaluate (Common.Database, context, e.Args)) {

                        if (idxMatch.Match.TypeOfMatch == Match.MatchType.count) {

                            // Expression is for 'count' type, ending iteration
                            e.Args.Value = idxMatch.Match.Count;
                            return;
                        }
                        foundResults = true;

                        // dependent upon type of expression, we either return a bunch of nodes, flat, with
                        // name being string.Empty, and value being matched value, or we append node itself back
                        // to caller. this allows us to select using expressions which are not of type 'node'
                        e.Args.Add (idxMatch.TypeOfMatch != Match.MatchType.node ? 
                                new Node (string.Empty, idxMatch.Value) : 
                                idxMatch.Node.Clone ());
                    }
                    if (!foundResults && ex.EvaluateExpressionType (context, e.Args) == Match.MatchType.count)
                        e.Args.Value = 0;
                    else
                        e.Args.Value = null;
                }
            }
        }
    }
}
