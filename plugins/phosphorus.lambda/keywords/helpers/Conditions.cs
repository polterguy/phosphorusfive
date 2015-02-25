/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Linq;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.lambda.keywords.helpers
{
    /// <summary>
    ///     class wrapping any statement, that somehow yields a condition, such as "pf.if", and "pf.while"
    /// </summary>
    public class Conditions
    {
        // application context needed to create expressions, in case expressions have typed values
        private readonly ApplicationContext _context;
        // root node for our condition object
        private readonly Node _statementNode;
        private bool _hasEvaluated;
        // for simple exist statements, there's no need for a [lambda] execution object since everything
        // inside of condition object automatically becomes a "lambda"
        private bool _isSimpleExist;

        /// <summary>
        ///     initializes a new instance of the condition class
        /// </summary>
        /// <param name="statementNode">the node of the conditional statement</param>
        /// <param name="context">application context</param>
        public Conditions (Node statementNode, ApplicationContext context)
        {
            if (statementNode == null)
                throw new ApplicationException ("you must submit a node to Condition, for it to be able to evaluate");
            _statementNode = statementNode;
            _isSimpleExist = false;
            _context = context;
        }

        /// <summary>
        ///     returns all execution lambda objects beneath the statement
        ///     to execute if statement evaluates to true
        /// </summary>
        /// <value>The execution lambdas.</value>
        public IEnumerable<Node> ExecutionLambdas
        {
            get { return _statementNode.Children.Where (idxChild => idxChild.Name.StartsWith ("lambda")); }
        }

        /// <summary>
        ///     returns true if this is a simple "exists" Condition, meaning it has only one expression or constant
        ///     being evaluated, and nothing to evaluate it against, and no children nodes of type [lambda.xxx]
        /// </summary>
        /// <value><c>true</c> if this instance is simple exist; otherwise, <c>false</c></value>
        public bool IsSimpleExist
        {
            get
            {
                if (!_hasEvaluated)
                    throw new ArgumentException ("you cannot check if Condition is a simple exist, until Condition has been evaluated");

                // if statement node only contains left-hand-side, and there are no [lambda.xxx] objects beneath
                // condition node, then this is a "simple exist" Condition, and consumers of this class might
                // choose to execute "everything" beneath statement node, instead of relying upon [lambda.xxx] children
                // to exist
                return _isSimpleExist && _statementNode.FindAll (idx => idx.Name.StartsWith ("lambda")).GetEnumerator ().MoveNext () == false;
            }
        }

        /// <summary>
        ///     returns true if statement evaluates to true
        /// </summary>
        public bool Evaluate ()
        {
            _hasEvaluated = true;
            return EvaluateStatement (_statementNode);
        }

        /*
         * actual evaluation of statement, invokes itself recursively for all "related 'or' and/or 'and' statements" if it is required
         */

        private bool EvaluateStatement (Node currentStatement)
        {
            var oper = GetOperator (currentStatement);
            switch (oper) {
                case Operator.Exist:
                    if (currentStatement == _statementNode &&
                        FindNextCondition (currentStatement.FirstChildNotOf (string.Empty), "or") == null &&
                        FindNextCondition (currentStatement.FirstChildNotOf (string.Empty), "and") == null)
                        _isSimpleExist = true;
                    return (Exist (currentStatement) && EvaluateRelatedAnd (currentStatement)) || EvaluateRelatedOr (currentStatement);
                case Operator.Not:
                    return (!Exist (currentStatement.FirstChild) && EvaluateRelatedAnd (currentStatement)) || EvaluateRelatedOr (currentStatement);
                case Operator.Equals:
                    return (Compare (currentStatement) == 0 && EvaluateRelatedAnd (currentStatement)) || EvaluateRelatedOr (currentStatement);
                case Operator.NotEquals:
                    return (Compare (currentStatement) != 0 && EvaluateRelatedAnd (currentStatement)) || EvaluateRelatedOr (currentStatement);
                case Operator.LessThan:
                    return (Compare (currentStatement) == -1 && EvaluateRelatedAnd (currentStatement)) || EvaluateRelatedOr (currentStatement);
                case Operator.LessThanEquals:
                    return (Compare (currentStatement) != 1 && EvaluateRelatedAnd (currentStatement)) || EvaluateRelatedOr (currentStatement);
                case Operator.MoreThan:
                    return (Compare (currentStatement) == 1 && EvaluateRelatedAnd (currentStatement)) || EvaluateRelatedOr (currentStatement);
                case Operator.MoreThanEquals:
                    return (Compare (currentStatement) != -1 && EvaluateRelatedAnd (currentStatement)) || EvaluateRelatedOr (currentStatement);
                default:
                    throw new ArgumentException (); // should never come here, but since compiler vommits unless we have this, we've added a "dummy default"
            }
        }

        /*
         * evaluates existence
         */

        private bool Exist (Node currentStatement)
        {
            if (!XUtil.IsExpression (currentStatement.Value))
                return currentStatement.Value != null; // constant in value of node always evaluates to true!

            // making sure we format expression if necessary
            var exp = XUtil.IsFormatted (currentStatement) ? XUtil.FormatNode (currentStatement, _context) : currentStatement.Get<string> (_context);

            // creating a Match object
            var match = Expression.Create (exp).Evaluate (currentStatement, _context);

            // easy versions
            if (match.TypeOfMatch == Match.MatchType.count ||
                match.TypeOfMatch == Match.MatchType.path ||
                match.TypeOfMatch == Match.MatchType.node)
                return match.Count > 0;

            // slightly harder versions
            foreach (var idx in match) {
                switch (match.TypeOfMatch) {
                    case Match.MatchType.name:
                        if (string.Empty == idx.Value as string)
                            return false;
                        break;
                    case Match.MatchType.value:
                        if (idx.Value == null)
                            return false;
                        break;
                }
                // neither of above checks evaluated to true, hence statement is true,
                // regardless of what other nodes contains, or don't contain, as long as there
                // are more than 0 nodes in result
                break;
            }
            return match.Count > 0;
        }

        /*
         * compres two expressions or constants
         */

        private int Compare (Node currentStatement)
        {
            // constructing nodes for simplicity, since Node implements IComparable, which does our heavy lifting
            var lhs = new Node (string.Empty, XUtil.Single<object> (currentStatement, _context));
            var rhs = new Node (string.Empty, XUtil.Single<object> (currentStatement.FirstChildNotOf (string.Empty), _context));
            return lhs.CompareTo (rhs);
        }

        /*
         * recursively evaluates all related "and" conditions
         */

        private bool EvaluateRelatedAnd (Node currentStatement)
        {
            var nextChild = FindNextCondition (currentStatement.FirstChildNotOf (string.Empty), "and");
            if (nextChild != null) {
                if (!EvaluateStatement (nextChild))
                    return false;
            }
            if (currentStatement != _statementNode) {
                var nextSibling = FindNextCondition (currentStatement.NextSibling, "and");
                if (nextSibling != null) {
                    if (!EvaluateStatement (nextSibling))
                        return false;
                }
            }
            return true;
        }

        /*
         * recursively evaluates all "or" conditions
         */

        private bool EvaluateRelatedOr (Node currentStatement)
        {
            var nextChild = FindNextCondition (currentStatement.FirstChildNotOf (string.Empty), "or");
            if (nextChild != null) {
                if (EvaluateStatement (nextChild))
                    return true;
            }
            if (currentStatement != _statementNode) {
                var nextSibling = FindNextCondition (currentStatement.NextSibling, "or");
                if (nextSibling != null) {
                    if (EvaluateStatement (nextSibling))
                        return true;
                }
            }
            return false;
        }

        /*
         * finds next condition
         */

        private Node FindNextCondition (Node node, string type)
        {
            if (node == null)
                return null;
            if (node.Name == type)
                return node;
            node = node.NextSibling;
            if (type == "or") {
                while (node != null) {
                    if (node.Name == "or")
                        return node;
                    node = node.NextSibling;
                }
            }
            if (node != null && node.Name == type)
                return node;
            return null;
        }

        /*
         * returns the operator for the current condition
         */

        private Operator GetOperator (Node currentStatement)
        {
            if (currentStatement.FirstChildNotOf (string.Empty) == null)
                return Operator.Exist;
            switch (currentStatement.FirstChildNotOf (string.Empty).Name) {
                case "=":
                    return Operator.Equals;
                case "!=":
                    return Operator.NotEquals;
                case ">":
                    return Operator.MoreThan;
                case "<":
                    return Operator.LessThan;
                case ">=":
                    return Operator.MoreThanEquals;
                case "<=":
                    return Operator.LessThanEquals;
                default:
                    if ("!" == currentStatement.Get<string> (_context))
                        return Operator.Not;
                    return Operator.Exist;
            }
        }

        /// <summary>
        ///     the different types of legal conditions you can create
        /// </summary>
        private enum Operator
        {
            /// <summary>
            ///     equality, token '='
            /// </summary>
            Equals,

            /// <summary>
            ///     inequality, token '!='
            /// </summary>
            NotEquals,

            /// <summary>
            ///     more than, token '>'
            /// </summary>
            MoreThan,

            /// <summary>
            ///     less than, token '&lt;'
            /// </summary>
            LessThan,

            /// <summary>
            ///     more than or equals, token '>='
            /// </summary>
            MoreThanEquals,

            /// <summary>
            ///     less than or equals, token 'lt;='
            /// </summary>
            LessThanEquals,

            /// <summary>
            ///     not, meaning "does not exist". checks if an expression returns anything,
            ///     and if it does, Not evaluates to false. opposite of Exist, token '!',
            ///     but token is in "front" of epxression or constant, meaning it changes position
            ///     with the expression or constant you wish to "not evaluate"
            /// </summary>
            Not,

            /// <summary>
            ///     checks if an expression returns anything, and if it does, Exist evaluates to true.
            ///     opposite of "Not". has no token, but is the default logical operator being used,
            ///     if no operator is given
            /// </summary>
            Exist
        }
    }
}