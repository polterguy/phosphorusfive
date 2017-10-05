/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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

namespace p5.lambda.helpers
{
    /// <summary>
    ///     Class wrapping commonalities for conditional Active Events, such as [if], [else-if] and [while].
    /// </summary>
    public class Condition
    {
        // Used to hold all conditional nodes for evaluation.
        List<Node> _conditions;

        readonly ApplicationContext _context;
        readonly Node _args;

        /// <summary>
        ///     Creates an instance of condition class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        public Condition (ApplicationContext context, Node args)
        {
            _context = context;
            _args = args;
        }

        /// <summary>
        ///     Evaluate a conditional Active Event node, and returns either true if condition yields true, otherwise false.
        /// </summary>
        /// <returns>True if condition yields true</returns>
        public bool Evaluate ()
        {
            // Retrieving conditions once for each evaluation, since they might change.
            _conditions = GetConditionalNodes ().ToList ();

            // If condition had no operator Active Events, we must evaluate a simple "exist" condition.
            if (_conditions.Count == 0) {

                // No conditions, evaluating simple "exists" condition.
                TryEvaluateSimpleExist ();

            } else {

                // Looping through each condition.
                foreach (var idx in _conditions) {

                    // Checking if this is an empty condition, which simply means it's a formatting value node.
                    if (idx.Name == "")
                        continue;

                    // Raising comparison operator Active Event, logical operator event, or any other event currently part of conditional operators.
                    _context.RaiseEvent (idx.Name, idx);

                    // Moving results of Active Event invocation up from conditional Active Event invocation result node's value,
                    // to the result of conditional statement, to "bubble" results up to the top-most branching Active Event branching node.
                    _args.Value = idx.Value;
                }

                // Notice, in case this was some generic event condition, without a comparison operator, such as a dynamically created Active Event, 
                // we need to run the same logic here, as we do in our comparison operators, which is to check if value is boolean, at which case we 
                // use the existing boolean value, or if it is not boolean, check for "simple exists" of "any type of object".
                // The point is that the value of our conditional node, should never leave this method, as anything else but either a true or a false value!
                // This is done because a dynamically created Active Event, used as a condition for a condition, might yield something which is not a boolean value.
                if (!(_args.Value is bool))
                    _args.Value = _args.Value != null;

                // House cleaning.
                _args ["_p5_conditions_state_"]?.UnTie ();
            }

            // Returning results of evaluation of conditional chain.
            return _args.Get<bool> (_context);
        }

        /// <summary>
        ///     Executes the current scope of branching.
        /// </summary>
        public void ExecuteCurrentScope ()
        {
            // Making sure we now add the formatting nodes to our _conditions, before we calculate [offset].
            var offset = _conditions.Count + _args.Children.Count (ix => ix.Name == "");

            // Making sure there actually is something to evaluate.
            if (_args.Count == offset)
                return;

            // Storing offset temporary in args, such that [eval-mutable] knows where to start execution.
            if (offset > 0)
                _args.Insert (0, new Node ("offset", offset));

            // Evaluating body of conditional statement, now with [offset] pointing to first non-comparison/non-formatting node.
            _context.RaiseEvent ("eval-mutable", _args);
        }

        /*
         * Will evaluate the given condition to true, if it is anything but a false boolean, null, 
         * or an expression returning anything but null or false
         */
        void TryEvaluateSimpleExist ()
        {
            // If value is not boolean type, we evaluate value, and set its value to true, if evaluation did not result in "null" or "false".
            if (_args.Value == null) {

                // Null evaluates to false.
                _args.Value = false;

            } else {

                // Checking if value already is boolean, at which case we don't evaluate any further, since it is already evaluated.
                if (!(_args.Value is bool)) {

                    var obj = XUtil.Single<object> (_context, _args);
                    if (obj == null) {

                        // Result of evaluated expression yields null, hence evaluation result is false.
                        _args.Value = false;
                    } else if (obj is bool) {

                        // Result of evaluated expression yields boolean, using this boolean as result.
                        _args.Value = obj;
                    } else {

                        // Anything but null and boolean, existence is true, hence evaluation becomes true
                        _args.Value = true;
                    }
                }
            }
        }

        /*
         * Returns all nodes that either comparison operators or logical operators, and hence should be evaluated
         */
        IEnumerable<Node> GetConditionalNodes ()
        {
            // Retrieving all comparison operators and logical operators in system.
            // Notice, this has to be done once for each condition we create, since theoretically the operators might change, due to
            // having instance event handler objects in the ApplicationContext.
            var operators = _context.RaiseEvent ("operators");

            // Checking if value of args is null, and if so, we use the first child of it as an Active Event operator, 
            // which simply will be evaluated, and its value checked for true afterwards, during evaluation of conditional nodes.
            Node idxOperator = _args.FirstChild;
            if (_args.Value == null) {

                // Basic syntax checking.
                if (_args.Count == 0)
                    throw new LambdaException ("Nothing to use as a condition for your branching Active Event", _args, _context);

                // Returning first child as Active Event condition.
                yield return idxOperator;

                // Incrementing our idxOperator node.
                idxOperator = idxOperator.NextSibling;
            }

            // Then returning operators, until we find something that is NOT in our list of comparison/logical operators.
            while (idxOperator != null) {

                // Making sure we simply continue if current node is a formatting node, to avoid breaking if expression in
                // main conditional node is formatted somehow.
                if (idxOperator.Name != "") {

                    // Checking if currently iterated node's name is in list of operators.
                    if (!operators.Children.Any (ix => ix.Name == idxOperator.Name))
                        yield break; // This is not an "operator" node, stopping further iteration.

                    // This is a comparison/logical operator, or an empty formatting node.
                    yield return idxOperator;
                }

                // Incrementing currently iterated node.
                idxOperator = idxOperator.NextSibling;
            }
        }
    }
}
