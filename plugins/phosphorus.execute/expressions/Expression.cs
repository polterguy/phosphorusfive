/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Globalization;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute
{
    /// <summary>
    /// expression class, for retrieving and changing values in node tree according to execution expressions
    /// </summary>
    public class Expression
    {
        private List<string> _tokens;
        private string _expression;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.Expression"/> class
        /// </summary>
        /// <param name="expression">execution engine expression</param>
        public Expression (string expression)
        {
            if (!IsExpression (expression))
                throw new ArgumentException (string.Format ("'{0}' is not a valid expression", expression));
            _expression = expression.Substring (1);

            _tokens = TokenizeExpression (_expression);
            if (_tokens.Count == 0)
                throw new ArgumentException (string.Format ("'{0}' is not a valid expression", expression));
        }

        /// <summary>
        /// determines if string is an expression or not
        /// </summary>
        /// <returns><c>true</c> if string is an expression; otherwise, <c>false</c>.</returns>
        /// <param name="value">string to check</param>
        public static bool IsExpression (string value)
        {
            return value != null && value.StartsWith ("@") && !value.StartsWith (@"@""");
        }

        /// <summary>
        /// returns formatted node according to children nodes
        /// </summary>
        /// <returns>the formatted string expression</returns>
        /// <param name="node">node to format</param>
        public static string FormatNode (Node node)
        {
            string retVal = node.Get<string> ();
            if (node.Count > 0) {
                string[] childrenValues = new string[node.Count];
                int idxNo = 0;
                foreach (Node idxNode in node.Children) {
                    string value = idxNode.Get<string> ();
                    if (idxNode.Count > 0)
                        value = FormatNode (idxNode);
                    if (IsExpression (value)) {
                        Match match = new Expression (value).Evaluate (idxNode);
                        value = match.GetValue (0, string.Empty);
                    }
                    childrenValues [idxNo++] = value;
                }
                retVal = string.Format (CultureInfo.InvariantCulture, retVal, childrenValues);
            }
            return retVal;
        }

        /// <summary>
        /// evaluates expression for given <see cref="phosphorus.core.Node"/>  and returns <see cref="phosphorus.execute.Expression.Match"/> 
        /// </summary>
        public Match Evaluate (Node node)
        {
            MatchIterator currentIterator = new MatchIteratorStart (node);
            List<string> previousTokens = new List<string> ();
            for (int idxNo = 0; idxNo < _tokens.Count - 1; idxNo ++) {
                previousTokens.Add (_tokens [idxNo]);
                currentIterator = FindMatches (currentIterator, previousTokens, ref idxNo);
            }
            if (!currentIterator.HasMatch)
                return null;

            string matchType = _tokens [_tokens.Count - 1];
            switch (matchType) {
                case "name":
                    return new Match (currentIterator, Match.MatchType.Name);
                case "value":
                   return new Match (currentIterator, Match.MatchType.Value);
                case "path":
                    return new Match (currentIterator, Match.MatchType.Path);
                case @"\":
                    return new Match (currentIterator, Match.MatchType.Node);
                case "/":
                    return new Match (currentIterator, Match.MatchType.Children);
                default:
                    throw new ArgumentException ("that is not a valid expression, don't know how to return; '" + matchType + "'");
            }
        }

        /*
         * responsible for tokenizing expression
         */
        private static List<string> TokenizeExpression (string expression)
        {
            List<string> tokens = new List<string> ();
            string buffer = string.Empty;
            for (int idxNo = 0; idxNo < expression.Length; idxNo++) {
                char idxChar = expression [idxNo];
                switch (idxChar) {
                    case '/':
                    case '\\':
                    case '.':
                    case '|':
                    case '&':
                    case '!':
                    case '^':
                    case '(':
                    case ')':
                    case '=':
                        if (buffer != string.Empty) {
                            tokens.Add (buffer);
                            buffer = string.Empty;
                        }
                        tokens.Add (idxChar.ToString ());
                        break;
                    case '"':
                        if (buffer != string.Empty) {
                            tokens.Add (buffer);
                            buffer = string.Empty;
                        }
                        tokens.Add (Utilities.GetStringToken (expression, ref idxNo));
                        idxNo -= 1;
                        break;
                    case '@':
                        if (buffer != string.Empty) {
                            tokens.Add (buffer);
                            buffer = string.Empty;
                        }
                        tokens.Add (Utilities.GetMultilineStringToken (expression, ref idxNo));
                        idxNo -= 1;
                        break;
                    default:
                        buffer += idxChar;
                        break;
                }
            }
            if (buffer != string.Empty)
                tokens.Add (buffer);
            return tokens;
        }

        /*
         * return matches according to token
         */
        private MatchIterator FindMatches (MatchIterator lastMatches, List<string> previousTokens, ref int idxNo)
        {
            string token = previousTokens [previousTokens.Count - 1];
            switch (token) {
                case "/":
                    return FindMatchesSlashToken (lastMatches, previousTokens);
                case "*":
                    return new MatchIteratorAllChildren (lastMatches);
                case "|":
                    return FindMatchesLogical (lastMatches, previousTokens, ref idxNo, MatchIteratorLogical.LogicalType.OR);
                case "&":
                    return FindMatchesLogical (lastMatches, previousTokens, ref idxNo, MatchIteratorLogical.LogicalType.AND);
                case "!":
                    return FindMatchesLogical (lastMatches, previousTokens, ref idxNo, MatchIteratorLogical.LogicalType.NOT);
                case "^":
                    return FindMatchesLogical (lastMatches, previousTokens, ref idxNo, MatchIteratorLogical.LogicalType.XOR);
                case "**":
                    return new MatchIteratorAllDescendants (lastMatches);
                case ".":
                    return new MatchIteratorAllParents (lastMatches);
                case "=":
                    return lastMatches;
                default:
                    return FindMatchesDefaultToken (lastMatches, previousTokens);
            }
        }

        /*
         * returns a logical match operator
         */
        private MatchIteratorLogical FindMatchesLogical (
            MatchIterator lastMatches, 
            List<string> previousTokens, 
            ref int idxNo, 
            MatchIteratorLogical.LogicalType type)
        {
            MatchIterator root = lastMatches;
            while (root.Parent != null)
                root = root.Parent;
            Node node = (root as MatchIteratorStart).Node;
            MatchIterator currentIterator = new MatchIteratorStart (node);
            for (idxNo++; idxNo < _tokens.Count - 1; idxNo++) {
                if (_tokens [idxNo].Length == 1 && "|&!^()".IndexOf (_tokens [idxNo]) != -1)
                    break;
                previousTokens.Add (_tokens [idxNo]);
                currentIterator = FindMatches (currentIterator, previousTokens, ref idxNo);
            }
            idxNo -= 1;
            return new MatchIteratorLogical (node, lastMatches, currentIterator, type);
        }
        
        /*
         * return matches when token is slash "/"
         */
        private MatchIterator FindMatchesSlashToken (MatchIterator lastMatches, List<string> previousTokens)
        {
            if (previousTokens.Count == 1 || 
                ("|&!^".IndexOf (previousTokens [previousTokens.Count - 2]) != -1 && 
                previousTokens [previousTokens.Count - 2].Length == 1)) {
                // returning root since "/" is found as first token of expression
                return new MatchIteratorRoot (lastMatches);
            } else if (previousTokens [previousTokens.Count - 2] == "/") {
                // returning all nodes with empty names since two / tokens have followed each other
                return new MatchIteratorNamedNode (lastMatches, string.Empty);
            } else {
                // token is simply here to separate one token from the next, hence we return simply last match
                return lastMatches;
            }
        }

        /*
         * return matches when token is not specialized token
         */
        private MatchIterator FindMatchesDefaultToken (MatchIterator lastMatches, List<string> previousTokens)
        {
            string token = previousTokens [previousTokens.Count - 1];
            if (previousTokens.Count > 1 && previousTokens [previousTokens.Count - 2] == "=") {
                // looking for value
                return new MatchIteratorValuedNode (lastMatches, token);
            } else {
                // looking for name
                if (IsAllNumbers (token)) {
                    return new MatchIteratorNumberedNode (lastMatches, int.Parse (token));
                } else {
                    return new MatchIteratorNamedNode (lastMatches, token);
                }
            }
        }

        /*
         * returns true if string contains nothing but numbers
         */
        private bool IsAllNumbers (string token)
        {
            foreach (char idx in token) {
                if ("0123456789".IndexOf (idx) == -1)
                    return false;
            }
            return true;
        }
    }
}

