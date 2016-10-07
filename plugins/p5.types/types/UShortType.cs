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

using System.Globalization;
using p5.core;

namespace p5.types.types {
    /// <summary>
    ///     Class helps converts from ushort to string, and vice versa
    /// </summary>
    public static class UShortType
    {
        /// <summary>
        ///     Creates an ushort from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlambda.get-object-value.ushort")]
        private static void p5_hyperlisp_get_object_value_ushort (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value is ushort) {
                return;
            } else {
                e.Args.Value = ushort.Parse (e.Args.Get<string> (context), CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        ///     Returns the Hyperlambda type-name for the ushort type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlambda.get-type-name.System.UInt16")]
        private static void p5_hyperlisp_get_type_name_System_UInt16 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "ushort";
        }
    }
}
