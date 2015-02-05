
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace phosphorus.core
{
    /// <summary>
    /// utilities class, contains helpers for common operations
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// converts the given value to type T
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <param name="context">application context</param>
        /// <param name="defaultValue">default value to return if no conversion is possible</param>
        /// <typeparam name="T">type to convert value to</typeparam>
        public static T Convert <T> (
            object value, 
            ApplicationContext context, 
            T defaultValue = default (T))
        {
            // no possible conversion exists
            if (value == null)
                return defaultValue;

            // checking to see if conversion is even necessary
            if (value is T)
                return (T)value;

            // trying installed converters from ApplicationContext
            if (typeof(T) == typeof(string)) {
                var retVal = Convert2String (value, context);
                if (retVal != null)
                    return (T)(object)retVal;
            } else if (value.GetType () == typeof(string)) {
                var retVal = Convert2Object<T> (value as string, context);
                if (retVal != null && !retVal.Equals (default (T)))
                    return retVal;
            }

            // checking if type is IConvertible
            if (value is IConvertible)
                return (T)Convert.ChangeType (value, typeof(T), System.Globalization.CultureInfo.InvariantCulture);

            // stuff like for instance Guids don't implement IConvertible, but still return sane values, if we
            // first do ToString on them, for then to cast them to object, for then to cast object to T, if the caller
            // is requesting to have them returned as string
            if (typeof(T) == typeof(string))
                return (T)(object)value.ToString ();
            return defaultValue;
        }

        /// <summary>
        /// returns true if string can be converted to an integer
        /// </summary>
        /// <returns><c>true</c> if this instance is a number; otherwise, <c>false</c></returns>
        /// <param name="value">string to check</param>
        public static bool IsNumber (string value)
        {
            foreach (char idx in value) {
                if ("0123456789".IndexOf (idx) == -1)
                    return false;
            }
            return value.Length > 0;
        }

        /// <summary>
        /// reads a single line string literal token from text reader
        /// </summary>
        /// <returns>the single line string literal</returns>
        /// <param name="reader">reader to read from</param>
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
                    builder.Append ((char)c);
                    break;
                }
            }
            throw new ArgumentException ("syntax error in hyperlisp, single line string literal not closed before end of input");
        }

        /// <summary>
        /// reads a multiline string literal token from text reader
        /// </summary>
        /// <returns>the multi line string literal</returns>
        /// <param name="reader">reader to read from</param>
        public static string ReadMultiLineStringLiteral (StringReader reader)
        {
            var builder = new StringBuilder ();
            for (var c = reader.Read (); c != -1; c = reader.Read ()) {
                switch (c) {
                case '"':
                    if ((char)reader.Peek () == '"') {
                        builder.Append ((char)reader.Read ());
                    } else {
                        return builder.ToString ();
                    }
                    break;
                case '\n':
                    builder.Append ("\r\n"); // normalizing carriage return
                    break;
                case '\r':
                    if ((char)reader.Read () != '\n')
                        throw new ArgumentException ("syntax error in hyperlisp, carriage return found but no new line character in multi line string literal");
                    builder.Append ("\r\n");
                    break;
                default:
                    builder.Append ((char)c);
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
            var c = reader.Read(); 
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
                if ((char)reader.Read () != '\\' || (char)reader.Read () != 'n')
                    throw new ArgumentException ("syntax error in hyperlisp, carriage return found, but no new line character found");
                return "\r\n";
            case 'x':
                return HexaCharacter (reader);
            default:
                throw new ArgumentException (string.Format ("invalid escape sequence found in hyperlisp string literal; '\\{0}'", 
                    (char)c));
            }
        }

        /*
         * returns a character represented as an octal character representation
         */
        private static string HexaCharacter (StringReader reader)
        {
            string hexNumberString = string.Empty;
            for (int idxNo = 0; idxNo < 4; idxNo++) {
                hexNumberString += (char)reader.Read ();
            }
            int hexNumber = Convert.ToInt32 (hexNumberString, 16);
            return new string ((char)hexNumber, 1);
        }
        
        /*
         * converts value to string using conversion Active Events
         */
        private static string Convert2String (object value, ApplicationContext context)
        {
            return context.Raise (
                "pf.hyperlist.get-string-value." + 
                value.GetType ().FullName, new Node (string.Empty, value)).Get<string> (context);
        }

        /*
         * converts string to object using conversion Active Events
         */
        private static T Convert2Object<T> (string value, ApplicationContext context)
        {
            var typeName = context.Raise (
                "pf.hyperlist.get-type-name." + typeof(T).FullName).Get<string> (context);
            return context.Raise (
                "pf.hyperlist.get-object-value." + 
                typeName, new Node (string.Empty, value)).Get<T> (context);
        }
    }
}
