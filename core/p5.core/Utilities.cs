/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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
    ///     Utility class, contains helpers for common operations
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        ///     Helper to remove all arguments passed into active events after invocation
        /// </summary>
        public class ArgsRemover : IDisposable
        {
            private List<Node> _nodes;
            private Node _args;

            /// <summary>
            ///     Initializes a new instance of the <see cref="p5.core.Utilities+ArgsRemover"/> class
            /// </summary>
            /// <param name="args">Arguments.</param>
            /// <param name="removeValue">If set to <c>true</c> removes value</param>
            /// <param name="onlyEmptyNames">If set to <c>true</c> removes only empty names</param>
            public ArgsRemover (Node args, bool removeValue = false, bool onlyEmptyNames = false)
            {
                if (onlyEmptyNames)
                    _nodes = new List<Node> (args.Children.Where (idx => idx.Name == ""));
                else
                    _nodes = new List<Node> (args.Children);
                if (removeValue)
                    _args = args;
            }

            /// <summary>
            ///     Prevents the removal of value of args node, even if object is initialized with such state
            /// </summary>
            public void StopRemovingArgsValue ()
            {
                _args = null;
            }

            /*
             * Private implementation
             */
            void IDisposable.Dispose ()
            {
                foreach (var idx in _nodes) {
                    if (idx.Parent != null)
                        idx.UnTie();
                }
                if (_args != null)
                    _args.Value = null;
            }
        }

        /// <summary>
        ///     Converts the given value to type T
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="context">Application context Needed since it might potentially have to raise "conversion Active Events" to convert your value</param>
        /// <param name="defaultValue">Default value to return, if no conversion is possible</param>
        /// <param name="encode">If true, then the value will be encoded as base64, if necessary, and value is byte[]</param>
        /// <typeparam name="T">Type to convert your value to</typeparam>
        public static T Convert<T> (
            ApplicationContext context,
            object value,
            T defaultValue = default (T),
            bool encode = false)
        {
            // Checking if value is null
            if (value == null)
                return defaultValue;

            // Checking to see if conversion is even necessary
            if (value is T)
                return (T) value;

            // Trying installed converters from ApplicationContext
            if (typeof(T) == typeof(string)) {

                // Converting from object, to string
                var retVal = Convert2String(value, context, encode);
                if (retVal != null)
                    return (T)(object)retVal;
            } else if (value.GetType() == typeof(string) || typeof(T) == typeof(byte[])) {

                // Converting from string to object
                var retVal = Convert2Object<T>(value, context);
                if (retVal != null && !retVal.Equals(default (T)))
                    return retVal;
            } else if (typeof(T) == typeof(Node)) {

                // Converting to Node somehow, and value is NOT string, creating string out of value first
                return (T)(object)Convert<Node>(context, Convert<string>(context, value), defaultValue as Node).FirstChild.UnTie();
            }

            // Checking if type is IConvertible
            if (value is IConvertible)
                return (T) System.Convert.ChangeType (value, typeof (T), CultureInfo.InvariantCulture);

            // Stuff like for instance Guids don't implement IConvertible, but still return sane values, if we
            // first do ToString on them, for then to cast them to object, for then to cast object to T, if the caller
            // is requesting to have them returned as string
            if (typeof (T) == typeof (string))
                return (T) (object) value.ToString ();

            // Conversion is not possible!
            return defaultValue;
        }

        /// <summary>
        ///     Returns true if string can be converted to an integer
        /// </summary>
        /// <returns><c>true</c> if this instance is a whole, positive, integer number; otherwise, <c>false</c></returns>
        /// <param name="value">String to check</param>
        public static bool IsNumber (string value)
        {
            if (value.Any (idx => "0123456789".IndexOf (idx) == -1)) {
                return false;
            }
            return value.Length > 0;
        }

        /// <summary>
        ///     Reads a single line string literal token from specified text reader
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
                        throw new ArgumentException (
                            string.Format ("Syntax error in hyperlambda, single line string literal contains new line near '{0}'", builder.ToString ()));
                    default:
                        builder.Append ((char) c);
                        break;
                }
            }
            throw new ArgumentException (
                string.Format ("Syntax error in hyperlambda, single line string literal not closed before end of input near '{0}'", builder.ToString ()));
        }

        /// <summary>
        ///     Reads a multi line string literal token from specified text reader
        /// </summary>
        /// <returns>The single line string literal, parsed</returns>
        /// <param name="reader">Reader to read from</param>
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
                        builder.Append ("\r\n"); // Normalizing carriage return
                        break;
                    case '\r':
                        if ((char) reader.Read () != '\n')
                            throw new ArgumentException (
                                string.Format ("Syntax error in hyperlambda, carriage return found but no new line character in multi line string literal near '{0}'", builder.ToString ()));
                        builder.Append ("\r\n");
                        break;
                    default:
                        builder.Append ((char) c);
                        break;
                }
            }
            throw new ArgumentException (
                string.Format ("Syntax error in hyperlambda, multiline string literal not closed before end of input near '{0}'", builder.ToString ()));
        }

        /*
         * appends an escape character to stringbuilder
         */
        private static string AppendEscapeCharacter (StringReader reader)
        {
            var c = reader.Read ();
            switch (c) {
                case -1:
                    throw new ArgumentException ("Syntax error in hyperlambda, end of input found when looking for escape character in single line string literal");
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
                        throw new ArgumentException ("Syntax error in hyperlambda, carriage return found, but no new line character found");
                    return "\r\n";
                case 'x':
                    return HexaCharacter (reader);
                default:
                    throw new ArgumentException (string.Format ("Invalid escape sequence found in hyperlambda string literal; '\\{0}'",
                        (char) c));
            }
        }

        /*
         * Returns a character represented as an octal character representation
         */
        private static string HexaCharacter (StringReader reader)
        {
            var hexNumberString = "";
            for (var idxNo = 0; idxNo < 4; idxNo++) {
                hexNumberString += (char) reader.Read ();
            }
            var hexNumber = System.Convert.ToInt32 (hexNumberString, 16);
            return new string ((char) hexNumber, 1);
        }

        /*
         * Converts value to string using conversion Active Events
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
                        "p5.hyperlambda.get-string-value." +
                        idx.GetType ().FullName, new Node ("", idx)).Value);
                }
                return builder.ToString ();
            }
            Node node = new Node ("", value);
            if (encode && value is byte[])
                node.Add ("encode", true);
            return context.Raise (
                "p5.hyperlambda.get-string-value." +
                value.GetType ().FullName, node).Value as string;
        }

        /*
         * Converts string to object using conversion Active Events
         */
        private static T Convert2Object<T> (object value, ApplicationContext context, T defaultValue = default (T))
        {
            var typeName = context.Raise (
                "p5.hyperlambda.get-type-name." + typeof (T).FullName).Get<string> (context);
            if (typeName == null)
                return defaultValue;
            return context.Raise (
                "p5.hyperlambda.get-object-value." +
                typeName, new Node ("", value)).Get<T> (context);
        }
    }
}
