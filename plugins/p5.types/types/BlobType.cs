/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
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
        [ActiveEvent (Name = "p5.hyperlambda.get-object-value.blob")]
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
        [ActiveEvent (Name = "p5.hyperlambda.get-string-value.System.Byte[]")]
        private static void p5_hyperlisp_get_string_value_System_ByteBlob (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.GetChildValue ("encode", context, false))
                e.Args.Value = Convert.ToBase64String (e.Args.Get<byte[]> (context));
            else
                e.Args.Value = Encoding.UTF8.GetString (e.Args.Get<byte[]> (context));
        }

        /// <summary>
        ///     Returns the Hyperlambda type-name for the blob/byte array type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlambda.get-type-name.System.Byte[]")]
        private static void p5_hyperlisp_get_type_name_System_ByteBlob (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "blob";
        }
    }
}
