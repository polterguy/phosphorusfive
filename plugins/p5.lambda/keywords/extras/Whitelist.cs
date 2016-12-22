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
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.keywords.extras
{
    /// <summary>
    ///     Class wrapping helper [pre-condition] and [post-condition] Active Events for the [eval-whitelist] Active Event.
    /// </summary>
    public static class Whitelist
    {
        /// <summary>
        ///     [post-condition] checking that children's names are only as specified.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.lambda.whitelist.post-condition.children-are-one-of")]
        public static void _p5_lambda_whitelist_post_condition_children_are_one_of (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving condition and lambda node from args.
            var condition = e.Args["post-condition"].Get<Node> (context);
            var lambda = e.Args["lambda"].Get<Node> (context);

            // Looping through each children of lambda, making sure it's name can be found in condition's children.
            foreach (var idxLambda in lambda.Children) {

                // Verifying currently iterated lambda child node's name can be found in [post-condition] of whitelist.
                if (condition.Children.Count (ix => ix.Name == idxLambda.Name) == 0) {

                    // Child of lambda was not found in [post-condition], making sure we remove all children of lambda, before we throw an exception.
                    lambda.Clear ();
                    throw new LambdaSecurityException ("[post-condition] of whitelist not met", lambda, context);
                }
            }
        }

        /// <summary>
        ///     [pre-condition] which evaluates the specified [lambda], and ensures it yields true.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.lambda.whitelist.pre-condition.evaluates-to-true")]
        public static void _p5_lambda_whitelist_pre_condition_evaluates_to_true (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving condition and lambda node from args, cloning it since we'll evaluate it using [eval].
            var condition = e.Args["pre-condition"].Get<Node> (context).Clone ();
            var lambda = e.Args["lambda"].Get<Node> (context);

            // Evaluating [pre-condition], asserting it yields true, detaching whitelist first, making sure we attach it afterwards.
            var old = context.Ticket.Whitelist;
            context.Ticket.Whitelist = null;
            try {
                context.RaiseEvent ("eval", condition);
            } finally {
                context.Ticket.Whitelist = old;
            }
            if (!condition.Get<bool> (context))
                throw new LambdaSecurityException ("[pre-condition] of whitelist not met", lambda, context);
        }
    }
}
