/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.hyperlisp.helpers
{
    /// <summary>
    ///     class responsible for creating a <see cref="phosphorus.core.Node" /> hierarchy from hyperlisp syntax
    /// </summary>
    public class NodeBuilder
    {
        private readonly ApplicationContext _context;
        private readonly string _hyperlisp;

        /// <summary>
        ///     initializes a new instance of the <see cref="NodeBuilder" /> class
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="hyperlisp">hyperlisp to convert into a list of <see cref="phosphorus.core.Node" />s</param>
        public NodeBuilder (ApplicationContext context, string hyperlisp)
        {
            _context = context;
            _hyperlisp = (hyperlisp ?? "").TrimStart ('\r', '\n').TrimEnd ('\r', '\n');
        }

        /// <summary>
        ///     creates a list of <see cref="phosphorus.core.Node" />s from the given hyperlisp
        /// </summary>
        /// <returns>the hyperlisp converted to a list of nodes</returns>
        public List<Node> Nodes
        {
            get
            {
                if (string.IsNullOrEmpty (_hyperlisp))
                    return new List<Node> (new[] {new Node (string.Empty)}); // empty result

                // we need a text reader for our tokenizer
                using (var tokenizer = new Tokenizer (_hyperlisp)) {
                    // creating root node
                    var node = new Node ();
                    Token previousToken = null;

                    // looping through all tokens sequentially
                    foreach (var idxToken in tokenizer.Tokens) {
                        node = TokensToNode (node, idxToken, previousToken);
                        previousToken = idxToken;
                    }

                    // return list of nodes back to caller
                    return new List<Node> (node.Root.UntieChildren ());
                }
            }
        }

        /*
         * helper method for NodesFromHyperlisp, creates a node tree hierarchy from a token
         */

        private Node TokensToNode (Node node, Token token, Token previousToken)
        {
            switch (token.Type) {
                case Token.TokenType.Name:

                    // this is the name of the node
                    node = NameTokenToNode (node, token, previousToken);
                    break;
                case Token.TokenType.TypeOrContent:

                    // this might either be the value or the type information of our node
                    HandleContentOrTypeToken (node, token, previousToken);
                    break;
            }
            return node;
        }

        /*
         * handles a "Name" token
         */

        private Node NameTokenToNode (Node node, Token token, Token previousToken)
        {
            if (previousToken == null || previousToken.Type == Token.TokenType.CarriageReturn) {
                // root node
                node = node.Root;
            } else if (previousToken.Type == Token.TokenType.Spacer && node.Path.Count > previousToken.Scope) {
                // some ancestor, finding the correct ancestor
                while (node.Path.Count != previousToken.Scope) {
                    node = node.Parent;
                }
            } else if (previousToken.Type != Token.TokenType.Spacer || node.Path.Count + 1 == previousToken.Scope) {
                // more than two consecutive spaces offset from previous token's name, which is a syntax error
                throw new ArgumentException ("syntax error in hyperlisp, too many consecutive spaces during the opening of child collection near; '" + token.Value + "'");
            }

            // now that we have position we can add new node
            node.Add (new Node (token.Value));
            node = node [node.Count - 1];
            return node;
        }

        /*
         * handles a "TypeOrContent" token
         */
        // ReSharper disable once UnusedParameter.Local
        private void HandleContentOrTypeToken (Node node, Token token, Token previousToken)
        {
            if (previousToken.Type != Token.TokenType.Separator)

                // syntax error, should never come here, but for clarity, and to make sure, we still handle
                throw new ArgumentException ("syntax error in hyperlisp file, missing ':' before; '" + token.Value + "'");
            node.Value = node.Value == null ? token.Value : ConvertStringValue (token.Value, node.Get<string> (_context));
        }

        /*
         * converts an object's string representation in hyperlisp to the correct object
         * notice that everything you can convert into a string representation somehow, you
         * can actually store in your hyperlisp file, as long as you have created the correct
         * converters and type information Active Events. for an example of how to do this,
         * check out the "typeconverters.cs" file, which handles all types natively supported
         * by phosphorus five
         */

        private object ConvertStringValue (string value, string typeInfo)
        {
            if (typeInfo == "string")
                return value; // string is default type

            // converting our string to the actual object, and returning back to caller
            return _context.Raise ("pf.hyperlisp.get-object-value." + (typeInfo == "node" ? "abs." : "") + typeInfo, new Node (string.Empty, value)).Value;
        }
    }
}