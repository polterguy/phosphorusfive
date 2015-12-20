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
    ///     Class helps converts from unsigned int to string, and vice versa
    /// </summary>
    public static class UIntConversion
    {

        /// <summary>
        ///     Creates a uint from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.uint", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_object_value_uint (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = uint.Parse (strValue, CultureInfo.InvariantCulture);
            } else {
                throw new LambdaException (
                    "Don't know how to convert that to a uint",
                    e.Args, 
                    context);
            }
        }

        /// <summary>
        ///     Returns the Hyperlisp type-name for the uint type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-type-name.System.UInt32", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_type_name_System_UInt32 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "uint";
        }
    }
}
