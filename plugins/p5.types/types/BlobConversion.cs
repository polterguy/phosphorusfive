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

/// <summary>
///     Main namespace for all the different types Phosphorus Five supports
/// </summary>
namespace p5.types.types
{
    /// <summary>
    ///     Class helps converts from blob/byte[] to string, and vice versa
    /// </summary>
    public static class BlobConversion
    {
        /// <summary>
        ///     Creates a byte array from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.blob", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_object_value_blob (ApplicationContext context, ActiveEventArgs e)
        {
            // Checking if this is a string
            var strValue = e.Args.Value as string;
            if (strValue != null) {

                // Value is string, checking to see if we should decode from base64, or simply return raw bytes
                if (e.Args.GetChildValue ("decode", context, false)) {

                    // Caller specified he wanted to decode value from base64
                    e.Args.Value = Convert.FromBase64String (strValue);
                } else {

                    // No decoding here, returning raw bytes through UTF8 encoding
                    e.Args.Value = Encoding.UTF8.GetBytes (strValue);
                }
            } else {

                // Checking if value is a Node
                var nodeValue = e.Args.Value as Node;
                if (nodeValue != null) {

                    // Value is Node, converting to string before we convert to blob
                    strValue = e.Args.Get<string> (context);
                    e.Args.Value = Encoding.UTF8.GetBytes (strValue);
                } else {

                    // DateTime cannot be marshalled
                    if (e.Args.Value is DateTime)
                        e.Args.Value = ((DateTime)e.Args.Value).ToBinary ();
                    else
                        throw new LambdaException (
                            "Don't know how to convert that to a blob",
                            e.Args, 
                            context);
                }
            }
        }

        /// <summary>
        ///     Creates a string from a blob/byte array
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

        /// <summary>
        ///     Returns the Hyperlisp type-name for the blob/byte array type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-type-name.System.Byte[]", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_type_name_System_ByteBlob (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "blob";
        }
    }
}
