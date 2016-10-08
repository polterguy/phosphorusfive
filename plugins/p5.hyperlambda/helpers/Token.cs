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

namespace p5.hyperlambda.helpers
{
    /*
     * Class used internally to tokenize Hyperlambda
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
                    throw new ApplicationException ("Odd number of spaces in front of Hyperlambda name");
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
