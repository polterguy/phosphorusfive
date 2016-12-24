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

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [for-each] Active Event.
    /// </summary>
    public static class ForEach
    {
        /// <summary>
        ///     The [for-each] event, allows you to iterate over the results of expressions.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "for-each")]
        public static void lambda_for_each (ApplicationContext context, ActiveEventArgs e)
        {
            // Storing old [for-each] lambda, such that we can make sure each iteration becomes "locally immutable".
            Node originalLambda = e.Args.Clone ();

            // Retrieving what to iterate.
            var match = XUtil.DestinationMatch (context, e.Args);

            // Evaluating lambda, until either all iterations are done, or stop flag condition tells us we're done, due to some condition stopping loop early.
            var first = true;
            foreach (var idx in match) {

                // Making sure we reset back lambda block, each consecutive time, after the initial iteration.
                // Notice, this logic first of all preserves some CPU cycles, not having to excessively clone the original lambda, but also actually makes
                // sure we can view the last iteration, if an exception or something similar occurs during execution.
                if (first) {
                    first = false;
                } else {
                    e.Args.Clear ().AddRange (originalLambda.Clone().Children);
                }

                // Perform a single iteration.
                if (!Iterate (context, idx.Value, e.Args))
                    break;
            }
        }

        /*
         * Invokes [for-each] lambda, setting the [_dp] to the currently iterated value, and returns true if iteration should continue.
         * If it returns false, then iteration has for some reasons been stopped early ...
         */
        static bool Iterate (ApplicationContext context, object dp, Node lambdaObj)
        {
            // Inserting data pointer for current iteration.
            lambdaObj.Insert (0, new Node ("_dp", dp));

            // Evaluating (mutably) [for-each] lambda object, such that loop has access to entire tree.
            context.RaiseEvent ("eval-mutable", lambdaObj);

            // Checking if we have some sort of stop condition.
            var stopNode = lambdaObj.Root.FirstChild;
            bool proceed = stopNode.Name != "_return" && stopNode.Name != "_break";

            // Checking if this is a "local stop" node, at which case, we simply untie it to clean up after ourselves.
            // Notice, we do NOT untie any [_return] nodes, only the ones which we might be locally interested in, inside of our [for-each].
            // We still however, stop further iteration if [_return] is supplied.
            if (stopNode.Name == "_break" || stopNode.Name == "_continue") {
                stopNode.UnTie ();
            }
            return proceed;
        }
    }
}