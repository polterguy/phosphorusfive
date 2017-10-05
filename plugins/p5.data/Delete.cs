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

using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.data.helpers;
using p5.exp.exceptions;

namespace p5.data
{
    /// <summary>
    ///     Class wrapping [p5.data.delete].
    /// </summary>
    public static class Delete
    {
        /// <summary>
        ///     [p5.data.delete] deletes items from your p5.data database.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-data")]
        [ActiveEvent (Name = "p5.data.delete")]
        public static void p5_data_delete (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check.
            if (!(e.Args.Value is Expression))
                throw new LambdaException ("[p5.data.delete] requires an expression as its value", e.Args, context);

            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Acquiring write lock on database.
                using (new Common.Lock (true)) {

                    // Making sure we keep track of nodes that are changed, and how many items were deleted.
                    var changed = new List<Node> ();
                    int affectedItems = 0;

                    // Notice, we don't provide any transaction support, but at least we serialize any changes to disc.
                    try {

                        // Looping through database matches and removing nodes while storing which files have been changed as a result of deletion.
                        foreach (var idxDest in e.Args.Get<Expression> (context).Evaluate (context, Common.Database, e.Args)) {

                            // Sanity check.
                            if (idxDest.Node.OffsetToRoot < 2)
                                throw new LambdaException ("[p5.data.delete] can only delete items, not files, or entire database", e.Args, context);

                            // Figuring out which file Node updated belongs to, and storing in changed list.
                            Common.AddNodeToChanges (idxDest.Node, changed);

                            // Setting value to null, which works if user chooses to delete "value", "name" or "node".
                            // Count though will throw an exception though.
                            idxDest.Value = null;

                            // Incrementing affected items.
                            affectedItems += 1;
                        }
                    } finally {

                        Common.SaveAffectedFiles (context, changed);
                        e.Args.Value = affectedItems;
                    }
                }
            }
        }
    }
}
