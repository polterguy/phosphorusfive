/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Linq;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

/// <summary>
///     Contains helper classes for p5.lambda conditional active events.
/// 
///     Contains common helper classes for p5.lambda conditional statements, such as [if] and [while].
/// </summary>
namespace p5.lambda.helpers
{
    /// <summary>
    ///     Class wrapping commonalities between conditional statements.
    /// 
    ///     This class is used in for instance [while], [if] and [else-if], and wraps the conditions used to
    ///     figure out if statement yields true or not.
    /// </summary>
    public class Conditions
    {
        /*
         * recursively run through conditions
         */
        public static bool Evaluate (ApplicationContext context, Node args)
        {
            // looping through all conditional children nodes
            foreach (var idx in GetConditionalEventNodes (args)) {

                switch (idx.Name) {

                case "or":
                    TryEvaluateSimpleExist (context, args);
                    if (args.Get<bool> (context)) {

                        // evaluated to true!
                        // since previous conditions evaluated to true, there is no need to evaluate any further
                        // hence, cleaning up, and returning true "early"
                        RemoveConditionalOperators (context, args);
                        return true;
                    }

                    // recursively loop through, if previous condition did NOT evaluate to true!
                    args.Value = Evaluate (context, idx);
                    break;

                case "and":
                    TryEvaluateSimpleExist (context, args);

                    // recursively loop through, but only if previous statements are true!
                    args.Value = args.Get<bool> (context) && Evaluate (context, idx);
                    break;

                case "xor":
                    TryEvaluateSimpleExist (context, args);

                    // only evaluates to true if conditions are NOT EQUAL
                    args.Value = args.Get<bool> (context) != Evaluate (context, idx);
                    break;

                case "not":
                    TryEvaluateSimpleExist (context, args);
                    if (idx.Value != null || idx.Count != 0)
                        throw new LambdaException ("Operator [not] cannot have neither any value, nor any children", idx, context);

                    // simply "nots" the previously evaluated conditional value
                    args.Value = !args.Get<bool> (context);
                    break;

                default:

                    // raising comparison operator Active Event
                    context.Raise (idx.Name, idx);
                    break;
                }
            }

            // if condition had no operator active event children, then we must evaluate a "simple exist" condition
            TryEvaluateSimpleExist (context, args);

            if (args.Get<bool> (context)) {

                // success, evaluated to true
                RemoveConditionalOperators (context, args);
                return true;
            } else {

                // condition evaluated to false, returning false, removing entire execution scope
                args.Clear ();
                return false;
            }
        }

        private static void RemoveConditionalOperators (ApplicationContext context, Node args)
        {
            var removeList = GetConditionalEventNodes (args);
            foreach (var idxOperators in removeList) {
                idxOperators.UnTie ();
            }
        }

        /*
         * executes current scope, after cleaning up all conditional nodes, 
         * but only if root node's value has evaluated to true!
         * Returns true if scope was successfully executed!
         */
        public static void ExecuteCurrentScope (ApplicationContext context, Node args)
        {
            // executing current scope
            context.Raise ("lambda-mutable", args);
        }

        /*
         * will evaluate the given condition to true if it is anything but a boolean or a null value
         */
        private static void TryEvaluateSimpleExist (ApplicationContext context, Node args)
        {
            // if value is not boolean type, we evaluate value, and set its value to true if evaluation did not
            // result in "null"
            if (!(args.Value is bool)) {

                var obj = XUtil.Single<object> (args, context, null);
                args.Value = obj != null;
            }
        }

        /*
         * returns all nodes that are part of evaluating conditional statements
         */
        private static List<Node> GetConditionalEventNodes (Node args)
        {
            List<Node> retVal = new List<Node> ();
            foreach (var idx in args.Children) {
                switch (idx.Name) {
                case "and":
                case "or":
                case "xor":
                case "not":
                case "equals":
                case "not-equals":
                case "more-than":
                case "less-than":
                case "more-than-equals":
                case "less-than-equals":
                    retVal.Add (idx);
                    break;
                }
            }
            return retVal;
        }
    }
}
