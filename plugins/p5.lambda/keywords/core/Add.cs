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

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [add] Active Event.
    /// </summary>
    public static class Add
    {
        /// <summary>
        ///     The [add] event allows you to append source lambda object(s) into destination lambda object(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "add")]
        public static void lambda_add (ApplicationContext context, ActiveEventArgs e)
        {
            // Finding source nodes, and returning early if no source is given.
            var src = XUtil.GetSourceNodes (context, e.Args);
            if (src == null)
                return;

            // Looping through each destination, adding all source nodes to it, cloning them before adding them.
            foreach (var idxDestination in XUtil.GetDestinationMatch (context, e.Args, true)) {

                idxDestination.Node.AddRange (src.Select (ix => ix.Clone ()));
            }
        }
    }
}
