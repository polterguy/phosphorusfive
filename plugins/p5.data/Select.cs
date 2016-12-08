/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
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

using System.Linq;
using p5.exp;
using p5.core;
using p5.data.helpers;
using p5.exp.exceptions;

namespace p5.data
{
    /// <summary>
    ///     Class wrapping [select-data].
    /// </summary>
    public static class Select
    {
        /// <summary>
        ///     [select-data] selects nodes, names or values from your database.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "select-data")]
        public static void select_data (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving expression and doing basic syntax checking.
            var ex = e.Args.Value as Expression;
            if (ex == null)
                throw new LambdaException ("[select-data] requires an expression to select items from database", e.Args, context);

            // Making sure we clean up and remove all arguments passed in after execution.
            // In case this is a select count operation though, we return the count as the value of [select-data], hence we cannot remove value of e.Args.
            using (new Utilities.ArgsRemover (e.Args)) {

                // Acquiring read lock on database.
                Common.Locker.EnterReadLock ();
                try {

                    // Retrieving match object, and checking what type of match it was.
                    var match = ex.Evaluate (context, Common.Database, e.Args);

                    // Removing value of [select-data] node by default.
                    e.Args.Value = null;

                    // Checking type of select, and acting accordingly.
                    if (match.TypeOfMatch == Match.MatchType.count) {

                        // Returning number of items found as main value of node.
                        e.Args.Value = match.Count;
                    } else if (match.TypeOfMatch == Match.MatchType.node) {

                        // Node match, returning cloned version of each node found.
                        e.Args.AddRange (match.Select (ix => ix.Node.Clone ()));
                    } else if (match.TypeOfMatch == Match.MatchType.name) {

                        // Name match, returning only names of nodes.
                        e.Args.AddRange (match.Select (ix => new Node (ix.Node.Name)));
                    } else {

                        // Value match, returning all values of nodes.
                        e.Args.AddRange (match.Select (ix => new Node ("", ix.Value)));
                    }
                } finally {

                    // Releasing write lock.
                    Common.Locker.ExitReadLock ();
                }
            }
        }
    }
}
