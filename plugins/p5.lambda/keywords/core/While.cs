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

using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5.lambda.helpers;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [while] Active Event.
    /// </summary>
    public static class While
    {
        /// <summary>
        ///     The [while] event, allows you to loop, for as long as some condition is true.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "while")]
        public static void lambda_while (ApplicationContext context, ActiveEventArgs e)
        {
            // Storing original lambda and value, such that we can evaluate [while] immutably for each iteration.
            Node originalLambda = e.Args.Clone ();

            // We prevent infinite loops, unless [_unchecked] is true.
            int noIterations = 0;
            bool uncheck = e.Args.GetExChildValue ("_unchecked", context, false);

            // Looping for as long as condition is true.
            var condition = new Condition (context, e.Args);
            while (condition.Evaluate ()) {

                // Executing current scope, since condition evaluated to true.
                condition.ExecuteCurrentScope ();

                // Checking if we got a [return] invocation during evaluation
                var rootChild = e.Args.Root.FirstChild;
                if (rootChild.Name == "_break") {

                    // [break] invocation, returning from method to avoid further iteration, while making sure we also clean up after ourselves.
                    rootChild.UnTie ();
                    return;

                }

                if (rootChild.Name == "_return") {

                    // [return] invocation, stopping execution of loop entirely.
                    // Notice, it is not up to us to clean up this signal node, but rather [eval], or the outer execution lambda's responsibility.
                    return;

                }

                if (rootChild.Name == "_continue") {

                    // [continue] instruction, simply cleaning up, to make sure we evaluate the nest iteration normally.
                    rootChild.UnTie ();

                }

                // Checking if we're overflowing maximum number of iterations, unless [_unchecked] was true.
                if (!uncheck && noIterations++ > 5000)
                    throw new LambdaException ("Possible infinite [while] loop encountered, more than 5.000 iterations", e.Args, context);

                // Making sure each iteration is immutable, and that the next condition is evaluated correctly.
                e.Args.Value = originalLambda.Value;
                e.Args.Clear ().AddRange (originalLambda.Clone ().Children);
            }
        }
    }
}
