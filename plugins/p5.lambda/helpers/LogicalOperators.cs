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

using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.helpers
{
    /// <summary>
    ///     Class wrapping all logical operator Active Events, such as [or], [and], [not] and [xor].
    /// </summary>
    public static class LogicalOperators
    {
        /// <summary>
        ///     Logical [or] conditional Active Event.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "or")]
        public static void or (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving previous node, and making sure parent has evaluated "exists" condition, if any, and doing some basical sanity check.
            var previous = EnsureParentFindPreviousCondition (context, e.Args);

            // If previous condition evaluated to true, then [or] also evaluates to true, and aborts the rest of the conditional checks altogether.
            if (previous.Get<bool> (context)) {

                // Previous condition yielded true, no needs to continue checks, or even evaluate this one, since it's true anyways.
                e.Args.Value = true;

            } else {

                // Figuring out results of condition.
                var result = new Condition (context, e.Args).Evaluate ();

                // To make sure [and] has presedence, we need to store a "maybe" flag in parent's state, which is only negated, if a [not] is encountered later.
                // This ensures that [and] gets presedence over [or].
                if (result) {

                    // Checking if there already exists a "state" node in condition, and if not, creating one, and inserting "maybe" results.
                    // "maybe" results is the result of previously evaluated condition.
                    e.Args.Parent.FindOrInsert ("_p5_conditions_state_", 0).Add ("_or_maybe_", previous.Get<bool> (context));
                }

                // Previous condition yielded false, try to evaluate this one, and returning results.
                e.Args.Value = result;
            }
        }

        /// <summary>
        ///     Logical [and] conditional Active Event.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "and")]
        public static void and (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving previous node, and making sure parent has evaluated "exists" condition, if any, and doing some basical sanity check.
            var previous = EnsureParentFindPreviousCondition (context, e.Args);

            // Checking if we've previously seen an [or], to make sure [and] has presedence.
            if (e.Args.Parent ["_p5_conditions_state_"]?["_or_maybe_"]?.Get (context, false) ?? false) {

                // No need to evaluate this condition.
                e.Args.Value = true;

            } else {

                // We only evaluate [and] if previous condition evaluated to true.
                if (previous.Get<bool> (context)) {

                    // Previous condition yielded true, now checking this one.
                    e.Args.Value = new Condition (context, e.Args).Evaluate ();

                } else {

                    // No needs to evaluate this one, since previous condition yielded false, this one also yields false.
                    // Notice however, we do NOT [_abort], to make sure [and] has presedence over [or], and there might theoretically come another
                    // [or] later down in our scope.
                    e.Args.Value = false;
                }
            }
        }

        /// <summary>
        ///     Logical [not] conditional Active Event.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "not")]
        public static void not (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure [not] has no children.
            if (e.Args.Children.Count > 0 || e.Args.Value != null)
                throw new LambdaException ("Logical [not] cannot have children or a value, it simply negates previous conditions", e.Args, context);

            // Retrieving previous node, and making sure parent has evaluated "exists" condition, if any, and doing some basical sanity check.
            var previous = EnsureParentFindPreviousCondition (context, e.Args);

            // Removing "maybe" condition, if it exists.
            e.Args.Parent["_p5_conditions_state_"]?["_or_maybe_"]?.UnTie ();

            // Negate the previous condition's results.
            e.Args.Value = !previous.Get<bool> (context);
        }

        /// <summary>
        ///     Returns all logical operators.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "operators")]
        public static void operators (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add ("or");
            e.Args.Add ("and");
            e.Args.Add ("not");
        }

        /*
         * Will evaluate the given condition to true, if it is anything but a false boolean, null, 
         * or an expression returning anything but null or false
         */
        private static Node EnsureParentFindPreviousCondition (ApplicationContext context, Node args)
        {
            // Sanity check.
            if (args.Parent == null || args.Parent.Name == "")
                throw new LambdaException (
                    string.Format ("[{0}] cannot be raised as a root node, only as a child of a conditional Active Event", args.Name), args, context);

            // If value is not boolean type, we evaluate value, and set its value to true, if evaluation did not result in "null" or "false".
            if (args.Parent.Value == null) {

                // Null evaluates to false.
                args.Parent.Value = false;

            } else {

                // Checking if value already is boolean, at which case we don't continue, since it is already evaluated.
                if (!(args.Parent.Value is bool)) {

                    var obj = XUtil.Single<object> (context, args.Parent);
                    if (obj == null) {

                        // Result of evaluated expression yields null, hence evaluation result is false.
                        args.Parent.Value = false;

                    } else if (obj is bool) {

                        // Result of evaluated expression yields boolean, using this boolean as result.
                        args.Parent.Value = obj;

                    } else {

                        // Anything but null and boolean, existence is true, hence evaluation becomes true.
                        args.Parent.Value = true;
                    }
                }
            }

            // Making sure we return previous conditional node.
            var retVal = args.PreviousSibling ?? args.Parent;
            while (retVal != null && retVal.Name == "") {
                retVal = retVal.PreviousSibling ?? retVal.Parent;
            }

            // Sanity check.
            if (retVal == null)
                throw new LambdaException (string.Format ("No previous condition found for [{0}]", args.Name), args, context);
            return retVal;
        }
    }
}
