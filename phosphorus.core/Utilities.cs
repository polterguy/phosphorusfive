/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
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
        /// reads multiline string literal from code, incrementing index to position of character after end of multiline text literal
        /// </summary>
        /// <returns>the multiline string token</returns>
        /// <param name="code">code to read string from</param>
        /// <param name="index">index of where to start, expected to be at the "@" position of the beginning of the multiline literal</param>
        private static string GetMultilineStringToken (string code, ref int index)
        {
            StringBuilder builder = new StringBuilder ();
            index += 2;
            while (index < code.Length) {
                char idxChar = code [index];
                if (builder.Length > 0 && idxChar != '"' && (builder.Length - builder.ToString ().TrimEnd (new char[] { '"' }).Length) % 2 == 1)
                    break;
                if (idxChar == '\n') {
                    builder.Append ("\r\n"); // normalizing carriage returns
                } else if (idxChar == '\r') {
                    builder.Append ("\r\n"); // normalizing carriage returns
                    index += 1;
                } else {
                    builder.Append (idxChar.ToString ());
                }
                index += 1;
            }
            return builder.ToString ().Substring (0, builder.Length - 1).Replace (@"""""", @"""");
        }

        /// <summary>
        /// reads string literal from code, and increments index to the position after the end of the string
        /// </summary>
        /// <returns>the string token</returns>
        /// <param name="code">code to read string from</param>
        /// <param name="index">index of where to sttart, expected to be at the opening " quote of the string</param>
        public static string GetStringToken (string code, ref int index)
        {
            if (code[index] == '@')
                return GetMultilineStringToken (code, ref index);
            StringBuilder builder = new StringBuilder ();
            index += 1;
            bool finished = false;
            while (index < code.Length) {
                char idxChar = code [index];
                if (idxChar == '"' && (builder.Length - builder.ToString ().TrimEnd (new char[] { '\\' }).Length) % 2 == 0) {
                    index ++;
                    finished = true;
                    break;
                }
                else if (idxChar == '\r' || idxChar == '\n')
                    throw new ArgumentException ("unfinished string literal in; " + code);
                else if (idxChar == '\\') {
                    char tmpChar = code [index + 1];
                    switch (tmpChar) {
                    case 'r':
                        index += 1;
                        idxChar = '\r';
                        break;
                    case 'n':
                        index += 1;
                        idxChar = '\n';
                        break;
                    case 't':
                        index += 1;
                        idxChar = '\t';
                        break;
                    case '"':
                        if ((builder.Length - builder.ToString ().TrimEnd (new char[] { '\\' }).Length) % 2 == 0) {
                            index += 1;
                            idxChar = '"';
                        }
                        break;
                    case '\\':
                        break;
                    default:
                        throw new ArgumentException ("unknown escaping character in; " + code);
                    }
                }
                builder.Append (idxChar.ToString ());
                index += 1;
            }
            if (!finished)
                throw new ArgumentException ("unfinished string literal in; " + code);
            return builder.ToString ().Replace (@"\\", @"\");
        }
    }
}

