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
using p5.lambda.helpers;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [eval-x] Active Event.
    /// </summary>
    public static class EvalExpression
    {
        /// <summary>
        ///     Forward evaluates all expressions in values of resulting node expression.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "eval-x")]
        public static void eval_x (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all destinations.
                foreach (var idxMatch in XUtil.DestinationMatch (context, e.Args, true)) {

                    // Checking type of node value.
                    if (idxMatch.Node.Value is Expression) {

                        // Evaluates result of expression, and substitues value with expression result.
                        idxMatch.Node.Value = idxMatch.Node.GetExValue<object> (context, null);

                        // Making sure we remove all formatting parameters for clarity.
                        idxMatch.Node.RemoveAll (ix => ix.Name == "");
                    } else {

                        // Formats value, and substitutes value with formatting result.
                        idxMatch.Node.Value = XUtil.FormatNode (context, idxMatch.Node);
                    } // Notice, we do not throw, to support recursive evaluations by using the /** iterator, where parts of results are not supposed to be forward evaluated.
                }
            }
        }
    }
}
