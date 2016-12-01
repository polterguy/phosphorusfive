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

using p5.core;
using p5.exp.exceptions;
using p5.lambda.helpers;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping conditional Active Events, such as [if], [else-if] and [else]
    /// </summary>
    public static class Branching
    {
        /// <summary>
        ///     The [if] Active Event allows for conditional execution of a lambda object.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "if")]
        public static void lambda_if (ApplicationContext context, ActiveEventArgs e)
        {
            // Evaluating condition
            var condition = new Condition (context, e.Args);
            if (condition.Evaluate ()) {

                // Executing current scope since evaluation of condition yielded true
                condition.ExecuteCurrentScope ();
            }
        }

        /// <summary>
        ///     The [else-if] Active Event allows for conditional execution of a lambda object.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "else-if")]
        public static void lambda_else_if (ApplicationContext context, ActiveEventArgs e)
        {
            // Syntax checking statement, making sure it has either an [if] or [else-if] as previous sibling
            VerifyElseSyntax (e.Args, context);

            // Checking if previous [if] or [else-if] returned true, and if not, evaluating current node
            if (!PreviousConditionEvaluatedTrue (e.Args, context)) {

                // Evaluating condition
                var condition = new Condition (context, e.Args);
                if (condition.Evaluate ()) {

                    // Executing current scope since evaluation of condition yielded true
                    condition.ExecuteCurrentScope ();
                }
            }
        }

        /// <summary>
        ///     The [else] Active Event allows for conditional execution of a lambda object.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "else")]
        public static void lambda_else (ApplicationContext context, ActiveEventArgs e)
        {
            // Syntax checking statement, making sure it has either an [if] or [else-if] as its previous sibling
            VerifyElseSyntax (e.Args, context);
            
            // Checking if previous conditional statement yielded true, and if not, evaluating current node
            if (!PreviousConditionEvaluatedTrue (e.Args, context)) {

                // Since no previous conditions evaluated to true, we simply execute this scope, without checking any conditions, 
                // since there are none!
                context.Raise ("eval-mutable", e.Args);
            }
        }

        /*
         * Verifies that an [else] or [else-if] has a previous [if].
         */
        private static void VerifyElseSyntax (Node args, ApplicationContext context)
        {
            /*
             * Note, we are taking advantage of that [if] never verifies its syntax here, such that
             * all [else] and [else-if] statements that runs through this, verifies their previous
             * statements are either [else-if] or [else], which means that it becomes impossible to
             * start a conditional chain with anything but an [if] statement.
             */

            // Retrieving previous node
            var previous = args.PreviousSibling;

            // Making sure previous statement is of type [if] or [else-if].
            if (previous == null || (previous.Name != "if" && previous.Name != "else-if"))
                throw new LambdaException ("[" + args.Name + "] must have a matching [if] as its previous sibling", args, context);
        }

        /*
         * Checks if a previous conditional statement ([if] or [else-if]) evaluated to true, and if so return true, otherwise return false.
         */
        private static bool PreviousConditionEvaluatedTrue (Node args, ApplicationContext context)
        {
            // Looping as long as we have conditional statements as previous siblings.
            for (var ix = args.PreviousSibling; ix != null && (ix.Name == "if" || ix.Name == "else-if"); ix = ix.PreviousSibling) {

                // Checking if currently iterated conditional Active Event evaluated to true.
                if (ix.Value is bool && (bool)ix.Value) {

                    // Currently iterated conditional statement yielded true.
                    return true;
                }
            }

            // No previous conditional Active Event(s) evaluated to true.
            return false;
        }
    }
}
