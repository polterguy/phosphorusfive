/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Globalization;
using phosphorus.core;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.types
{
    /// <summary>
    ///     Helper class to create string representation of objects of different types.
    /// 
    ///     Contains all Active Events necessary to create a string from an object.
    /// </summary>
    public static class GetStringValue
    {
        /*
         * retrieving value in string format Active Events. all types that support automatic conversion
         * from their object representation to a string, do not need their own event handlers, since the
         * default logic is to use "Convert.ChangeType". hence you only need to implement Active Event converters
         * for types that do not implement IConvertible, or whos default implementation of IConvertible is
         * not sufficient for creating a [*sane*] string representation of your object. examples are DateTime and 
         * bool, since it creates a non-ISO date string representation by default, and Boolean, since it creates "True" and
         * "False", instead of "true" and "false" - [capital letters are avoided in hyperlisp, if we can]
         * 
         * the name of all of these Active Events is "pf.hyperlisp.get-string-value." + the fully qualified name of your type,
         * or the return value of "typeof(YourType).FullName"
         */

        /// <summary>
        ///     Creates a string from a Node.
        /// 
        ///     Will convert the given <see cref="phosphorus.core.Node">Node</see> to its string representation. The string representation
        ///     of a node, is Hyperlisp.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-string-value.phosphorus.core.Node")]
        private static void pf_hyperlisp_get_string_value_phosphorus_core_Node (ApplicationContext context, ActiveEventArgs e)
        {
            var tmp = new Node ("", e.Args.Value);
            context.Raise ("pf.hyperlisp.lambda2hyperlisp", tmp);
            e.Args.Value = tmp.Value;
        }

        /// <summary>
        ///     Creates a string from a date.
        /// 
        ///     Will convert the given System.DateTime object to its string representation. The string representation of a DateTime is
        ///     its ISO standardized format, meaning for instance; "yy-MM-dd". Its time parts, and milliseconds parts, will only be
        ///     converted, if they contain anything but "0".
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-string-value.System.DateTime")]
        private static void pf_hyperlisp_get_string_value_System_DateTime (ApplicationContext context, ActiveEventArgs e)
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
        ///     Creates a string from a timespan.
        /// 
        ///     Will convert the given System.TimeSpan to its string representation.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-string-value.System.TimeSpan")]
        private static void pf_hyperlisp_get_string_value_System_TimeSpan (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.Get<TimeSpan> (context).ToString ("c", CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Creates a string from a bool.
        /// 
        ///     Will convert the given System.Boolean to its string representation, meaning either "false" or "true". Notice, no capital letters.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-string-value.System.Boolean")]
        private static void pf_hyperlisp_get_string_value_System_Boolean (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.Get<bool> (context).ToString ().ToLower ();
        }

        /// <summary>
        ///     Creates a string from a byte array.
        /// 
        ///     Will convert the given System.Byte[] to its string representation. The string representation of a byte array, is its 
        ///     BASE64 encoded bytes.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-string-value.System.Byte[]")]
        private static void pf_hyperlisp_get_string_value_System_ByteBlob (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Convert.ToBase64String (e.Args.Get<byte[]> (context));
        }
    }
}
