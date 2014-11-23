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
        private TextReader _reader;
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
                Token previousToken = null;
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
            string buffer = string.Empty;
            if (_reader.Peek () == ':' && (previousToken == null || previousToken.Type == Token.TokenType.Spacer)) {
                // empty name
                return new Token (Token.TokenType.Name, string.Empty);
            }
            if ((_reader.Peek () == '\r' || _reader.Peek () == '\n') && (previousToken != null && previousToken.Type == Token.TokenType.Separator)) {
                return new Token (Token.TokenType.TypeOrContent, string.Empty);
            }
            int nextChar = _reader.Read ();
            if (nextChar == -1)
                return null;
            buffer += (char)nextChar;
            if (buffer == ":")
                return new Token (Token.TokenType.Separator, ":");
            if (buffer == " ") {
                if (previousToken.Type == Token.TokenType.CarriageReturn) {
                    return NextSpaceToken ();
                } else {
                    // whitespace only carry semantics as first token in each line in hyperlisp content
                    // therefor we must "left-trim" the reader, before retrieving next token
                    TrimReader ();
                    return NextToken (previousToken);
                }
            } else if (buffer == "\r") {
                return NextCRLFToken ();
            } else if (buffer == "\n") {
                return new Token (Token.TokenType.CarriageReturn, "\r\n"); // normalizing carriage returns
            } else {
                return NextDefaultToken (buffer, previousToken);
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
        private Token NextDefaultToken (string buffer, Token previousToken)
        {
            if (buffer == @"""") {
                return new Token (
                    previousToken != null && previousToken.Type == Token.TokenType.Separator ? 
                        Token.TokenType.TypeOrContent : 
                        Token.TokenType.Name, 
                    ReadSingleLineStringLiteral ());
            } else if (buffer == "@") {
                int nextMultilineChar = _reader.Peek ();
                if (nextMultilineChar != -1) {
                    if (nextMultilineChar == '"') {
                        _reader.Read (); // skipping '"' part
                        return new Token (
                            previousToken != null && previousToken.Type == Token.TokenType.Separator ? 
                                Token.TokenType.TypeOrContent : 
                                Token.TokenType.Name, 
                            ReadMultiLineStringLiteral ());
                    }
                }
            }
            int nextChar = _reader.Peek ();
            while (nextChar != -1 && (char)nextChar != '\r' && (char)nextChar != '\n' && (char)nextChar != ':') {
                buffer += (char)nextChar;
                _reader.Read ();
                nextChar = _reader.Peek ();
            }
            return new Token (
                previousToken != null && previousToken.Type == Token.TokenType.Separator ? 
                    Token.TokenType.TypeOrContent : 
                    Token.TokenType.Name, 
                buffer.Trim ()); // whitespace has no semantics, and are not part of tokens, except if within string literals, or before name token type
        }

        /*
         * reads a single line string literal token from teext reader
         */
        private string ReadSingleLineStringLiteral ()
        {
            string buffer = string.Empty;
            int nextChar = _reader.Read ();
            while (nextChar != -1) {
                buffer += (char)nextChar;
                nextChar = _reader.Read ();
                if (nextChar == '"' && 
                    (buffer.Length == 0 || 
                         buffer [buffer.Length - 1] != '\\' || 
                         (buffer.Length - buffer.TrimEnd ('\\').Length) % 2 == 0)) {
                    buffer += (char)nextChar;
                    break;
                }
            }
            if (buffer [buffer.Length - 1] != '"')
                throw new ArgumentException ("unclosed string literal in hyperlisp file");
            return buffer.Substring (0, buffer.Length - 1)
                .Replace ("\n", "\r\n") // normalizing carriage returns
                .Replace ("\r\r\n", "\r\n")
                .Replace ("\\\"", "\"")
                .Replace ("\\\\", "\\");
        }

        /*
         * reads a multiline string literal token from text reader
         */
        private string ReadMultiLineStringLiteral ()
        {
            string buffer = string.Empty;
            int nextChar = _reader.Read ();
            while (nextChar != -1) {
                buffer += (char)nextChar;
                nextChar = _reader.Peek ();
                if (nextChar != '"' && (buffer.Length - buffer.TrimEnd ('"').Length) % 2 == 1)
                    break;
                nextChar = _reader.Read ();
            }
            if (buffer.Length == 0 || buffer [buffer.Length - 1] != '"')
                throw new ArgumentException ("unclosed multiline string literal in hyperlisp close to end of hyperlisp");
            return buffer.Substring (0, buffer.Length - 1)
                .Replace (@"""""", @"""")
                .Replace ("\n", "\r\n") // normalizing carriage returns
                .Replace ("\r\r\n", "\r\n");
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

