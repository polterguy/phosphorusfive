/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Globalization;
using phosphorus.core;
using phosphorus.expressions.iterators;

namespace phosphorus.expressions
{
    public class Expression
    {
        private readonly string _expression;

        private Expression (string expression)
        {
            _expression = expression;
        }

        public static Expression Create (string expression)
        {
            if (!XUtil.IsExpression (expression))
                throw new ExpressionException (string.Format ("'{0}' is not a valid expression.", expression));
            return new Expression (expression);
        }

        public Match Evaluate (ApplicationContext context, Node node)
        {
            // creating our "root group iterator"
            var current = new IteratorGroup (node);

            string typeOfExpression = null, previousToken = null;
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
                    if (current.ParentGroup == null)
                        throw new ExpressionException (string.Format ("'{0}' had a missing opening parenthesis, or too many closing parenthesis.", _expression));
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
                        DefaultToken (current, token, previousToken);
                    } else if (current.IsReference) {
                        throw new ExpressionException (string.Format ("'{0}' had syntax errors, too many '@' characters at start of expression.", _expression));
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
                    DefaultToken (current, token, previousToken);
                    break;
            }

            // defaulting to returning what we came in with
            return current;
        }

        private static void LogicalToken (IteratorGroup current, string token, string previousToken)
        {
            switch (token) {
                case "|":

                    // OR logical boolean algebraic operator
                    current.AddLogical (new Logical (Logical.LogicalType.OR));
                    break;
                case "&":

                    // AND logical boolean algebraic operator
                    current.AddLogical (new Logical (Logical.LogicalType.AND));
                    break;
                case "!":

                    // NOT logical boolean algebraic operator
                    current.AddLogical (new Logical (Logical.LogicalType.NOT));
                    break;
                case "^":

                    // XOR logical boolean algebraic operator
                    current.AddLogical (new Logical (Logical.LogicalType.XOR));
                    break;
            }
        }

        private void DefaultToken (
            IteratorGroup current,
            string token,
            string previousToken)
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
                current.AddIterator (new IteratorNamedRegex (token, _expression));
            } else {
                if (Utilities.IsNumber (token)) {

                    // numbered child token
                    current.AddIterator (new IteratorNumbered (int.Parse (token)));
                } else {

                    // defaulting to "named iterator"
                    current.AddIterator (new IteratorNamed (token));
                }
            }
        }

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

        private void ValueTokenRegex (IteratorGroup current, string token)
        {
            token = token.Substring (1); // removing equal sign (=)
            current.AddIterator (new IteratorValuedRegex (token, _expression, _evaluatedNode, _context));
        }

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

        private void RangeToken (IteratorGroup current, string token)
        {
            // verifying token ends with "]"
            if (token [token.Length - 1] != ']')
                throw new ExpressionException (string.Format ("'{0}' had syntax errors, close to '{1}'.", _expression, token));

            if (token.IndexOf (',') == -1)
                throw new ExpressionException (string.Format ("'{0}' had syntax errors, close to '{1}'. Expected two values, found one.", _expression, token));

            token = token.Substring (1, token.Length - 2);
            var values = token.Split (',');

            // verifying token has only two integer values, separated by ","
            if (values.Length != 2)
                throw new ExpressionException (string.Format ("'{0}' was missing a ',' close to '{1}'.", _expression, token));

            var start = -1;
            var end = -1;
            var startStr = values [0].Trim ();
            var endStr = values [1].Trim ();
            if (startStr.Length > 0) {
                if (!Utilities.IsNumber (startStr))
                    throw new ExpressionException (string.Format ("'{0}' had syntax errors, expected an integer close to '{1}'", _expression, token));
                start = int.Parse (startStr, CultureInfo.InvariantCulture);
            }
            if (endStr.Length > 0) {
                if (!Utilities.IsNumber (endStr))
                    throw new ExpressionException (string.Format ("'{0}' had syntax errors, expected an integer, found '{1}'", _expression, token));
                end = int.Parse (endStr, CultureInfo.InvariantCulture);
                if (end <= start)
                    throw new ExpressionException (string.Format ("'{0}' had syntax errors, close to '{1}'. End must be larger than start.", _expression, token));
            }
            current.AddIterator (new IteratorRange (start, end));
        }

        private void ModuloToken (IteratorGroup current, string token)
        {
            // removing "%" character
            token = token.Substring (1);

            // making sure we're given a number
            if (!Utilities.IsNumber (token))
                throw new ExpressionException (string.Format ("'{0}' had syntax errors, close to '{1}'. Expected integer value.", _expression, token));
            current.AddIterator (new IteratorModulo (int.Parse (token)));
        }

        private void SiblingToken (IteratorGroup current, string token)
        {
            var intValue = token.Substring (1);
            var oper = token [0];
            var value = 1;
            if (intValue.Length > 0 && !Utilities.IsNumber (intValue))
                throw new ExpressionException (string.Format ("'{0}' had syntax errors, close to '{1}'. Expected integer value.", _expression, token));
            if (intValue.Length > 0)
                value = int.Parse (intValue);
            current.AddIterator (new IteratorSibling (value * (oper == '+' ? 1 : -1)));
        }

        private Match CreateMatchFromIterator (IteratorGroup group, string type)
        {
            // checking to see if we have open groups, which is an error
            if (group.ParentGroup != null)
                throw new ExpressionException (string.Format ("'{0}' had syntax errors, expression contains an unclosed group.", _expression));

            string tail = null;
            if (type.Contains ("[")) {
                tail = type.Substring (type.IndexOf ("["));
                type = type.Substring (0, type.IndexOf ("["));
            }

            Match.MatchType matchType;
            switch (type) {
                case "node":
                case "value":
                case "count":
                case "name":
                    matchType = (Match.MatchType) Enum.Parse (typeof (Match.MatchType), type);
                    break;
                default:
                    throw new ExpressionException (string.Format ("'{0}' had syntax errors, close to '{1}'. Unknown type declaration.", _expression, type));
            }

            // checking if expression is a reference expression, 
            // at which point we'll have to evaluate all referenced expressions
            if (group.IsReference) {

                // expression is a "reference expression", 
                // meaning we'll have to evaluate all referenced expressions
                var match = new Match (group.Evaluate, matchType);
                return EvaluateReferenceExpression (match, tail);
            }

            // returning simple match object
            return new Match (@group.Evaluate, matchType, tail);
        }

        private Match EvaluateReferenceExpression (Match match, ApplicationContext context, string tail)
        {
            // making sure only 'value' and 'name' expression types can be "reference expressions"
            if (match.TypeOfMatch != Match.MatchType.name && match.TypeOfMatch != Match.MatchType.value) {

                // logical error, since only 'value' and 'name' expressions can possibly do something intelligent
                // as reference expressions
                throw new ExpressionException (string.Format ("'{0}' had syntax errors, only 'value' and 'name' expressions can be reference expressions.", _expression));
            }

            // looping through referenced expressions, yielding result from these referenced expression(s)
            var retVal = new Match (match.TypeOfMatch, tail);

            // looping through each match from reference expression
            foreach (var idxMatch in match) {

                // evaluating referenced expressions, but only if they actually contain expressions
                if (!XUtil.IsExpression (idxMatch.Value)) {

                    // current MatchEntity is not an expression, adding entity as it is
                    retVal.Entities.Add (idxMatch);
                } else {

                    // current MatchEntity contains an expression as its value, evaluating expression, and
                    // adding result of expression
                    var innerMatch = Create ((string)idxMatch.Value).Evaluate (idxMatch.Node, context);
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
