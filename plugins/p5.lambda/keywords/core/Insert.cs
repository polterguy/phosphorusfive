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
using p5.core;
using p5.lambda.helpers;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [insert-before] and [insert-after] Active Events.
    /// </summary>
    public static class Insert
    {
        /// <summary>
        ///     The [insert-before] event allows you to insert nodes before any specified node(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "insert-before")]
        public static void lambda_insert_before (ApplicationContext context, ActiveEventArgs e)
        {
            InsertNodes (context, e.Args, false);
        }

        /// <summary>
        ///     The [insert-after] event allows you to insert nodes after any specified node(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "insert-after")]
        public static void lambda_insert_after (ApplicationContext context, ActiveEventArgs e)
        {
            InsertNodes (context, e.Args, true);
        }

        /*
         * Common insertion method for both of the above Active Events.
         */
        private static void InsertNodes (ApplicationContext context, Node args, bool after)
        {
            // Finding source nodes, and returning early if no source is given.
            var src = Helper.GetSourceNodes (context, args);
            if (src == null || src.Count == 0)
                return;

            // Looping through each destination, and inserting all source node at specified position, in order of appearance.
            foreach (var idxDestination in Helper.GetDestinationMatch (context, args, after ? "insert-after" : "insert-before", true)) {

                // Figuring out insertion point before we insert nodes.
                var index = idxDestination.Node.Parent.IndexOf (idxDestination.Node) + (after ? 1 : 0);
                idxDestination.Node.Parent.InsertRange (index, src.Select (ix => ix.Clone ()));
            }
        }
    }
}
