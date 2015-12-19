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
    ///     Class helps converts from DateTime and associated types, to object, and vice versa
    /// </summary>
    public static class DateConversion
    {
        /// <summary>
        ///     Creates a date from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.date", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_object_value_date (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                if (strValue.Length == 10)
                    e.Args.Value = DateTime.ParseExact (strValue, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                else if (strValue.Length == 19)
                    e.Args.Value = DateTime.ParseExact (strValue, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                else if (strValue.Length == 23)
                    e.Args.Value = DateTime.ParseExact (strValue, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
                else
                    throw new ArgumentException ("date; '" + strValue + "' is not recognized as a valid date");
            } else {
                throw new LambdaException (
                    "Don't know how to convert that to a date",
                    e.Args, 
                    context);
            }
        }

        /// <summary>
        ///     Creates a timespan from its string representation
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
        ///     Creates a string from a date
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-string-value.System.DateTime", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_string_value_System_DateTime (ApplicationContext context, ActiveEventArgs e)
        {
            var value = e.Args.Get<DateTime> (context);
            if (value.Hour == 0 && value.Minute == 0 && value.Second == 0 && value.Millisecond == 0)
                e.Args.Value = value.ToString ("yyyy-MM-dd", CultureInfo.InvariantCulture);
            else if (value.Millisecond == 0)
                e.Args.Value = value.ToString ("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            else
                e.Args.Value = value.ToString ("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Creates a string from a timespan
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-string-value.System.TimeSpan", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_string_value_System_TimeSpan (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.Get<TimeSpan> (context).ToString ("c", CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Returns the Hyperlisp type-name for the date type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-type-name.System.DateTime", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_type_name_System_DateTime (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "date";
        }

        /// <summary>
        ///     Returns the Hyperlisp type-name for the timespan type
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
