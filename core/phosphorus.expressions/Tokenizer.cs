/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions
{
    public sealed class Tokenizer : IDisposable
    {
        private readonly StringReader _reader;
        private bool _disposed;

        public Tokenizer (string expression)
        {
            // removing first "@" character, since it's not a part of our iterators
            _reader = new StringReader (expression.Substring (1));
        }

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

        void IDisposable.Dispose () { Dispose (true); }

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

        private void Dispose (bool disposing)
        {
            if (disposing && _reader != null) {
                _reader.Dispose ();
                _reader = null;
            }
        }
    }
}
