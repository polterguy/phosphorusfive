/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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
    ///     Class wrapping [update-data]
    /// </summary>
    public static class Update
    {
        /// <summary>
        ///     Updates lambda objects in your database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "update-data")]
        public static void update_data (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving expression and doing some basic syntax checking
            var ex = e.Args.Value as Expression;
            if (ex == null)
                throw new LambdaException ("[update-data] requires an expression to select items from database", e.Args, context);

            // Acquiring lock on database
            lock (Common.Lock) {

                // Used for storing all affected database nodes, such that we know which files to update
                var changed = new List<Node> ();

                // Iterating through each destinations, updating with source
                foreach (var idxDestination in e.Args.Get<Expression> (context).Evaluate (context, Common.Database, e.Args)) {

                    // Figuring out source, possibly relative to destination
                    var source = XUtil.Source (context, e.Args, idxDestination.Node);

                    // Making sure we're only given ONE source!
                    if (source.Count != 1)
                        throw new LambdaException ("[update-data] requires exactly one source", e.Args, context);

                    // Figuring out which file Node updated belongs to, and storing in changed list
                    Common.AddNodeToChanges (idxDestination.Node, changed);

                    // Doing actual update, which depends upon whether or not update is updating an entire node hierarchy, or only a single value
                    Node newNode = source[0] as Node;
                    if (newNode != null) {

                        // Checking if this is a "root node" update, and if so, make sure we keep the old ID, unless a new one is
                        // explicitly given
                        if (idxDestination.Node.Parent.Parent.Parent == null) {

                            // "Root node" update, making sure node keep old ID, or if a new ID is explicitly given, make sure it is UNIQUE
                            if (newNode.Value == null) {

                                // We're keeping our old ID, no need to check for unique ID
                                newNode.Value = idxDestination.Node.Value;
                            } else {

                                // User gave us an "explicit new ID", making sure that if it exists from before, then it is the node we are
                                // updating that has it, an no OTHER nodes in our database!
                                foreach (var fileNodeIdx in Common.Database.Children) {
                                    foreach (var rootNodeIdx in fileNodeIdx.Children) {
                                        if (rootNodeIdx != idxDestination.Node && rootNodeIdx.Value.Equals (newNode.Value)) {

                                            // Explicit new ID exists from before, and it is NOT the node we're currently updating!
                                            // This is an error!
                                            throw new LambdaException ("Sorry, your new node needs to have a unique ID, or use the ID it already had from before", e.Args, context);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Since we only consumed source [0] by reference above, in our ID check, we simply use it directly, since the above logic correctly 
                    // (possibly) changed the underlaying reference
                    idxDestination.Value = source [0];
                }
            
                // Saving all affected files
                Common.SaveAffectedFiles (context, changed);
            }
        }
    }
}
