/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
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
using System.Linq;
using System.Collections.Generic;
using p5.core;
using p5.exp.exceptions;

namespace p5.hyperlambda.helpers
{
    /// <summary>
    ///     Class encapsulating internals of parsing of Hyperlambda
    /// </summary>
    public class NodeBuilder
    {
        private readonly ApplicationContext _context;
        private readonly string _hyperlisp;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NodeBuilder" /> class
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="hyperlambda">Hyperlambda to convert into a list of nodes</param>
        public NodeBuilder (ApplicationContext context, string hyperlambda)
        {
            _context = context;
            _hyperlisp = (hyperlambda ?? "").TrimStart ('\r', '\n').TrimEnd ('\r', '\n');
        }

        /// <summary>
        ///     Creates a list of <see cref="phosphorus.core.Node" />s from the given Hyperlambda
        /// </summary>
        /// <returns>The Hyperlambda converted to p5 lambda</returns>
        public List<Node> Nodes
        {
            get
            {
                if (string.IsNullOrEmpty (_hyperlisp))
                    return new List<Node> (new[] {new Node ("")}); // Empty result

                // Creating root node such that we have access to it outside of iteration of tokens
                var node = new Node ();

                try
                {
                    // Creating tokenizer
                    using (var tokenizer = new Tokenizer (_hyperlisp)) {

                        Token previousToken = null;

                        // Looping through all tokens sequentially
                        foreach (var idxToken in tokenizer.Tokens) {
                            node = TokensToNode (node, idxToken, previousToken);
                            previousToken = idxToken;
                        }

                        // Return list of nodes back to caller
                        return node.Root.Children.ToList ();
                    }
                }
                catch (Exception err)
                {
                    // Since we want to have LambaException leave this bugger, and not "whatever exception", we transform
                    // current eception into LambdaException to give user intelligent feedback with stack trace
                    if (err is LambdaException)
                        throw;

                    throw new LambdaException (err.Message, node, _context);
                }
            }
        }

        /*
         * Helper method for NodesFromHyperlisp, creates a node tree hierarchy from a Token object
         */
        private Node TokensToNode (Node node, Token token, Token previousToken)
        {
            switch (token.Type) {
                case Token.TokenType.Name:

                    // This is the name of the node
                    node = NameTokenToNode (node, token, previousToken);
                    break;
                case Token.TokenType.TypeOrContent:

                    // This might either be the value or the type information of our node
                    HandleContentOrTypeToken (node, token, previousToken);
                    break;
            }
            return node;
        }

        /*
         * Handles a "Name" token
         */
        private Node NameTokenToNode (Node node, Token token, Token previousToken)
        {
            if (previousToken == null || previousToken.Type == Token.TokenType.CarriageReturn) {

                // Root node
                node = node.Root;
            } else if (previousToken.Type == Token.TokenType.Spacer && node.OffsetToRoot > previousToken.Scope) {

                // Some ancestor, finding the right one
                while (node.OffsetToRoot != previousToken.Scope) {
                    node = node.Parent;
                }
            } else if (previousToken.Type != Token.TokenType.Spacer || node.OffsetToRoot + 1 == previousToken.Scope) {

                // More than two consecutive spaces offset from previous token's name, which is a syntax error
                throw new LambdaException (
                    string.Format ("Too many consecutive spaces in opening of child collection in Hyperlambda near '{0}'", token.Value), 
                    node, 
                    _context);
            }

            // Now that we have position we can add new node
            node.Add (new Node (token.Value));
            node = node [node.Children.Count - 1];
            return node;
        }

        /*
         * Handles a "TypeOrContent" token
         */
        private void HandleContentOrTypeToken (Node node, Token token, Token previousToken)
        {
            // If there's no existing value for node, then there's not any type information associated with object neither,
            // hence we don't have to attempt to convert token's value before setting the value of the node
            node.Value = node.Value == null ? token.Value : ConvertStringValue (token.Value, node.Get<string> (_context));
        }

        /*
         * Converts an object's string representation in hyperlambda to the correct object
         * notice that everything you can convert into a string representation somehow, you
         * can actually store in your hyperlambda file, as long as you have created the correct
         * converters and type information Active Events.
         */
        private object ConvertStringValue (string value, string typeInfo)
        {
            if (typeInfo == "string")
                return value; // string is default type

            // Converting our string to the actual object, and returning back to caller
            return _context.Raise (
                "p5.hyperlambda.get-object-value." + (typeInfo == "node" ? "abs.node" : typeInfo), 
                new Node ("", value, new Node [] { new Node ("decode", true) })).Value;
        }
    }
}
