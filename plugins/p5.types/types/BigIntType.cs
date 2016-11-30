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

using System.Numerics;
using System.Globalization;
using p5.core;

/// <summary>
///     Main namespace for all the different types Phosphorus Five supports
/// </summary>
namespace p5.types.types {
    /// <summary>
    ///     Class helps converts from BigInteger to string, and vice versa
    /// </summary>
    public static class BigInt
    {
        /// <summary>
        ///     Creates a bigint from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-object-value.bigint")]
        private static void p5_hyperlisp_get_object_value_bigint (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value is BigInteger) {
                return;
            } else {
                e.Args.Value = BigInteger.Parse (e.Args.Get<string> (context), CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        ///     Returns the Hyperlambda type-name for the bigint type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-type-name.System.Numerics.BigInteger")]
        private static void p5_hyperlisp_get_type_name_System_Numerics_BigInteger (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "bigint";
        }
    }
}
