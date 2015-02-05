
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Globalization;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions.iterators;

namespace phosphorus.expressions
{
    /// <summary>
    /// expression class, for retrieving and changing values in node trees according to
    /// pf.lambda expressions
    /// </summary>
    public class Expression
    {
        private string _expression;

        /*
         * private ctor, to make sure we can extend creation logic in the future
         */
        private Expression (string expression)
        {
            if (!XUtil.IsExpression (expression))
                throw new ArgumentException (string.Format ("'{0}' is not a valid expression", expression));
            _expression = expression;
        }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.Expression"/> class
        /// </summary>
        /// <param name="expression">execution engine expression</param>
        public static Expression Create (string expression)
        {
            return new Expression (expression);
        }

        /// <summary>
        /// evaluates expression for given <see cref="phosphorus.core.Node"/>  and returns 
        /// <see cref="phosphorus.execute.Expression.Match"/>
        /// </summary>
        public Match Evaluate (Node node, ApplicationContext context)
        {
            // creating our "root group iterator"
            IteratorGroup current = new IteratorGroup (node);
            string typeOfExpression = null, previousToken = null;

            // Tokenizer uses StringReader to tokenize, making sure tokenizer is disposed when finished
            using (Tokenizer tokenizer = new Tokenizer (_expression)) {

                // looping through every token in espression, building up our Iterator tree hierarchy
                foreach (string idxToken in tokenizer.Tokens) {
                    if (previousToken == "?") {

                        // this is our last token, storing it as "expression type", before ending iteration
                        typeOfExpression = idxToken;
                        break;
                    } else {

                        // building expression tree
                        current = FindMatches (current, idxToken, previousToken, context);
                    }

                    // storing previous token, since some iterators are dependent upon knowing it
                    previousToken = idxToken;
                }
            }

            // creating a Match object, and returning to caller
            return CreateMatchFromIterator (current, typeOfExpression, context);
        }

        /*
         * create a Match object from an Iterator group
         */
        private Match CreateMatchFromIterator (IteratorGroup group, string type, ApplicationContext context)
        {
            // checking to see if we have open groups, which is a bug
            if (group.ParentGroup != null)
                throw new ArgumentException ("unclosed group while evaluating; " + _expression);

            // parsing type of match
            Match.MatchType matchType = (Match.MatchType)Enum.Parse (typeof(Match.MatchType), type);

            // checking if expression is a reference expression, 
            // at which point we'll have to evaluate all referenced expressions
            if (group.IsReference) {

                // expression is a "reference expression", 
                // meaning we'll have to evaluate all referenced expressions
                var match = new Match (group.Evaluate, matchType, context);
                return EvaluateReferenceExpression (match, context);
            } else {

                // returning simple match object
                return new Match (group.Evaluate, matchType, context);
            }
        }
        
        /*
         * return matches according to token
         */
        private IteratorGroup FindMatches (
            IteratorGroup current, 
            string token, 
            string previousToken,
            ApplicationContext context)
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
            case "@":
                return FindMatchReferenceExpressionToken (current, token);
            default:
                return FindMatchDefaultToken (current, token, previousToken, context);
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
        private IteratorGroup FindMatchDefaultToken (
            IteratorGroup current, 
            string token, 
            string previousToken,
            ApplicationContext context)
        {
            if (previousToken == "=") {
                ((IteratorValued)current.LastIterator).Value = token;
                return current;
            } else if (previousToken == ":") {
                if (!(current.LastIterator is IteratorValued))
                    throw new ArgumentException ("syntax error in expression, ':' found at unexpected position");
                if (string.IsNullOrEmpty (((IteratorValued)current.LastIterator).Type)) {
                    ((IteratorValued)current.LastIterator).Type = token;
                    ((IteratorValued)current.LastIterator).Context = context;
                } else {
                    ((IteratorValued)current.LastIterator).Value = token;
                }
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
                if (Utilities.IsNumber (token)) {
                    current.AddIterator (new IteratorNumbered (int.Parse (token)));
                    return current;
                } else {
                    return FindMatchNamedToken (current, token);
                }
            }
        }

        /*
         * handles "reference" expressions
         */
        private IteratorGroup FindMatchReferenceExpressionToken (IteratorGroup current, string token)
        {
            if (current.IsReference) {
                throw new ArgumentException ("you cannot set the reference expression flag twice");
            }
            current.IsReference = true;
            return current;
        }
        
        /*
         * handles integer value following "+" and "-" tokens
         */
        private IteratorGroup FindMatchSiblingIntegerToken (IteratorGroup current, string token, string previousToken)
        {
            if (!Utilities.IsNumber (token)) {
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
            if (!Utilities.IsNumber (token)) {
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
            if (!Utilities.IsNumber (token)) {
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
            if (!Utilities.IsNumber (token)) {
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
         * evaluates a reference expression
         */
        private Match EvaluateReferenceExpression (Match match, ApplicationContext context)
        {
            // looping through referenced expressions, yielding result from these referenced expression(s)
            List<Node> newNodes = new List<Node> ();
            Match.MatchType? matchType = new Match.MatchType? ();

            // looping through each match from reference expression
            foreach (var idxMatch in match) {

                // evaluating reference expressions
                var innerMatch = Expression.Create (
                    Utilities.Convert<string> (idxMatch.Value, context))
                    .Evaluate (idxMatch.Node, context);

                // making sure all referenced expressions have the same type
                if (!matchType.HasValue)
                    matchType = innerMatch.TypeOfMatch;
                else if (matchType.Value != innerMatch.TypeOfMatch)
                    throw new ArgumentException ("a reference expression referenced two different types of expressions");

                // adding result from current referenced expression
                foreach (var idx in innerMatch) {
                    newNodes.Add (idx.Node);
                }
            }

            return new Match (newNodes, matchType.Value, context);
        }
    }
}
