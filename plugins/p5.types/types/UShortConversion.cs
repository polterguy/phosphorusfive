/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.types.types
{
    /// <summary>
    ///     Class helps converts from ushort to string, and vice versa
    /// </summary>
    public static class UShortConversion
    {
        /// <summary>
        ///     Creates an ushort from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.ushort", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_object_value_ushort (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = ushort.Parse (strValue, CultureInfo.InvariantCulture);
            } else {
                throw new LambdaException (
                    "Don't know how to convert that to a ushort",
                    e.Args, 
                    context);
            }
        }

        /// <summary>
        ///     Returns the Hyperlisp type-name for the ushort type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-type-name.System.UInt16", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_type_name_System_UInt16 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "ushort";
        }
    }
}
