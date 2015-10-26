/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace pf.core
{
    /// <summary>
    ///     Utility class, contains helpers for common operations.
    /// 
    ///     This class contains helper methods used both by the library internally, and exposed for you to use in your programs.
    ///     One particularly interesting method for you, is the Convert method, that allows you to convert any object, to any other 
    ///     type of object, using the internal type system of Phosphorus.Five.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        ///     Converts the given value to type T.
        /// 
        ///     Will convert the given value to an object of type T for you, either by checking to see if the object
        ///     implements IConvertible, if the object already is of type T, or by using the internal type system of Phosphorus.Five, with
        ///     its conversion Active Events.
        /// 
        ///     This method allows you to convert for instance between strings and Node, and is at the core of the type system of Phosphorus.Five.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="context">Application context. Needed since it might potentially have to raise "conversion Active Events" to convert your value.</param>
        /// <param name="defaultValue">Default value to return, if no conversion is possible.</param>
        /// <param name="encode">If true, then the value will be encoded as base64, if necessary, and value is byte[].</param>
        /// <typeparam name="T">Type to convert your value to.</typeparam>
        public static T Convert<T> (
            object value,
            ApplicationContext context,
            T defaultValue = default (T),
            bool encode = false)
        {
            // no possible conversion exists
            if (value == null)
                return defaultValue;

            // checking to see if conversion is even necessary
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            if (value is T)
                return (T) value;

            // trying installed converters from ApplicationContext
            if (typeof (T) == typeof (string)) {
                var retVal = Convert2String (value, context, encode);
                if (retVal != null)
                    return (T) (object) retVal;
            } else {
                var retVal = Convert2Object<T> (value, context);
                if (retVal != null && !retVal.Equals (default (T)))
                    return retVal;
            }

            // checking if type is IConvertible
            if (value is IConvertible)
                return (T) System.Convert.ChangeType (value, typeof (T), CultureInfo.InvariantCulture);

            // stuff like for instance Guids don't implement IConvertible, but still return sane values, if we
            // first do ToString on them, for then to cast them to object, for then to cast object to T, if the caller
            // is requesting to have them returned as string
            if (typeof (T) == typeof (string))
                return (T) (object) value.ToString ();
            return defaultValue;
        }

        /// <summary>
        ///     Returns true if string can be converted to an integer.
        /// 
        ///     Returns true if string contains nothing but whole integer numbers. Used in the parsing of
        ///     pf.lambda expressions, when constructing iterators, among other things.
        /// </summary>
        /// <returns><c>true</c> if this instance is a whole, positive, integer number; otherwise, <c>false</c>.</returns>
        /// <param name="value">String to check.</param>
        public static bool IsNumber (string value)
        {
            if (value.Any (idx => "0123456789".IndexOf (idx) == -1)) {
                return false;
            }
            return value.Length > 0;
        }

        /// <summary>
        ///     Reads a single line string literal token from specified text reader.
        /// 
        ///     Will read a single line string literal in C# style from the specified reader, advancing the reader's position,
        ///     such that it is just beyond the string literal when done.
        /// 
        ///     Used in among other things while parsing Hyperlisp and pf.lambda expressions.
        /// </summary>
        /// <returns>The single line string literal, parsed.</returns>
        /// <param name="reader">Reader to read from.</param>
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
                        throw new ArgumentException ("syntax error in hyperlisp, single line string literal contains new line");
                    default:
                        builder.Append ((char) c);
                        break;
                }
            }
            throw new ArgumentException ("syntax error in hyperlisp, single line string literal not closed before end of input");
        }

        /// <summary>
        ///     Reads a multi line string literal token from specified text reader.
        /// 
        ///     Will read a multi line string literal in C# style from the specified reader, advancing the reader's position,
        ///     such that it is just beyond the string literal when done.
        /// 
        ///     Used in among other things while parsing Hyperlisp and pf.lambda expressions.
        /// </summary>
        /// <returns>The single line string literal, parsed.</returns>
        /// <param name="reader">Reader to read from.</param>
        public static string ReadMultiLineStringLiteral (StringReader reader)
        {
            var builder = new StringBuilder ();
            for (var c = reader.Read (); c != -1; c = reader.Read ()) {
                switch (c) {
                    case '"':
                        if ((char) reader.Peek () == '"') {
                            builder.Append ((char) reader.Read ());
                        } else {
                            return builder.ToString ();
                        }
                        break;
                    case '\n':
                        builder.Append ("\r\n"); // normalizing carriage return
                        break;
                    case '\r':
                        if ((char) reader.Read () != '\n')
                            throw new ArgumentException ("syntax error in hyperlisp, carriage return found but no new line character in multi line string literal");
                        builder.Append ("\r\n");
                        break;
                    default:
                        builder.Append ((char) c);
                        break;
                }
            }
            throw new ArgumentException ("syntax error in hyperlisp, multiline string literal not closed before end of input");
        }

        /*
         * appends an escape character to stringbuilder
         */
        private static string AppendEscapeCharacter (StringReader reader)
        {
            var c = reader.Read ();
            switch (c) {
                case -1:
                    throw new ArgumentException ("syntax error in hyperlisp, end of input found when looking for escape character in single line string literal");
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
                    return "\r\n"; // normalizing carriage return
                case 'r':
                    // '\r' must be followed by '\n'
                    if ((char) reader.Read () != '\\' || (char) reader.Read () != 'n')
                        throw new ArgumentException ("syntax error in hyperlisp, carriage return found, but no new line character found");
                    return "\r\n";
                case 'x':
                    return HexaCharacter (reader);
                default:
                    throw new ArgumentException (string.Format ("invalid escape sequence found in hyperlisp string literal; '\\{0}'",
                        (char) c));
            }
        }

        /*
         * returns a character represented as an octal character representation
         */
        private static string HexaCharacter (StringReader reader)
        {
            var hexNumberString = string.Empty;
            for (var idxNo = 0; idxNo < 4; idxNo++) {
                hexNumberString += (char) reader.Read ();
            }
            var hexNumber = System.Convert.ToInt32 (hexNumberString, 16);
            return new string ((char) hexNumber, 1);
        }

        /*
         * converts value to string using conversion Active Events
         */
        private static string Convert2String (object value, ApplicationContext context, bool encode)
        {
            var nodes = value as IEnumerable<Node>;
            if (nodes != null) {
                var builder = new StringBuilder ();
                var first = true;
                foreach (var idx in nodes) {
                    if (first) {
                        first = false;
                    } else {
                        builder.Append ("\r\n");
                    }
                    builder.Append (context.Raise (
                        "pf.hyperlisp.get-string-value." +
                        idx.GetType ().FullName, new Node (string.Empty, idx)).Value);
                }
                return builder.ToString ();
            }
            Node node = new Node (string.Empty, value);
            if (encode && value is byte[])
                node.Add ("encode", true);
            return context.Raise (
                "pf.hyperlisp.get-string-value." +
                value.GetType ().FullName, node).Value as string;
        }

        /*
         * converts string to object using conversion Active Events
         */
        private static T Convert2Object<T> (object value, ApplicationContext context, T defaultValue = default (T))
        {
            var typeName = context.Raise (
                "pf.hyperlisp.get-type-name." + typeof (T).FullName).Get<string> (context);
            if (typeName == null)
                return defaultValue;
            return context.Raise (
                "pf.hyperlisp.get-object-value." +
                typeName, new Node (string.Empty, value)).Get<T> (context);
        }
    }
}
