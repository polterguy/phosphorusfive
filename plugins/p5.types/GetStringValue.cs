/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Text;
using System.Globalization;
using p5.core;
using p5.exp;

namespace p5.types
{
    /// <summary>
    ///     Helper class to create string representation of objects of different types
    /// </summary>
    public static class GetStringValue
    {
        /*
         * Retrieving value in string format Active Events. All types that support automatic conversion
         * from their object representation to a string, do not need their own event handlers, since the
         * default logic is to use "Convert.ChangeType". Hence you only need to implement Active Event converters
         * for types that do not implement IConvertible, or who's default implementation of IConvertible is
         * not sufficient for creating a [*sane*] string representation of your object. Examples are DateTime and 
         * bool, since it creates a non-ISO date string representation by default, and Boolean, since it creates "True" and
         * "False", instead of "true" and "false" - [capital letters are avoided in hyperlisp, if we can]
         * 
         * The name of all of these Active Events is "p5.hyperlisp.get-string-value." + the fully qualified name of your type,
         * or the return value of "typeof(YourType).FullName"
         */

        /// <summary>
        ///     Creates a string from a Node
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-string-value.p5.core.Node", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_string_value_p5_core_Node (ApplicationContext context, ActiveEventArgs e)
        {
            var tmp = new Node ("", e.Args.Value);
            context.RaiseNative ("lambda2lisp", tmp);
            e.Args.Value = tmp.Value;
        }

        /// <summary>
        ///     Creates a string from an Expression
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-string-value.p5.exp.Expression", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_string_value_p5_exp_Expression (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.Get<Expression> (context).Value;
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
        ///     Creates a string from a byte array
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-string-value.System.Byte[]", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_string_value_System_ByteBlob (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.GetChildValue ("encode", context, false))
                e.Args.Value = Convert.ToBase64String (e.Args.Get<byte[]> (context));
            else
                e.Args.Value = Encoding.UTF8.GetString (e.Args.Get<byte[]> (context));
        }
    }
}
