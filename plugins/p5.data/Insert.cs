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

using System;
using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.data.helpers;
using p5.exp.exceptions;

namespace p5.data
{
    /// <summary>
    ///     Class wrapping [p5.data.insert] and [p5.data.append] Active Events.
    /// </summary>
    public static class Insert
    {
        /// <summary>
        ///     [p5.data.insert] and [p5.data.append] inserts or appends data nodes into your p5.data database.
        ///     The former chooses the first available file node, the latter always appends at the end.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "insert-data")]
        [ActiveEvent (Name = "append-data")]
        [ActiveEvent (Name = "p5.data.insert")]
        [ActiveEvent (Name = "p5.data.append")]
        public static void p5_data_insert_append (ApplicationContext context, ActiveEventArgs e)
        {
            // Acquiring write lock on database.
            using (new Common.Lock (true)) {

                // Making sure we keep track of which files are changed, and how many items were affected.
                var changed = new List<Node> ();
                int affectedItems = 0;

                // Checking if we should force insertion at the end or not.
                var forceAppend = e.Name == "p5.data.append" || e.Name == "append-data";

                // Notice, we don't provide any transactional support here, but at least we make sure any changes are serialized to disc.
                try {

                    // Looping through all nodes given as children, value, or as the result of an expression.
                    foreach (var idx in XUtil.Iterate<Node> (context, e.Args)) {

                        // Inserting node, clearing children, and incrementing number of affected items.
                        InsertNode (idx, context, changed, forceAppend);
                        idx.Clear ();
                        affectedItems += 1;
                    }
                } finally {

                    Common.SaveAffectedFiles (context, changed);
                    e.Args.Value = affectedItems;
                }
            }
        }

        /*
         * Inserts one node into database.
         */
        static void InsertNode (
            Node node,
            ApplicationContext context,
            List<Node> changed,
            bool forceAppend)
        {
            // Syntax checking and decorating insert node.
            DecorateNode (node, context);

            // Finding next available database file node.
            var fileNode = Common.GetAvailableFileNode (context, forceAppend);

            // Figuring out which file Node updated belongs to, and storing it in changed list.
            if (!changed.Contains (fileNode))
                changed.Add (fileNode);

            // Actually appending node into database.
            fileNode.Add (node.Clone ());
        }

        /*
         * Syntax checks node before insertion is allowed.
         */
        static void DecorateNode (Node node, ApplicationContext context)
        {
            // Making sure it is impossible to insert items without a name into database.
            if (string.IsNullOrEmpty (node.Name))
                throw new LambdaException ("[p5.data.insert] requires that each item you insert has a name", node, context);

            // Making sure insert node gets an ID, unless one is explicitly given.
            if (node.Value == null) {

                // Automatically generating an ID for item, since no ID was supplied by caller.
                node.Value = Guid.NewGuid ();

            } else {

                // User gave us an "explicit new ID", making sure that it does not exist from before.
                if (Common.Database.Children.Any (ix => ix.Children.Any (ix2 => ix2.Value.Equals (node.Value)))) {

                    // Explicit new ID exists from before.
                    throw new LambdaException ("Sorry, your new node needs to have a unique ID, or use the ID it already had from before", node, context);
                }
            }
        }
    }
}
