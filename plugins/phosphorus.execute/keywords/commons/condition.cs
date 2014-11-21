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
        /// the legal types of conditions you can create
        /// </summary>
        private enum Operator
        {
            /// <summary>
            /// equality
            /// </summary>
            Equals,

            /// <summary>
            /// inequality
            /// </summary>
            NotEquals,

            /// <summary>
            /// more than
            /// </summary>
            MoreThan,

            /// <summary>
            /// less than
            /// </summary>
            LessThan,

            /// <summary>
            /// more than or equals
            /// </summary>
            MoreThanEquals,

            /// <summary>
            /// less than or equals
            /// </summary>
            LessThanEquals,

            /// <summary>
            /// not, meaning "does not exist". checks if an expression returns anything, and if it does, Not evaluates to false.
            /// opposite of Exist
            /// </summary>
            Not,

            /// <summary>
            /// exist, meaning "do exist". checks if an expression returns anything, and if it does, Exist evaluates to true.
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
            _statementNode = statementNode;
        }

        /// <summary>
        /// returns true if statement evaluates to true
        /// </summary>
        public bool Evaluate ()
        {
            return Evaluate (_statementNode);
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

        /*
         * evaluates a comparison and returns true if evaluation yields true, otherwise false
         */
        private bool Evaluate (Node node)
        {
            bool retVal = false;
            Operator oper = GetOperator (node);
            Node lhsNode;
            object lhs = GetLeftHandSide (node, oper, out lhsNode);
            if (oper == Operator.Exist) {
                retVal = CheckExistence (node, lhs);
            } else if (oper == Operator.Not) {
                retVal = !CheckExistence (node, lhs);
            } else {
                Node rhsNode;
                object rhs = GetRightHandSide (node, out rhsNode);
                retVal = CompareValues (lhs, rhs, lhsNode, rhsNode, oper);
            }
            return EvaluateRelatedComparisons (node, retVal);
        }

        /*
         * checks related comparisons
         */
        private bool EvaluateRelatedComparisons (Node node, bool isTrue)
        {
            if (isTrue) {
                if (node.FirstChild != null) {

                    // checking nested "and" statements
                    isTrue = EvaluateConsecutiveAndNodes (node.FirstChild);
                }
                if (isTrue && (node.Name == "or" || node.Name == "and")) {

                    // checking consecutive "and" statements
                    Node nextSibling = node.NextSibling;
                    if (nextSibling != null && nextSibling.Name == "and")
                        isTrue = EvaluateConsecutiveAndNodes (nextSibling);
                }
            } else {
                if (node.FirstChild != null) {

                    // checking nested "or" statements
                    isTrue = EvaluateConsecutiveOrNodes (node.FirstChild);
                }
                if (!isTrue && (node.Name == "or" || node.Name == "and")) {

                    // checking consecutive "or" statements
                    Node nextSibling = node.NextSibling;
                    if (nextSibling != null && nextSibling.Name == "or")
                        isTrue = EvaluateConsecutiveOrNodes (nextSibling);
                }
            }
            return isTrue;
        }
        
        /*
         * evaluates consecutive "and" nodes
         */
        private bool EvaluateConsecutiveAndNodes (Node node)
        {
            bool retVal = true;
            Node next = GetNextComparisonInChain (node);
            while (next != null) {
                if (next.Name == "and") {
                    retVal = Evaluate (next);
                    if (!retVal)
                        break;
                } else {
                    break;
                }
                next = GetNextComparisonInChain (next.NextSibling);
            }
            if (!retVal && next != null)
                retVal = EvaluateConsecutiveOrNodes (next);
            return retVal;
        }

        /*
         * evaluates consecutive "or" nodes
         */
        private bool EvaluateConsecutiveOrNodes (Node node)
        {
            bool retVal = false;
            Node next = GetNextComparisonInChain (node);
            while (next != null) {
                if (next.Name == "or") {
                    retVal = Evaluate (next);
                    if (retVal)
                        break;
                }
                next = GetNextComparisonInChain (next.NextSibling);
            }
            if (retVal && next != null)
                retVal = EvaluateConsecutiveAndNodes (next);
            return retVal;
        }

        /*
         * returns next comparison node in current chain
         */
        private Node GetNextComparisonInChain (Node index)
        {
            if (index == null)
                return null;
            while (index != null) {
                if (index.Name == "or" || index.Name == "and")
                    break;
                index = index.NextSibling;
            }
            return index;
        }

        /*
         * compares the lhs to the rhs and returns true if comparison yields true, otherwise false
         */
        private bool CompareValues (object lhs, object rhs, Node lhsNode, Node rhsNode, Operator oper)
        {
            switch (oper) {
            case Operator.Equals:
                return Compare (lhs, rhs, lhsNode, rhsNode) == 0;
            case Operator.NotEquals:
                return Compare (lhs, rhs, lhsNode, rhsNode) != 0;
            case Operator.LessThan:
                return Compare (lhs, rhs, lhsNode, rhsNode) == -1;
            case Operator.MoreThan:
                return Compare (lhs, rhs, lhsNode, rhsNode) == 1;
            case Operator.LessThanEquals:
                return Compare (lhs, rhs, lhsNode, rhsNode) != 1;
            case Operator.MoreThanEquals:
                return Compare (lhs, rhs, lhsNode, rhsNode) != -1;
            }
            throw new ArgumentException ("shouldn't be here ...??");
        }

        /*
         * compares lhs to rhs and returns 0 if they are equal, -1 if lhs is "less" and +1 if rhs is "less"
         */
        private int Compare (object lhs, object rhs, Node lhsNode, Node rhsNode)
        {
            if (Expression.IsExpression (lhs)) {
                lhs = GetNodeListFromExpression (lhs as string, lhsNode);
            }
            if (Expression.IsExpression (rhs)) {
                rhs = GetNodeListFromExpression (rhs as string, rhsNode);
            }
            return CompareObjects (lhs, rhs);
        }

        /*
         * returns an object according to what type of expression we're dealing with
         */
        private object GetNodeListFromExpression (string expression, Node node)
        {
            var match = new Expression (expression).Evaluate (node);
            if (match.TypeOfMatch == Match.MatchType.Count)
                return match.Count;

            List<Node> retVal = new List<Node> ();
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
            return retVal;
        }

        /*
         * compares two objects against each other, and returns -1 if lhs is "less", 1 if rhs is "less", otherwise 0
         */
        private int CompareObjects (object lhs, object rhs)
        {
            if (lhs == null) {
                if (rhs == null)
                    return 0;
                return -1;
            } else if (rhs == null) {
                return 1;
            }

            int retVal = 0;

            // none of our objects are "null" here, and if they originally were expressions, then they're now "list<Node>" from matches,
            // so lhs and rhs is either a "List<Node>", or any other "System" type
            if (lhs.GetType () != rhs.GetType ()) {
                List<Node> lhsAsNodes = lhs as List<Node>;
                List<Node> rhsAsNodes = rhs as List<Node>;
                if (lhsAsNodes != null) {
                    retVal = -1;
                    foreach (Node idx in lhsAsNodes) {
                        retVal = CompareObjects (idx.Value, rhs);
                        if (retVal != 0)
                            break;
                    }
                } else if (rhsAsNodes != null) {
                    retVal = 1;
                    foreach (Node idx in rhsAsNodes) {
                        retVal = CompareObjects (lhs, idx.Value);
                        if (retVal != 0)
                            break;
                    }
                } else {
                    rhs = Convert.ChangeType (rhs, lhs.GetType (), System.Globalization.CultureInfo.InvariantCulture);
                    MethodInfo method = lhs.GetType ().GetMethod ("CompareTo", new Type[] { typeof(object) });
                    retVal = (int)method.Invoke (lhs, new object[] { rhs });
                }
            } else if (lhs is List<Node>) {

                // comparing node lists
                retVal = CompareNodeLists (lhs as List<Node>, rhs as List<Node>);
            } else {

                // using reflection to find "CompareTo" method of type
                MethodInfo method = lhs.GetType ().GetMethod ("CompareTo", new Type[] { typeof(object) });
                retVal = (int)method.Invoke (lhs, new object[] { rhs });
            }
            return retVal;
        }

        /*
         * compares two node lists for equality
         */
        private int CompareNodeLists (List<Node> lhs, List<Node> rhs)
        {
            if (lhs.Count < rhs.Count) {
                return -1;
            } else if (rhs.Count < lhs.Count) {
                return 1;
            } else {
                for (int idxNo = 0; idxNo < lhs.Count; idxNo++) {
                    int retVal = lhs [idxNo].CompareTo (rhs [idxNo]);
                    if (retVal != 0)
                        return retVal;
                }
            }
            return 0;
        }

        /*
         * checks for existence of expression, object or string
         */
        private bool CheckExistence (Node node, object lhs)
        {
            if (lhs is string) {
                string lhsStr = lhs as string;
                if (Expression.IsExpression (lhsStr)) {
                    var match = new Expression (lhsStr).Evaluate (node);
                    return match.Count > 0;
                } else {
                    return true;
                }
            } else {
                return true;
            }
        }

        /*
         * returns left hand side of comparison
         */
        private object GetLeftHandSide (Node node, Operator oper, out Node lhsNode)
        {
            if (oper == Operator.Not) {
                Node firstChild = node.FirstChild;
                if (firstChild == null)
                    throw new ArgumentException ("syntax error in comparison, no value beneath comparison node; '" + node.Name + "'");
                else if (firstChild.Name != string.Empty)
                    throw new ArgumentException ("syntax error in comparison, unexpected 'name' of node; '" + firstChild.Name + "'");
                else if (firstChild.Value == null)
                    throw new ArgumentException ("syntax error in comparison, 'value' didn't exist");
                lhsNode = firstChild;
                return firstChild.Value;
            }
            if (node.Value == null)
                throw new ArgumentException ("syntax error in comparison, 'value' didn't exist");
            lhsNode = node;
            return node.Value;
        }

        /*
         * returns right hand side of comparison
         */
        private object GetRightHandSide (Node node, out Node rhsNode)
        {
            rhsNode = node.FirstChild;
            if (rhsNode.Value == null) {
                // "node" comparison, meaning rhs is a static node
                return new List<Node> (rhsNode.Children);
            }
            return rhsNode.Value;
        }

        /*
         * returns operator for comparison
         */
        private Operator GetOperator (Node node)
        {
            if (node.FirstChild == null) {
                return Operator.Exist;
            } else {
                switch (node.FirstChild.Name) {
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
                }
                if (node.Name == "!")
                    return Operator.Not;
                return Operator.Exist;
            }
        }
    }
}

