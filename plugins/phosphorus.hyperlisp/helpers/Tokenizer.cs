/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.hyperlisp
{
    /// <summary>
    /// class responsible for tokenizing hyperlisp
    /// </summary>
    public class Tokenizer : IDisposable
    {
        private StringReader _reader;
        private bool _disposed;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.hyperlisp.Tokenizer"/> class
        /// </summary>
        /// <param name="hyperlisp">hyperlisp to tokenize</param>
        public Tokenizer (string hyperlisp)
        {
            _reader = new StringReader (hyperlisp);
        }

        /// <summary>
        /// returns all <see cref="phosphorus.hyperlisp.Token"/>s for given hyperlisp
        /// </summary>
        /// <value>tokens from hyperlisp</value>
        public IEnumerable<Token> Tokens {
            get {
                Token previousToken = new Token (Token.TokenType.CarriageReturn, "\r\n"); // we start out with a CR/LF token
                while (true) {
                    Token token = NextToken (previousToken);
                    if (token == null)
                        yield break;
                    previousToken = token;
                    yield return token;
                }
            }
        }

        /// <summary>
        /// disposing the tokenizer
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> disposing will occur, otherwise false</param>
        protected virtual void Dispose (bool disposing)
        {
            if (!_disposed && disposing) {
                _disposed = true;
                _reader.Dispose ();
            }
        }

        /*
         * retrieves next hyperlisp token from text reader
         */
        private Token NextToken (Token previousToken)
        {
            if (_reader.Peek () == ':' && previousToken.Type == Token.TokenType.Spacer) {
                // empty name
                return new Token (Token.TokenType.Name, string.Empty);
            }
            if ((_reader.Peek () == '\r' || _reader.Peek () == '\n') && previousToken.Type == Token.TokenType.Separator) {
                return new Token (Token.TokenType.TypeOrContent, string.Empty);
            }
            int nextChar = _reader.Read ();
            if (nextChar == -1)
                return null;
            if (nextChar == ':')
                return new Token (Token.TokenType.Separator, ":");
            if (nextChar == ' ') {
                if (previousToken.Type == Token.TokenType.CarriageReturn) {
                    return NextSpaceToken ();
                } else {
                    // whitespace only carry semantics as first token in each line in hyperlisp content
                    // therefor we must "left-trim" the reader, before retrieving next token
                    TrimReader ();
                    return NextToken (previousToken);
                }
            }
            if (nextChar == '\r') {
                return NextCRLFToken ();
            } else if (nextChar == '\n') {
                return new Token (Token.TokenType.CarriageReturn, "\r\n"); // normalizing carriage returns
            } else {
                StringBuilder builder = new StringBuilder ();
                builder.Append ((char)nextChar);
                return NextDefaultToken (builder, previousToken);
            }
        }

        /*
         * trims reader until reader head is at first non-space character
         */
        private void TrimReader ()
        {
            int nextChar = _reader.Peek ();
            while (nextChar == ' ') {
                _reader.Read ();
                nextChar = _reader.Peek ();
            }
        }

        /*
         * reads and validates next space token ("  ") from text reader
         */
        private Token NextSpaceToken ()
        {
            string buffer = " ";
            int nextChar = _reader.Peek ();
            while (nextChar == ' ') {
                buffer += (char)_reader.Read ();
                nextChar = _reader.Peek ();
            }
            return new Token (Token.TokenType.Spacer, buffer);
        }

        /*
         * reads and validates next carriage return / line feed token ("\r\n" or "\n")
         */
        private Token NextCRLFToken ()
        {
            int nextChar = _reader.Read ();
            if (nextChar == -1)
                throw new ArgumentException ("syntax error in hyperlisp, carriage return character found, but no new line character found at end of file");
            if (nextChar != '\n')
                throw new ArgumentException ("syntax error in hyperlisp, carriage return character found, but no new line character found");
            return new Token (Token.TokenType.CarriageReturn, "\r\n");
        }

        /*
         * reads next "default token" from text reader, can be string, multiline string or simply legal unescaped characters
         */
        private Token NextDefaultToken (StringBuilder builder, Token previousToken)
        {
            if (builder [0] == '"') {
                return new Token (GetTokenType (previousToken), ReadSingleLineStringLiteral ());
            } else if (builder [0] == '@') {
                if ((char)_reader.Peek () == '"') {
                    _reader.Read (); // skipping '"' part
                    return new Token (GetTokenType (previousToken), ReadMultiLineStringLiteral ());
                }
            }
            int nextChar = _reader.Peek ();
            while (nextChar != -1 && "\r\n:".IndexOf ((char)nextChar) == -1) {
                builder.Append ((char)nextChar);
                _reader.Read ();
                nextChar = _reader.Peek ();
            }
            return new Token (GetTokenType (previousToken), builder.ToString ().Trim ()); // whitespace has no semantics, and are not part of tokens, except if within string literals, or before name token type
        }

        /*
         * returns the curent token's type according to the previous token type
         */
        private Token.TokenType GetTokenType (Token previousToken)
        {
            if (previousToken != null && previousToken.Type == Token.TokenType.Separator)
                return Token.TokenType.TypeOrContent;
            return Token.TokenType.Name;
        }

        /*
         * reads a single line string literal token from teext reader
         */
        private string ReadSingleLineStringLiteral ()
        {
            var builder = new StringBuilder ();
            for (var c = _reader.Read (); c != -1; c = _reader.Read ()) {
                switch (c) {
                case '"':
                    return builder.ToString ();
                case '\\':
                    AppendEscapeCharacter (builder);
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

        /*
         * appends an escape character to stringbuilder
         */
        private void AppendEscapeCharacter (StringBuilder builder)
        {
            var c = _reader.Read(); 
            switch (c) {
            case -1:
                throw new ArgumentException ("syntax error in hyperlisp, end of input found when looking for escape character in single line string literal");
            case '"':
            case '\\':
                builder.Append((char)c);
                break;
            case 'n':
                builder.Append("\r\n"); // normalizing carriage return
                break;
            case 'r':
                // '\r' must be followed by '\n'
                if ((char)_reader.Read () != '\\' || (char)_reader.Read () != 'n')
                    throw new ArgumentException ("syntax error in hyperlisp, carriage return found, but no new line character found");
                builder.Append("\r\n");
                break;
            default:
                throw new ArgumentException (string.Format ("invalid escape sequence in hyperlisp; '\\{0}'", (char)c));
            }
        }

        /*
         * reads a multiline string literal token from text reader
         */
        private string ReadMultiLineStringLiteral ()
        {
            var builder = new StringBuilder ();
            for (var c = _reader.Read (); c != -1; c = _reader.Read ()) {
                switch (c) {
                case '"':
                    if ((char)_reader.Peek () == '"') {
                        builder.Append ((char)_reader.Read ());
                    } else {
                        return builder.ToString ();
                    }
                    break;
                case '\n':
                    builder.Append ("\r\n"); // normalizing carriage return
                    break;
                case '\r':
                    if ((char)_reader.Read () != '\n')
                        throw new ArgumentException ("syntax error in hyperlisp, carriage return found but no new line character in multi line string literal");
                    builder.Append ("\r\n");
                    break;
                default:
                    builder.Append ((char)c);
                    break;
                }
            }
            throw new ArgumentException ("syntax error in hyperlisp, multi line string literal not closed before end of input");
        }

        /*
         * private implementation of IDisposable interface
         */
        void IDisposable.Dispose ()
        {
            Dispose (true);
        }
    }
}

