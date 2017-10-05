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
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.data.helpers;
using p5.exp.exceptions;

namespace p5.data
{
    /// <summary>
    ///     Class wrapping [p5.data.update].
    /// </summary>
    public static class Update
    {
        /// <summary>
        ///     [p5.data.update] updates nodes, values or names in your p5.data database.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "update-data")]
        [ActiveEvent (Name = "p5.data.update")]
        public static void p5_data_update (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving expression and doing some basic sanity checks.
            if (!(e.Args.Value is Expression))
                throw new LambdaException ("[p5.data.update] requires an expression leading to whatever you want to be updated in your database", e.Args, context);

            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Acquiring write lock on database.
                using (new Common.Lock (true)) {

                    // Storing affected nodes, and number of affected items.
                    var changed = new List<Node> ();
                    int affectedItems = 0;

                    // Retrieving source, and iterating through each destination, updating with source value.
                    // Notice, we don't provide transaction support here, but we do make sure at least any changes
                    // that actually occurs, are serialized to disc!
                    var source = XUtil.Source (context, e.Args);
                    try {

                        foreach (var idxDestination in e.Args.Get<Expression> (context).Evaluate (context, Common.Database, e.Args)) {

                            // Updates the destination with the source, making sure we can keep track of files that are changed, and that we throw if update is unsuccessful.
                            if (!UpdateDestination (context, source, idxDestination))
                                throw new LambdaException ("[p5.data.update] requires your new node needs to have a unique ID, or use its old ID by not providing one", e.Args, context);

                            Common.AddNodeToChanges (idxDestination.Node, changed);
                            affectedItems += 1;
                        }
                    } finally {

                        Common.SaveAffectedFiles (context, changed);
                        e.Args.Value = affectedItems;
                    }
                }
            }
        }

        /*
         * Updates a single destination with the given source.
         */
        static bool UpdateDestination (
            ApplicationContext context,
            object source,
            exp.matchentities.MatchEntity destination)
        {
            // Figuring out if source is a Node.
            if (source is Node) {

                // Checking if this is a "root node" update, and if so, make sure we keep the old ID, unless a new one is explicitly given.
                var newNode = source as Node;
                if (destination.Node.OffsetToRoot == 2) {

                    // "Root node" update, making sure node keep old ID, or if a new ID is explicitly given, make sure it is unique.
                    if (newNode.Value == null) {

                        // We're keeping our old ID, no need to check for unique ID.
                        destination.Node.Clear ().AddRange (newNode.Clone ().Children);
                        destination.Node.Name = newNode.Name;

                    } else {

                        // User gave us an explicit ID, making sure it is either the same, or a new unique ID.
                        if (Common.Database.Children.Any (ix => ix.Children.Any (ix2 => ix2 != destination.Node && ix2.Value.Equals (newNode.Value)))) {

                            // Explicit new ID exists from before, and it is not the node we're currently updating.
                            return false;
                        }
                        destination.Node.Clear ().AddRange (newNode.Clone ().Children);
                        destination.Node.Name = newNode.Name;
                        destination.Node.Value = newNode.Value;
                    }
                } else {
                    destination.Node.Clear ().AddRange (newNode.Clone ().Children);
                    destination.Node.Name = newNode.Name;
                    destination.Node.Value = newNode.Value;
                }
            } else {
                destination.Value = source;
            }
            return true;
        }
    }
}
