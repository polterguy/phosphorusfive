/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using p5.exp.exceptions;

namespace p5.hyperlisp.helpers
{
    /*
     * Class used internally to tokenize Hyperlisp
     */
    internal class Token
    {
        /*
         * Type of token
         */
        internal enum TokenType
        {
            /*
             * "\r\n" token
             */
            CarriageReturn,

            /*
             * ":" token
             */
            Separator,

            /*
             * "  " [two spaces] token
             */
            Spacer,

            /*
             * Name of node token
             */
            Name,

            /*
             * Type, or content token.
             * Whether or not it's a type or a content token, is defined if another TypeOrContent token follows it.
             * If another ContentOrType token follows it, with a Separator token between them, but no CarriageReturn
             * token between them, the token is a "type token", otherwise it's a "content token" type
             */
            TypeOrContent
        }

        /*
         * Constructor taking type and value or "content" of token
         */
        internal Token (TokenType type, string value)
        {
            if (type == TokenType.Spacer) {
                if (value.Length % 2 != 0)
                    throw new ApplicationException ("Odd number of spaces in front of Hyperlisp name");
            }
            Type = type;
            Value = value;
        }

        /*
         * Type of token
         */
        internal TokenType Type { get; private set; }

        /*
         * Value or "content" of token, typically ":", "\r\n", an even number of spaces, or any other arbitrary 
         * string value, depending upon the Type of token
         */
        internal string Value { get; private set; }

        /*
         * Only valid if type is "Spacer", returns the offset from the root node for the current token
         */
        internal int Scope
        {
            get
            {
                if (Type != TokenType.Spacer)
                    return -1;
                return Value.Length / 2;
            }
        }
    }
}
