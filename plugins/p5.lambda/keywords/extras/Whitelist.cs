/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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
    ///     Class wrapping the [whitelist] keyword in p5 lambda.
    /// </summary>
    public static class Whitelist
    {
        /// <summary>
        ///     The [whitelist] keyword, allows you to create a lambda context of pre-defined legal Active Events
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "whitelist")]
        public static void lambda_whitelist (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning.
            using (new Utilities.ArgsRemover (e.Args)) {

                // Checking that caller does not apply a new [whitelist], to reduce restrictions.
                if (context.Whitelist != null)
                    throw new LambdaSecurityException ("Caller tried to apply a [whitelist], when one was already applied to the context.", e.Args, context);

                // Retrieves the legal keywords, and the lambda object to evaluate within this context.
                // Notice, we ONLY fetch the first [_events] definition, to avoid making it possible for a malicious caller to inject an additional [_events] definition here.
                var whitelist = e.Args["_events"];

                // Sanity check.
                if (whitelist == null)
                    throw new LambdaException ("[whitelist] requires you to supply an [_events] definition.", e.Args, context);

                // Making sure that whitelist is reset after exiting current scope.
                context.Whitelist = whitelist.Clone ();
                try {

                    // Evaluating [eval], now with our Whitelist definition.
                    context.Raise ("eval", e.Args);

                } finally {

                    // Making sure we reset whitelist before exiting current scope.
                    context.Whitelist = null;
                }
            }
        }

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

                // Verifying currently iterated lambda child node's name can be found in [post-condition] of [whitelist].
                if (condition.Children.Count (ix => ix.Name == idxLambda.Name) == 0) {

                    // Child of lambda was not found in [post-condition], making sure we remove all children of lambda, before we throw an exception.
                    lambda.Clear ();
                    throw new LambdaSecurityException ("[post-condition] of [whitelist] not met", lambda, context);
                }
            }
        }
    }
}
