/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

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
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "select-data")]
        public static void select_data (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving expression and doing basic syntax checking
            var ex = e.Args.Value as Expression;
            if (ex == null)
                throw new LambdaException ("[select-data] requires an expression to select items from database", e.Args, context);

            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Acquiring lock on database
                lock (Common.Lock) {

                    // Iterating through each result from database node tree
                    var match = ex.Evaluate (context, Common.Database, e.Args);
                    if (match.TypeOfMatch == Match.MatchType.count) {

                        // Returning number of items found as main value of node if expression was of type 'count'
                        e.Args.Value = match.Count;
                    } else {

                        // Looping through each match in expression result
                        foreach (var idxMatch in match) {

                            // Dependent upon type of expression, we either return a bunch of nodes, flat, with
                            // name being "", and value being matched value, or we append node itself back
                            // to caller. This allows us to select using expressions which are not of type 'node'
                            if (match.Convert == "node") {
                                e.Args.AddRange ((idxMatch.Value as Node).Clone ().Children);
                            } else {
                                e.Args.Add (idxMatch.TypeOfMatch != Match.MatchType.node ? 
                                    new Node ("", idxMatch.Value) : 
                                    idxMatch.Node.Clone ());
                            }
                        }

                        // Removing argument
                        e.Args.Value = null;
                    }
                }
            }
        }
    }
}
