/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [eval-x] keyword in p5 lambda
    /// </summary>
    public static class EvalExpression
    {
        /// <summary>
        ///     Forward evaluates all expressions in values of resulting node expression
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "eval-x")]
        public static void eval_x (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving expression from main node, making sure it was in fact an expression
            var exp = e.Args.Value as Expression;
            if (exp == null) {

                // Oops...!
                throw new LambdaException (
                    "[eval-x] was not given a valid expression",
                    e.Args,
                    context);
            } else {

                // Looping through each match in expression
                foreach (var idxMatch in exp.Evaluate (context, e.Args, e.Args)) {

                    // Checking that currently iterated result is a node
                    if (idxMatch.TypeOfMatch != Match.MatchType.node) {
                        throw new LambdaException (
                            "[eval-x] was not given an expression exclusively yielding node results",
                            e.Args,
                            context);
                    }

                    // Checking type of node value
                    if (XUtil.IsExpression (idxMatch.Node.Value)) {

                        // Evaluates result of expression, and substitues value with expression result
                        idxMatch.Node.Value = idxMatch.Node.GetExValue<object> (context, null);
                        idxMatch.Node.Children.RemoveAll (ix => ix.Name == ""); // In case expression was formatted
                    } else if (XUtil.IsFormatted (idxMatch.Node)) {

                        // Formats value, and substitutes value with formatted value, before removing all formatting values
                        idxMatch.Node.Value = XUtil.FormatNode (context, idxMatch.Node);
                        idxMatch.Node.Children.RemoveAll (ix => ix.Name == "");
                    } // Notice, we do not throw, to support recursive evaluations by using the /** iterator
                }
            }
        }
    }
}
