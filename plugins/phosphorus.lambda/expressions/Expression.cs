
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Globalization;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.lambda.iterators;

namespace phosphorus.lambda
{
    /// <summary>
    /// expression class, for retrieving and changing values in node trees according to
    /// pf.lambda expressions
    /// </summary>
    public class Expression
    {
        private string _expression;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.Expression"/> class
        /// </summary>
        /// <param name="expression">execution engine expression</param>
        public static Expression Create (string expression)
        {
            return new Expression (expression);
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
            return IsExpression (value as string);
        }

        /// <summary>
        /// determines if object is an expression or not
        /// </summary>
        /// <returns><c>true</c> if object is an expression; otherwise, <c>false</c></returns>
        /// <param name="strValue">string to check</param>
        public static bool IsExpression (string strValue)
        {
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
        /// iterator callback for iterating node expressions
        /// </summary>
        public delegate bool IteratorCallbackBool<T> (T idx);

        /// <summary>
        /// iterator callback for iterating node expressions
        /// </summary>
        public delegate void IteratorCallbackVoid<T> (T idx);

        /// <summary>
        /// iterates a value of a node, which might be either a constant, or an expression, and
        /// invokes callback for each value in constant or expression result. if callback returns
        /// false, iteration will stop and this method will return false. if iterations are all
        /// returning true, this method will return true
        /// </summary>
        /// <returns>true if all iterations are successfully completed without being stopped</returns>
        /// <param name="node">node that contains constant or expression as value</param>
        /// <param name="formatExpression">if set to <c>true</c> will format expressions</param>
        /// <param name="callback">code to invoke once for each result</param>
        /// <typeparam name="T">the type of object 'value' is</typeparam>
        public static bool Iterate<T> (Node node, bool formatExpression, IteratorCallbackBool<T> callback)
        {
            object nodeValue = null;
            if (formatExpression && node.Count > 0) {
                nodeValue = FormatNode (node);
            } else {
                nodeValue = node.Value;
            }
            if (IsExpression (nodeValue)) {
                var match = Create (nodeValue as string).Evaluate (node);
                for (int idxNo = 0; idxNo < match.Count; idxNo++) {
                    if (!callback (match.GetValue<T> (idxNo)))
                        return false;
                }
                return true;
            } else {
                return callback (nodeValue == null ? default (T) : (T)nodeValue);
            }
        }

        /// <summary>
        /// iterates a value of a node, which might be either a constant, or an expression, and
        /// invokes callback for each value in constant or expression result
        /// </summary>
        /// <param name="node">node that contains constant or expression as value</param>
        /// <param name="formatExpression">if set to <c>true</c> will format expressions</param>
        /// <param name="callback">code to invoke once for each result</param>
        /// <typeparam name="T">the type of object 'value' is</typeparam>
        public static void Iterate<T> (Node node, bool formatExpression, IteratorCallbackVoid<T> callback)
        {
            object nodeValue = null;
            if (formatExpression && node.Count > 0) {
                nodeValue = FormatNode (node);
            } else {
                nodeValue = node.Value;
            }
            if (IsExpression (nodeValue)) {
                var match = Create (nodeValue as string).Evaluate (node);
                for (int idxNo = 0; idxNo < match.Count; idxNo++) {
                    callback (match.GetValue<T> (idxNo));
                }
            } else {
                if (!formatExpression || node.Count == 0) {

                    // short hand helper for converting type correctly
                    callback (node.Get<T> ());
                } else {
                    callback (nodeValue == null ? default (T) : (T)nodeValue);
                }
            }
        }

        /// <summary>
        /// returns a single value of type T from expression in node's value given
        /// </summary>
        /// <param name="node">node containing expression, being current node for expression</param>
        /// <param name="formatExpression">if set to <c>true</c> will format expression</param>
        /// <typeparam name="T">the 1st type parameter</typeparam>
        public static T Single<T> (Node node, bool formatExpression)
        {
            object nodeValue = null;
            if (formatExpression && node.Count > 0) {
                nodeValue = FormatNode (node);
            } else {
                nodeValue = node.Value;
            }
            if (IsExpression (nodeValue)) {
                var match = Create (nodeValue as string).Evaluate (node);
                if (!match.IsSingleLiteral)
                    throw new ArgumentException ("Single expected single value of expression, but expression returned multiple results");
                return match.GetValue<T> (0);
            } else {
                if (!formatExpression || node.Count == 0)
                    return node.Get<T> (); // short hand helper for converting type correctly
                return nodeValue == null ? default (T) : (T)nodeValue;
            }
        }

        /// <summary>
        /// returns a single string value from expression in node's value given by concatenating results
        /// if expression results to multiple results
        /// </summary>
        /// <param name="node">node containing expression, being current node for expression</param>
        /// <param name="formatExpression">if set to <c>true</c> will format expression</param>
        /// <param name="spacingString">string to put between all results before returning value to caller</param>
        public static string Single (Node node, bool formatExpression, string spacingString = "")
        {
            string nodeValue = null;
            if (formatExpression && node.Count > 0) {
                nodeValue = FormatNode (node);
            } else {
                nodeValue = node.Get<string> ();
            }
            if (IsExpression (nodeValue)) {
                var match = Create (nodeValue).Evaluate (node);
                if (match.IsSingleLiteral)
                    return match.GetValue<string> (0);
                string retVal = "";
                bool first = true;
                for (int idxNo = 0; idxNo < match.Count; idxNo++) {
                    if (first) {
                        first = false;
                    } else {
                        retVal += spacingString;
                    }
                    retVal += match.GetValue (idxNo).ToString ();
                }
                return retVal;
            } else {
                return nodeValue;
            }
        }

        /// <summary>
        /// returns a single string value from expression in node's value given by concatenating results
        /// if expression results to multiple results
        /// </summary>
        /// <param name="node">node containing expression, being current node for expression</param>
        /// <param name="formatExpression">if set to <c>true</c> will format expression</param>
        /// <param name="spacingString">string to put between all results before returning value to caller</param>
        public static string SingleNameValuePair (Node node, bool formatExpression, string spacingString = "", string inbetweenPairString = "")
        {
            string nodeValue = null;
            if (formatExpression && node.Count > 0) {
                nodeValue = FormatNode (node);
            } else {
                nodeValue = node.Get<string> ();
            }
            if (IsExpression (nodeValue)) {
                var match = Create (nodeValue).Evaluate (node);
                if (match.IsSingleLiteral)
                    return match [0].Name + spacingString + match.GetValue<string> (0);
                string retVal = "";
                bool first = true;
                for (int idxNo = 0; idxNo < match.Count; idxNo++) {
                    if (first) {
                        first = false;
                    } else {
                        retVal += spacingString;
                    }
                    retVal += match [idxNo].Name + inbetweenPairString + match.GetValue (idxNo).ToString ();
                }
                return retVal;
            } else {
                return nodeValue;
            }
        }

        /// <summary>
        /// evaluates expression for given <see cref="phosphorus.core.Node"/>  and returns <see cref="phosphorus.execute.Expression.Match"/>
        /// </summary>
        public Match Evaluate (Node node)
        {
            IteratorGroup current = new IteratorGroup (node);
            string typeOfExpression = null, previousToken = null;
            using (Tokenizer tokenizer = new Tokenizer (_expression)) {
                foreach (string idxToken in tokenizer.Tokens) {
                    if (previousToken == "?") {
                        typeOfExpression = idxToken;
                        break;
                    } else {
                        current = FindMatches (current, idxToken, previousToken);
                    }
                    previousToken = idxToken;
                }
            }

            // checking to see if we have open groups, which is a bug
            if (current.ParentGroup != null)
                throw new ArgumentException ("unclosed group while evaluating; " + _expression);

            // returning match object
            return new Match (current, typeOfExpression);
        }
        
        /*
         * private ctor, to make sure we can in future versions do lokkup against cache, for instance
         */
        private Expression (string expression)
        {
            if (!IsExpression (expression))
                throw new ArgumentException (string.Format ("'{0}' is not a valid expression", expression));
            _expression = expression;
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
            case ":":
                if (!(current.LastIterator is IteratorValued))
                    throw new ArgumentException ("syntax error in expression, ':' found at unexpected position");
                // "typed" value expression
                return current;
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
            if (previousToken == "[") {
                // empty "start" part
                current.AddIterator (new IteratorRange (0));
            }
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
            if (previousToken != "/" && previousToken != ")") {
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
            if (current.ParentGroup == null) {
                throw new ArgumentException ("too many parantheses ')' in expression; '" + 
                    _expression + 
                    "', tried to close a group that didn't exist");
            }
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
                ((IteratorValued)current.LastIterator).Value = token;
                return current;
            } else if (previousToken == ":") {
                if (!(current.LastIterator is IteratorValued))
                    throw new ArgumentException ("syntax error in expression, ':' found at unexpected position");
                if (string.IsNullOrEmpty (((IteratorValued)current.LastIterator).Type))
                    ((IteratorValued)current.LastIterator).Type = token;
                else
                    ((IteratorValued)current.LastIterator).Value = token;
                return current;
            } else if (previousToken == "+" || previousToken == "-") {
                return FindMatchSiblingIntegerToken (current, token, previousToken);
            } else if (previousToken == "[") {
                return FindMatchRangeBeginToken (current, token);
            } else if (previousToken == ",") {
                return FindMatchRangeEndToken (current, token);
            } else if (previousToken == "%") {
                return FindMatchModuloIntegerToken (current, token);
            } else {
                if (previousToken != "/") {
                    throw new ArgumentException ("syntax error in expression; '" + 
                        _expression + 
                        "' probably missing a slash '/' before token; '" + 
                        token + "'");
                }
                if (IsNumber (token)) {
                    current.AddIterator (new IteratorNumbered (int.Parse (token)));
                    return current;
                } else {
                    return FindMatchNamedToken (current, token);
                }
            }
        }
        
        /*
         * handles integer value following "+" and "-" tokens
         */
        private IteratorGroup FindMatchSiblingIntegerToken (IteratorGroup current, string token, string previousToken)
        {
            if (!IsNumber (token)) {
                throw new ArgumentException ("a sibling operator must have an integer number as its next token, syntax error close to; '" + 
                    token + "' in expression; '" + _expression + "'");
            }
            ((IteratorSibling)current.LastIterator).Offset = previousToken == "-" ? -int.Parse (token) : int.Parse (token);
            return current;
        }
        
        /*
         * handles integer value following "[" token
         */
        private IteratorGroup FindMatchRangeBeginToken (IteratorGroup current, string token)
        {
            if (!IsNumber (token)) {
                throw new ArgumentException ("start of range was not a number, syntax error at; '" + 
                    token + 
                    "' in expression; '" + 
                    _expression + "'");
            }
            current.AddIterator (new IteratorRange (int.Parse (token)));
            return current;
        }
        
        /*
         * handles integer value following "," token
         */
        private IteratorGroup FindMatchRangeEndToken (IteratorGroup current, string token)
        {
            if (!IsNumber (token)) {
                throw new ArgumentException ("end of range was not a number, syntax error at; '" + 
                    token + 
                    "' in expression; '" + 
                    _expression + "'");
            }
            ((IteratorRange)current.LastIterator).End = int.Parse (token);
            return current;
        }
        
        /*
         * handles integer value following "%" token
         */
        private IteratorGroup FindMatchModuloIntegerToken (IteratorGroup current, string token)
        {
            if (!IsNumber (token)) {
                throw new ArgumentException ("modulo was not a number, syntax error at; '" + 
                    token + 
                    "' in expression; '" + 
                    _expression + "'");
            }
            ((IteratorModulo)current.LastIterator).Modulo = int.Parse (token);
            return current;
        }

        /*
         * handles named tokens, both named ancestors, named children and regex-named children
         */
        private IteratorGroup FindMatchNamedToken (IteratorGroup current, string token)
        {
            if (token.StartsWith ("/")) {
                if (token.LastIndexOf ("/") == 0) {
                    throw new ArgumentException ("token; '" + 
                        token + 
                        "' in expression; '" + 
                        _expression + 
                        "' is not a valid regular expression");
                }
                current.AddIterator (new IteratorNamedRegex (token));
            } else if (token.StartsWith ("..")) {
                // named ancestor
                current.AddIterator (new IteratorNamedAncestor (token.Substring (2)));
            } else {
                current.AddIterator (new IteratorNamed (token));
            }
            return current;
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
