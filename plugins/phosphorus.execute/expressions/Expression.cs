/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Globalization;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.execute.iterators;

namespace phosphorus.execute
{
    /// <summary>
    /// expression class, for retrieving and changing values in node tree according to execution expressions
    /// </summary>
    public class Expression
    {
        private string _expression;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.Expression"/> class
        /// </summary>
        /// <param name="expression">execution engine expression</param>
        public Expression (string expression)
        {
            if (!IsExpression (expression))
                throw new ArgumentException (string.Format ("'{0}' is not a valid expression", expression));
            _expression = expression;
        }

        /// <summary>
        /// determines if object is an expression or not
        /// </summary>
        /// <returns><c>true</c> if object is an expression; otherwise, <c>false</c></returns>
        /// <param name="value">object to check</param>
        public static bool IsExpression (object value)
        {
            if (value == null)
                return false;
            string strValue = value as string;
            return strValue != null && 
                strValue.StartsWith ("@") && 
                strValue.Length > 1;
        }

        /// <summary>
        /// returns formatted node according to children nodes
        /// </summary>
        /// <returns>the formatted string expression</returns>
        /// <param name="node">node to format</param>
        public static string FormatNode (Node node)
        {
            string retVal = null;
            if (node.Count > 0) {
                List<string> childrenValues = new List<string> ();
                foreach (Node idxNode in node.Children) {
                    string value = idxNode.Count == 0 ? 
                        idxNode.Get<string> () : // simple value
                        FormatNode (idxNode); // recursive formatting string literal
                    if (IsExpression (value))
                        value = new Expression (value).Evaluate (idxNode).GetValue (0, string.Empty);
                    childrenValues.Add (value);
                }
                retVal = string.Format (CultureInfo.InvariantCulture, node.Get<string> (), childrenValues.ToArray ());
            } else {
                retVal = node.Get<string> ();
            }
            return retVal;
        }

        /// <summary>
        /// evaluates expression for given <see cref="phosphorus.core.Node"/>  and returns <see cref="phosphorus.execute.Expression.Match"/>
        /// </summary>
        public Match Evaluate (Node node)
        {
            IteratorGroup current = new IteratorGroup (node);
            string typeOfExpression = null, previousToken = null;
            foreach (string idxToken in TokenizeExpression (_expression)) {
                if (previousToken == "?") {
                    typeOfExpression = idxToken;
                    break;
                } else {
                    current = FindMatches (current, idxToken, previousToken);
                }
                previousToken = idxToken;
            }

            // checking to see if we have open groups, which is a bug
            if (current.ParentGroup != null)
                throw new ArgumentException ("unclosed group while evaluating; " + _expression);

            // returning match object
            return new Match (current, typeOfExpression);
        }

        /*
         * return matches according to token
         */
        private IteratorGroup FindMatches (IteratorGroup current, string token, string previousToken)
        {
            switch (token) {
            case "?":
                return FindMatchQuestionMarkToken (current, previousToken);
            case "(":
                return FindMatchOpenGroup (current, previousToken);
            case ")":
                return FindMatchCloseGroup (current, previousToken);
            case "/":
                return FindMatchSlashToken (current, previousToken);
            case "*":
                return FindMatchAsterixToken (current, previousToken);
            case "**":
                return FindMatchDoubleAsterixToken (current, previousToken);
            case "+":
            case "-":
                return FindMatchSiblingToken (current, token, previousToken);
            case ".":
                return FindMatchDotToken (current, previousToken);
            case "..":
                return FindMatchDoubleDotToken (current, previousToken);
            case "|":
            case "&":
            case "^":
            case "!":
                return FindMatchLogicalToken (current, token, previousToken);
            case "=":
                return FindMatchEqualSignToken (current, previousToken);
            case "[":
                return FindMatchOpenRangeToken (current, previousToken);
            case ",":
                return FindMatchCommaToken (current, previousToken);
            case "]":
                return FindMatchCloseRangeToken (current, previousToken);
            case "%":
                return FindMatchModuloToken (current, previousToken);
            case "#":
                return FindMatchHashToken (current, previousToken);
            case "<":
                return FindMatchShiftLeftToken (current, previousToken);
            case ">":
                return FindMatchShiftRightToken (current, previousToken);
            default:
                return FindMatchDefaultToken (current, token, previousToken);
            }
        }
        
        /*
         * handles ">" token
         */
        private IteratorGroup FindMatchShiftRightToken (IteratorGroup current, string previousToken)
        {
            if (previousToken != "/") {
                throw new ArgumentException ("unclosed iterator before shift right '>' in expression; '" + _expression + "'");
            }
            current.AddIterator (new IteratorShiftRight ());
            return current;
        }

        /*
         * handles "<" token
         */
        private IteratorGroup FindMatchShiftLeftToken (IteratorGroup current, string previousToken)
        {
            if (previousToken != "/") {
                throw new ArgumentException ("unclosed iterator before shift left '<' in expression; '" + _expression + "'");
            }
            current.AddIterator (new IteratorShiftLeft ());
            return current;
        }

        /*
         * handles "#" token
         */
        private IteratorGroup FindMatchHashToken (IteratorGroup current, string previousToken)
        {
            if (previousToken != "/") {
                throw new ArgumentException ("unclosed iterator before hash '#' in expression; '" + _expression + "'");
            }
            current.AddIterator (new IteratorReference ());
            return current;
        }

        /*
         * handles "%" token
         */
        private IteratorGroup FindMatchModuloToken (IteratorGroup current, string previousToken)
        {
            if (previousToken != "/") {
                throw new ArgumentException ("unclosed iterator before square bracket '[' in expression; '" + _expression + "'");
            }
            current.AddIterator (new IteratorModulo (-1));
            return current;
        }

        /*
         * handles "[" token
         */
        private IteratorGroup FindMatchOpenRangeToken (IteratorGroup current, string previousToken)
        {
            if (previousToken != "/") {
                throw new ArgumentException ("unclosed iterator before square bracket '[' in expression; '" + _expression + "'");
            }
            return current;
        }
        
        /*
         * handles "," token
         */
        private IteratorGroup FindMatchCommaToken (IteratorGroup current, string previousToken)
        {
            return current;
        }

        /*
         * handles "]" token
         */
        private IteratorGroup FindMatchCloseRangeToken (IteratorGroup current, string previousToken)
        {
            if (previousToken == ",")
                ((IteratorRange)current.LastIterator).End = int.MaxValue;
            return current;
        }

        /*
         * handles "?" token
         */
        private IteratorGroup FindMatchQuestionMarkToken (IteratorGroup current, string previousToken)
        {
            if (previousToken != "/") {
                throw new ArgumentException ("unclosed iterator before question mark '?' in expression; '" + _expression + "'");
            }
            return current;
        }

        /*
         * handles "(" token
         */
        private IteratorGroup FindMatchOpenGroup (IteratorGroup current, string previousToken)
        {
            if (previousToken == null || (previousToken.Length != 1 || "(|&^!/".IndexOf (previousToken) == - 1)) {
                throw new ArgumentException ("syntax error in expression; '" + 
                    _expression + 
                    "' probably missing a slash '/' before group was opened in one of your '(' tokens");
            }
            return new IteratorGroup (current);
        }
        
        /*
         * handles ")" token
         */
        private IteratorGroup FindMatchCloseGroup (IteratorGroup current, string previousToken)
        {
            if (previousToken != "/" && previousToken != ")") {
                throw new ArgumentException ("syntax error in expression; '" + 
                    _expression + 
                    "' probably missing a slash '/' before group was closed in one of your ')' tokens");
            }
            if (current.ParentGroup == null)
                throw new ArgumentException ("too many parantheses ')' in expression; '" + _expression + "', tried to close a group that didn't exist");
            return current.ParentGroup;
        }

        /*
         * handles "/" token
         */
        private IteratorGroup FindMatchSlashToken (IteratorGroup current, string previousToken)
        {
            if (previousToken == "/") {
                // two slashes "//" preceding each other, hence we're looking for a named value, where its name is string.Empty
                current.AddIterator (new IteratorNamed (string.Empty));
            }
            return current;
        }
        
        /*
         * handles "\" token
         */
        private IteratorGroup FindMatchDoubleDotToken (IteratorGroup current, string previousToken)
        {
            if (previousToken != "/") {
                throw new ArgumentException ("syntax error in expression; '" + 
                    _expression + 
                    "' probably missing a slash '/' before root token '..'");
            }
            current.AddIterator (new IteratorRoot ());
            return current;
        }
        
        /*
         * handles "*" token
         */
        private IteratorGroup FindMatchAsterixToken (IteratorGroup current, string previousToken)
        {
            if (previousToken != "/") {
                throw new ArgumentException ("syntax error in expression; '" + 
                    _expression + 
                    "' probably missing a slash '/' before '*' token");
            }
            current.AddIterator (new IteratorChildren ());
            return current;
        }
        
        /*
         * handles "**" token
         */
        private IteratorGroup FindMatchDoubleAsterixToken (IteratorGroup current, string previousToken)
        {
            if (previousToken != "/") {
                throw new ArgumentException ("syntax error in expression; '" + 
                    _expression + 
                    "' probably missing a slash '/' before '**' token");
            }
            current.AddIterator (new IteratorFlatten ());
            return current;
        }

        /*
         * handles "-" && "+" tokens
         */
        private IteratorGroup FindMatchSiblingToken (IteratorGroup current, string token, string previousToken)
        {
            if (previousToken != "/") {
                throw new ArgumentException ("syntax error in expression; '" + 
                    _expression + 
                    "' probably missing a slash '/' before '" + token + "' token");
            }
            current.AddIterator (new IteratorSibling (token == "+" ? 1 : -1));
            return current;
        }
        
        /*
         * handles "." token
         */
        private IteratorGroup FindMatchDotToken (IteratorGroup current, string previousToken)
        {
            if (previousToken != "/") {
                throw new ArgumentException ("syntax error in expression; '" + 
                    _expression + 
                    "' probably missing a slash '/' before '**' token");
            }
            current.AddIterator (new IteratorParent ());
            return current;
        }
        
        /*
         * handles "|", "&", "^" and "!" tokens
         */
        private IteratorGroup FindMatchLogicalToken (IteratorGroup current, string token, string previousToken)
        {
            switch (token) {
            case "|":
                current.AddLogical (new Logical (Logical.LogicalType.OR));
                break;
            case "&":
                current.AddLogical (new Logical (Logical.LogicalType.AND));
                break;
            case "^":
                current.AddLogical (new Logical (Logical.LogicalType.XOR));
                break;
            case "!":
                current.AddLogical (new Logical (Logical.LogicalType.NOT));
                break;
            }
            return current;
        }
        
        /*
         * handles "=" token
         */
        private IteratorGroup FindMatchEqualSignToken (IteratorGroup current, string previousToken)
        {
            if (previousToken != "/") {
                throw new ArgumentException ("syntax error in expression; '" + 
                    _expression + 
                    "' probably missing a slash '/' before valued token '='");
            }
            current.AddIterator (new IteratorValued ()); // actual value will be set in next token
            return current;
        }
        
        /*
         * handles all other tokens, such as "named tokens" and "valued tokens"
         */
        private IteratorGroup FindMatchDefaultToken (IteratorGroup current, string token, string previousToken)
        {
            if (previousToken == "=") {

                // looking for value, current token is value to look for, changing Value of current MatchIterator
                ((IteratorValued)current.LastIterator).Value = token;
            } else if (previousToken == "+" || previousToken == "-") {

                // looking for sibling, current token is offset to look for
                if (!IsNumber (token)) {
                    throw new ArgumentException ("a sibling operator must have an integer number as its next token, syntax error close to; '" + 
                        token + "' in expression; '" + _expression + "'");
                }
                ((IteratorSibling)current.LastIterator).Offset = previousToken == "-" ? -int.Parse (token) : int.Parse (token);
            } else if (previousToken == "[") {

                // looking for range token, current token is "start"
                if (!IsNumber (token))
                    throw new ArgumentException ("start of range was not a number, syntax error at; '" + token + "' in expression; '" + _expression + "'");
                current.AddIterator (new IteratorRange (int.Parse (token)));
            } else if (previousToken == ",") {

                // looking for range token, current token is "end"
                if (!IsNumber (token))
                    throw new ArgumentException ("end of range was not a number, syntax error at; '" + token + "' in expression; '" + _expression + "'");
                ((IteratorRange)current.LastIterator).End = int.Parse (token);
            } else if (previousToken == "%") {

                // looking for range token, current token is "end"
                if (!IsNumber (token))
                    throw new ArgumentException ("modulo was not a number, syntax error at; '" + token + "' in expression; '" + _expression + "'");
                ((IteratorModulo)current.LastIterator).Modulo = int.Parse (token);
            } else {

                // looking for named or numbered node
                if (previousToken != "/") {
                    throw new ArgumentException ("syntax error in expression; '" + 
                        _expression + 
                        "' probably missing a slash '/' before token; '" + 
                        token + "'");
                }
                if (IsNumber (token)) {
                    current.AddIterator (new IteratorNumbered (int.Parse (token)));
                } else {
                    if (token.StartsWith ("/")) {
                        if (token.LastIndexOf ("/") == 0)
                            throw new ArgumentException ("token; '" + token + "' in expression; '" + _expression + "' is not a valid regular expression");
                        current.AddIterator (new IteratorNamedRegex (token));
                    } else if (token.StartsWith ("..")) {
                        // named ancestor
                        current.AddIterator (new IteratorNamedAncestor (token.Substring (2)));
                    } else {
                        current.AddIterator (new IteratorNamed (token));
                    }
                }
            }
            return current;
        }

        /*
         * responsible for tokenizing expression
         */
        private static IEnumerable<string> TokenizeExpression (string expression)
        {
            string buffer = string.Empty;
            for (int idxNo = 1 /* skipping first @ character */; idxNo < expression.Length; idxNo++) {
                char idxChar = expression [idxNo];
                if ("/.|&^!()=?+-[],%#<>".IndexOf (idxChar) > -1) {
                    if (buffer != string.Empty) {
                        yield return buffer;
                        buffer = string.Empty;
                    }
                    if (idxChar == '.' && idxNo + 1 < expression.Length && expression [idxNo + 1] == '.') {
                        idxNo += 1;
                        yield return "..";
                    } else {
                        yield return idxChar.ToString ();
                    }
                } else if (@"""@".IndexOf (idxChar) > -1) {
                    if (buffer != string.Empty) {
                        yield return buffer;
                        buffer = string.Empty;
                    }
                    yield return Utilities.GetStringToken (expression, ref idxNo);
                    idxNo -= 1;
                } else {
                    buffer += idxChar;
                }
            }
            if (buffer != string.Empty)
                yield return buffer;
        }

        /*
         * returns true if string can be converted to an integer
         */
        private bool IsNumber (string token)
        {
            foreach (char idx in token) {
                if ("0123456789".IndexOf (idx) == -1)
                    return false;
            }
            return token.Length > 0;
        }
    }
}

