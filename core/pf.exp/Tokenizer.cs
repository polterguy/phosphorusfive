/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pf.core;

namespace phosphorus.expressions
{
    /// <summary>
    ///     Tokenizer for the Expression class.
    /// 
    ///     Responsible for tokenizing expressions, by breaking them up into tokens. Not something you'd normally fiddle
    ///     with yourself.
    /// </summary>
    public sealed class Tokenizer : IDisposable
    {
        private readonly StringReader _reader;
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.Tokenizer" /> class.
        /// </summary>
        /// <param name="expression">Expression to tokenize.</param>
        public Tokenizer (string expression)
        {
            // removing first "@" character, since it's not a part of our iterators
            _reader = new StringReader (expression.Substring (1));
        }

        /// <summary>
        ///     Returns all tokens in expression.
        /// 
        ///     Iterates through all tokens in your Expression.
        /// </summary>
        /// <value>The tokens consisting your Expression.</value>
        public IEnumerable<string> Tokens
        {
            get
            {
                string previousToken = null;
                while (true) {
                    var token = GetNextToken (previousToken);
                    if (token == null)
                        yield break;
                    yield return token;
                    previousToken = token;
                }
            }
        }

        /*
         * private implementation of IDisposable interface
         */
        void IDisposable.Dispose () { Dispose (true); }

        /// \todo refactor, too complex
        /*
         * finds next token and returns to caller, returns null if there are no more tokens in expression
         */
        private string GetNextToken (string previousToken)
        {
            var nextChar = _reader.Read ();

            // left trimming white spaces
            while (nextChar != -1 && "\r\n \t".IndexOf ((char) nextChar) != -1) {
                if (nextChar == -1)
                    return null;
                nextChar = _reader.Read ();
            }

            // buffer to keep our token in
            var builder = new StringBuilder ();
            builder.Append ((char) nextChar);

            // returning token immediately, if it's a single character token,
            // or "@" character as first token, in stream of tokens
            if ("/|&^!()?".IndexOf ((char) nextChar) != -1 ||
                ("@".IndexOf ((char) nextChar) != -1 && previousToken == null && _reader.Peek () != '"')) {
                return builder.ToString (); // single character token, or "@" as first token, and not multi line string
            }

            // checking to see if this token is a "string literal", either single line, or multiline
            if (nextChar == '"')
                return Utilities.ReadSingleLineStringLiteral (_reader);
            if (nextChar == '@' && _reader.Peek () == '"') {
                _reader.Read (); // skipping opening '"'
                return Utilities.ReadMultiLineStringLiteral (_reader);
            }

            // looping until "end of token", which is defined as either "/" or "end of stream"
            nextChar = _reader.Peek ();
            while (nextChar != -1 && "/|&^!()?".IndexOf ((char) nextChar) == -1) {
                builder.Append ((char) _reader.Read ());
                nextChar = _reader.Peek ();
            }

            // returning token back to caller, making sure we trim any white space characters away,
            // to support nicely formatted expressions spanning multiple lines
            return builder.ToString ().TrimEnd (' ', '\r', '\n', '\t');
        }

        /*
         * disposes the tokenizer
         */
        private void Dispose (bool disposing)
        {
            if (!_disposed && disposing) {
                _disposed = true;
                _reader.Dispose ();
            }
        }
    }
}
