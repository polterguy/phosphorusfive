/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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

using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

/// <summary>
///     Contains helper classes for p5 lambda conditional active events
/// </summary>
namespace p5.lambda.helpers
{
    /// <summary>
    ///     Class wrapping commonalities for conditional Active Events, such as [if], [else-if] and [while].
    /// </summary>
    public class Conditions
    {
        // Used as buffer for all comparison operators and logical operators in system, such as [or], [and], [=] and [!=] etc.
        // Only retreieved once, and cached for the duration of application life cycle.
        private static Node _operators;

        // Used to hold the number of operators used to evaluate the root conditional node.
        // This is used to calculate [offset] if condition evaluates to true, such that we do not raise conditional operator Active Events 
        // as part of the execution process of the current conditional scope.
        private int _numberOfConditions;

        private ApplicationContext _context;
        private Node _args;

        /// <summary>
        ///     Creates an instance of condition class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        public Conditions (ApplicationContext context, Node args)
        {
            _context = context;
            _args = args;
        }

        /// <summary>
        ///     Evaluate a conditional Active Event node, and returns either true if condition yields true, otherwise false.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool Evaluate ()
        {
            // Retrieving all conditional children nodes.
            var conditions = GetConditionalNodes ().ToList ();

            // Storing how many operators are part of our root conditional node, such that we can now the offset we should use later.
            // We only store this number on the "root condition node", which means our first run-through, and since this method recursively invokes
            // itself indirectly multiple times, we check if value is -1 before we decide if we should set it or not.
            _numberOfConditions = conditions.Count;

            // If condition had no operator Active Events, we must evaluate a "simple exist" condition.
            if (conditions.Count == 0) {

                // No conditions, evaluating simple "exists" condition.
                TryEvaluateSimpleExist ();

            } else {

                // Looping through each condition.
                foreach (var idx in conditions) {

                    // Raising comparison operator Active Event, logical operator event, or any other event currently part of conditional operators.
                    _context.Raise (idx.Name, idx);

                    // Moving results of Active Event invocation up from conditional Active Event invocation result node's value,
                    // to the result of conditional statement, to "bubble" results up to the top-most branching Active Event branching node.
                    _args.Value = idx.Value;

                    // Checking if conditional event created the [_abort] flag for us.
                    if (idx.Children.Count > 0 && idx[0].Name == "_abort" && idx[0].Get<bool> (_context))
                        return idx.Get (_context, false);
                }
            }

            // Returning results of evaluation of conditional chain.
            return _args.Get<bool> (_context);
        }

        /// <summary>
        ///     Executes the current scope of branching.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        public void ExecuteCurrentScope ()
        {
            // Making sure there actually is something to evaluate.
            if (_args.Children.Count == _numberOfConditions)
                return;

            // Storing offset temporary in args, such that [eval-mutable] knows where to start execution.
            if (_numberOfConditions > 0)
                _args.Insert (0, new Node ("offset", _numberOfConditions + 1 /* Remember [offset] node itself */));

            // Evaluating body of conditional statement, now with [offset] pointing to first non-comparison/non-formatting node.
            _context.Raise ("eval-mutable", _args);
        }

        /*
         * Will evaluate the given condition to true, if it is anything but a false boolean, null, 
         * or an expression returning anything but null or false
         */
        private void TryEvaluateSimpleExist ()
        {
            // If value is not boolean type, we evaluate value, and set its value to true, if evaluation did not
            // result in "null" or "false"
            if (_args.Value == null) {

                // Null evaluates to false
                _args.Value = false;
            } else {

                // Checking if value already is boolean, at which case we don't evaluate any further, since it is already evaluated
                if (!(_args.Value is bool)) {

                    var obj = XUtil.Single<object> (_context, _args, false, null);
                    if (obj == null) {

                        // Result of evaluated expression yields null, hence evaluation result is false
                        _args.Value = false;
                    } else if (obj is bool) {

                        // Result of evaluated expression yields boolean, using this boolean as result
                        _args.Value = obj;
                    } else {

                        // Anything but null and boolean, existence is true, hence evaluation becomes true!
                        _args.Value = true;
                    }
                }
            }
        }

        /*
         * Returns all nodes that either comparison operators or logical operators, and hence should be evaluated
         */
        private IEnumerable<Node> GetConditionalNodes ()
        {
            // Checking if we have retrieved operators, and if not, retrieving them.
            if (_operators == null) {
            
                // Retrieving all comparison operators and logical operators in system.
                _operators = _context.Raise ("operators");

                // Then adding empty node, since it can be used to formatted expressions, etc.
                _operators.Add ("");
            }

            // Checking if value of args is null, and if so, we use the first child of it as an Active Event operator, 
            // which simply will be evaluated, and its value checked for true afterwards, during evaluation of conditional nodes.
            Node idxOperator = _args.FirstChild;
            if (_args.Value == null) {

                // Basic syntax checking.
                if (_args.Children.Count == 0)
                    throw new LambdaException ("Nothing to use as a condition for your branching Active Event", _args, _context);

                // Returning first child as Active Event condition.
                yield return _args.FirstChild;

                // Incrementing our idxOperator node.
                idxOperator = idxOperator.NextSibling;
            }

            // Then returning operators, until we find something that is NOT in our list of comparison/logical operators.
            while (idxOperator != null) {

                // Checking if currently iterated node's name is in list of operators.
                if (!_operators.Children.Exists (ix => ix.Name == idxOperator.Name))
                    yield break; // This is not an "operator" node, stopping further iteration.

                // This is a comparison/logical operator, or an empty formatting node.
                yield return idxOperator;

                // Incrementing currently iterated node.
                idxOperator = idxOperator.NextSibling;
            }
        }
    }
}
