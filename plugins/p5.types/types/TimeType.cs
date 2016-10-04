/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Globalization;
using p5.core;

namespace p5.types.types {
    /// <summary>
    ///     Class helps converts from TimeSpan to string, and vice versa
    /// </summary>
    public static class TimeType
    {
        /// <summary>
        ///     Creates a time from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlambda.get-object-value.time")]
        private static void p5_hyperlisp_get_object_value_time (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value is TimeSpan) {
                return;
            } else {
                e.Args.Value = TimeSpan.ParseExact (e.Args.Get<string> (context), "c", CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        ///     Creates a string from a time
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlambda.get-string-value.System.TimeSpan")]
        private static void p5_hyperlisp_get_string_value_System_TimeSpan (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.Get<TimeSpan> (context).ToString ("c", CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Returns the Hyperlambda type-name for the time type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlambda.get-type-name.System.TimeSpan")]
        private static void p5_hyperlisp_get_type_name_System_TimeSpan (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "time";
        }
    }
}
