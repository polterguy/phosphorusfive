/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;
using p5.exp.exceptions;
using p5.lambda.helpers;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping conditional p5 lambda keywords, such as [if], [else-if] and [else]
    /// </summary>
    public static class Branching
    {
        /// <summary>
        ///     The [if] keyword allows for conditional execution of p5 lambda nodes
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "if")]
        public static void lambda_if (ApplicationContext context, ActiveEventArgs e)
        {
            // Evaluating condition
            var condition = new Conditions ();
            if (condition.Evaluate (context, e.Args)) {

                // Executing current scope since evaluation of condition yielded true
                condition.ExecuteCurrentScope (context, e.Args);
            }
        }

        /// <summary>
        ///     The [else-if] keyword allows for conditional execution of p5 lambda nodes
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "else-if")]
        public static void lambda_else_if (ApplicationContext context, ActiveEventArgs e)
        {
            // Syntax checking statement, making sure it has either an [if] or [else-if] as previous sibling
            VerifyElseSyntax (e.Args, context, "else-if");

            // Checking if previous [if] or [else-if] returned true, and if not, evaluating current node
            if (PreviousConditionEvaluatedTrue (e.Args, context)) {

                // Clearing entire scope, setting value of [else-if] to false, 
                // before returning, without even evaluating condition
                e.Args.Clear ();
                e.Args.Value = false;
                return;
            }

            // Evaluating condition
            var condition = new Conditions ();
            if (condition.Evaluate (context, e.Args)) {

                // Executing current scope since evaluation of condition yielded true
                condition.ExecuteCurrentScope (context, e.Args);
            }
        }

        /// <summary>
        ///     The [else] keyword allows for conditional execution of p5 lambda nodes
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "else")]
        public static void lambda_else (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Value != null)
                throw new LambdaException ("[else] does not take any arguments", e.Args, context);

            // Syntax checking statement, making sure it has either an [if] or [else-if] as its previous sibling
            VerifyElseSyntax (e.Args, context, "else");
            
            // Checking if previous conditional statement yielded true, and if not, evaluating current node
            if (PreviousConditionEvaluatedTrue (e.Args, context)) {

                // Not executing [else], since previous conditional statement evaluated to true!
                return;
            }

            // Since no previous conditions evaluated to true, we simply execute this scope, 
            // without checking any conditions, since there are none!
            e.Args.Value = true;
            var condition = new Conditions ();
            condition.ExecuteCurrentScope (context, e.Args);
        }

        /*
         * Checks if previous conditional statement ([if] or [else-if]) evaluated to true, and if so
         * return true, else return false to caller
         */
        private static bool PreviousConditionEvaluatedTrue (Node args, ApplicationContext context)
        {
            // Retrieving previous sibling
            Node curIdx = args.PreviousSibling;

            // Looping until we reach [if], or true as value of [else-if]
            while (true) {

                // Finding evaluation value of currently iterated conditional statement
                var val = curIdx.Get<bool> (context);
                if (val) {

                    // Currently iterated conditional statement yielded true
                    return true;
                } else if (curIdx.Name == "if") {

                    // No previous conditional statements evaluated to true, returning false to caller
                    return false;
                }

                // Finding previous statement
                curIdx = curIdx.PreviousSibling;
            }
        }

        /*
         * Verifies that an [else] or [else-if] has a previous [if]
         */
        private static void VerifyElseSyntax (Node node, ApplicationContext context, string statement)
        {
            /*
             * Note, we are taking advantage of that [if] never verifies its syntax here, such that
             * all [else] and [else-if] statements that runs through this, verifies their previous
             * statements are either [else-if] or [else], which means that it becomes impossible to
             * start a conditional chain with anything but an [if] statement
             */

            // Retrieving previous node
            var previous = node.PreviousSibling;

            // Making sure previous statement is of type [if] or [else-if]
            if (previous == null || (previous.Name != "if" && previous.Name != "else-if"))
                throw new LambdaException ("[" + statement + "] must have a matching [if] and/or [else-if] as its previous sibling", node, context);
        }
    }
}
