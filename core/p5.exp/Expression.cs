/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.Linq;
using System.Globalization;
using p5.core;
using p5.exp.iterators;
using p5.exp.exceptions;

/// <summary>
///     Main namespace for the Expression engine in Phosphorus Five
/// </summary>
namespace p5.exp
{
    /// <summary>
    ///     The main expression class in Phosphorus Five
    /// </summary>
    [Serializable]
    public class Expression : IComparable
    {
        // If expression is referencing other expressions, this field is true
        private bool _isReferenceExpression;

        // Type of expression (node, value, name, count)
        private Match.MatchType _expressionType;

        // If value(s) of expression results should be converted to another type, this will contains
        // the Hyperlambda type name (int, float, bool, etc)
        private string _convertResultsType;

        /*
         * Private ctor, use static Create method to create instances.
         */
        private Expression (string expression, ApplicationContext context)
        {
            Value = expression;
        }

        /// <summary>
        ///     Returns actual expression in string format
        /// </summary>
        /// <value>The expression value</value>
        public string Value {
            get;
            private set;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.Expression" /> class
        /// </summary>
        /// <param name="expression">Expression to evaluate</param>
        /// <param name="context">Application context, necessary to convert types value iterators, among other things</param>
        public static Expression Create (string expression, ApplicationContext context)
        {
            return new Expression (expression, context);
        }

        /// <summary>
        ///     Evaluates expression for given <see cref="phosphorus.core.Node">node</see>
        /// </summary>
        /// <param name="evaluatedNode">Node to evaluate expression for</param>
        /// <param name="context">Application Context</param>
        /// <param name="exNode">Node that contained expression, optional, necessary for 
        /// formatting operations</param>
        public Match Evaluate (ApplicationContext context, Node evaluatedNode, Node exNode, Node formattingNode = null)
        {
            if (evaluatedNode == null || exNode == null)
                throw new ExpressionException ("[null]", "No actual node or expression node given to evaluate");

            // Building expression
            var iteratorGroup = BuildExpression (context, evaluatedNode, exNode, formattingNode);

            // Creating a Match object, and returning to caller.
            return new Match (iteratorGroup.Evaluate (context), _expressionType, context, _convertResultsType, _isReferenceExpression);
        }

        /*
         * Tokenizes and initializes expression object
         */
        private IteratorGroup BuildExpression (
            ApplicationContext context, 
            Node evaluatedNode, 
            Node exNode,
            Node formattingNode)
        {
            // Setting up a return value
            var retVal = new IteratorGroup ();

            // Checking to see if we should run formatting logic on expression before parsing iterators
            var expression = FormatExpression (context, exNode, formattingNode);

            if (expression.StartsWith ("@")) {
                expression = expression.Substring (1);
                _isReferenceExpression = true;
            }

            string previousToken = null; // Needed to keep track of previous token
            var current = retVal; // Used as index iterator during tokenizing process

            // Tokenizer uses StringReader to tokenize, making sure tokenizer is disposed when finished
            using (var tokenizer = new Tokenizer (expression)) {

                // Looping through every token in espression, building up our Iterator tree hierarchy
                foreach (var idxToken in tokenizer.Tokens) {
                    if (previousToken == "?") {

                        // Last token, initializing type of expression and conversion type, if given
                        InitializeType (idxToken);
                        break; // finished
                    } else if (previousToken == null && idxToken != "/" && idxToken != "?" && idxToken != "(") {

                        // Missing '/' before iterator
                        throw new ExpressionException (Value, "Syntax error in expression, missing iterator declaration, after evaluation expression yields; " + expression);
                    } else if (idxToken != "?") {

                        // '?' token is handled in next iteration
                        current = AppendToken (context, current, idxToken, previousToken);
                    } else if (previousToken == "/") {

                        // Checking for '/' at end of token, before type declaration, which means we have an empty name iterator
                        current.AddIterator (new IteratorNamed (""));
                    }

                    // Storing previous token, since some iterators are dependent upon knowing it
                    previousToken = idxToken;
                }
            }
            // Checking to see if we have open groups, which is an error
            if (current.ParentGroup != null)
                throw new ExpressionException (Value, "Group in expression was not closed. Probably missing ')' token, after evaluation expression yields; " + expression);

            // Returning IteratorGroup to caller, but first making sure it has the right Root Group Iterator
            retVal.GroupRootNode = evaluatedNode;
            return retVal;
        }

        /*
         * Formats expression with formatting values recursively
         */
        private string FormatExpression (ApplicationContext context, Node exNode, Node formattingNode)
        {
            var retVal = Value;
            var formatNodes = (from idxNode in exNode.Children where idxNode.Name == "" select idxNode).ToList ();

            // Iterating all formatting parameters
            for (int idx = 0; idx < formatNodes.Count; idx++) {
                var val = formatNodes [idx].Value;
                var exVal = val as Expression;
                if (exVal != null) {
                    var match = exVal.Evaluate (context, formattingNode ?? formatNodes [idx], formatNodes [idx], formattingNode);
                    val = "";
                    foreach (var idxMatch in match) {
                        val += Utilities.Convert<string> (context, idxMatch.Value);
                    }
                } else {
                    val = XUtil.FormatNode (context, formatNodes [idx], formattingNode ?? formatNodes [idx]);
                }
                retVal = retVal.Replace ("{" + idx + "}", Utilities.Convert<string> (context, val));
            }
            return retVal;
        }

        /*
         * Initializes expression type, and optionally a conversion type
         */
        private void InitializeType (string token)
        {
            // this is our last token, storing it as "expression type", and optionally a "convert", before ending iteration
            string typeOfExpression = null;
            if (token.IndexOf ('.') != -1) {
                _convertResultsType = token.Substring (token.IndexOf ('.') + 1);
                typeOfExpression = token.Substring (0, token.IndexOf ('.'));
            } else {
                typeOfExpression = token;
            }
            switch (typeOfExpression) {
                case "name":
                case "value":
                case "node":
                case "count":
                    break;
                default:
                    throw new ExpressionException (Value, "Type declaration of expression was not valid");
            }
            _expressionType = (Match.MatchType)Enum.Parse (typeof(Match.MatchType), typeOfExpression);
        }

        /*
         * Handles an expression iterator token
         */
        private IteratorGroup AppendToken (
            ApplicationContext context,
            IteratorGroup current,
            string token,
            string previousToken)
        {
            switch (token) {
                case "(":

                    // Opening new group, checking for empty name iterator first
                    if (previousToken == "/")
                        current.AddIterator (new IteratorNamed (""));
                    return new IteratorGroup (current);
                case ")":

                    // Closing group, checking for empty name iterator first and missing group opening first
                    if (current.ParentGroup == null) // making sure there's actually an open group first
                        throw new ExpressionException (
                            Value,
                            "Closing parenthesis ')' has no matching '(' in expression.");
                    if (previousToken == "/")
                        current.AddIterator (new IteratorNamed (""));
                    return current.ParentGroup;
                case "/":

                    // New token iterator
                    if (previousToken == "/") {

                        // Two slashes "//" preceding each other, hence we're looking for a named value,
                        // where its name is ""
                        current.AddIterator (new IteratorNamed (""));
                    } // Else, ignoring token, since it's simply declaring the beginning (or the end) of another token
                    break;
                case "|":
                case "&":
                case "^":
                case "!":

                    // Boolean algebraic operator, opening up a new sibling-expression, checking for empty name iterator first
                    if (previousToken == "/")
                        current.AddIterator (new IteratorNamed (""));
                    LogicalToken (current, token, previousToken);
                    break;
                case "..":

                    // Sanity check!
                    if (previousToken != "/")
                        throw new ExpressionException (Value, "Missing '/' before possible iterator");

                    // Root node token
                    current.AddIterator (new IteratorRoot ());
                    break;
                case "*":

                    // Sanity check!
                    if (previousToken != "/")
                        throw new ExpressionException (Value, "Missing '/' before possible iterator");

                    // All children token
                    current.AddIterator (new IteratorChildren ());
                    break;
                case "**":

                    // Sanity check!
                    if (previousToken != "/")
                        throw new ExpressionException (Value, "Missing '/' before possible iterator");

                    // Flatten descendants token
                    current.AddIterator (new IteratorDescendants ());
                    break;
                case ".":

                    // Sanity check!
                    if (previousToken != "/")
                        throw new ExpressionException (Value, "Missing '/' before possible iterator");

                    // Parent node token
                    current.AddIterator (new IteratorParent ());
                    break;
                case "#":

                    // Sanity check!
                    if (previousToken != "/")
                        throw new ExpressionException (Value, "Missing '/' before possible iterator");

                    // Reference node token
                    current.AddIterator (new IteratorReference ());
                    break;
                case "<":

                    // Sanity check!
                    if (previousToken != "/")
                        throw new ExpressionException (Value, "Missing '/' before possible iterator");

                    // Left shift token
                    current.AddIterator (new IteratorShiftLeft ());
                    break;
                case ">":

                    // Sanity check!
                    if (previousToken != "/")
                        throw new ExpressionException (Value, "Missing '/' before possible iterator");

                    // Right shift token
                    current.AddIterator (new IteratorShiftRight ());
                    break;
                case "=$":

                    // Sanity check!
                    if (previousToken != "/")
                        throw new ExpressionException (Value, "Missing '/' before possible iterator");

                    // Right shift token
                    current.AddIterator (new IteratorDistinctValue ());
                    break;
                case "$":

                    // Sanity check!
                    if (previousToken != "/")
                        throw new ExpressionException (Value, "Missing '/' before possible iterator");

                    // Right shift token
                    current.AddIterator (new IteratorDistinctName ());
                    break;
                case "++":

                    // Sanity check!
                    if (previousToken != "/")
                        throw new ExpressionException (Value, "Missing '/' before possible iterator");

                    // Right shift token
                    current.AddIterator (new IteratorSiblingOlder ());
                    break;
                case "--":

                    // Sanity check!
                    if (previousToken != "/")
                        throw new ExpressionException (Value, "Missing '/' before possible iterator");

                    // Right shift token
                    current.AddIterator (new IteratorSiblingYounger ());
                    break;
                default:

                    // Sanity check!
                    if (previousToken != "/")
                        throw new ExpressionException (Value, "Missing '/' before possible iterator");

                    // Handles everything else
                    if (token.StartsWith ("=")) {

                        // Some type of value token, either normal value, or regex value
                        ValueToken (context, current, token);
                    } else if (token.StartsWith ("[")) {

                        // Range iterator token
                        RangeToken (current, token);
                    } else if (token.StartsWith ("..") && token.Length > 2) {

                        // Named ancestor token
                        current.AddIterator (new IteratorNamedAncestor (token.Substring (2)));
                    } else if (token.StartsWith ("%")) {

                        // Modulo token
                        ModuloToken (current, token);
                    } else if (token.StartsWith ("-") || token.StartsWith ("+")) {

                        // Sibling offset
                        SiblingToken (current, token);
                    } else if (token.StartsWith ("@")) {

                        // Sibling offset
                        ElderRelativeToken (current, token);
                    } else {

                        if (Utilities.IsNumber (token)) {

                            // Numbered child token
                            current.AddIterator (new IteratorNumberedChild (int.Parse (token)));
                        } else {

                            // Defaulting to "named iterator"
                            current.AddIterator (new IteratorNamed (token));
                        }
                    }
                    break;
            }

            // Defaulting to returning what we came in with
            return current;
        }

        /*
         * Handles "|", "&", "!" and "^" tokens
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
         * Creates a valued token
         */
        private void ValueToken (ApplicationContext context, IteratorGroup current, string token)
        {
            token = token.Substring (1); // Removing equal sign (=)
            string type = null; // Defaulting to "no type", meaning "string" type basically

            // Might contain a type declaration, checking here
            if (token.StartsWith (":")) {

                // Yup, we've got a type declaration for our token ...
                type = token.Substring (1, token.IndexOf (":", 1, StringComparison.Ordinal) - 1);
                token = token.Substring (type.Length + 2);
            }
            current.AddIterator (new IteratorValued (token, type));
        }

        /*
         * Creates a range token [x,y]
         */
        private void RangeToken (IteratorGroup current, string token)
        {
            // Verifying token ends with "]"
            token = token.TrimEnd ();
            if (token [token.Length - 1] != ']')
                throw new ExpressionException (
                    Value,
                    string.Format ("Syntax error in range token '{0}', no ']' at end of token", token));

            token = token.Substring (1, token.Length - 2); // Removing [] square brackets

            if (token.IndexOf (',') == -1)
                throw new ExpressionException (
                    Value,
                    string.Format ("Syntax error in range token '{0}', range token must have at the very least a ',' character.", token));

            var values = token.Split (',');

            // Verifyies token has only two integer values, separated by ","
            if (values.Length != 2)
                throw new ExpressionException (
                    Value,
                    string.Format ("Syntax error in range token '[{0}]', ranged iterator takes two integer values, separated by ','", token));
            var start = -1;
            var end = -1;
            var startStr = values [0].Trim ();
            var endStr = values [1].Trim ();
            if (startStr.Length > 0) { // start index was explicitly given
                if (!Utilities.IsNumber (startStr))
                    throw new ExpressionException (
                        Value,
                        string.Format ("Syntax error in range token '[{0}]', expected number, found string", token));
                start = int.Parse (startStr, CultureInfo.InvariantCulture);
            }
            if (endStr.Length > 0) { // end index was explicitly given
                if (!Utilities.IsNumber (endStr))
                    throw new ExpressionException (
                        Value,
                        string.Format ("Syntax error in range token '[{0}]', expected number, found string", token));
                end = int.Parse (endStr, CultureInfo.InvariantCulture);
                if (end <= start)
                    throw new ExpressionException (
                        Value,
                        string.Format ("Syntax error in range token '[{0}]', end must be larger than start", token));
            }
            current.AddIterator (new IteratorRange (start, end));
        }

        /*
         * Creates a modulo token
         */
        private void ModuloToken (IteratorGroup current, string token)
        {
            // Removing "%" character
            token = token.Substring (1);

            // Making sure we're given a number
            if (!Utilities.IsNumber (token))
                throw new ExpressionException (
                    Value,
                    string.Format ("Syntax error in modulo token '{0}', expected integer value, found string", token));
            current.AddIterator (new IteratorModulo (int.Parse (token)));
        }

        /*
         * Creates a sibling token
         */
        private void SiblingToken (IteratorGroup current, string token)
        {
            var intValue = token.Substring (1);
            var oper = token[0];
            var value = 1;
            if (intValue.Length > 0 && !Utilities.IsNumber (intValue))
                throw new ExpressionException (
                    Value,
                    string.Format ("Syntax error in sibling token '{0}', expected integer value, found string", token));
            if (intValue.Length > 0)
                value = int.Parse (intValue);
            current.AddIterator (new IteratorSibling (value * (oper == '+' ? 1 : -1)));
        }

        /*
         * Creates an elder relative token
         */
        private void ElderRelativeToken (IteratorGroup current, string token)
        {
            var name = token.Substring (1);
            current.AddIterator (new IteratorNamedElderRelative (name));
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
            if (_expressionType != rhs._expressionType)
                return _expressionType.CompareTo (rhs._expressionType);
            if (_isReferenceExpression != rhs._isReferenceExpression)
                return _isReferenceExpression.CompareTo (rhs._isReferenceExpression);
            if (_convertResultsType != rhs._convertResultsType)
                return _convertResultsType.CompareTo (rhs._convertResultsType);
            return Value.CompareTo (rhs.Value);
        }

        public override string ToString ()
        {
            return string.Format ("[:x:{0}]", Value);
        }

        public override int GetHashCode ()
        {
            return ToString ().GetHashCode ();
        }
    }
}
