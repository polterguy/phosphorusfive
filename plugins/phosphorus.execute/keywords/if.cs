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
    /// class wrapping execution engine keyword "if", which allows for branching or conditional execution of nodes
    /// </summary>
    public static class pfIf
    {
        private enum Operator
        {
            Equals,
            NotEquals,
            MoreThan,
            LessThan,
            MoreThanEquals,
            LessThanEquals,
            Not,
            Exist
        }

        /// <summary>
        /// if keyword for execution engine
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.if")]
        private static void pf_if (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Count == 0)
                throw new ArgumentException ("syntax error in [pf.if], no children makes for invalid statement");

            if (Evaluate (e.Args, e.Args.FirstChild)) {
                foreach (Node idxExe in FindExecutionNodes (e.Args)) {
                    context.Raise (idxExe.Name, idxExe);
                }
            }
        }

        /*
         * returns all "lambda" nodes beneath root execution node
         */
        private static IEnumerable<Node> FindExecutionNodes (Node node)
        {
            foreach (Node idxChild in node.Children) {
                if (idxChild.Name == "lambda" || idxChild.Name == "pf.lambda")
                    yield return idxChild;
            }
        }

        /*
         * evaluates a comparison and returns true if evaluation yields true, otherwise false
         */
        private static bool Evaluate (Node node, Node nextComp)
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
            if (retVal) {
                if (node.FirstChild != null) {

                    // checking nested "and" statements
                    retVal = EvaluateConsecutiveAndNodes (node.FirstChild);
                }
                if (retVal && nextComp != null) {

                    // checking consecutive "and" statements
                    retVal = EvaluateConsecutiveAndNodes (nextComp);
                }
            } else {
                if (node.FirstChild != null) {

                    // checking nested "and" statements
                    retVal = EvaluateConsecutiveOrNodes (node.FirstChild);
                }
                if (!retVal && nextComp != null) {
                    retVal = EvaluateConsecutiveOrNodes (nextComp);
                }
            }
            return retVal;
        }
        
        /*
         * evaluates consecutive "and" nodes
         */
        private static bool EvaluateConsecutiveAndNodes (Node node)
        {
            bool retVal = true;
            Node next = GetNextComparisonInChain (node);
            while (next != null) {
                if (next.Name == "and") {
                    retVal = Evaluate (next, next.NextSibling);
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
        private static bool EvaluateConsecutiveOrNodes (Node node)
        {
            bool retVal = false;
            Node next = GetNextComparisonInChain (node);
            while (next != null) {
                if (next.Name == "or") {
                    retVal = Evaluate (next, next.NextSibling);
                    if (retVal)
                        break;
                } else {
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
        private static Node GetNextComparisonInChain (Node index)
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
        private static bool CompareValues (object lhs, object rhs, Node lhsNode, Node rhsNode, Operator oper)
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
        private static int Compare (object lhs, object rhs, Node lhsNode, Node rhsNode)
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
        private static object GetNodeListFromExpression (string expression, Node node)
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
        private static int CompareObjects (object lhs, object rhs)
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
        private static int CompareNodeLists (List<Node> lhs, List<Node> rhs)
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
        private static bool CheckExistence (Node node, object lhs)
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
        private static object GetLeftHandSide (Node node, Operator oper, out Node lhsNode)
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
        private static object GetRightHandSide (Node node, out Node rhsNode)
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
        private static Operator GetOperator (Node node)
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

