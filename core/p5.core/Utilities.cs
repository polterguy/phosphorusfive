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
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace p5.core
{
    /// <summary>
    ///     Utility class, contains helpers for common operations.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        ///     Converts the given object "value" to type T.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="context">Application context</param>
        /// <param name="defaultValue">Default value to return, if no conversion is possible, or value is null</param>
        /// <typeparam name="T">Type to convert your value to</typeparam>
        /// <returns>Converted value, or defaultValue if no conversion is possible</returns>
        public static T Convert<T> (ApplicationContext context, object value, T defaultValue = default (T))
        {
            // Checking if value is null.
            if (value == null)
                return defaultValue;

            // Checking to see if conversion is even necessary.
            if (value is T)
                return (T)value;

            // Then checking if we're doing a "ToString" conversion.
            if (typeof (T) == typeof (string))
                return Convert (context, Convert2String (context, value), defaultValue);

            // Then the "whatever case".
            return Convert2Object (value, context, defaultValue);
        }

        /// <summary>
        ///     Converts the specified value to a string using conversion Active Events.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="context">ApplicationContext to convert within</param>
        /// <param name="defaultValue">Default string to return, if no conversion is possible, or value is null</param>
        /// <returns>The converted object as a string</returns>
        public static string Convert2String (ApplicationContext context, object value, string defaultValue = null)
        {
            // Sanity check.
            if (value == null)
                return defaultValue;

            // Checking if conversion is even necessarily.
            if (value is string)
                return (string)value;

            // Special handling for IEnumerable<Node>, to make sure we are able to "hit" our conversion Active Event.
            // This is done to make sure we only need one event handler for array types of conversions.
            if (value is IEnumerable<Node>)
                value = ((IEnumerable<Node>)value).ToArray ();

            // Notice, if Active Event conversion yields null, we invoke "System.Convert.ToString" as a failsafe default, which means Active Event conversions
            // does not need to be implemented for types where this method yields something sane, such as integers, Guids, floats, etc ...
            return context.RaiseEvent (".p5.hyperlambda.get-string-value." + value.GetType ().FullName, new Node ("", value)).Value as string
                          ?? System.Convert.ToString (value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Base64 encodes the given value.
        /// </summary>
        /// <returns>The encoded array</returns>
        /// <param name="context">Context to perform conversion from within</param>
        /// <param name="value">What to base64 encode</param>
        public static string Base64Encode (ApplicationContext context, byte [] value)
        {
            // Sanity check.
            if (value == null)
                return null;

            // Invoking conversion Active Event with "encode" set to true.
            var node = new Node ("", value);
            node.Add ("encode", true);

            // Notice, if Active Event conversion yields null, we invoke "System.Convert.ToString" as a failsafe default, which means Active Event conversions
            // does not need to be implemented for types where this method yields something sane, such as integers, Guids, floats, etc ...
            return context.RaiseEvent (".p5.hyperlambda.get-string-value.System.Byte[]", node).Value as string;
        }

        /// <summary>
        ///     Converts the specified value to an object of type T using conversion Active Events.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="context">Context to convert within</param>
        /// <param name="defaultValue">Default value to return if no conversion is possible</param>
        /// <typeparam name="T">The type to convert object to</typeparam>
        /// <returns>The value converted to type T</returns>
        public static T Convert2Object<T> (object value, ApplicationContext context, T defaultValue = default (T))
        {
            // Sanity check, before we attempt conversion.
            if (value == null || value.Equals (default (T)))
                return defaultValue;

            // Checking if conversion is even necessary.
            if (value is T)
                return (T)value;

            // Retrieving type name for object type, such that we can figure out which Active Event to use for conversion.
            var typeName = context.RaiseEvent (".p5.hyperlambda.get-type-name." + typeof (T).FullName).Value as string;

            // Checking if we have a native typename installed in context, and if not, using IConvertible if possible, resorting to defaultValue if not.
            if (typeName == null)
                return value is IConvertible ? (T)System.Convert.ChangeType (value, typeof (T), CultureInfo.InvariantCulture) : defaultValue;

            // This is a native Phosphorus Five type, attempting to convert it to the specified type.
            var retVal = context.RaiseEvent (".p5.hyperlambda.get-object-value." + typeName, new Node ("", value)).Value ?? (typeName == "node" ? new Node () : null);

            // If above invocation was not successful, we try IConvertible for object.
            if (retVal == null || retVal.Equals (default (T)))
                return value is IConvertible ? (T)System.Convert.ChangeType (value, typeof (T), CultureInfo.InvariantCulture) : defaultValue;
            return (T)retVal;
        }

        /// <summary>
        ///     Reads a single line string literal from the specified text reader.
        /// </summary>
        /// <returns>The single line string literal, parsed</returns>
        /// <param name="reader">Reader to read from</param>
        public static string ReadSingleLineStringLiteral (StringReader reader)
        {
            var builder = new StringBuilder ();
            for (var c = reader.Read (); c != -1; c = reader.Read ()) {
                switch (c) {
                case '"':
                    return builder.ToString ();
                case '\\':
                    builder.Append (AppendEscapeCharacter (reader));
                    break;
                case '\n':
                case '\r':
                    throw new ApplicationException (string.Format ("Syntax error, string literal unexpected CR/LF near '{0}'", builder));
                default:
                    builder.Append ((char)c);
                    break;
                }
            }
            throw new ApplicationException (string.Format ("Syntax error, string literal not closed before end of input near '{0}'", builder));
        }

        /// <summary>
        ///     Reads a multi line string literal from the specified text reader.
        /// </summary>
        /// <returns>The single line string literal, parsed</returns>
        /// <param name="reader">Reader to read from</param>
        public static string ReadMultiLineStringLiteral (StringReader reader)
        {
            var builder = new StringBuilder ();
            for (var c = reader.Read (); c != -1; c = reader.Read ()) {
                switch (c) {
                case '"':
                    if ((char)reader.Peek () == '"')
                        builder.Append ((char)reader.Read ());
                    else
                        return builder.ToString ();
                    break;
                case '\n':
                    builder.Append ("\r\n"); // Normalizing carriage return
                    break;
                case '\r':
                    if ((char)reader.Read () != '\n')
                        throw new ArgumentException (string.Format ("Unexpected CR found without any matching LF near '{0}'", builder));
                    builder.Append ("\r\n");
                    break;
                default:
                    builder.Append ((char)c);
                    break;
                }
            }
            throw new ArgumentException (string.Format ("String literal not closed before end of input near '{0}'", builder));
        }

        /*
         * Appends an escape character intoto StringBuilder from specified StringReader.
         */
        static string AppendEscapeCharacter (StringReader reader)
        {
            switch (reader.Read ()) {
            case -1:
                throw new ArgumentException ("End of input found when looking for escape character in single line string literal");
            case '"':
                return "\"";
            case '\'':
                return "'";
            case '\\':
                return "\\";
            case 'a':
                return "\a";
            case 'b':
                return "\b";
            case 'f':
                return "\f";
            case 't':
                return "\t";
            case 'v':
                return "\v";
            case 'n':
                return "\r\n"; // Normalizing carriage return
            case 'r':
                // CR must be followed by LF.
                if ((char)reader.Read () != '\\' || (char)reader.Read () != 'n')
                    throw new ArgumentException ("CR found, but no matching LF found");
                return "\r\n";
            case 'x':
                return HexaCharacter (reader);
            default:
                throw new ArgumentException ("Invalid escape sequence found in string literal");
            }
        }

        /*
         * Returns a character represented as an octal character representation.
         */
        static string HexaCharacter (StringReader reader)
        {
            var hexNumberString = "";
            for (var idxNo = 0; idxNo < 4; idxNo++)
                hexNumberString += (char)reader.Read ();
            var integerNo = System.Convert.ToInt32 (hexNumberString, 16);
            return Encoding.UTF8.GetString (BitConverter.GetBytes (integerNo).Reverse ().ToArray ());
        }
    }
}
