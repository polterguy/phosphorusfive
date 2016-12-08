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

using System;
using p5.exp;
using p5.core;

/// <summary>
///     Main namespace for types and type-conversions in Phosphorus Five
/// </summary>
namespace p5.types {
    /// <summary>
    ///     Class helps figuring out if some value(s) can be converted to another type
    /// </summary>
    public static class CanConvert
    {
        /// <summary>
        ///     Returns true if value(s) can be converted to type of [type]
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "can-convert")]
        public static void can_convert (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Syntax checking invocation
                if (!(e.Args.Value is Expression) || e.Args ["type"] == null)
                    throw new ArgumentException ("[can-convert] needs both an expression as its value, and a [type] parameter");

                // Figuring out type to check for
                string type = e.Args.GetExChildValue<string> ("type", context);

                // Exploiting the fact that conversions will throw exception if conversion is not possible
                try {

                    // Looping through all arguments supplied
                    foreach (var idx in XUtil.Iterate<object> (context, e.Args)) {
                        var objValue = context.Raise (".p5.hyperlambda.get-object-value." + type, new Node ("", idx)).Value;
                    }

                    // No exception occurred, conversion is possible
                    e.Args.Value = true;
                } catch {

                    // Oops, conversion is not possible!
                    e.Args.Value = false;
                }
            }
        }
    }
}
