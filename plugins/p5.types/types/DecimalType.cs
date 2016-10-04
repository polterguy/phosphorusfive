/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Globalization;
using p5.core;

namespace p5.types.types {
    /// <summary>
    ///     Class helps converts from decimal to string, and vice versa
    /// </summary>
    public static class DecimalType
    {
        /// <summary>
        ///     Creates a decimal from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlambda.get-object-value.decimal")]
        private static void p5_hyperlisp_get_object_value_decimal (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value is decimal) {
                return;
            } else {
                e.Args.Value = decimal.Parse (e.Args.Get<string> (context), CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        ///     Returns the Hyperlambda type-name for the decimal type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlambda.get-type-name.System.Decimal")]
        private static void p5_hyperlisp_get_type_name_System_Decimal (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "decimal";
        }
    }
}
