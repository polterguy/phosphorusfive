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
    ///     Class helps converts from boolean to string, and vice versa
    /// </summary>
    public static class BoolType
    {
        /// <summary>
        ///     Creates a bool from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.bool", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_object_value_bool (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = e.Args.Get<string> (context).ToLower () == "true";
            } else if (e.Args.Value is bool) {
                return; // do nothing, no conversion necessary!
            } else {
                e.Args.Value = e.Args.Value != null;
            }
        }

        /// <summary>
        ///     Creates a string from a bool
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-string-value.System.Boolean", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_string_value_System_Boolean (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.Get<bool> (context).ToString ().ToLower ();
        }

        /// <summary>
        ///     Returns the Hyperlisp type-name for the bool type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-type-name.System.Boolean", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_type_name_System_Boolean (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "bool";
        }
    }
}
