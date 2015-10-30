/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Linq;
using p5.core;
using p5.exp;

/// \todo Make sure [and] has precedence in Conditions
/// <summary>
///     Contains helper classes for p5.lambda keywords.
/// 
///     Contains common helper classes for p5.lambda keywords.
/// </summary>
namespace p5.lambda.helpers
{
    /// <summary>
    ///     Class wrapping conditional statements.
    /// 
    ///     This class is used in for instance [while], [if] and [else-if], and wraps the conditions used to
    ///     figure out if statement yields true or not.
    /// 
    ///     There exists 8 basic types of conditions, and they're all defined through their <em>"operators"<em/>, or lack of operator.
    ///     These 8 types of conditions are;
    /// 
    ///     1. = is the equals operator.
    ///     2. != is the not-equals operator.
    ///     3. &gt; is the more-than operator.
    ///     4. &lt; is the less-than operator.
    ///     5. &gt;= is the more-than-or-equals operator.
    ///     6. &lt;= is the less-than-or-equals operator.
    ///     7. ! is the not operator.
    ///     8. Exists, has no operator, and is the default condition, if no operator exists.
    /// 
    ///     A condition is normally created through some Active Event reference, such as the [if] Active Event, for then to have a
    ///     value being checked, using <see cref="phosphorus.expressions.Expression">Expressions</see>, according to its 
    ///     condition in its main node. For instance, to check if a node exists, you might do something
    ///     like this;
    /// 
    ///     <pre>if:@/../5?node</pre>
    /// 
    ///     The above example will create an exists condition, and yield true if there are 6 or more nodes beneath the root node of your tree, since
    ///     it is checking for the existence of your 5th node, which is really your 6th of course. Below is an example of how to check if a value
    ///     of one node equals the constant of "foo";
    /// 
    ///     <pre>if:@/../0?value
    ///   =:foo
    ///   lambda
    ///     do-something-here</pre>
    /// 
    ///     The last example above, shows a crucial point, which is that if your condition is a complex compound condition, and not a simple exists
    ///     condition, then you have to create a [lambda.xxx] node beneath it, where you put your p5.lambda code to be executed if condition yields
    ///     true. If your condition is a simple exists condition, you do not need to create a [lambda.xxx] child for your condition, since then
    ///     by default, everything beneath your condition node will be executed using the [lambda] Active Event.
    /// 
    ///     You can also nest conditions, and create and and/or or conditions, which are to be checked in addition to your rot condition. For 
    ///     instance, to check if a node has a specific name, and a specific value, you might do something like this;
    /// 
    ///     <pre>if:@/../0?name
    ///   =:foo
    ///   and:@/../0?value
    ///     =:bar
    ///   lambda
    ///     do-stuff</pre>
    /// 
    ///     You can also create the equivalent of what you'd normally use parenthesis for when creating conditional statements in traditional 
    ///     programming languages, by grouping your conditions inside of each other. The next example would for instance yield true, and 
    ///     execute its child [lambda] node, even though the [and] condition yields false. This is because the first [or] child of our 
    ///     [if] node will evaluated to true, in addition to its inner [and] condition.
    /// 
    ///     <pre>_foo:bar
    ///   foo:bar
    /// if:@/-?name
    ///   =:_foo
    ///   and:@/./-?value
    ///     =:foo
    ///   or:@/./-?value
    ///     =:bar
    ///     and:@/././-?name
    ///       =:_foo
    ///       or:@/./././-?name
    ///         =:_foo2
    ///         and:@/././././-/0?value
    ///   lambda
    ///     do-stuff</pre>
    /// 
    ///     If you change the name of the [_foo] node to [_foo2], the condition will still yield true, but then it will have to check the deepest 
    ///     levels of is parts, and won't yield true, before entering the last [or] statement, which in itself has a nested [and]. If you
    ///     change the name of [_foo] to [_foo2], in addition to removing the [foo] node, who's value is bar, then your condition will not 
    ///     yield true, because then your last <em>"exists condition"</em> will not yield true, which is a pre-requisite for your last [or] to be true.
    /// 
    ///     BTW, for the record; The above piece of code, is a ridiculously complex conditional statement, and something you'd very rarely, if
    ///     ever, actually encounter. Partially because in p5.lambda, you can largely avoid conditions completely, due to p5.lambda's richness in
    ///     its p5.lambda expressions, which will often completely eliminate the need for conditional statements.
    /// 
    ///     You can also of course compare the results of two expressions against each other, such as for instance;
    /// 
    ///     <pre>_source1
    ///   foo1:su
    ///   foo2:cc
    ///   foo3:ess
    /// _source2:success
    /// if:@/-?value
    ///   =:@/./-2/"*"?value
    ///   lambda
    ///     do-stuff</pre>
    /// 
    ///     In the above condition, the first expression will create a single result out of all the values of the children of the [_source1] node,
    ///     for then to compare that single result against the result of the second expression, which returns the value of [_source2]. Since they
    ///     both yield <em>"success"</em>, the condition will evaluate to true, and p5.lambda code inside the [lambda] node will be executed.
    /// 
    ///     Instead of using [lambda], you can also use any other [lambda.xxx] Active Events you wish, as the nodes to be executed when 
    ///     your expressions yields true. You can also use [lambda.copy], [lambda.immutable], [lambda.single], or for that matter [lambda.fork] 
    ///     if you wish. Any Active Event starting with the text <em>"lambda"</em>, can be used as the conditional p5.lambda code to execute 
    ///     upon success of evaluation of your conditions. For instance, consider this code, evaluating two integers, to see if one of them is larger
    ///     than the other, and if so, invokes a [lambda.single] statement.
    /// 
    ///     <pre>_exe
    ///   set:@/.?value
    ///     source:success
    /// _src1:int:5
    /// _src2:int:6
    /// if:@/-?value
    ///   \>:@/./-2?value
    ///   lambda.single:@/./-3/"*"/?node</pre>
    /// </summary>
    public class Conditions
    {
        
        /*
         * the operators you can legally use for your conditions.
         */
        private enum Operator
        {
            // "=" operator
            Equals,

            // "!=" operator
            NotEquals,

            // ">" operator
            MoreThan,

            // "<" operator
            LessThan,

            // ">=" operator
            MoreThanEquals,

            // "<=" operator
            LessThanEquals,

            // "!" operator
            Not,

            // default, simple exists. No explicit operator.
            Exist
        }

        // application context needed to create expressions, in case expressions have typed values
        private readonly ApplicationContext _context;
        // root node for our condition object
        private readonly Node _statementNode;
        private bool _hasEvaluated;
        // for simple exist statements, there's no need for a [lambda] execution object since everything
        // inside of condition object automatically becomes a "lambda"
        private bool _isSimpleExist;

        /// <summary>
        ///     Initializes a new instance of the condition class.
        /// </summary>
        /// <param name="statementNode">The node of the conditional statement.</param>
        /// <param name="context">Application context.</param>
        public Conditions (Node statementNode, ApplicationContext context)
        {
            if (statementNode == null)
                throw new ApplicationException ("you must submit a node to Condition, for it to be able to evaluate");
            _statementNode = statementNode;
            _isSimpleExist = false;
            _context = context;
        }

        /// <summary>
        ///     Returns lambda objects to execute if condition yields true.
        /// 
        ///     Returns all execution lambda objects beneath the statement to execute if statement evaluates to true.
        /// </summary>
        /// <value>The p5.lambdas nodes to execute if statement yields true.</value>
        public IEnumerable<Node> ExecutionLambdas
        {
            get { return _statementNode.Children.Where (idxChild => idxChild.Name.StartsWith ("lambda")); }
        }

        /// <summary>
        ///     Returns true if value "exists".
        /// 
        ///     Returns true if this is a simple "exists" Condition, meaning it has only one expression, or constant
        ///     being evaluated, and nothing to evaluate it against, and no children nodes of type [lambda.xxx].
        /// 
        ///     A condition can be "complex", meaning you compare one value against another value, or it can be a "simple exists check",
        ///     meaning you simply check if some expression or constant yields "true". If it is a "simple exists check", then this will
        ///     return true.
        /// </summary>
        /// <value><c>true</c> if this instance is simple exist; otherwise, <c>false</c>.</value>
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
        ///     Evaluates Condition, and returns result.
        /// 
        ///     Will return true if Condition is true, otherwise false.
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
            var match = Expression.Create (exp, _context).Evaluate (currentStatement, _context);

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
         * compares two expressions or constants
         */
        private int Compare (Node currentStatement)
        {
            // constructing nodes for simplicity, since Node implements IComparable, which does our heavy lifting
            var lhs = new Node (string.Empty, XUtil.Single<object> (currentStatement, _context, null));
            var rhs = new Node (string.Empty, XUtil.Single<object> (currentStatement.FirstChildNotOf (string.Empty), _context, null));
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
         * recursively evaluates all related "or" conditions
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
    }
}
