/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Globalization;
using phosphorus.core;
using phosphorus.expressions.exceptions;
using phosphorus.expressions.iterators;

// ReSharper disable UnusedParameter.Local

namespace phosphorus.expressions
{
    /// <summary>
    ///     expression class, for retrieving and changing values in node trees, according to
    ///     pf.lambda expressions
    /// </summary>
    public class Expression
    {
        // contains actual expression we're evaluating
        private readonly string _expression;
        private ApplicationContext _context;
        // these next two buggers are kept around to provide contextual information for exceptions,
        // among other things, and to make conversions possible
        private Node _evaluatedNode;
        /*
         * private ctor, to make sure we can extend creation logic in the future
         */

        private Expression (string expression)
        {
            // ps, postponing syntax checking until "Evaluate", since we've got "context" in Evaluate
            _expression = expression;
        }

        /// <summary>
        ///     initializes a new instance of the <see cref="phosphorus.expressions.Expression" /> class
        /// </summary>
        /// <param name="expression">expression to evaluate</param>
        public static Expression Create (string expression) { return new Expression (expression); }

        /// <summary>
        ///     evaluates expression for given <see cref="phosphorus.core.Node" />, and returns
        ///     <see cref="phosphorus.expressions.Match" /> object wrapping all matches for
        ///     evaluated expression
        /// </summary>
        public Match Evaluate (Node node, ApplicationContext context)
        {
            // verifying we've got an actual expression, since expression should be finished formatted
            // at this point, we can use "Exact" version
            if (!XUtil.IsExpression (_expression))
                throw new ExpressionException (_expression, node, context);

            // storing these bugger for later references, used in exceptions, among other things
            _evaluatedNode = node;
            _context = context;

            // creating our "root group iterator"
            var current = new IteratorGroup (node);
            string typeOfExpression = null, previousToken = null;

            // Tokenizer uses StringReader to tokenize, making sure tokenizer is disposed when finished
            using (var tokenizer = new Tokenizer (_expression)) {
                // looping through every token in espression, building up our Iterator tree hierarchy
                foreach (var idxToken in tokenizer.Tokens) {
                    if (previousToken == "?") {
                        // this is our last token, storing it as "expression type", before ending iteration
                        typeOfExpression = idxToken;
                        break;
                    }
                    if (idxToken != "?") {
                        // ignoring "?", handled in next iteration

                        // building expression tree
                        current = AppendToken (current, idxToken, previousToken);
                    }

                    // storing previous token, since some iterators are dependent upon knowing it
                    previousToken = idxToken;
                }
            }

            // creating a Match object, and returning to caller
            return CreateMatchFromIterator (current, typeOfExpression);
        }

        /*
         * handles an expression iterator token
         */

        private IteratorGroup AppendToken (
            IteratorGroup current,
            string token,
            string previousToken)
        {
            switch (token) {
                case "(":

                    // opening new group
                    return new IteratorGroup (current);
                case ")":

                    // closing group
                    if (current.ParentGroup == null) // making sure there's actually an open group first
                        throw new ExpressionException (
                            _expression,
                            "Closing parenthesis ')' has no matching '(' in expression.",
                            _evaluatedNode,
                            _context);
                    return current.ParentGroup;
                case "/":

                    // new token iterator
                    if (previousToken == "/") {
                        // two slashes "//" preceding each other, hence we're looking for a named value,
                        // where its name is string.Empty
                        current.AddIterator (new IteratorNamed (string.Empty));
                    }
                    break;
                case "|":
                case "&":
                case "^":
                case "!":

                    // boolean algebraic operator, opening up a new sub-expression
                    LogicalToken (current, token, previousToken);
                    break;
                case "@":

                    // reference expression, but only if it is the first token in expression
                    if (previousToken != null) {
                        DefaultToken (current, token, previousToken, _context);
                    } else if (current.IsReference) {
// making sure reference expressions can only be declared once
                        throw new ExpressionException (
                            _expression,
                            "You cannot declare your expression to be a reference expression more than once.",
                            _evaluatedNode,
                            _context);
                    }
                    current.IsReference = true;
                    break;
                case "..":

                    // root node token
                    current.AddIterator (new IteratorRoot ());
                    break;
                case "*":

                    // all children token
                    current.AddIterator (new IteratorChildren ());
                    break;
                case "**":

                    // flatten descendants token
                    current.AddIterator (new IteratorFlatten ());
                    break;
                case ".":

                    // parent node token
                    current.AddIterator (new IteratorParent ());
                    break;
                case "#":

                    // reference node token
                    current.AddIterator (new IteratorReference ());
                    break;
                case "<":

                    // left shift token
                    current.AddIterator (new IteratorShiftLeft ());
                    break;
                case ">":

                    // right shift token
                    current.AddIterator (new IteratorShiftRight ());
                    break;
                default:

                    // handles everything else
                    DefaultToken (current, token, previousToken, _context);
                    break;
            }

            // defaulting to returning what we came in with
            return current;
        }

        /*
         * handles "|", "&", "!" and "^" tokens
         */

        private static void LogicalToken (IteratorGroup current, string token, string previousToken)
        {
            switch (token) {
                case "|":

                    // OR logical boolean algebraic operator
                    current.AddLogical (new Logical (Logical.LogicalType.Or));
                    break;
                case "&":

                    // AND logical boolean algebraic operator
                    current.AddLogical (new Logical (Logical.LogicalType.And));
                    break;
                case "!":

                    // NOT logical boolean algebraic operator
                    current.AddLogical (new Logical (Logical.LogicalType.Not));
                    break;
                case "^":

                    // XOR logical boolean algebraic operator
                    current.AddLogical (new Logical (Logical.LogicalType.Xor));
                    break;
            }
        }

        /*
         * handles all other tokens, such as "named tokens" and "valued tokens"
         */

        private void DefaultToken (
            IteratorGroup current,
            string token,
            string previousToken,
            ApplicationContext context)
        {
            if (token.StartsWith ("=")) {
                // some type of value token, either normal value, or regex value
                ValueToken (current, token);
            } else if (token.StartsWith ("[")) {
                // range iterator token
                RangeToken (current, token);
            } else if (token.StartsWith ("..") && token.Length > 2) {
                // named ancestor token
                current.AddIterator (new IteratorNamedAncestor (token.Substring (2)));
            } else if (token.StartsWith ("%")) {
                // modulo token
                ModuloToken (current, token);
            } else if (token.StartsWith ("-") || token.StartsWith ("+")) {
                // modulo token
                SiblingToken (current, token);
            } else if (token.StartsWith ("/")) {
                // named regex token
                current.AddIterator (new IteratorNamedRegex (token, _expression, _evaluatedNode, _context));
            } else {
                if (Utilities.IsNumber (token)) {
                    // numbered child token
                    current.AddIterator (new IteratorNumbered (int.Parse (token)));
                } else {
                    // defaulting to "named iterator", making sure we escape any prepending back slashes,
                    // to support escaped "\", numbers, "..xx" named nodes, and similar constructs
                    if (token.StartsWith ("\\"))
                        token = token.Substring (1);
                    current.AddIterator (new IteratorNamed (token));
                }
            }
        }

        /*
         * value token, either a regular expression token, or a normal value comparison token,
         * optionally with a type declaration
         */

        private void ValueToken (IteratorGroup current, string token)
        {
            if (token.IndexOf ('/') == 1) {
                // value token, with regular expression
                ValueTokenRegex (current, token);
            } else {
                // value token, not regex, possibly a type declaration though
                ValueTokenNormal (current, token);
            }
        }

        /*
         * creates a value token, which is also a regular expression
         */

        private void ValueTokenRegex (IteratorGroup current, string token)
        {
            token = token.Substring (1); // removing equal sign (=)
            current.AddIterator (new IteratorValuedRegex (token, _expression, _evaluatedNode, _context));
        }

        /*
         * creates a value token, which is not a regular expression
         */

        private void ValueTokenNormal (IteratorGroup current, string token)
        {
            token = token.Substring (1); // removing equal sign (=)
            string type = null; // defaulting to "no type", meaning "string" type basically
            if (token.IndexOf ('\\') == 0) {
                // escaped equality token, necessary to support ":" as beginning of string values,
                // without having type information tranformation logic kicking in
                token = token.Substring (1);
            } else {
                // might contain a type declaration, checking here
                if (token.IndexOf (':') == 0) {
                    // yup, we've got a type declaration for our token ...
                    type = token.Substring (1, token.IndexOf (":", 1, StringComparison.Ordinal) - 1);
                    token = token.Substring (type.Length + 2);
                }
            }
            current.AddIterator (new IteratorValued (token, type, _context));
        }

        // TODO: cleanup, too long ...
        /*
         * creates a range token [x,y]
         */

        private void RangeToken (IteratorGroup current, string token)
        {
            // verifying token ends with "]"
            if (token [token.Length - 1] != ']')
                throw new ExpressionException (
                    _expression,
                    string.Format ("Syntax error in range token '{0}', no ']' at end of token", token),
                    _evaluatedNode,
                    _context);

            if (token.IndexOf (',') != -1) {
                token = token.Substring (1, token.Length - 2);
                var values = token.Split (',');

                // verifying token has only two integer values, separated by ","
                if (values.Length != 2)
                    throw new ExpressionException (
                        _expression,
                        string.Format ("Syntax error in range token '[{0}]', ranged iterator takes two integer values, separated by ','", token),
                        _evaluatedNode,
                        _context);
                var start = -1;
                var end = -1;
                var startStr = values [0].Trim ();
                var endStr = values [1].Trim ();
                if (startStr.Length > 0) {
                    if (!Utilities.IsNumber (startStr))
                        throw new ExpressionException (
                            _expression,
                            string.Format ("Syntax error in range token '[{0}]', expected number, found string", token),
                            _evaluatedNode,
                            _context);
                    start = int.Parse (startStr, CultureInfo.InvariantCulture);
                }
                if (endStr.Length > 0) {
                    if (!Utilities.IsNumber (endStr))
                        throw new ExpressionException (
                            _expression,
                            string.Format ("Syntax error in range token '[{0}]', expected number, found string", token),
                            _evaluatedNode,
                            _context);
                    end = int.Parse (endStr, CultureInfo.InvariantCulture);
                    if (end <= start)
                        throw new ExpressionException (
                            _expression,
                            string.Format ("Syntax error in range token '[{0}]', end must be larger than start", token),
                            _evaluatedNode,
                            _context);
                }
                current.AddIterator (new IteratorRange (start, end));
            } else {
                throw new ExpressionException (
                    _expression,
                    string.Format ("Syntax error in range token '{0}', expected two values, found one", token),
                    _evaluatedNode,
                    _context);
            }
        }

        /*
         * creates a range iterator
         */

        private void ModuloToken (IteratorGroup current, string token)
        {
            // removing "%" character
            token = token.Substring (1);

            // making sure we're given a number
            if (!Utilities.IsNumber (token))
                throw new ExpressionException (
                    _expression,
                    string.Format ("Syntax error in modulo token '{0}', expected integer value, found string", token),
                    _evaluatedNode,
                    _context);
            current.AddIterator (new IteratorModulo (int.Parse (token)));
        }

        /*
         * creates a sibling iterator
         */

        private void SiblingToken (IteratorGroup current, string token)
        {
            var intValue = token.Substring (1);
            var oper = token [0];
            var value = 1;
            if (intValue.Length > 0 && !Utilities.IsNumber (intValue))
                throw new ExpressionException (
                    _expression,
                    string.Format ("Syntax error in sibling token '{0}', expected integer value, found string", token),
                    _evaluatedNode,
                    _context);
            if (intValue.Length > 0)
                value = int.Parse (intValue);
            current.AddIterator (new IteratorSibling (value*(oper == '+' ? 1 : -1)));
        }

        /*
         * create a Match object from an Iterator group
         */

        private Match CreateMatchFromIterator (IteratorGroup group, string type)
        {
            // checking to see if we have open groups, which is an error
            if (group.ParentGroup != null)
                throw new ExpressionException (_expression, "Group in expression was not closed.", _evaluatedNode, _context);

            // parsing type of match
            // TODO: shares a lot of functionality with XUtil.ExpressionType, try to refactor
            string convert = null;
            if (type.Contains (".")) {
                convert = type.Substring (type.IndexOf ('.') + 1);
                type = type.Substring (0, type.IndexOf ('.'));
            }
            Match.MatchType matchType;
            switch (type) {
                case "node":
                case "value":
                case "count":
                case "name":
                case "path":
                    matchType = (Match.MatchType) Enum.Parse (typeof (Match.MatchType), type);
                    break;
                default:
                    throw new ExpressionException (
                        _expression,
                        string.Format ("'{0}' is an unknown type declaration for your expression", type),
                        _evaluatedNode,
                        _context);
            }

            // checking if expression is a reference expression, 
            // at which point we'll have to evaluate all referenced expressions
            if (group.IsReference) {
                // expression is a "reference expression", 
                // meaning we'll have to evaluate all referenced expressions
                var match = new Match (group.Evaluate, matchType, _context, convert);
                return EvaluateReferenceExpression (match, _context, convert);
            }
            // returning simple match object
            return new Match (@group.Evaluate, matchType, _context, convert);
        }

        /*
         * evaluates a reference expression
         */

        private Match EvaluateReferenceExpression (Match match, ApplicationContext context, string convert)
        {
            // making sure only 'value' and 'name' expression types can be "reference expressions"
            if (match.TypeOfMatch != Match.MatchType.name && match.TypeOfMatch != Match.MatchType.value) {
                // logical error, since only 'value' and 'name' expressions can possibly do something intelligent
                // as reference expressions
                throw new ExpressionException (
                    _expression,
                    "Only 'value' and 'name' expressions can be reference expressions",
                    _evaluatedNode,
                    context);
            }

            // looping through referenced expressions, yielding result from these referenced expression(s)
            var retVal = new Match (match.TypeOfMatch, context, convert);

            // looping through each match from reference expression
            foreach (var idxMatch in match) {
                // evaluating referenced expressions, but only if they actually contain expressions
                if (!XUtil.IsExpression (idxMatch.Value)) {
                    // current MatchEntity is not an expression, adding entity as it is
                    retVal.Entities.Add (idxMatch);
                } else {
                    // current MatchEntity contains an expression as its value, evaluating expression, and
                    // adding result of expression
                    // TODO: support formatting expressions through delegate callbacks, such that XUtil.Iterate
                    // and similar constructs can handle reference expressions, through callback, where referenced
                    // expression contains formatting parameters
                    var innerMatch = Create (Utilities.Convert<string> (idxMatch.Value, context)).Evaluate (idxMatch.Node, context);
                    foreach (var idxInner in innerMatch) {
                        retVal.Entities.Add (idxInner);
                    }
                }
            }

            // returning new Match, created by evaluating given "match" parameter
            return retVal;
        }
    }
}