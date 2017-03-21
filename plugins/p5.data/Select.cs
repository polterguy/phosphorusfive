/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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
    ///     Class wrapping [p5.data.select].
    /// </summary>
    public static class Select
    {
        /// <summary>
        ///     [p5.data.select] selects nodes, names or values from your database.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "select-data")]
        [ActiveEvent (Name = "p5.data.select")]
        public static void p5_data_select (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving expression and doing basic syntax checking.
            var ex = e.Args.Value as Expression;
            if (ex == null)
                throw new LambdaException ("[p5.data.select] requires an expression to select items from database", e.Args, context);

            // Making sure we clean up and remove all arguments passed in after execution.
            // In case this is a select count operation though, we return the count as the value of [p5.data.select], hence we cannot remove value of e.Args.
            using (new ArgsRemover (e.Args)) {

                // Acquiring read lock on database.
                Common.Locker.EnterReadLock ();
                try {

                    // Retrieving match object, and checking what type of match it was.
                    var match = ex.Evaluate (context, Common.Database, e.Args);

                    // Retrieving result of match.
                    e.Args.Value = null;
                    switch (match.TypeOfMatch) {
                        case Match.MatchType.count:
                            e.Args.Value = match.Count;
                            break;
                        case Match.MatchType.node:
                            e.Args.AddRange (match.Select (ix => ix.Node.Clone ()));
                            break;
                        case Match.MatchType.name:
                            e.Args.AddRange (match.Select (ix => new Node (ix.Node.Name)));
                            break;
                        case Match.MatchType.value:
                            e.Args.AddRange (match.Select (ix => new Node ("", ix.Value)));
                            break;
                    }
                } finally {

                    // Releasing write lock.
                    Common.Locker.ExitReadLock ();
                }
            }
        }
    }
}
