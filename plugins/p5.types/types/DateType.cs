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
    ///     Class helps converts from DateTime to string, and vice versa
    /// </summary>
    public static class DateType
    {
        /// <summary>
        ///     Returns the current server date and time
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "date-now", Protection = EventProtection.LambdaClosed)]
        public static void date_now (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = DateTime.Now.ToUniversalTime ();
        }

        /// <summary>
        ///     Formats date according to [format] and/or [culture], which both are optional
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "format-date", Protection = EventProtection.LambdaClosed)]
        public static void format_date (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up arguments afterwards
            using (new Utilities.ArgsRemover (e.Args)) {

                // Retrieving DateTime given, and returning formatted date accordingly, defaulting to "full date and time" format
                DateTime date = e.Args.GetExValue<DateTime> (context);
                CultureInfo culture = 
                    CultureInfo.CreateSpecificCulture (
                        e.Args.GetExChildValue ("culture", context, CultureInfo.CurrentUICulture.Name));
                e.Args.Value = date.ToString (e.Args.GetExChildValue ("format", context, "f"), culture);
            }
        }

        /// <summary>
        ///     Creates a date from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.date", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_object_value_date (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value is DateTime) {
                return;
            } else {
                var strValue = e.Args.Get<string> (context);
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
        ///     Returns the Hyperlisp type-name for the date type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-type-name.System.DateTime", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_type_name_System_DateTime (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "date";
        }
    }
}
