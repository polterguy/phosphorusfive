
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions
{
    /// <summary>
    /// tokenizer for expressions
    /// </summary>
    public class Tokenizer : IDisposable
    {
        private StringReader _reader;
        private bool _disposed;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.lambda.Tokenizer"/> class
        /// </summary>
        /// <param name="expression">expression to tokenize</param>
        public Tokenizer (string expression)
        {
            _reader = new StringReader (expression.Substring (1));
        }

        /// <summary>
        /// responsible for tokenizing the expression given during initialization, and return them
        /// back to caller
        /// </summary>
        /// <value>The evaluate.</value>
        public IEnumerable<string> Tokens {
            get {
                while (true) {
                    string token = GetNextToken ();
                    if (token == null)
                        yield break;
                    yield return token;
                }
            }
        }

        /*
         * finds next token and returns to caller, returns null if there are no more tokens in expression
         */
        private string GetNextToken ()
        {
            int nextChar = _reader.Read ();
            if (nextChar == -1)
                return null; // end of stream

            StringBuilder builder = new StringBuilder ();
            builder.Append ((char)nextChar);
            if ("/|&^!()=?+-[],%:".IndexOf ((char)nextChar) != -1) {
                return builder.ToString (); // single character token
            }

            if (nextChar == '"')
                return Utilities.ReadSingleLineStringLiteral (_reader);
            if (nextChar == '@' && _reader.Peek () == '"') {
                _reader.Read (); // skipping opening '"'
                return Utilities.ReadMultiLineStringLiteral (_reader);
            }

            nextChar = _reader.Peek ();
            while (nextChar != -1 && "/,]:".IndexOf ((char)nextChar) == -1) {
                builder.Append ((char)_reader.Read ());
                nextChar = _reader.Peek ();
            }
            return builder.ToString ();
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
         * private implementation of IDisposable interface
         */
        void IDisposable.Dispose ()
        {
            Dispose (true);
        }
    }
}
