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
        private int _noRootConditions = -1;

        /*
         * Recursively run through conditions.
         */
        /// <summary>
        ///     Evaluate a conditional Active Event node, and returns either true if condition yields true, otherwise false.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool Evaluate (ApplicationContext context, Node args)
        {
            // Retrieving all conditional children nodes.
            var conditions = GetConditionalEventNodes (context, args).ToList ();

            // Storing how many operators are part of our root conditional node, such that we can now the offset we should use later.
            // We only store this number on the "root condition node", which means our first run-through, and since this method recursively invokes
            // itself indirectly multiple times, we check if value is -1 before we decide if we should set it or not.
            if (_noRootConditions == -1)
                _noRootConditions = conditions.Count;

            // Looping through each condition
            foreach (var idx in conditions) {

                switch (idx.Name) {

                case "or":
                    TryEvaluateSimpleExist (context, args);
                    if (args.Get<bool> (context)) {

                        // Evaluated to true! Aborting the rest of conditional checks, since condition before [or] evaluated to true!
                        return true;
                    }

                    // Recursively loop through next condition, if previous condition did NOT evaluate to true!
                    args.Value = Evaluate (context, idx);
                    break;

                case "and":
                    TryEvaluateSimpleExist (context, args);

                    // Recursively loop through, but only if previous statements are true! If previous condition evaluated to false, we abort!
                    args.Value = args.Get<bool> (context) && Evaluate (context, idx);
                    break;

                case "xor":
                    TryEvaluateSimpleExist (context, args);

                    // Only evaluates to true if conditions are NOT EQUAL
                    args.Value = args.Get<bool> (context) != Evaluate (context, idx);
                    break;

                case "not":
                    TryEvaluateSimpleExist (context, args);

                    // Basic syntax checking
                    if (idx.Value != null || idx.Children.Count != 0)
                        throw new LambdaException ("Operator [not] cannot have neither any value, nor any children", idx, context);

                    // Simply "negates" the previously evaluated condition
                    args.Value = !args.Get<bool> (context);
                    break;

                default:

                    // Raising comparison operator Active Event, 
                    // or any other Active Event currently part of conditional operators
                    context.Raise (idx.Name, idx);

                    // Moving results of Active Event invocation up from conditional Active Event invocation result node's value,
                    // to the result of conditional statement, to "bubble" results up to the top-most branching Active Event result
                    args.Value = idx.Value;
                    break;
                }
            }

            // If condition had no operator active event children, then we must evaluate a "simple exist" condition
            TryEvaluateSimpleExist (context, args);

            return args.Get<bool> (context);
        }

        /*
         * Executes current scope
         */
        public void ExecuteCurrentScope (ApplicationContext context, Node args)
        {
            // Making sure there actually is something to evaluate.
            if (args.Children.Count == 0)
                return;

            // Storing offset temporary in args, such that [eval-mutable] knows where to start execution.
            if (_noRootConditions > 0)
                args.Insert (0, new Node ("offset", _noRootConditions + 1 /* Remember [offset] node itself */));

            // Evaluating body of conditional statement, now with [offset] pointing to first non-comparison/non-formatting node.
            context.Raise ("eval-mutable", args);
        }

        /*
         * Will evaluate the given condition to true, if it is anything but a false boolean, null, 
         * or an expression returning anything but null or false
         */
        private void TryEvaluateSimpleExist (ApplicationContext context, Node args)
        {
            // If value is not boolean type, we evaluate value, and set its value to true, if evaluation did not
            // result in "null" or "false"
            if (args.Value == null) {

                // Null evaluates to false
                args.Value = false;
            } else {

                // Checking if value already is boolean, at which case we don't evaluate any further, since it is already evaluated
                if (!(args.Value is bool)) {

                    var obj = XUtil.Single<object> (context, args, false, null);
                    if (obj == null) {

                        // Result of evaluated expression yields null, hence evaluation result is false
                        args.Value = false;
                    } else if (obj is bool) {

                        // Result of evaluated expression yields boolean, using this boolean as result
                        args.Value = obj;
                    } else {

                        // Anything but null and boolean, existence is true, hence evaluation becomes true!
                        args.Value = true;
                    }
                }
            }
        }

        /*
         * Returns all nodes that either comparison operators or logical operators, and hence should be evaluated
         */
        private static IEnumerable<Node> GetConditionalEventNodes (ApplicationContext context, Node args)
        {
            // Checking if we have retrieved operators, and if not, retrieving them.
            if (_operators == null) {
            
                // Retrieving all comparison operators and logical operators in system.
                _operators = context.Raise ("operators");

                // Then adding all logical operators.
                _operators.Add ("or");
                _operators.Add ("xor");
                _operators.Add ("and");
                _operators.Add ("not");

                // Then adding empty node.
                _operators.Add ("");
            }

            // Checking if value of args is null, and if so, we use the first child of it as an Active Event operator, 
            // which simply will be evaluated, and its value checked for true afterwards, during evaluation of conditional nodes.
            Node idxOperator = args.FirstChild;
            if (args.Value == null) {

                // Basic syntax checking.
                if (args.Children.Count == 0)
                    throw new LambdaException ("Nothing to use as a condition for your branching Active Event", args, context);

                // Returning first child as Active Event condition.
                yield return args.FirstChild;

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
