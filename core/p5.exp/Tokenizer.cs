/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using p5.core;

namespace p5.exp
{
    /// <summary>
    ///     Tokenizer for the Expression class
    /// </summary>
    public sealed class Tokenizer : IDisposable
    {
        private readonly StringReader _reader;
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.Tokenizer" /> class
        /// </summary>
        /// <param name="expression">Expression to tokenize</param>
        public Tokenizer (string expression)
        {
            _reader = new StringReader (expression);
        }

        /// <summary>
        ///     Returns all tokens in expression
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
         * Private implementation of IDisposable interface
         */
        void IDisposable.Dispose ()
        {
            Dispose (true);
        }
        
        /*
         * Disposes the tokenizer
         */
        private void Dispose (bool disposing)
        {
            if (!_disposed && disposing) {
                _disposed = true;
                _reader.Dispose ();
            }
        }

        /*
         * Finds next token and returns to caller, returns null if there are no more tokens in expression
         */
        private string GetNextToken (string previousToken)
        {
            // Left trimming white spaces from StringReader, and putting first non-white-space character in "buffer"
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
                return Utilities.ReadSingleLineStringLiteral (_reader); // Singleline string literal token
            } else if (nextChar == '@' && _reader.Peek () == '"') {
                _reader.Read (); // skipping opening '"'
                return Utilities.ReadMultiLineStringLiteral (_reader); // Multiline string literal token
            }

            // Looping until "end of token", which is defined as either "/|&^!()?" or "end of stream"
            bool lastEscaped = nextChar == '\\';
            for (nextChar = _reader.Peek (); 
                nextChar != -1 && ("/|&^!()?".IndexOf ((char) nextChar) == -1 || lastEscaped); 
                nextChar = _reader.Peek ()) {
                builder.Append ((char) _reader.Read ());
                if (lastEscaped)
                    lastEscaped = false;
                else if (nextChar == '\\')
                    lastEscaped = true;
            }

            // Returning token back to caller, making sure we trim any white space characters away,
            // to support nicely formatted expressions spanning multiple lines
            return builder.ToString ().TrimEnd (' ', '\r', '\n', '\t');
        }
    }
}
