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

using p5.core;
using p5.lambda.helpers;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [set] keyword in p5 lambda
    /// </summary>
    public static class Set
    {
        /// <summary>
        ///     The [set] keyword, allows you to change nodes through p5 lambda
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set")]
        public static void lambda_set (ApplicationContext context, ActiveEventArgs e)
        {
            // Asserting destination is expression.
            var destEx = Helper.GetDestinationExpression (context, e.Args, "set");

            // Finding source node, and invoking source event to retrieve source for operation.
            var src = Helper.GetSourceValue (context, e.Args);

            // Updating destination(s) with value of source.
            foreach (var idxDestination in destEx.Evaluate (context, e.Args, e.Args)) {

                // Single source, or null source.
                idxDestination.Value = src;
            }
        }
    }
}
