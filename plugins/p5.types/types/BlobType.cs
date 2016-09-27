/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Text;
using p5.core;

namespace p5.types.types {
    /// <summary>
    ///     Class helps converts from blob/byte[] to string, and vice versa
    /// </summary>
    public static class BlobType
    {
        /// <summary>
        ///     Creates a byte array from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.blob")]
        private static void p5_hyperlisp_get_object_value_blob (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value is byte[]) {
                return;
            } else {
                if (e.Args.GetChildValue ("decode", context, false)) {

                    // Caller specified he wanted to decode value from base64
                    e.Args.Value = Convert.FromBase64String (e.Args.Get<string>(context));
                } else {

                    // No decoding here, returning raw bytes through UTF8 encoding
                    e.Args.Value = Encoding.UTF8.GetBytes (e.Args.Get<string>(context));
                }
            }
        }

        /// <summary>
        ///     Creates a string from a blob/byte array
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-string-value.System.Byte[]")]
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
        [ActiveEvent (Name = "p5.hyperlisp.get-type-name.System.Byte[]")]
        private static void p5_hyperlisp_get_type_name_System_ByteBlob (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "blob";
        }
    }
}
