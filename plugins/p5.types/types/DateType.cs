/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.Globalization;
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
        [ActiveEvent (Name = "p5.types.date.now")]
        public static void p5_types_date_now (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.GetExChildValue ("local", context, false) ? DateTime.Now : DateTime.Now.ToUniversalTime ();
        }

        /// <summary>
        ///     Formats date according to [format] and/or [culture], which both are optional
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.types.date.format")]
        public static void p5_types_date_format (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up arguments afterwards
            using (new ArgsRemover (e.Args)) {

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
        [ActiveEvent (Name = ".p5.hyperlambda.get-object-value.date")]
        static void p5_hyperlisp_get_object_value_date (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value is DateTime)
                return;
            var strValue = e.Args.Get<string> (context);
            if (strValue != null) {
                switch (strValue.Length) {
                    case 10:
                        e.Args.Value = DateTime.ParseExact (strValue, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        break;
                    case 16:
                        e.Args.Value = DateTime.ParseExact (strValue, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                        break;
                    case 19:
                        e.Args.Value = DateTime.ParseExact (strValue, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                        break;
                    case 23:
                        e.Args.Value = DateTime.ParseExact (strValue, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
                        break;
                    default:
                        throw new ArgumentException ("date; '" + strValue + "' is not recognized as a valid date");
                }
            } else {
                throw new LambdaException (
                    "Don't know how to convert that to a date",
                    e.Args,
                    context);
            }
        }

        /// <summary>
        ///     Creates a string from a date
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-string-value.System.DateTime")]
        static void p5_hyperlisp_get_string_value_System_DateTime (ApplicationContext context, ActiveEventArgs e)
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
        ///     Returns the Hyperlambda type-name for the date type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-type-name.System.DateTime")]
        static void p5_hyperlisp_get_type_name_System_DateTime (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "date";
        }
    }
}
