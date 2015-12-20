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
    ///     Class helps converts from TimeSpan to string, and vice versa
    /// </summary>
    public static class TimeConversion
    {
        /// <summary>
        ///     Creates a time from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.time", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_object_value_time (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = TimeSpan.ParseExact (strValue, "c", CultureInfo.InvariantCulture);
            } else {
                throw new LambdaException (
                    "Don't know how to convert that to a time",
                    e.Args, 
                    context);
            }
        }

        /// <summary>
        ///     Creates a string from a time
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-string-value.System.TimeSpan", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_string_value_System_TimeSpan (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.Get<TimeSpan> (context).ToString ("c", CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Returns the Hyperlisp type-name for the time type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-type-name.System.TimeSpan", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_type_name_System_TimeSpan (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "time";
        }
    }
}
