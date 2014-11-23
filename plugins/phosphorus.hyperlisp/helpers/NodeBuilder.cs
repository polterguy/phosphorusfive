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
    /// <summary>
    /// class responsible for creating a <see cref="phosphorus.core.Node"/> hierarchy from hyperlisp syntax
    /// </summary>
    public static class NodeBuilder
    {
        /// <summary>
        /// creates a <see cref="phosphorus.core.Node"/> hierarchy from the given hyperlisp
        /// </summary>
        /// <returns>the hyperlisp converted to a list of nodes</returns>
        /// <param name="context">application context</param>
        /// <param name="hyperlisp">the hyperlisp you wish to convert</param>
        public static List<Node> NodesFromHyperlisp (ApplicationContext context, string hyperlisp)
        {
            hyperlisp = hyperlisp.Trim ();
            if (string.IsNullOrEmpty (hyperlisp))
                return new List<Node> (); // empty result
            using (TextReader reader = new StringReader (hyperlisp)) {
                Node node = new Node ();
                Token previousToken = null;
                foreach (Token idxToken in Tokenize (reader)) {
                    node = TokensToNode (context, node, idxToken, previousToken);
                    previousToken = idxToken;
                }
                return new List<Node> (node.Root.UntieChildren ());
            }
        }

        /*
         * helper method for NodesFromHyperlisp, creates a node tree hierarchy from a token
         */
        private static Node TokensToNode (ApplicationContext context, Node node, Token token, Token previousToken)
        {
            switch (token.Type) {
            case Token.TokenType.Name:
                node = NameTokenToNode (node, token, previousToken);
                break;
            case Token.TokenType.ContentOrType:
                HandleContentOrTypeToken (context, node, token, previousToken);
                break;
            }
            return node;
        }

        /*
         * handles a "Name" token
         */
        private static Node NameTokenToNode (Node node, Token token, Token previousToken)
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
                // more than two consecutive spaces offset from previous token's name, syntax error
                throw new ArgumentException ("syntax error in hyperlisp, too many consecutive spaces during the opening of child collection near; '" + token.Value + "'");
            }

            // now that we have position we can add new node
            node.Add (new Node (token.Value));
            node = node [node.Count - 1];
            return node;
        }

        /*
         * handles a "ContentOrType" token
         */
        private static void HandleContentOrTypeToken (ApplicationContext context, Node node, Token token, Token previousToken)
        {
            if (previousToken.Type != Token.TokenType.Separator)

                // syntax error, should never come here, but for clarity, and to make sure, we still handle
                throw new ArgumentException ("syntax error in hyperlisp file, missing ':' before; '" + token.Value + "'");
            if (node.Value == null) {

                // notice, this can either be value or type information, defaulting to value, then if tokens have "additional" value
                // token later, before CarriageReturn, we retrieve the actual value according to the type information, otherwise this is value
                node.Value = token.Value;
            } else {

                // old value is actually type information, this is the actual value
                switch (node.Get<string> ()) {
                case "node":
                    Node tmp = new Node (string.Empty, token.Value);
                    context.Raise ("pf.hyperlisp-2-nodes", tmp);
                    if (tmp.Count == 0) {
                        node.Value = null;
                    } else if (tmp.Count == 1) {
                        node.Value = tmp [0].Untie ();
                    } else {
                        throw new ArgumentException ("when representing a node as the value of another node through hyperlisp, you can not have multiple root nodes");
                    }
                    break;
                case "path":
                    node.Value = new Node.DNA (token.Value);
                    break;
                default:
                    node.Value = ConvertStringValue (context, token.Value, node.Get<string> ());
                    break;
                }
            }
        }

        /*
         * converts an object's string representation in hyperlisp to the correct object
         * notice that everything you can convert into a string representation somehow, you
         * can actually store in your hyperlisp file, as long as you have created the correct
         * converters and type information Active Events. for an example of how to do this,
         * check out the "typeconverters.cs" file, which handles all types natively supported
         * by phosphorus five
         */
        private static object ConvertStringValue (ApplicationContext context, string value, string typeInfo)
        {
            Node tmp = new Node (string.Empty, value);
            string activeEventName = "pf.hyperlist.get-object-value." + typeInfo;
            context.Raise (activeEventName, tmp);
            return tmp.Value;
        }

        /*
         * tokenizing hyperlisp from text reader containing hyperlisp code
         */
        private static IEnumerable<Token> Tokenize (TextReader reader)
        {
            Token previousToken = null;
            while (true) {
                Token token = NextToken (reader, previousToken);
                if (token == null)
                    yield break;
                previousToken = token;
                yield return token;
            }
        }

        /*
         * retrieves next hyperlisp token from text reader
         */
        private static Token NextToken (TextReader reader, Token previousToken)
        {
            string buffer = string.Empty;
            if (reader.Peek () == ':' && (previousToken == null || previousToken.Type == Token.TokenType.Spacer)) {
                // empty name
                return new Token (Token.TokenType.Name, "");
            }
            int nextChar = reader.Read ();
            if (nextChar == -1)
                return null;
            buffer += (char)nextChar;
            if (buffer == ":")
                return new Token (Token.TokenType.Separator, ":");
            if (buffer == " ") {
                if (previousToken.Type == Token.TokenType.CarriageReturn) {
                    return NextSpaceToken (reader);
                } else {
                    // whitespace only carry semantics as first token in each line in hyperlisp content
                    // therefor we must "trim" the reader, before retrieving next token
                    nextChar = reader.Peek ();
                    while (nextChar == ' ') {
                        reader.Read ();
                        nextChar = reader.Peek ();
                    }
                    return NextToken (reader, previousToken);
                }
            } else if (buffer == "\r") {
                return NextCRLFToken (reader);
            } else if (buffer == "\n") {
                return new Token (Token.TokenType.CarriageReturn, "\r\n"); // normalizing carriage returns
            } else {
                return NextDefaultToken (reader, buffer, previousToken);
            }
        }

        /*
         * reads and validates next space token ("  ") from text reader
         */
        private static Token NextSpaceToken (TextReader reader)
        {
            string buffer = " ";
            int nextChar = reader.Peek ();
            while (nextChar == ' ') {
                buffer += (char)reader.Read ();
                nextChar = reader.Peek ();
            }
            return new Token (Token.TokenType.Spacer, buffer);
        }

        /*
         * reads and validates next carriage return / line feed token ("\r\n" or "\n")
         */
        private static Token NextCRLFToken (TextReader reader)
        {
            int nextChar = reader.Read ();
            if (nextChar == -1)
                throw new ArgumentException ("syntax error in hyperlisp carriage return character found, but no new line character found at end of file");
            if (nextChar != '\n')
                throw new ArgumentException ("syntax error in hyperlisp carriage return character found, but no new line character found");
            return new Token (Token.TokenType.CarriageReturn, "\r\n");
        }

        /*
         * reads next "default token" from text reader, can be string, multiline string or simply legal unescaped characters
         */
        private static Token NextDefaultToken (TextReader reader, string buffer, Token previousToken)
        {
            if (buffer == @"""") {
                return new Token (
                    previousToken != null && previousToken.Type == Token.TokenType.Separator ? 
                        Token.TokenType.ContentOrType : 
                        Token.TokenType.Name, 
                    ReadSingleLineStringLiteral (reader));
            } else if (buffer == "@") {
                int nextMultilineChar = reader.Peek ();
                if (nextMultilineChar != -1) {
                    if (nextMultilineChar == '"') {
                        reader.Read (); // skipping '"' part
                        return new Token (
                            previousToken != null && previousToken.Type == Token.TokenType.Separator ? 
                                Token.TokenType.ContentOrType : 
                                Token.TokenType.Name, 
                            ReadMultiLineStringLiteral (reader));
                    }
                }
            }
            int nextChar = reader.Peek ();
            while (nextChar != -1 && (char)nextChar != '\r' && (char)nextChar != '\n' && (char)nextChar != ':') {
                buffer += (char)nextChar;
                reader.Read ();
                nextChar = reader.Peek ();
            }
            return new Token (
                previousToken != null && previousToken.Type == Token.TokenType.Separator ? 
                    Token.TokenType.ContentOrType : 
                    Token.TokenType.Name, 
                buffer.Trim ());
        }

        /*
         * reads a single line string literal token from teext reader
         */
        private static string ReadSingleLineStringLiteral (TextReader reader)
        {
            string buffer = string.Empty;
            int nextChar = reader.Read ();
            while (nextChar != -1) {
                buffer += (char)nextChar;
                nextChar = reader.Read ();
                if (nextChar == '"' && 
                    (buffer.Length == 0 || 
                         buffer [buffer.Length - 1] != '\\' || 
                         (buffer.Length - buffer.TrimEnd ('\\').Length) % 2 == 0)) {
                    buffer += (char)nextChar;
                    break;
                }
            }
            if (buffer [buffer.Length - 1] != '"')
                throw new ArgumentException ("unclosed string literal in hyperlisp file");
            return buffer.Substring (0, buffer.Length - 1)
                .Replace ("\n", "\r\n") // normalizing carriage returns
                .Replace ("\r\r\n", "\r\n")
                .Replace ("\\\"", "\"")
                .Replace ("\\\\", "\\");
        }

        /*
         * reads a multiline string literal token from text reader
         */
        private static string ReadMultiLineStringLiteral (TextReader reader)
        {
            string buffer = string.Empty;
            int nextChar = reader.Read ();
            while (nextChar != -1) {
                buffer += (char)nextChar;
                nextChar = reader.Peek ();
                if (nextChar != '"' && (buffer.Length - buffer.TrimEnd ('"').Length) % 2 == 1)
                    break;
                nextChar = reader.Read ();
            }
            if (buffer.Length == 0 || buffer [buffer.Length - 1] != '"')
                throw new ArgumentException ("unclosed multiline string literal in hyperlisp close to end of hyperlisp");
            return buffer.Substring (0, buffer.Length - 1)
                .Replace (@"""""", @"""")
                .Replace ("\n", "\r\n") // normalizing carriage returns
                .Replace ("\r\r\n", "\r\n");
        }
    }
}

