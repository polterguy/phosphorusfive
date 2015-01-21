
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
    }
}
