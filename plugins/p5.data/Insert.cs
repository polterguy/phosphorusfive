/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
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
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5.data.helpers;

/// <summary>
///     Main namespace for all p5.data Active Events
/// </summary>
namespace p5.data
{
    /// <summary>
    ///     Class wrapping [insert-data]
    /// </summary>
    public static class Insert
    {
        /// <summary>
        ///     Inserts or appends nodes into database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "insert-data")]
        [ActiveEvent (Name = "append-data")]
        public static void insert_data (ApplicationContext context, ActiveEventArgs e)
        {
            /*
             * Note, since [insert-data] creates an ID for items not explicitly giving an ID,
             * we do NOT remove arguments in this Active Event, since sometimes caller needs to
             * know the ID of the node inserted into database, and is not ready to create an ID
             * for it himself. Therefor the generated ID is returned as the value of each item inserted,
             * and hence we cannot remove all arguments passed into Active Event
             * 
             * However, we DO remove children of each root node inserted, unless string is submitted 
             * as source somehow ...
             */

            // Acquiring lock on database
            lock (Common.Lock) {

                var forceAppend = e.Name == "append-data";

                // Used to store how many items are actually affected
                int affectedItems = 0;

                // Looping through all nodes given as children, value, or as result from expression
                var changed = new List<Node> ();
                foreach (var idx in XUtil.Iterate<Node> (context, e.Args, false, false, true)) {

                    // Inserting node
                    if (e.Args.Value is string) {

                        // Source is a string, and not an expression, making sure we add children of converted
                        // string, since conversion routine creates a root node wrapping actual nodes in string
                        foreach (var idxInner in idx.Children) {

                            // Inserting node
                            InsertNode (idxInner, context, changed, forceAppend);
                        }
                    } else {

                        // Making sure we clean up and remove all arguments of inserted node passed in after execution
                        using (new Utilities.ArgsRemover (idx)) {

                            // Inserting node
                            InsertNode (idx, context, changed, forceAppend);
                        }
                    }

                    // Incrementing affected items
                    affectedItems += 1;
                }

                // Saving all affected files
                Common.SaveAffectedFiles (context, changed);

                // Returning number of affected items
                e.Args.Value = affectedItems;
            }
        }

        /*
         * Inserts one node into database
         */
        private static void InsertNode (
            Node node, 
            ApplicationContext context, 
            List<Node> changed,
            bool forceAppend)
        {
            // Syntax checking insert node
            SyntaxCheckInsertNode (node, context);

            // Finding next available database file node
            var fileNode = Common.GetAvailableFileNode (context, forceAppend);

            // Figuring out which file Node updated belongs to, and storing in changed list
            if (!changed.Contains (fileNode))
                changed.Add (fileNode);

            // Actually appending node into database
            fileNode.Add (node.Clone ());
        }

        /*
         * Syntax checks node before insertion is allowed
         */
        private static void SyntaxCheckInsertNode (Node node, ApplicationContext context)
        {
            // Making sure it is impossible to insert items without a name into database
            if (string.IsNullOrEmpty (node.Name))
                throw new LambdaException ("[insert-data] requires that each item you insert has a name", node, context);

            // Making sure insert node gets an ID, unless one is explicitly given
            if (node.Value == null) {

                // Automatically generating an ID for item, since no ID was supplied by caller
                node.Value = Guid.NewGuid ();
            } else {

                // User gave us an "explicit new ID", making sure that it does not exist from before
                foreach (var fileNodeIdx in Common.Database.Children) {
                    foreach (var rootNodeIdx in fileNodeIdx.Children) {
                        if (rootNodeIdx.Value.Equals (node.Value)) {

                            // Explicit new ID exists from before!
                            throw new LambdaException ("Sorry, your new node needs to have a unique ID, or use the ID it already had from before", node, context);
                        }
                    }
                }
            }
        }
    }
}
