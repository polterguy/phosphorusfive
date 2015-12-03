/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using p5.core;
using p5.exp;

/// <summary>
///     Main namespace for types and type-conversions in Phosphorus Five.
/// </summary>
namespace p5.types
{
    /// <summary>
    ///     Class helps converts from string to object.
    /// </summary>
    public static class GetObjectValue
    {
        /*
         * retrieving object value from string/byte[] representation Active Events
         * 
         * the name of all of these Active Events is "p5.hyperlisp.get-object-value." + the hyperlisp typename returned
         * in your "p5.hyperlisp.get-type-name.*" Active Events. if you create support for your own types in hyperlisp, then
         * make sure you return an application wide unique hyperlisp typename in your "p5.hyperlisp.get-type-name.*" Active 
         * Event. this can be done by using a "your-company.your-type" format for your hyperlisp type and "get-type-name"
         * Active Event
         */


        /// <summary>
        ///     Creates a <see cref="phosphorus.core.Node">Node</see> list from its string representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.node", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_node (ApplicationContext context, ActiveEventArgs e)
        {
            var code = e.Args.Get<string> (context);
            var tmp = new Node (string.Empty, code);
            context.RaiseNative ("lisp2lambda", tmp);
            e.Args.Value = tmp.Count > 0 ? new Node (string.Empty, null, tmp.Children) : null;
        }

        /// <summary>
        ///     Creates a single <see cref="phosphorus.core.Node">Node</see> from its string representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.abs.node", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_abs_node (ApplicationContext context, ActiveEventArgs e)
        {
            var code = e.Args.Get<string> (context);
            var tmp = new Node (string.Empty, code);
            context.RaiseNative ("lisp2lambda", tmp);
            if (tmp.Count == 1) {
                // if there's only one node, we return that as result
                e.Args.Value = tmp [0].Clone ();
            } else if (tmp.Count > 1) {
                throw new ArgumentException ("cannot convert string to 'abs' Node, since it would create more than one resulting root node");
            } else {
                e.Args.Value = null;
            }
        }

        /// <summary>
        ///     Creates a <see cref="phosphorus.core.Node.Dna">DNA</see> from its string representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.path", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_path (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = new Node.Dna (e.Args.Get<string> (context));
        }
        
        /// <summary>
        ///     Creates a <see cref="p5.exp.Expression">Expression</see> from its string representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.x", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_x (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Expression.Create (e.Args.Get<string> (context), context);
        }

        /// <summary>
        ///     Creates a Guid from its string/byte[] representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.guid", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_guid (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = Guid.Parse (strValue);
            } else {
                var byteValue = e.Args.Value as byte [];
                if (byteValue != null) {
                    e.Args.Value = FromBytes<Guid> (byteValue);
                }
            }
        }

        /// <summary>
        ///     Creates a long from its string/byte[] representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.long", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_long (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = long.Parse (strValue, CultureInfo.InvariantCulture);
            } else {
                var byteValue = e.Args.Value as byte [];
                if (byteValue != null) {
                    e.Args.Value = FromBytes<long> (byteValue);
                }
            }
        }

        /// <summary>
        ///     Creates a ulong from its string/byte[] representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.ulong", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_ulong (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = ulong.Parse (strValue, CultureInfo.InvariantCulture);
            } else {
                var byteValue = e.Args.Value as byte [];
                if (byteValue != null) {
                    e.Args.Value = FromBytes<ulong> (byteValue);
                }
            }
        }

        /// <summary>
        ///     Creates an int from its string/byte[] representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.int", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_int (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = int.Parse (strValue, CultureInfo.InvariantCulture);
            } else {
                var byteValue = e.Args.Value as byte [];
                if (byteValue != null) {
                    e.Args.Value = FromBytes<int> (byteValue);
                }
            }
        }

        /// <summary>
        ///     Creates a uint from its string/byte[] representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.uint", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_uint (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = uint.Parse (strValue, CultureInfo.InvariantCulture);
            } else {
                var byteValue = e.Args.Value as byte [];
                if (byteValue != null) {
                    e.Args.Value = FromBytes<uint> (byteValue);
                }
            }
        }

        /// <summary>
        ///     Creates a short from its string/byte[] representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.short", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_short (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = short.Parse (strValue, CultureInfo.InvariantCulture);
            } else {
                var byteValue = e.Args.Value as byte [];
                if (byteValue != null) {
                    e.Args.Value = FromBytes<short> (byteValue);
                }
            }
        }

        /// <summary>
        ///     Creates a single from its string/byte[] representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.float", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_float (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = float.Parse (strValue, CultureInfo.InvariantCulture);
            } else {
                var byteValue = e.Args.Value as byte [];
                if (byteValue != null) {
                    e.Args.Value = FromBytes<float> (byteValue);
                }
            }
        }

        /// <summary>
        ///     Creates a double from its string/byte[] representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.double", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_double (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = double.Parse (strValue, CultureInfo.InvariantCulture);
            } else {
                var byteValue = e.Args.Value as byte [];
                if (byteValue != null) {
                    e.Args.Value = FromBytes<double> (byteValue);
                }
            }
        }

        /// <summary>
        ///     Creates a decimal from its string/byte[] representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.decimal", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_decimal (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = decimal.Parse (strValue, CultureInfo.InvariantCulture);
            } else {
                var byteValue = e.Args.Value as byte [];
                if (byteValue != null) {
                    e.Args.Value = FromBytes<decimal> (byteValue);
                }
            }
        }

        /// <summary>
        ///     Creates a bool from its string/byte[] representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.bool", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_bool (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = e.Args.Get<string> (context).ToLower () == "true";
            } else {
                var byteValue = e.Args.Value as byte [];
                if (byteValue != null) {
                    e.Args.Value = FromBytes<bool> (byteValue);
                }
            }
        }

        /// <summary>
        ///     Creates a byte from its string/byte[] representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.byte", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_byte (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = byte.Parse (strValue, CultureInfo.InvariantCulture);
            } else {
                var byteValue = e.Args.Value as byte [];
                if (byteValue != null) {
                    e.Args.Value = FromBytes<byte> (byteValue);
                }
            }
        }

        /// <summary>
        ///     Creates a byte array from its string/object representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.blob", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_blob (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {

                // value is string, checking to see if we should decode from base64, or simply return raw bytes
                if (e.Args.GetChildValue ("decode", context, false))
                    e.Args.Value = Convert.FromBase64String (strValue);
                else
                    e.Args.Value = Encoding.UTF8.GetBytes (strValue);
            } else {

                var nodeValue = e.Args.Value as Node;
                if (nodeValue != null) {
                    strValue = e.Args.Get<string> (context);
                    e.Args.Value = Encoding.UTF8.GetBytes (strValue);
                } else {

                    // DateTime cannot be marshalled
                    if (e.Args.Value is DateTime)
                        e.Args.Value = ((DateTime)e.Args.Value).ToBinary ();

                    // marshalling raw bytes
                    var size = Marshal.SizeOf (e.Args.Value);
                    var ptr = Marshal.AllocHGlobal (size);
                    Marshal.StructureToPtr (e.Args.Value, ptr, false);
                    var bytes = new byte [size];
                    Marshal.Copy (ptr, bytes, 0, size);
                    Marshal.FreeHGlobal (ptr);
                    e.Args.Value = bytes;
                }
            }
        }

        /// <summary>
        ///     Creates an sbyte from its string representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.sbyte", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_sbyte (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = sbyte.Parse (strValue, CultureInfo.InvariantCulture);
            } else {
                var byteValue = e.Args.Value as byte [];
                if (byteValue != null) {
                    e.Args.Value = FromBytes<sbyte> (byteValue);
                }
            }
        }

        /// <summary>
        ///     Creates a char from its string representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.char", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_char (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = char.Parse (strValue);
            } else {
                var byteValue = e.Args.Value as byte [];
                if (byteValue != null) {
                    e.Args.Value = FromBytes<char> (byteValue);
                }
            }
        }

        /// <summary>
        ///     Creates a date from its string representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.date", Protection = EventProtection.NativeOnly)]
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
                var byteValue = e.Args.Value as byte [];
                if (byteValue != null) {
                    e.Args.Value = DateTime.FromBinary (FromBytes<long> (byteValue));
                }
            }
        }

        /// <summary>
        ///     Creates a timespan from its string representation.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.time", Protection = EventProtection.NativeOnly)]
        private static void p5_hyperlisp_get_object_value_time (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = TimeSpan.ParseExact (strValue, "c", CultureInfo.InvariantCulture);
            } else {
                var byteValue = e.Args.Value as byte [];
                if (byteValue != null) {
                    e.Args.Value = FromBytes<TimeSpan> (byteValue);
                }
            }
        }
        
        /*
         * helper to convert from bytes to type T
         */
        private static T FromBytes<T> (byte [] bytes)
        {
            var ptr = Marshal.AllocHGlobal (bytes.Length);
            Marshal.Copy (bytes, 0, ptr, bytes.Length);
            T retVal = (T) Marshal.PtrToStructure (ptr, typeof(T));
            Marshal.FreeHGlobal (ptr);
            return retVal;
        }
    }
}
