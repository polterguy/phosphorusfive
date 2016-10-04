/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
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
        [ActiveEvent (Name = "p5.hyperlambda.get-object-value.bigint")]
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
        [ActiveEvent (Name = "p5.hyperlambda.get-type-name.System.Numerics.BigInteger")]
        private static void p5_hyperlisp_get_type_name_System_Numerics_BigInteger (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "bigint";
        }
    }
}
