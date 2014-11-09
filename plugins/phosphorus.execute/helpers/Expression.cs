/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute
{
    /// <summary>
    /// expression class, for retrieving and changing values in node tree according to execution expressions
    /// </summary>
    public class Expression
    {
        private string[] _tokens;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.Expression"/> class
        /// </summary>
        /// <param name="expression">execution engine expression</param>
        public Expression (string expression)
        {
            if (string.IsNullOrEmpty(expression) || !expression.StartsWith ("@"))
                throw new ArgumentException (string.Format ("'{0}' is not a valid expression", expression));
            expression = expression.Substring (1);

            _tokens = TokenizeExpression (expression);
            if (_tokens.Length == 0)
                throw new ArgumentException (string.Format ("'{0}' is not a valid expression", expression));
        }

        /// <summary>
        /// evaluates expression for given <see cref="phosphorus.core.Node"/>  and returns <see cref="phosphorus.execute.Expression.Match"/> 
        /// </summary>
        public Match Evaluate (Node node)
        {
            List<Node> result = new List<Node> (new Node[] { node });
            string lastToken = null;
            for (int idxNo = 0; idxNo < _tokens.Length - 1; idxNo ++) {
                string idxToken = _tokens [idxNo];
                result = FindMatches (result, idxToken, lastToken);
                if (result.Count == 0)
                    return null;
                lastToken = idxToken;
            }
            switch (_tokens [_tokens.Length - 1]) {
                case "name":
                    return new Match (result, Match.MatchType.Name);
                case "value":
                   return new Match (result, Match.MatchType.Value);
                case "path":
                    return new Match (result, Match.MatchType.Path);
                case @"\":
                    return new Match (result, Match.MatchType.Node);
                case "/":
                    return new Match (result, Match.MatchType.Children);
                default:
                    throw new ArgumentException ("that is not a valid expression, don't know how to return; '" + lastToken + "'");
            }
        }
        
        /*
         * return matches according to token
         */
        private List<Node> FindMatches (List<Node> lastMatches, string token, string lastToken)
        {
            switch (token) {
                case "/":
                    return FindSlashMatches (lastMatches, lastToken);
                case "*":
                    return FindAsterixMatches (lastMatches);
                case "**":
                    return FindDoubleAsterixMatches (lastMatches);
                case ".":
                    return FindDotMatches (lastMatches);
                default:
                    return FindNamedMatches (lastMatches, token);
            }
        }

        /*
         * returns matches for / token
         */
        private List<Node> FindSlashMatches (List<Node> lastMatches, string lastToken)
        {
            if (lastToken == null) {

                // returning root
                return new List<Node> (new Node[] { lastMatches [0].Root });
            } else if (lastToken == "/") {

                // returning all nodes with empty names
                List<Node> retVal = new List<Node> ();
                foreach (Node idxOldResult in lastMatches) {
                    foreach (Node idxInner in idxOldResult.Children) {
                        if (idxInner.Name == string.Empty)
                            retVal.Add (idxInner);
                    }
                }
                return retVal;
            }

            // token is simply there to separate one token from the next
            return lastMatches;
        }

        /*
         * returns matches for * token
         */
        private List<Node> FindAsterixMatches (List<Node> lastMatches)
        {
            List<Node> retValAllChildren = new List<Node> ();
            foreach (Node idxOldResult in lastMatches) {
                retValAllChildren.AddRange (idxOldResult.Children);
            }
            return retValAllChildren;
        }
        
        /*
         * returns matches for ** token
         */
        private List<Node> FindDoubleAsterixMatches (List<Node> lastMatches)
        {
            List<Node> retValAllDescendants = new List<Node> ();
            foreach (Node idxDescendant in lastMatches) {
                retValAllDescendants.AddRange (FindAllDescendants (idxDescendant));
            }
            return retValAllDescendants;
        }
        
        /*
         * returns matches for . token
         */
        private List<Node> FindDotMatches (List<Node> lastMatches)
        {
            List<Node> retValAllParents = new List<Node> ();
            foreach (Node idxOldResult in lastMatches) {
                if (idxOldResult.Parent != null)
                    retValAllParents.Add (idxOldResult.Parent);
            }
            return retValAllParents;
        }
        
        /*
         * returns matches for named token
         */
        private List<Node> FindNamedMatches (List<Node> lastMatches, string token)
        {
            List<Node> retValNamedNodes = new List<Node> ();
            foreach (Node idxOldResult in lastMatches) {
                foreach (Node idxOldResultChild in idxOldResult.Children) {
                    if (idxOldResultChild.Name == token)
                        retValNamedNodes.Add (idxOldResultChild);
                }
            }
            return retValNamedNodes;
        }

        /*
         * returns all descendants of given node
         */
        IEnumerable<Node> FindAllDescendants (Node node)
        {
            foreach (Node idx in node.Children) {
                yield return idx;
                foreach (Node idxInner in FindAllDescendants (idx)) {
                    yield return idxInner;
                }
            }
        }

        /*
         * responsible for tokenizing expression
         */
        private string[] TokenizeExpression (string expression)
        {
            List<string> tokens = new List<string> ();
            string buffer = string.Empty;
            for (int idxNo = 0; idxNo < expression.Length; idxNo++) {
                char idxChar = expression [idxNo];
                switch (idxChar) {
                    case '/':
                    case '\\':
                    case '.':
                        if (buffer != string.Empty) {
                            tokens.Add (buffer);
                            buffer = string.Empty;
                        }
                        tokens.Add (idxChar.ToString ());
                        break;
                    case '@':
                        if (buffer != string.Empty) {
                            tokens.Add (buffer);
                            buffer = string.Empty;
                        }
                        tokens.Add (ReadStringLiteral (expression, ref idxNo));
                        break;
                    default:
                        buffer += idxChar;
                        break;
                }
            }
            if (buffer != string.Empty)
                tokens.Add (buffer);
            return tokens.ToArray ();
        }
        
        /*
         * reads string literal during tokenization process
         */
        private string ReadStringLiteral (string expression, ref int idxNo)
        {
            string buffer = "";
            idxNo += 2; // skipping @" parts, and looping until end of string literal
            while (true) {
                char idxChar = expression [idxNo];
                if (buffer.Length > 0 && idxChar != '"' && (buffer.Length - buffer.TrimEnd (new char[] { '"' }).Length) % 2 == 1)
                    break;
                buffer += idxChar.ToString ();
                idxNo += 1;
            }
            --idxNo;
            return buffer.Substring (0, buffer.Length - 1).Replace (@"""""", @"""");
        }
    }
}

