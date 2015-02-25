
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;

namespace phosphorus.hyperlisp
{
    /*
     * class used internall to tokenize hyperlisp
     */
    public class Token
    {
        /*
         * type of token
         */
        public enum TokenType
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
             * name of node token
             */
            Name,

            /*
             * type, or content token
             * whether or not it's a type or a content token is defined if another TypeOrContent token follows it.
             * if another ContentOrType token follows it, with a Separator token between then, but no CarriageReturn
             * token between them, the token is a "type token". otherwise it's a "content token" type
             */
            TypeOrContent
        }

        /*
         * constructor taking type and value or "content" of token
         */
        public Token (TokenType type, string value)
        {
            if (type == TokenType.Spacer) {
                if (value.Length % 2 != 0)
                    throw new ArgumentException ("odd number of spaces in front of hyperlisp name, which is a syntax error in hyperlisp");
            }
            Type = type;
            Value = value;
        }

        /*
         * type of token
         */
        public TokenType Type {
            get;
            private set;
        }

        /*
         * value or "content" of token, typically ":", "\r\n", an even number of spaces, or any other arbitrary string value, depending
         * upon the Type of token
         */
        public string Value {
            get;
            private set;
        }

        /*
         * only valid if type is "Spacer", returns the offset from the root node for the current token
         */
        public int Scope {
            get {
                if (Type != TokenType.Spacer)
                    return -1;
                return Value.Length / 2;
            }
        }
    }
}
