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
using System.Text;
using System.Linq;
using p5.core;

namespace p5.hyperlambda.helpers
{
    /// <summary>
    ///     Class encapsulating Hyperlambda parsing internals.
    /// </summary>
    public class HyperlambdaParser
    {
        ApplicationContext _context;

        internal HyperlambdaParser (ApplicationContext context)
        {
            _context = context;
        }

        internal void Parse (StreamReader reader, Node curRoot)
        {
            var level = 0;
            Node curNode = curRoot;
            while (!reader.EndOfStream) {

                // Retrieving next token.
                bool eol;
                int spaces;
                string token = GetNameToken (reader, out spaces, out eol);
                if (token == null)
                    break; // We're done!

                if (spaces % 2 != 0) {

                    // Oops, severe syntax error.
                    throw new Exception ("Syntax error in Hyperlambda, even spacing close to '" + token + "'.");

                } else if (spaces == level) {

                    // Adding child node.
                    curNode = curRoot.Add (token ?? "").LastChild;

                } else if (spaces == level + 2) {

                    // New scope.
                    curRoot = curNode;
                    curNode = curRoot.Add (token ?? "").LastChild;
                    level = spaces;

                } else {

                    if (spaces > level + 2)
                        throw new Exception ("Syntax error in Hyperlambda, too many consecutive spaces close to '" + token + "'.");

                    // Going up.
                    while (level > spaces) {
                        level -= 2;
                        curRoot = curRoot.Parent;
                    }
                    curNode = curRoot.Add (token ?? "").LastChild;
                }

                // Checking if we're at EOF.
                if (reader.EndOfStream)
                    break;

                // Checking if name token was not followed by CR/LF, at which point we have a value/type token following it.
                if (!eol) {

                    // Value or a possibly type declaration.
                    var valueOrType = GetValueTypeToken (reader, out eol) ?? "";
                    if (eol) {

                        // Simple value.
                        curNode.Value = valueOrType;

                    } else {

                        // Type declaration, conversion necessary.
                        var value = GetValueTypeToken (reader, out eol) ?? "";
                        if (string.IsNullOrEmpty (valueOrType) || valueOrType == "string") {

                            // No need to convert value.
                            curNode.Value = value;

                        } else {

                            // Conversion is necessary.
                            curNode.Value = _context.RaiseEvent (
                                ".p5.hyperlambda.get-object-value." + (valueOrType == "node" ? "abs.node" : valueOrType),
                                new Node ("", value ?? "", new Node [] { new Node ("decode", true) })).Value;
                        }
                    }
                }
            }
        }

        /*
         * Retrieves the next name token from stream.
         */
        string GetNameToken (StreamReader reader, out int spaces, out bool eol)
        {
            var token = new StringBuilder ();
            spaces = 0;
            while (!reader.EndOfStream) {
                var curChar = reader.Read ();
                if (token.Length == 0 && curChar == '@' && reader.Peek () == '"') {

                    // Multiline string token.
                    reader.Read (); // Skipping '"'.
                    eol = true;
                    var retVal =  ReadMultiLineString (reader);
                    EatWhite (reader);
                    return retVal;

                } else if (token.Length == 0 && curChar == '"') {

                    // Singleline line token.
                    eol = true;
                    var retVal = ReadQuotedString (reader);
                    EatWhite (reader);
                    return retVal;

                } else if (token.Length == 0 && curChar == '/' && reader.Peek () == '*') {

                    // Multiline comment, simply ignoring until end of comment.
                    reader.Read (); // Skipping opening '*'.
                    EatMultilineComment (reader);
                    EatWhite (reader);
                    spaces = 0;

                } else if (token.Length == 0 && curChar == '/' && reader.Peek () == '/') {

                    // Single line comment, simply ignoring the rest of our line.
                    EatLine (reader);
                    spaces = 0;

                } else if (token.Length == 0 && curChar == ' ') {

                    // Adding to spaces.
                    spaces += 1;

                } else if (curChar == ':') {

                    // Done fetching token, returning to caller, signaling we're not at end of line.
                    eol = false;
                    return token.ToString ().TrimEnd ();

                } else if (curChar == '\n' || curChar == '\r') {

                    // Carriage return, either we have found a token, or an empty line.
                    if (curChar == '\r')
                        reader.Read (); // Ignoring '\n'.

                    if (token.Length == 0) {

                        // Empty line.
                        spaces = 0;
                        continue;

                    } else {

                        // Done with reading token, and line.
                        eol = true;
                        return token.ToString ().TrimEnd ();
                    }

                } else {

                    // Sanity checking level.
                    token.Append ((char)curChar);
                }
            }
            eol = true;
            if (token.Length == 0)
                return null;
            return token.ToString ().TrimEnd ();
        }

        /*
         * Retrieves the next type or value token from stream.
         */
        string GetValueTypeToken (StreamReader reader, out bool eol)
        {
            var token = new StringBuilder ();
            while (!reader.EndOfStream) {
                var curChar = reader.Read ();
                if (token.Length == 0 && curChar == '@' && reader.Peek () == '"') {

                    // Multiline string token.
                    reader.Read (); // Skipping '"'.
                    eol = true;
                    var retVal = ReadMultiLineString (reader);
                    EatWhite (reader);
                    return retVal;

                } else if (token.Length == 0 && curChar == '"') {

                    // Singleline line token.
                    eol = true;
                    var retVal = ReadQuotedString (reader);
                    EatWhite (reader);
                    return retVal;

                } else if (token.Length == 0 && curChar == ' ') {

                    // Ignoring initial whitespaces.
                    continue;

                } else if (curChar == ':') {

                    // Done fetching token, returning to caller, signaling we're not at end of line.
                    eol = false;
                    return token.ToString ().TrimEnd ();

                } else if (curChar == '\n' || curChar == '\r') {

                    // Carriage return, either we have found a token, or an empty line.
                    if (curChar == '\r')
                        reader.Read (); // Ignoring '\n'.

                    // Done with reading token, and line.
                    eol = true;
                    return token.ToString ().TrimEnd ();

                } else {

                    // Sanity checking level.
                    token.Append ((char)curChar);
                }
            }
            eol = true;
            return token.ToString ().TrimEnd ();
        }

        /*
         * Eats the rest of the line, and discards it.
         */
        private void EatLine (StreamReader reader)
        {
            while (reader.Peek () != '\n' && !reader.EndOfStream) {
                reader.Read ();
            }
            if (!reader.EndOfStream && reader.Peek () == '\n')
                reader.Read ();
        }

        /*
         * Eats the rest of the line, and discards it, assuming it contains whitespaces.
         */
        private void EatWhite (StreamReader reader)
        {
            while (reader.Peek () != '\n' && !reader.EndOfStream) {
                var cur = reader.Read ();
                if (cur != ' ' && cur != '\t' && cur != '\r' && cur != '\n')
                    throw new Exception ("Syntax error at end of line in Hyperlambda");
            }
            if (!reader.EndOfStream && reader.Peek () == '\n')
                reader.Read ();
        }

        /*
         * Eats and discards a multiline comment.
         */
        private void EatMultilineComment (StreamReader reader)
        {
            // Eating comment, with basic sanity check.
            var curChar = reader.Read ();
            while (true) {
                if (reader.EndOfStream)
                    throw new Exception ("Multiline comment not closed");
                if (curChar == '*' && reader.Peek () == '/') {
                    curChar = reader.Read ();
                    break;
                }
                curChar = reader.Read ();
            }
        }

        /*
         * Reads a multiline string literal, and returns to caller.
         */
        string ReadMultiLineString (StreamReader reader)
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
                        throw new Exception (string.Format ("Unexpected CR found without any matching LF near '{0}'", builder));
                    builder.Append ("\r\n");
                    break;
                default:
                    builder.Append ((char)c);
                    break;
                }
            }
            throw new Exception (string.Format ("String literal not closed before end of input near '{0}'", builder));
        }

        /*
         * Reads a quoted string, and returns to caller.
         */
        string ReadQuotedString (StreamReader reader)
        {
            var builder = new StringBuilder ();
            for (var c = reader.Read (); c != -1; c = reader.Read ()) {
                switch (c) {
                case '"':
                    return builder.ToString ();
                case '\\':
                    builder.Append (GetEscapeCharacter (reader));
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

        /*
         * Returns escape character.
         */
        string GetEscapeCharacter (StreamReader reader)
        {
            switch (reader.Read ()) {
            case -1:
                throw new Exception ("End of input found when looking for escape character in single line string literal");
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
                    throw new Exception ("CR found, but no matching LF found");
                return "\r\n";
            case 'x':
                return HexaCharacter (reader);
            default:
                throw new Exception ("Invalid escape sequence found in string literal");
            }
        }

        /*
         * Returns hexa encoded character.
         */
        string HexaCharacter (StreamReader reader)
        {
            var hexNumberString = "";
            for (var idxNo = 0; idxNo < 4; idxNo++)
                hexNumberString += (char)reader.Read ();
            var integerNo = Convert.ToInt32 (hexNumberString, 16);
            return Encoding.UTF8.GetString (BitConverter.GetBytes (integerNo).Reverse ().ToArray ());
        }
    }
}
