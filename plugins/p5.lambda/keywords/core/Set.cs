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

using p5.exp;
using p5.core;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [set] Active Event.
    /// </summary>
    public static class Set
    {
        /// <summary>
        ///     The [set] event, allows you to change nodes, values, and names of nodes.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set")]
        public static void lambda_set (ApplicationContext context, ActiveEventArgs e)
        {
            // Finding source value, notice that if we have no source, we still iterate each destination, such that we can set it to a "null value".
            var src = XUtil.GetSourceValue (context, e.Args);

            // Updating destination(s) with value of source.
            foreach (var idxDestination in XUtil.GetDestinationMatch (context, e.Args)) {

                // Single source, or null source.
                idxDestination.Value = src;
            }
        }
    }
}
