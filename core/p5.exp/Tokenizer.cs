/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using p5.core;

namespace p5.exp
{
    /// <summary>
    ///     Tokenizer for the Expression class.
    /// 
    ///     Responsible for tokenizing expressions, by breaking them up into tokens. Not something you'd normally fiddle
    ///     with yourself, but heavily in use by the p5.lambda expression engine.
    /// </summary>
    public sealed class Tokenizer : IDisposable
    {
        private readonly StringReader _reader;
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.Tokenizer" /> class.
        /// </summary>
        /// <param name="expression">Expression to tokenize</param>
        public Tokenizer (string expression)
        {
            _reader = new StringReader (expression);
        }

        /// <summary>
        ///     Returns all tokens in expression.
        /// 
        ///     Iterates through all tokens in your Expression.
        /// </summary>
        /// <value>The tokens consisting your Expression</value>
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
        void IDisposable.Dispose ()
        {
            Dispose (true);
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

        /*
         * finds next token and returns to caller, returns null if there are no more tokens in expression
         */
        private string GetNextToken (string previousToken)
        {
            // left trimming white spaces from StringReader, and putting first non-white-space character in "buffer"
            var nextChar = _reader.Read ();
            for (; nextChar != -1 && "\r\n \t".IndexOf ((char) nextChar) != -1; nextChar = _reader.Read ())
            { }
            if (nextChar == -1)
                return null;
            var builder = new StringBuilder ();
            builder.Append ((char)nextChar);

            if ("/|&^!()?".IndexOf ((char)nextChar) != -1) {
                return builder.ToString (); // single character token
            } else if (nextChar == '"') {
                return Utilities.ReadSingleLineStringLiteral (_reader); // singleline string literal token
            } else if (nextChar == '@' && _reader.Peek () == '"') {
                _reader.Read (); // skipping opening '"'
                return Utilities.ReadMultiLineStringLiteral (_reader); // multiline string literal token
            }

            // looping until "end of token", which is defined as either "/|&^!()?" or "end of stream"
            for (nextChar = _reader.Peek (); nextChar != -1 && "/|&^!()?".IndexOf ((char) nextChar) == -1; nextChar = _reader.Peek ()) {
                builder.Append ((char) _reader.Read ());
            }

            // returning token back to caller, making sure we trim any white space characters away,
            // to support nicely formatted expressions spanning multiple lines
            return builder.ToString ().TrimEnd (' ', '\r', '\n', '\t');
        }
    }
}
