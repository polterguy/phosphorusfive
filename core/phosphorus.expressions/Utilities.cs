/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace phosphorus.expressions
{
    public static class Utilities
    {
        public static bool IsNumber (string value)
        {
            if (value.Any (idx => "0123456789".IndexOf (idx) == -1)) {
                return false;
            }
            return value.Length > 0;
        }

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

        private static string HexaCharacter (StringReader reader)
        {
            var hexNumberString = string.Empty;
            for (var idxNo = 0; idxNo < 4; idxNo++) {
                hexNumberString += (char) reader.Read ();
            }
            var hexNumber = System.Convert.ToInt32 (hexNumberString, 16);
            return new string ((char) hexNumber, 1);
        }
    }
}
