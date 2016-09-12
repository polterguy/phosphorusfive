/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Globalization;
using p5.core;

namespace p5.types.types {
    /// <summary>
    ///     Class helps converts from float to string, and vice versa
    /// </summary>
    public static class FloatType
    {
        /// <summary>
        ///     Creates a float from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.float", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_object_value_float (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value is float) {
                return;
            } else {
                e.Args.Value = float.Parse (e.Args.Get<string> (context), CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        ///     Returns the Hyperlisp type-name for the float type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-type-name.System.Single", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_type_name_System_Single (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "float";
        }
    }
}
