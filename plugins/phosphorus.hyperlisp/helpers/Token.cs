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
    /*
     * class used internall to tokenize hyperlisp
     */
    internal class Token
    {
        /*
         * type of token
         */
        public enum TokenType
        {
            CarriageReturn,
            Separator,
            Spacer,
            Name,
            ContentOrType
        }

        /*
         * constructor taking type and value or "content" of token
         */
        public Token (TokenType type, string value)
        {
            if (type == TokenType.Spacer) {
                if (value.Length % 2 != 0)
                    throw new ArgumentException ("number of spaces in front of hyperlisp value was not even");
            }
            Type = type;
            Value = value;
        }

        /*
         * type of token
         */
        public TokenType Type {
            get;
            set;
        }

        /*
         * value or "content" of token, typically ":", "\r\n", an even number of spaces, or any other arbitrary string value, depending
         * upon the Type of token
         */
        public string Value {
            get;
            set;
        }

        /*
         * only valid if type is "Spacer", returns the offset from the root node for the current token
         */
        public int Scope {
            get {
                if (Type != TokenType.Spacer)
                    return 0;
                return Value.Length / 2;
            }
        }
    }
}

