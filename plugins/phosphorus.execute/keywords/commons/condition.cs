/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute
{
    /// <summary>
    /// class wrapping any statement that somehow yields a condition, such as "pf.if" and "pf.while"
    /// </summary>
    public class Condition
    {
        /// <summary>
        /// the different types of legal conditions you can create
        /// </summary>
        private enum Operator
        {
            /// <summary>
            /// equality, token '='
            /// </summary>
            Equals,

            /// <summary>
            /// inequality, token '!='
            /// </summary>
            NotEquals,

            /// <summary>
            /// more than, token '>'
            /// </summary>
            MoreThan,

            /// <summary>
            /// less than, token '<'
            /// </summary>
            LessThan,

            /// <summary>
            /// more than or equals, token '>='
            /// </summary>
            MoreThanEquals,

            /// <summary>
            /// less than or equals, token '<='
            /// </summary>
            LessThanEquals,

            /// <summary>
            /// not, meaning "does not exist". checks if an expression returns anything, and if it does, Not evaluates to false.
            /// opposite of Exist, token '!', but token is in "front" of epxression or constant, meaning it changes position
            /// with the expression or constant you wish to "not"
            /// </summary>
            Not,

            /// <summary>
            /// exist, meaning "do exist". checks if an expression returns anything, and if it does, Exist evaluates to true.
            /// opposite of "Not". has no token, but is the default logical operator being used if no operator is given
            /// opposite of Not
            /// </summary>
            Exist
        }

        private Node _statementNode;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.execute.Condition"/> class
        /// </summary>
        /// <param name="statementNode">the node of the conditional statement</param>
        public Condition (Node statementNode)
        {
            if (statementNode == null)
                throw new ArgumentException ("you must submit a node to Condition for it to be able to evaluate a statement, statementNode was null");
            _statementNode = statementNode;
        }

        /// <summary>
        /// returns true if statement evaluates to true
        /// </summary>
        public bool Evaluate ()
        {
            return EvaluateStatement (_statementNode);
        }

        /// <summary>
        /// returns all execution lambda objects beneath the statement to execute if statement evaluates to true
        /// </summary>
        /// <value>The execution lambdas.</value>
        public IEnumerable<Node> ExecutionLambdas
        {
            get {
                foreach (Node idxChild in _statementNode.Children) {
                    if (idxChild.Name == "lambda" || idxChild.Name == "pf.lambda")
                        yield return idxChild;
                }
            }
        }

        private bool EvaluateStatement (Node currentStatement)
        {
            Operator oper = GetOperator (currentStatement);
            switch (oper) {
            case Operator.Exist:
                return (Exist (currentStatement) && EvaluateRelatedAnd (currentStatement)) || EvaluateRelatedOr (currentStatement);
            case Operator.Not:
                return (!Exist (currentStatement) && EvaluateRelatedAnd (currentStatement)) || EvaluateRelatedOr (currentStatement);
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

        private bool Exist (Node currentStatement)
        {
            return new Expression (currentStatement.Get<string> ()).Evaluate (currentStatement).Count > 0;
        }

        private int Compare (Node currentStatement)
        {
            var lhs = GetNodeList (currentStatement);
            var rhs = GetNodeList (currentStatement.FirstChild);
            if (lhs.Count < rhs.Count)
                return -1;
            if (lhs.Count > rhs.Count)
                return 1;
            for (int idxNo = 0; idxNo < lhs.Count; idxNo++) {
                int idxCompare = lhs [idxNo].CompareTo (rhs [idxNo]);
                if (idxCompare != 0)
                    return idxCompare;
            }
            return 0;
        }

        private List<Node> GetNodeList (Node currentStatement)
        {
            if (Expression.IsExpression (currentStatement.Value)) {
                List<Node> retVal = new List<Node> ();
                var match = new Expression (currentStatement.Get<string> ()).Evaluate (currentStatement);
                if (match.TypeOfMatch == Match.MatchType.Count) {
                    retVal.Add (new Node (string.Empty, match.Count));
                } else {
                    foreach (Node idxMatch in match.Matches) {
                        switch (match.TypeOfMatch) {
                        case Match.MatchType.Name:
                            retVal.Add (new Node (string.Empty, idxMatch.Name));
                            break;
                        case Match.MatchType.Node:
                            retVal.Add (idxMatch);
                            break;
                        case Match.MatchType.Path:
                            retVal.Add (new Node (string.Empty, idxMatch.Path));
                            break;
                        case Match.MatchType.Value:
                            retVal.Add (new Node (string.Empty, idxMatch.Value));
                            break;
                        }
                    }
                }
                return retVal;
            }
            if (currentStatement.Value == null)
                return new List<Node> (currentStatement.Children);
            return new List<Node> (new Node[] { new Node (string.Empty, currentStatement.Value) });
        }

        private bool EvaluateRelatedAnd (Node currentStatement)
        {
            Node nextChild = FindNextStatement (currentStatement.FirstChild, "and");
            if (nextChild != null) {
                if (!EvaluateStatement (nextChild))
                    return false;
            }
            if (currentStatement != _statementNode) {
                Node nextSibling = FindNextStatement (currentStatement.NextSibling, "and");
                if (nextSibling != null) {
                    if (!EvaluateStatement (nextSibling))
                        return false;
                }
            }
            return true;
        }

        private bool EvaluateRelatedOr (Node currentStatement)
        {
            Node nextChild = FindNextStatement (currentStatement.FirstChild, "or");
            if (nextChild != null) {
                if (EvaluateStatement (nextChild))
                    return true;
            }
            if (currentStatement != _statementNode) {
                Node nextSibling = FindNextStatement (currentStatement.NextSibling, "or");
                if (nextSibling != null) {
                    if (EvaluateStatement (nextSibling))
                        return true;
                }
            }
            return false;
        }

        private Node FindNextStatement (Node node, string type)
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
        
        private Operator GetOperator (Node currentStatement)
        {
            switch (currentStatement.FirstChild.Name) {
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
                if (currentStatement.Name == "!")
                    return Operator.Not;
                return Operator.Exist;
            }
        }
    }
}

