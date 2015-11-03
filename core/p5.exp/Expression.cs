/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using p5.core;
using p5.exp.exceptions;
using p5.exp.iterators;

/// <summary>
///     Main namespace for the Expression engine in Phosphorus Five.
/// 
///     This namespace contains the Expression engine for Phosphorus Five, and is what allows you to compose
///     expressions, extracting node result-set from your p5.lambda execution tree.
/// </summary>
namespace p5.exp
{
    /// <summary>
    ///     The main expression class in Phosphorus Five.
    /// </summary>
    [Serializable]
    public class Expression : IComparable
    {
        // holds the actual expression
        private string _expression;

        // holds the root group iterator of expression
        private IteratorGroup _rootGroup;

        /*
         * private ctor, use static Create method to create instances.
         */
        private Expression (string expression, ApplicationContext context)
        {
            if (expression.StartsWith ("@")) {
                _expression = expression.Substring (1);
                Reference = true;
            } else {
                _expression = expression;
            }

            // checking to see if we should lazy build expression
            if (!_expression.Contains ("{0}"))
                BuildExpression (context); // building immediately, since there are no formatting parameters
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.Expression" /> class.
        /// </summary>
        /// <param name="expression">Expression to evaluate</param>
        /// <param name="context">Application context, necessary to convert types value iterators, among other things</param>
        public static Expression Create (string expression, ApplicationContext context)
        {
            return new Expression (expression, context);
        }

        /// <summary>
        ///     Returns true if referenced expressions should be evaluated, false if they should 
        ///     be returned as expression objects.
        /// </summary>
        /// <value><c>true</c> if reference; otherwise, <c>false</c>.</value>
        public bool Reference {
            get;
            private set;
        }

        /// <summary>
        ///     Returns actual expression in string format.
        /// </summary>
        /// <value>The expression value.</value>
        public string Value
        {
            get { return Reference ? "@" + _expression : _expression; }
        }

        /// <summary>
        ///     Returns the type of the expression.
        /// </summary>
        /// <value>The type of the expression.</value>
        public Match.MatchType ExpressionType {
            get;
            private set;
        }

        /// <summary>
        ///     Returns the type the expression value should be converted to before returned to caller during evaluation.
        /// </summary>
        /// <value>The casting type.</value>
        public string Casting {
            get;
            private set;
        }

        /// <summary>
        ///     Returns true if Expression is lazy binded.
        /// </summary>
        /// <value>The value.</value>
        public bool Lazy
        {
            get { return _rootGroup == null; }
        }

        public Expression Build (Node expressionNode, ApplicationContext context)
        {
            // checking to see if we're in lazy build mode, and if so, forcing build process
            if (_rootGroup == null)
                BuildExpression (context, expressionNode);
            return this;
        }

        /// <summary>
        ///     Evaluates expression for given <see cref="phosphorus.core.Node">node</see>.
        /// 
        ///     Returns a match object wrapping the result from your Expression.
        /// </summary>
        /// <param name="evaluatedNode">Node to evaluate expression for</param>
        /// <param name="context">Application context</param>
        /// <param name="exNode">Node that contained expression, optional, necessary for 
        /// formatting operations</param>
        public Match Evaluate (Node evaluatedNode, ApplicationContext context, Node exNode = null)
        {
            if (evaluatedNode == null)
                throw new ArgumentException ("No actual node given to evaluate.", "node");

            // checking to see if we're in lazy build mode ...
            if (_rootGroup == null)
                BuildExpression (context, exNode);

            // creating a Match object, and returning to caller
            // at this point, the entire Iterator hierarchy is already built, and the only remaining parts
            // is to set the node for the root group iterator and start evaluating, and pass the results to Match
            _rootGroup.GroupRootNode = evaluatedNode;
            return new Match (@_rootGroup.Evaluate (context), ExpressionType, context, Casting, Reference);
        }

        /*
         * Tokenizes and initializes expression object
         */
        private void BuildExpression (ApplicationContext context, Node exNode = null)
        {
            // checking to see if we should run formatting logic on expression before parsing iterators
            if (exNode != null)
                FormatExpression (context, exNode); // Lazy building, needs to apply formatting parameters

            _rootGroup = new IteratorGroup ();
            string previousToken = null; // needed to keep track of previous token
            var current = _rootGroup; // used as index iterator during tokenizing process

            // Tokenizer uses StringReader to tokenize, making sure tokenizer is disposed when finished
            using (var tokenizer = new Tokenizer (_expression)) {

                // looping through every token in espression, building up our Iterator tree hierarchy
                foreach (var idxToken in tokenizer.Tokens) {
                    if (previousToken == "?") {

                        // last token, initializing type of expression and conversion type, if given
                        InitializeType (idxToken);
                        break; // finished
                    } else if (previousToken == null && idxToken != "/" && idxToken != "?") {

                        // missing '/' before iterator
                        throw new ExpressionException (_expression, "Syntax error in expression, missing iterator declaration");
                    } else if (idxToken != "?") {

                        // '?' token is handled in next iteration
                        current = AppendToken (context, current, idxToken, previousToken);
                    } else if (previousToken == "/") {

                        // checking for '/' at end of token, before type declaration, which means we have an empty name iterator
                        current.AddIterator (new IteratorNamed (string.Empty));
                    }

                    // storing previous token, since some iterators are dependent upon knowing it
                    previousToken = idxToken;
                }
            }
            // checking to see if we have open groups, which is an error
            if (current.ParentGroup != null)
                throw new ExpressionException (_expression, "Group in expression was not closed. Probably missing ')' token.");
        }

        private void FormatExpression (ApplicationContext context, Node exNode)
        {
            var formatNodes = new List<Node> (from idxNode in exNode.Children where idxNode.Name == string.Empty select idxNode);

            // iterating all formatting parameters
            for (int idx = 0; idx < formatNodes.Count; idx++) {
                var val = formatNodes [idx].Value;
                var exVal = val as Expression;
                if (exVal != null) {
                    var match = exVal.Evaluate (formatNodes [idx], context, formatNodes [idx]);
                    val = "";
                    foreach (var idxMatch in match) {
                        val += Utilities.Convert<string> (idxMatch.Value, context);
                    }
                } else {
                    var strVal = val as string;
                    if (strVal != null) {
                        val = XUtil.FormatNode (formatNodes [idx], context);
                    }
                }
                _expression = _expression.Replace ("{" + idx + "}", Utilities.Convert<string> (val, context));
            }
        }

        /*
         * initializes expression type, and optionally a conversion type
         */
        private void InitializeType (string token)
        {
            // this is our last token, storing it as "expression type", and optionally a "convert", before ending iteration
            string typeOfExpression = null;
            if (token.IndexOf ('.') != -1) {
                Casting = token.Substring (token.IndexOf ('.') + 1);
                typeOfExpression = token.Substring (0, token.IndexOf ('.'));
            } else {
                typeOfExpression = token;
            }
            switch (typeOfExpression) {
                case "name":
                case "value":
                case "node":
                case "count":
                case "path":
                    break;
                default:
                    throw new ExpressionException (_expression, "Type declaration of expression was not valid");
            }
            ExpressionType = (Match.MatchType)Enum.Parse (typeof(Match.MatchType), typeOfExpression);
        }

        /*
         * handles an expression iterator token
         */
        private IteratorGroup AppendToken (
            ApplicationContext context,
            IteratorGroup current,
            string token,
            string previousToken)
        {
            switch (token) {
                case "(":

                    // opening new group, checking for empty name iterator first
                    if (previousToken == "/")
                        current.AddIterator (new IteratorNamed (string.Empty));
                    return new IteratorGroup (current);
                case ")":

                    // closing group, checking for empty name iterator first and missing group opening first
                    if (current.ParentGroup == null) // making sure there's actually an open group first
                        throw new ExpressionException (
                            _expression,
                            "Closing parenthesis ')' has no matching '(' in expression.");
                    if (previousToken == "/")
                        current.AddIterator (new IteratorNamed (string.Empty));
                    return current.ParentGroup;
                case "/":

                    // new token iterator
                    if (previousToken == "/") {
                        // two slashes "//" preceding each other, hence we're looking for a named value,
                        // where its name is string.Empty
                        current.AddIterator (new IteratorNamed (string.Empty));
                    } // else, ignoring token, since it's simply declaring the beginning (or the end) of another token
                    break;
                case "|":
                case "&":
                case "^":
                case "!":

                    // boolean algebraic operator, opening up a new sub-expression, checking for empty name iterator first
                    if (previousToken == "/")
                        current.AddIterator (new IteratorNamed (string.Empty));
                    LogicalToken (current, token, previousToken);
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
                    if (token [0] == '=') {
                        // some type of value token, either normal value, or regex value
                        ValueToken (context, current, token);
                    } else if (token [0] == '[') {
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
                    } else {
                        if (Utilities.IsNumber (token)) {
                            // numbered child token
                            current.AddIterator (new IteratorNumbered (int.Parse (token)));
                        } else {
                            // defaulting to "named iterator"
                            current.AddIterator (new IteratorNamed (token));
                        }
                    }
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
         * creates a valued token
         */
        private void ValueToken (ApplicationContext context, IteratorGroup current, string token)
        {
            token = token.Substring (1); // removing equal sign (=)
            string type = null; // defaulting to "no type", meaning "string" type basically

            // might contain a type declaration, checking here
            if (token.StartsWith (":")) {

                // yup, we've got a type declaration for our token ...
                type = token.Substring (1, token.IndexOf (":", 1, StringComparison.Ordinal) - 1);
                token = token.Substring (type.Length + 2);
            }
            current.AddIterator (new IteratorValued (token, type));
        }

        /*
         * creates a range token [x,y]
         */
        private void RangeToken (IteratorGroup current, string token)
        {
            // verifying token ends with "]"
            token = token.TrimEnd ();
            if (token [token.Length - 1] != ']')
                throw new ExpressionException (
                    _expression,
                    string.Format ("Syntax error in range token '{0}', no ']' at end of token", token));

            token = token.Substring (1, token.Length - 2); // removing [] square brackets

            if (token.IndexOf (',') == -1)
                throw new ExpressionException (
                    _expression,
                    string.Format ("Syntax error in range token '{0}', range token must have at the very least a ',' character.", token));

            var values = token.Split (',');

            // verifying token has only two integer values, separated by ","
            if (values.Length != 2)
                throw new ExpressionException (
                    _expression,
                    string.Format ("Syntax error in range token '[{0}]', ranged iterator takes two integer values, separated by ','", token));
            var start = -1;
            var end = -1;
            var startStr = values [0].Trim ();
            var endStr = values [1].Trim ();
            if (startStr.Length > 0) { // start index was explicitly given
                if (!Utilities.IsNumber (startStr))
                    throw new ExpressionException (
                        _expression,
                        string.Format ("Syntax error in range token '[{0}]', expected number, found string", token));
                start = int.Parse (startStr, CultureInfo.InvariantCulture);
            }
            if (endStr.Length > 0) { // end index was explicitly given
                if (!Utilities.IsNumber (endStr))
                    throw new ExpressionException (
                        _expression,
                        string.Format ("Syntax error in range token '[{0}]', expected number, found string", token));
                end = int.Parse (endStr, CultureInfo.InvariantCulture);
                if (end <= start)
                    throw new ExpressionException (
                        _expression,
                        string.Format ("Syntax error in range token '[{0}]', end must be larger than start", token));
            }
            current.AddIterator (new IteratorRange (start, end));
        }

        /*
         * creates a modulo token
         */
        private void ModuloToken (IteratorGroup current, string token)
        {
            // removing "%" character
            token = token.Substring (1);

            // making sure we're given a number
            if (!Utilities.IsNumber (token))
                throw new ExpressionException (
                    _expression,
                    string.Format ("Syntax error in modulo token '{0}', expected integer value, found string", token));
            current.AddIterator (new IteratorModulo (int.Parse (token)));
        }

        /*
         * creates a sibling token
         */
        private void SiblingToken (IteratorGroup current, string token)
        {
            var intValue = token.Substring (1);
            var oper = token [0];
            var value = 1;
            if (intValue.Length > 0 && !Utilities.IsNumber (intValue))
                throw new ExpressionException (
                    _expression,
                    string.Format ("Syntax error in sibling token '{0}', expected integer value, found string", token));
            if (intValue.Length > 0)
                value = int.Parse (intValue);
            current.AddIterator (new IteratorSibling (value*(oper == '+' ? 1 : -1)));
        }

        public int CompareTo (object rhs)
        {
            var rhsNode = rhs as Expression;
            if (rhsNode == null)
                return 1;
            return CompareTo (rhsNode);
        }
        
        public int CompareTo (Expression rhs)
        {
            if (ExpressionType != rhs.ExpressionType)
                return ExpressionType.CompareTo (rhs.ExpressionType);
            if (Reference != rhs.Reference)
                return Reference.CompareTo (rhs.Reference);
            if (Casting != rhs.Casting)
                return Casting.CompareTo (rhs.Casting);
            return Value.CompareTo (rhs.Value);
        }

        public override string ToString ()
        {
            return string.Format ("[Expression: Value={0}]", Value);
        }

        public override int GetHashCode ()
        {
            return ToString ().GetHashCode ();
        }
    }
}
